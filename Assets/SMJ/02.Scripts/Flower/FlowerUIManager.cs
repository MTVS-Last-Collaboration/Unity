using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;
using System.Linq;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.Android;

public class FlowerUIManager : MonoBehaviourPunCallbacks
{
    public MidnightChecker dateChanger;
    public FlowerUIManager partnerFlower;
    private bool isInitialized = false;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text completeText;
    [SerializeField] private TMP_Text listenCompleteText;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Image flowerImg;
    [SerializeField] private GameObject alertEmoji;

    [SerializeField] private GameObject exitButton;

    [SerializeField] private GameObject recordPanel;
    [SerializeField] private GameObject[] recordButtons;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] public bool testRecord = false;
    [SerializeField] private GameObject hoonUI;

    private UIPopupAnimation uiPopup;

    private VoiceRecorder recorder;
    private FlowerEvolution flowerEvol;
    private Flower flower;
    private ClickFlower click;

    public int recordCount = 0;

    public bool isRecordComplete = false;
    public bool isListenComplete = false;

    private string restTime = string.Empty;
    Coroutine recordingCor;
    private const int CHUNK_SIZE = 5000;
    private List<byte[]> voiceDataChunks = new List<byte[]>();

    private HoonSoundManagerLogin sound;

    private string flowerId;

    private string playerToken;

    [SerializeField] private TMP_Text failCount;
    [SerializeField] private GameObject successPanel;

    public bool isSuccess = false;

    [SerializeField] private GameObject loadingObj;
    [SerializeField] private CoinCollectionEffect coinEffect;
    [SerializeField] private GameObject coinStartVecObj;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 15;

        // 타임아웃 값을 더 길게 설정
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 300000; //5분
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.TimePingInterval = 2000;
    }
    private void Start()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            Permission.RequestUserPermission(Permission.ExternalStorageWrite);
        }
#endif
        StartCoroutine(Delay());
        flowerId = photonView.ViewID.ToString();
        sound = GameObject.Find("SMJ").GetComponent<HoonSoundManagerLogin>();
        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true; // 신뢰성 있는 전송
        sendOptions.Channel = 0; // 채널 설정
        flower = GetComponent<Flower>();
        recorder = GetComponent<VoiceRecorder>();
        flowerEvol = GetComponent<FlowerEvolution>();
        click = GetComponent<ClickFlower>();
        uiPopup = GetComponent<UIPopupAnimation>();
        uiPopup.SetTarget(uiPanel.GetComponent<RectTransform>());
        coinEffect.GetComponent<CoinCollectionEffect>();
        hoonUI = GameObject.Find("HoonLoobyCanvas");
        InitializeComponents();
        if (photonView.IsMine)
        {
            isSuccess = PlayerPrefs.GetInt($"IsSuccess_{photonView.ViewID}", 0) == 1;
        }
        // 자신이 소유한 오브젝트의 경우에만 토큰 설정 및 API 호출
        if (photonView.IsMine)
        {
            Debug.Log($"[Start] Initializing owned object - ViewID: {photonView.ViewID}");
            playerToken = PlayerPrefs.GetString("token");
            print("누구? : " + gameObject.name + "내토큰 : " + playerToken);
            StartCoroutine(InitializeAfterDelay());
            // 파트너 오브젝트가 설정될 때까지 대기 후 API 호출
            StartCoroutine(WaitForPartnerAndInitialize());
        }
        else
        {
            Debug.Log($"[Start] Not owner of object - ViewID: {photonView.ViewID}");
        }

        // 초기 상태 텍스트 설정
        UpdateStateText(flower.curState);
        if (photonView.IsMine)
        {
            StartCoroutine(InitialStateSync());
        }
        PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(1f);
    }
    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        if (flowerEvol != null)
        {
            photonView.RPC("RPC_SetPlayerToken", RpcTarget.All, playerToken, photonView.ViewID);
            photonView.RPC("RPC_InitialEvolutionCheck", RpcTarget.All);
            StartCoroutine(WaitForPartnerAndInitialize());
            StartCoroutine(InitialStateSync());
        }
        else
        {
            Debug.LogError("FlowerEvolution component not initialized!");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        // 마스터 클라이언트만 새로 들어온 플레이어에게 상태를 전송
        if (PhotonNetwork.IsMasterClient && photonView.IsMine)
        {
            StartCoroutine(SyncStateForNewPlayer(newPlayer));
        }
    }

    public IEnumerator SyncStateForNewPlayer(Player newPlayer)
    {
        // API를 통해 최신 상태를 가져옴
        yield return GetVoiceStatus();

        // 내가 마스터 클라이언트인 경우에만 두 오브젝트 모두의 상태를 전송
        if (PhotonNetwork.IsMasterClient)
        {
            // 오브젝트 1의 상태 전송
            if (photonView.IsMine)
            {
                var myState = new FlowerFullState
                {
                    viewId = photonView.ViewID,
                    state = flower.curState,
                    name = flower.nickName,
                    recordComplete = isRecordComplete,
                    listenComplete = isListenComplete,
                    evolutionCount = flower.evolutionCount,
                    hasVoiceClip = flower.voiceClip != null
                };
                print("오브젝트1 받");
                photonView.RPC("RPC_SyncInitialState", newPlayer, JsonUtility.ToJson(myState));
            }

            // 오브젝트 2의 상태 전송
            if (partnerFlower != null && partnerFlower.photonView.IsMine)
            {
                var partnerState = new FlowerFullState
                {
                    viewId = partnerFlower.photonView.ViewID,
                    state = partnerFlower.flower.curState,
                    name = partnerFlower.flower.nickName,
                    recordComplete = partnerFlower.isRecordComplete,
                    listenComplete = partnerFlower.isListenComplete,
                    evolutionCount = partnerFlower.flower.evolutionCount,
                    hasVoiceClip = partnerFlower.flower.voiceClip != null
                };
                print("오브젝트2 받");
                partnerFlower.photonView.RPC("RPC_SyncInitialState", newPlayer, JsonUtility.ToJson(partnerState));
            }
        }

        // 음성 데이터가 있다면 전송
        if (photonView.IsMine && flower.voiceClip != null && recorder != null)
        {
            byte[] voiceData = recorder.GetRecordedData();
            StartCoroutine(SendVoiceDataInChunks(voiceData));
        }

        if (partnerFlower != null && partnerFlower.photonView.IsMine &&
            partnerFlower.flower.voiceClip != null && partnerFlower.recorder != null)
        {
            byte[] voiceData = partnerFlower.recorder.GetRecordedData();
            partnerFlower.StartCoroutine(partnerFlower.SendVoiceDataInChunks(voiceData));
        }
    }

    [PunRPC]
    private void RPC_SyncInitialState(string stateJson)
    {
        var state = JsonUtility.FromJson<FlowerFullState>(stateJson);
        FlowerUIManager targetManager = null;

        // 해당하는 매니저 찾기
        if (photonView.ViewID == state.viewId)
        {
            targetManager = this;
        }
        else if (partnerFlower != null && partnerFlower.photonView.ViewID == state.viewId)
        {
            targetManager = partnerFlower;
        }

        // 찾은 매니저에 상태 적용
        if (targetManager != null)
        {
            targetManager.flower.curState = state.state;
            targetManager.flower.nickName = state.name;
            targetManager.isRecordComplete = state.recordComplete;
            targetManager.isListenComplete = state.listenComplete;
            targetManager.flower.evolutionCount = state.evolutionCount;
            targetManager.flower.curState = state.state;
            targetManager.nameInput.text = state.name;

            targetManager.UpdateUI(targetManager.flower);
            targetManager.UpdateUIText();
        }
        flowerEvol.CheckEvolutionCount(true);
    }

    // 전체 상태를 담는 클래스
    [System.Serializable]
    private class FlowerFullState
    {
        public int viewId;         // 해당 오브젝트의 ViewID
        public Flower.States state;
        public string name;
        public bool recordComplete;
        public bool listenComplete;
        public int evolutionCount;
        public bool hasVoiceClip;
    }

    private IEnumerator WaitForPartnerAndInitialize()
    {
        Debug.Log($"[WaitForPartnerAndInitialize] Starting for ViewID: {photonView.ViewID}");

        float timeout = 10f;
        float elapsed = 0f;

        // 파트너 오브젝트를 찾을 때까지 대기
        while (elapsed < timeout && partnerFlower == null)
        {
            Debug.Log($"[WaitForPartnerAndInitialize] Waiting for partner... Time: {elapsed}s, ViewID: {photonView.ViewID}");
            elapsed += 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        if (partnerFlower == null)
        {
            Debug.LogError($"[WaitForPartnerAndInitialize] Failed to find partner - ViewID: {photonView.ViewID}");
            yield break;
        }

        Debug.Log($"[WaitForPartnerAndInitialize] Partner found - MyID: {photonView.ViewID}, PartnerID: {partnerFlower.photonView.ViewID}");
        isInitialized = true;

        // 실제 API 호출
        StartCoroutine(GetVoiceStatus());
    }
    private void InitializeComponents()
    {
        sound = GameObject.Find("SMJ").GetComponent<HoonSoundManagerLogin>();
        flower = GetComponent<Flower>();
        recorder = GetComponent<VoiceRecorder>();
        flowerEvol = GetComponent<FlowerEvolution>();
        click = GetComponent<ClickFlower>();
        uiPopup = GetComponent<UIPopupAnimation>();
        uiPopup.SetTarget(uiPanel.GetComponent<RectTransform>());
        hoonUI = GameObject.Find("HoonLoobyCanvas");

        // 초기 상태 텍스트 설정
        UpdateStateText(flower.curState);
    }

    [PunRPC]
    private void RPC_SetPlayerToken(string token, int viewId)
    {
        if (photonView.ViewID == viewId)
        {
            Debug.Log($"[RPC_SetPlayerToken] Setting token for ViewID: {viewId}");
            playerToken = token;
        }
    }

    private void OnStateChanged(ClientState state, ClientState previousState)
    {
        if (state == ClientState.Disconnected)
        {
            Debug.Log("Disconnected from server. Attempting to reconnect...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void UpdateStateText(Flower.States state)
    {
        string statusMsg = "";
        switch (state)
        {
            case Flower.States.SEED:
                statusMsg = "상태: 작은 씨앗";
                break;
            case Flower.States.SPROUT:
                statusMsg = "상태: 아기 새싹";
                break;
            case Flower.States.BUD:
                statusMsg = "상태: 꽃봉오리";
                break;
            case Flower.States.BLOSSOM:
                statusMsg = "상태: 만개한 꽃";
                break;
        }
        statusText.text = statusMsg;
    }
    private IEnumerator InitialStateSync()
    {
        yield return new WaitForSeconds(0.5f);

        if (PhotonNetwork.IsMessageQueueRunning && photonView.IsMine)
        {
            Debug.Log($"Requesting initial state sync for ViewID: {photonView.ViewID}");
            photonView.RPC("RPC_RequestInitialState", RpcTarget.All);
        }
    }

    private void Update()
    {
        restTime = $"{dateChanger.timeUntilAvailable.Hours} : {dateChanger.timeUntilAvailable.Minutes}";
        UpdateUIText();
        UpdateAlertEmoji();
        failCount.text = $"{recordCount}/3";
        if (dateChanger.isNewDay && photonView.IsMine)
        {
            // 자정이 지났을 때 값 초기화
            recordCount = 0;
            isSuccess = false;
            successPanel.SetActive(false);

            // 초기화된 값 저장
            PlayerPrefs.SetInt($"IsSuccess_{photonView.ViewID}", isSuccess ? 1 : 0);
            PlayerPrefs.Save();

            // 실패 횟수 UI 업데이트
            if (failCount != null)
            {
                failCount.text = "0 / 3";
            }
        }
    }

    private void UpdateUIText()
    {
        if (dateChanger.UseFeature() == false && isListenComplete == true)
        {
            listenCompleteText.text = "연인의 말\n한마디 듣기\n" + restTime;
            if (buttons != null && buttons.Length > 2 && buttons[2] != null)
            {
                buttons[2].GetComponent<Button>().interactable = false;
            }
        }
        else if (dateChanger.UseFeature() == true && isRecordComplete == false)
        {
            completeText.text = "연인에게 따뜻한 한마디 말하기";
            if (buttons != null && buttons.Length > 2 && buttons[2] != null)
            {
                buttons[2].GetComponent<Button>().interactable = true;
            }
            listenCompleteText.text = "연인의 말한마디 듣기";
            isListenComplete = false;
        }
        if (isListenComplete == false)
        {
            listenCompleteText.text = "연인의 말한마디 듣기";
            buttons[2].GetComponent<Button>().interactable = true;
        }
    }

    private void UpdateAlertEmoji()
    {
        // click이나 flower가 null인 경우 체크 추가
        if (click == null || click.checkID == null || flower == null)
        {
            alertEmoji.SetActive(false);
            return;
        }

        bool isMyFlower = false;
        if (click.checkID != null)
        {
            isMyFlower = click.checkID.IsMine(flower);
        }
        bool hasRecording = flower.voiceClip != null || (recorder != null && recorder.HasRecording());

        if (dateChanger.UseFeature() == false && isListenComplete == true)
        {
            alertEmoji.SetActive(false);
        }
        else if (hasRecording && !isMyFlower && !isListenComplete)
        {
            alertEmoji.SetActive(true);
        }
        else
        {
            alertEmoji.SetActive(false);
        }
    }

    [PunRPC]
    private void RPC_RequestInitialState()
    {
        if (!photonView.IsMine || flower == null) return;

        Debug.Log($"Processing initial state sync for ViewID: {photonView.ViewID}");
        photonView.RPC("RPC_SyncFlowerState", RpcTarget.All,
        flower.curState,
        flower.nickName,
        isRecordComplete,
        isListenComplete,
        flower.evolutionCount,
        flower.voiceClip != null,
        photonView.ViewID);

        // 상태 텍스트도 동기화
        string statusMsg = "";
        switch (flower.curState)
        {
            case Flower.States.SEED:
                statusMsg = "상태: 작은 씨앗";
                break;
            case Flower.States.SPROUT:
                statusMsg = "상태: 아기 새싹";
                break;
            case Flower.States.BUD:
                statusMsg = "상태: 꽃봉오리";
                break;
            case Flower.States.BLOSSOM:
                statusMsg = "상태: 만개한 꽃";
                break;
        }
        photonView.RPC("RPC_UpdateStatusText", RpcTarget.All, statusMsg);
    }

    [PunRPC]
    private void RPC_UpdateStatusText(string statusMsg)
    {
        if (statusText != null)
        {
            statusText.text = statusMsg;
        }
    }
    [PunRPC]
    private void RPC_SyncVoiceClip(byte[] voiceData)
    {
        if (voiceData != null && voiceData.Length > 0)
        {
            recorder.SetRecordedData(voiceData);
            flower.voiceClip = recorder.GetAudioClip();
            UpdateAlertEmoji();
        }
    }

    [PunRPC]
    private void RPC_SyncFlowerState(Flower.States state, string name, bool recordComplete,
        bool listenComplete, int evolutionCount, bool hasRecording, int targetViewId)
    {
        // 현재 오브젝트의 ViewID와 일치할 때만 상태 업데이트
        if (photonView.ViewID == targetViewId)
        {
            if (flower == null || flowerEvol == null) return;

            flower.curState = state;
            flower.nickName = name;
            isRecordComplete = recordComplete;
            isListenComplete = listenComplete;
            flower.evolutionCount = evolutionCount;

            if (click.checkID != null)
            {
                if (flower.curState == Flower.States.BLOSSOM && isRecordComplete && click.checkID.IsMine(flower))
                {
                    SwapButtonUI(5);
                }
            }

            bool isInitialSync = Time.timeSinceLevelLoad < 1f;
            //flowerEvol.CheckEvolutionCount(isInitialSync);
            
            UpdateUI(flower);
            UpdateUIText();
        }
    }
    [PunRPC]
    private void RPC_InitialEvolutionCheck()
    {
        // 씬이 시작될 때 모든 클라이언트에서 진화 체크 (isFirst = true)
        if (flowerEvol != null)
        {
            flowerEvol.CheckEvolutionCount(true);
        }
        else
        {
            Debug.LogError("FlowerEvolution is null in RPC_InitialEvolutionCheck");
        }
    }
    private IEnumerator DelayedUIUpdate()
    {
        yield return new WaitForSeconds(1f);

        // null 체크 추가
        if (flower == null || click == null || click.checkID == null) yield break;

        // UI 업데이트 전에 모든 컴포넌트 null 체크
        if (isActiveAndEnabled)  // 스크립트가 활성화된 상태인지 확인
        {
            if (click.checkID.IsMine(flower))
            {
                if (flower.curState == Flower.States.BLOSSOM && isRecordComplete)
                {
                    SwapButtonUI(5);
                }
            }

            UpdateUI(flower);
            UpdateUIText();
        }
    }

    [PunRPC]
    private void RPC_UpdateEvolutionCount(int count)
    {
        flower.evolutionCount = count;
        flowerEvol.CheckEvolutionCount(false);

        // 진화 후 상태 확인하여 UI 업데이트
        if (flower.curState == Flower.States.BLOSSOM && isRecordComplete && click.checkID.IsMine(flower))
        {
            SwapButtonUI(5);
        }
    }


    [PunRPC]
    private void RPC_UpdateFlowerName(string newName, int targetViewId)
    {
        // 현재 오브젝트의 ViewID와 일치할 때만 이름 업데이트
        if (photonView.ViewID == targetViewId)
        {
            flower.nickName = newName;
            nameInput.text = newName;
        }
    }

    [PunRPC]
    private void RPC_UpdateRecordStatus(bool recordComplete, bool listenComplete)
    {
        isRecordComplete = recordComplete;
        isListenComplete = listenComplete;

        if (click.checkID == null)
        {
            
        }
        // 녹음 완료 시 상태 다시 체크
        else if (isRecordComplete && flower.curState == Flower.States.BLOSSOM && click.checkID.IsMine(flower))
        {
            SwapButtonUI(5);
        }

        UpdateUI(flower);
        UpdateUIText();
    }

    [PunRPC]
    private void RPC_NotifyRecordComplete(byte[] voiceData)
    {
        if (!click.checkID.IsMine(flower))
        {
            if (voiceData != null && voiceData.Length > 0)
            {
                recorder.SetRecordedData(voiceData);
                flower.voiceClip = recorder.GetAudioClip();
            }

            if (flower.curState == Flower.States.BLOSSOM)
            {
                SwapButtonUI(2);  // 듣기 버튼
            }

            isRecordComplete = true;
            UpdateUIText();
            UpdateAlertEmoji();
        }
    }


    [PunRPC]
    private void RPC_ShowAlertEmoji(bool show)
    {
        alertEmoji.SetActive(show);
    }

    [PunRPC]
    private void RPC_UpdateUI(Flower.States state, string statusMsg, bool hasRecording)
    {
        statusText.text = statusMsg;

        bool isMyFlower = false;
        if (click.checkID != null)
        {
            isMyFlower = click.checkID.IsMine(flower);
        }

        if (state == Flower.States.BLOSSOM)
        {
            if (isMyFlower)
            {
                if (isRecordComplete)
                {
                    SwapButtonUI(5);  // 내 꽃이고 녹음이 완료된 경우에만 새 꽃 심기 버튼
                }
            }
            else
            {
                // 상대방 꽃은 항상 듣기 버튼만 표시
                if (hasRecording && !isListenComplete)
                {
                    SwapButtonUI(2);
                }
                else if (isListenComplete)
                {
                    SwapButtonUI(2);
                }
            }
        }
    }
    public void ShowFlowerInfo(Flower targetFlower, int idx)
    {
        if (hoonUI.activeInHierarchy == true)
        {
            hoonUI.SetActive(false);
        }
        
        // 현재 선택된 flower를 저장
        flower = targetFlower;
        
        if (click.isFirstClick == true)
        {
            click.mainCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player_CheckFlower"));
            uiPopup.PlayPopupAnimation(uiPanel.GetComponent<RectTransform>());
        }
        if (targetFlower == null)
        {
            return;
        }
        bool isMyFlower = false;
        if (click.checkID != null)
        {
            isMyFlower = click.checkID.IsMine(targetFlower);
        }
        UpdateUI(targetFlower);
        uiPanel.SetActive(true);

        // 먼저 진화 상태와 녹음 상태를 확인
        if (isMyFlower && targetFlower.curState == Flower.States.BLOSSOM && isRecordComplete)
        {
            sound.PlaySound("smjAudioClopAttay", 0);
            SwapButtonUI(5);  // 새 꽃 심기 버튼
            return;  // 여기서 종료
        }

        // 다른 상태들 처리
        if (isMyFlower)
        {
            if (isRecordComplete == true)
            {
                print("녹음완! 3번!");
                sound.PlaySound("smjAudioClopAttay", 0);
                SwapButtonUI(3);
            }
            else if (isRecordComplete == false || isListenComplete == true)
            {
                sound.PlaySound("smjAudioClopAttay", 0);
                SwapButtonUI(idx);
            }
            else
            {
                print("녹음완X! 3번!");
                sound.PlaySound("smjAudioClopAttay", 0);
                SwapButtonUI(3);
            }
        }
        else
        {
            if (isListenComplete == true && isRecordComplete == true)
            {
                sound.PlaySound("smjAudioClopAttay", 0);
                SwapButtonUI(2);
            }
            else
            {
                sound.PlaySound("smjAudioClopAttay", 0);
                SwapButtonUI(idx);
            }
        }
    }

    public void HideFlowerInfo()
    {
        click.mainCam.cullingMask |= (1 << LayerMask.NameToLayer("Player_CheckFlower"));
        hoonUI.SetActive(true);
        uiPopup.Hide(uiPanel.GetComponent<RectTransform>());
        //uiPanel.SetActive(false);
        //uiPopup.Hide(recordPanel.GetComponent<RectTransform>());
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
        recordPanel.SetActive(false);
    }

    public void SwapButtonUI(int onIdx)
    {
        //sound.PlaySound("smjAudioClopAttay", 0);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
        buttons[onIdx].SetActive(true);
    }

    public void UpdateUI(Flower flower)
    {
        if (flower == null) return;

        // 상태 텍스트 업데이트
        UpdateStateText(flower.curState);

        if (photonView.IsMine)
        {
            bool hasRecording = flower.voiceClip != null;
            photonView.RPC("RPC_UpdateUI", RpcTarget.All, flower.curState, statusText.text, hasRecording);
        }
    }

    public void UpdateButtonInteractable(bool isInteractable, int idx)
    {
        if (!click.checkID.IsMine(flower)) return;
        buttons[idx].GetComponent<Button>().interactable = isInteractable;
    }

    public void OnCloseButtonClick()
    {
        sound.PlaySound("smjAudioClopAttay", 1);
        click.particle.EnableChecking();
        HideFlowerInfo();
        click.ReturnCamera();
    }

    public void OnTalkButtonClick()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        if (click.checkID.IsMine(flower))
        {
            recordPanel.SetActive(true);
        }
    }

    public void OffPanel()
    {
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnRecordingButtonClick(float second)
    {
        //if (!click.checkID.IsMine(flower)) return;
        sound.PlaySound("smjAudioClopAttay", 0);
        exitButton.SetActive(false);
        OffPanel();
        recordButtons[1].SetActive(true);
        recordingCor = StartCoroutine(RecordingVoice(second));
    }

    IEnumerator RecordingVoice(float second)
    {
        recorder.StartRecording();
        yield return new WaitForSeconds(second);
        OffPanel();
        recordButtons[2].SetActive(true);
    }
    public void SubmitRecord_Temp()
    {
        if (!click.checkID.IsMine(flower)) return;

        // 기존의 테스트 모드일 경우
        if (testRecord == true)
        {
            OffPanel();
            recordButtons[2].SetActive(false);

            // 서버 검증 시작
            StartCoroutine(ValidateAndTransferVoice());
        }
        else
        {
            testRecord = true;
            recordCount++;
            if (recordCount < 3)
            {
                OffPanel();
                recordButtons[2].SetActive(false);
                recordButtons[3].SetActive(true);
            }
            else
            {
                OffPanel();
                recordButtons[2].SetActive(false);
                recordButtons[5].SetActive(true);
                recordCount = 0;
            }
        }
    }
    public void SubmitRecord()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        if (!click.checkID.IsMine(flower)) return;

        OffPanel();
        recordButtons[2].SetActive(false);
        loadingObj.SetActive(true);
        StartCoroutine(ValidateAndTransferVoice());
    }
    private IEnumerator WaitForConnectionAndSendVoice(byte[] voiceData)
    {
        // 연결될 때까지 대기
        while (!PhotonNetwork.IsConnected || !PhotonNetwork.InRoom)
        {
            yield return new WaitForSeconds(0.5f);
        }

        StartCoroutine(SendVoiceDataInChunks(voiceData));
    }

    [System.Serializable]
    private class VoiceValidationResponse
    {
        public string mood;      // "긍정", "중립", "부정" 중 하나
        public string nickname;
    }

    private IEnumerator ValidateAndTransferVoice()
    {
        // 1. 오디오 데이터 가져오기
        byte[] audioData = recorder.GetRecordedData();
        if (audioData == null)
        {
            Debug.LogError("Failed to get audio data");
            recordButtons[5].SetActive(true);
            yield break;
        }

        // 2. 임시 WAV 파일로 저장하고 다시 읽기
        string tempPath = Path.Combine(Application.temporaryCachePath, "temp_voice.wav");
        try
        {
            File.WriteAllBytes(tempPath, audioData);
            byte[] fileData = File.ReadAllBytes(tempPath);

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>
            {
                new MultipartFormFileSection("voice", audioData, "audio.wav", "audio/wav")
            };

            NetworkManager.Instance.Initialize("http://125.132.216.190:12223", playerToken);
            yield return NetworkManager.Instance.PostMultipartData("/api/flower/analyze-mood", formData,
                (success, response) =>
                {
                    if (success)
                    {
                        loadingObj.SetActive(false);
                        try
                        {
                            var validationResponse = JsonUtility.FromJson<VoiceValidationResponse>(response);
                            if (validationResponse.mood != null)
                            {
                                if (validationResponse.mood == "부정")
                                {
                                    recordCount++;
                                    failCount.text = $"{recordCount}/3";
                                    print(recordCount);
                                    if (recordCount < 3)
                                    {
                                        sound.PlaySound("smjAudioClopAttay", 6);
                                        OffPanel();
                                        recordButtons[3].SetActive(true); // 재녹음 버튼
                                    }
                                    else
                                    {
                                        sound.PlaySound("smjAudioClopAttay", 6);
                                        OffPanel();
                                        recordButtons[5].SetActive(true); // 최종 실패 UI
                                    }
                                }
                                else // "긍정" 또는 "중립"
                                {
                                    sound.PlaySound("smjAudioClopAttay", 5);
                                    isSuccess = true;
                                    PlayerPrefs.SetInt($"IsSuccess_{photonView.ViewID}", 1);
                                    PlayerPrefs.Save();
                                    successPanel.SetActive(true);
                                    recordButtons[4].SetActive(true); // 성공 UI
                                    flower.evolutionCount++;
                                    photonView.RPC("RPC_UpdateEvolutionCount", RpcTarget.All, flower.evolutionCount);
                                    StartCoroutine(WaitForConnectionAndSendVoice(audioData));
                                    // 녹음 성공 시 상태 업데이트
                                    StartCoroutine(GetVoiceStatus());
                                }
                            }
                            else
                            {
                                print("답변 없음!");
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Error parsing response: {e.Message}");
                            recordButtons[5].SetActive(true);
                        }
                    }
                    else
                    {
                        Debug.LogError($"Server request failed: {response}");
                        //recordButtons[5].SetActive(true);
                    }
                });
        }
        finally
        {
            // 임시 파일 삭제
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private void OnEnable()
    {
        if (isSuccess == true)
        {
            successPanel.SetActive(true);
        }
        else
        {
            successPanel.SetActive(false);
        }
    }

    private IEnumerator SendVoiceDataInChunks(byte[] voiceData)
    {
        const int CHUNK_SIZE = 16384; // 16KB
        int chunks = Mathf.CeilToInt(voiceData.Length / (float)CHUNK_SIZE);

        photonView.RPC("RPC_InitializeVoiceTransfer", RpcTarget.All, chunks);

        for (int i = 0; i < chunks; i++)
        {
            int size = Mathf.Min(CHUNK_SIZE, voiceData.Length - i * CHUNK_SIZE);
            byte[] chunk = new byte[size];
            Array.Copy(voiceData, i * CHUNK_SIZE, chunk, 0, size);

            photonView.RPC("RPC_ReceiveVoiceChunk", RpcTarget.All, chunk, i);

            yield return new WaitForSeconds(0.1f);
        }

        photonView.RPC("RPC_FinalizeVoiceTransfer", RpcTarget.All);
    }

    [PunRPC]
    private void RPC_InitializeVoiceTransfer(int totalChunks)
    {
        voiceDataChunks = new List<byte[]>(totalChunks);
        for (int i = 0; i < totalChunks; i++)
        {
            voiceDataChunks.Add(null);
        }
    }

    [PunRPC]
    private void RPC_ReceiveVoiceChunk(byte[] chunk, int index)
    {
        if (index < voiceDataChunks.Count)
        {
            voiceDataChunks[index] = chunk;
        }
    }

    [PunRPC]
    private void RPC_FinalizeVoiceTransfer()
    {
        try
        {
            if (voiceDataChunks == null || voiceDataChunks.Count == 0)
            {
                Debug.LogError("No voice data chunks available");
                return;
            }

            // 전체 데이터 크기 계산
            int totalSize = voiceDataChunks.Sum(chunk => chunk?.Length ?? 0);
            if (totalSize == 0)
            {
                Debug.LogError("Total voice data size is 0");
                return;
            }

            byte[] completeVoiceData = new byte[totalSize];
            int offset = 0;

            foreach (byte[] chunk in voiceDataChunks.Where(c => c != null))
            {
                if (offset + chunk.Length <= completeVoiceData.Length)
                {
                    Buffer.BlockCopy(chunk, 0, completeVoiceData, offset, chunk.Length);
                    offset += chunk.Length;
                }
            }

            if (recorder.SetRawAudioData(completeVoiceData))
            {
                if (!click.checkID.IsMine(flower))
                {
                    if (flower.curState == Flower.States.BLOSSOM)
                    {
                        SwapButtonUI(2);
                    }
                    isRecordComplete = true;
                    UpdateUIText();
                    UpdateAlertEmoji();
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in RPC_FinalizeVoiceTransfer: {e.Message}");
        }
        finally
        {
            voiceDataChunks?.Clear();
            voiceDataChunks = null;
        }
    }

    public void OnRecordCompleteToday()
    {
        if (!click.checkID.IsMine(flower)) return;

        byte[] voiceData = recorder.GetRecordedData();
        photonView.RPC("RPC_SyncVoiceClip", RpcTarget.All, voiceData);

        recordPanel.SetActive(false);
        exitButton.SetActive(true);
        isRecordComplete = true;
        ShowFlowerInfo(flower, 3);
        //SwapButtonUI(3);
        UpdateUI(flower);
        OffPanel();
    }

    public void OnReRecordingClick()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        if (!click.checkID.IsMine(flower)) return;

        OffPanel();
    }

    public void OnStopRecordingButtonClick()
    {
        sound.PlaySound("smjAudioClopAttay", 1);
        if (!click.checkID.IsMine(flower)) return;

        StopCoroutine(recordingCor);
        recorder.StopRecording();
        exitButton.SetActive(true);
        OffPanel();
        recordButtons[2].SetActive(true);
    }

    //public void OnListenVoiceButtonClick()
    //{
    //    //if (click == null || click.checkID == null || flower == null) return;
    //    //if (click.checkID.IsMine(flower)) return;
    //    if (click == null || flower == null) return;

    //    SwapButtonUI(4);  // 재생 중 UI
    //    recorder.PlayRecording();
    //    StartCoroutine(CheckAudioCompletion());
    //}
    public void OnListenVoiceButtonClick()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        if (click == null || flower == null) return;

        SwapButtonUI(4);  // 재생 중 UI

        // 제출 전이라면 로컬 녹음 재생
        if (!isRecordComplete)
        {
            recorder.PlayRecording();
            StartCoroutine(CheckAudioCompletion());
        }
        // 제출된 상태라면 서버에서 받아와서 재생
        else
        {
            StartCoroutine(GetAndPlayVoiceMessage());
        }
    }

    [System.Serializable]
    public class VoiceStatus
    {
        public bool partnerRecordComplete;
        public bool partnerListenComplete;
        public DateTime partnerSavedAt;
        public DateTime partnerListenedAt;
        public int partnerMoodCount;
        public string partnerFlowerName;
        public bool myRecordComplete;
        public bool myListenComplete;
        public DateTime mySavedAt;
        public DateTime myListenedAt;
        public int myMoodCount;
        public string myFlowerName;
    }
    
    private IEnumerator GetVoiceStatus()
    {
        Debug.Log($"{gameObject.name} > [GetVoiceStatus] Starting for ViewID: {photonView.ViewID}");
        if (string.IsNullOrEmpty(playerToken))
        {
            Debug.LogError($"[GetVoiceStatus] Token is empty for ViewID: {photonView.ViewID}");
            yield break;
        }
        if (!photonView.IsMine)
        {
            Debug.Log($"[GetVoiceStatus] Skipping - Not owner of ViewID: {photonView.ViewID}");
            yield break;
        }
        
        if (flower.managerId != "Male")
        {
            yield return new WaitForSeconds(0.5f);
        }
        if (click.checkID == null)
        {
            click.CheckForPlayer();
            yield return new WaitForSeconds(0.6f);
        }
        //print("누구? : " + gameObject.name + ", 내꺼니? : " + click.checkID.IsMine(flower));
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", playerToken);
        yield return NetworkManager.Instance.Get<VoiceStatus>("api/flower/voice/status",
            (success, response) =>
            {
                if (success && response != null)
                {
                    print("누구? : " + gameObject.name + ", 내꺼니? : " + click.checkID.IsMine(flower));
                    bool isFirstSync = Time.timeSinceLevelLoad < 1f;
                    FlowerUIManager[] flowers = GameObject.FindObjectsOfType<FlowerUIManager>();

                    // API를 호출한 오브젝트에서 모든 꽃의 정보를 업데이트
                    foreach (var f in flowers)
                    {
                        bool isTokenOwner = f.click.checkID != null && f.click.checkID.IsMine(f.flower);

                        if (isTokenOwner)
                        {
                            // 토큰 소유자의 꽃이면 my 정보 사용
                            f.isRecordComplete = response.myRecordComplete;
                            f.isListenComplete = response.myListenComplete;
                            f.flower.evolutionCount = response.myMoodCount;
                            f.flower.nickName = response.myFlowerName;
                            f.nameInput.text = response.myFlowerName;
                            print($"[{gameObject.name}] 토큰 소유자의 꽃({f.gameObject.name})에 my 정보 적용");
                        }
                        else
                        {
                            // 토큰 소유자가 아닌 꽃이면 partner 정보 사용
                            f.isRecordComplete = response.partnerRecordComplete;
                            f.isListenComplete = response.partnerListenComplete;
                            f.flower.evolutionCount = response.partnerMoodCount;
                            f.flower.nickName = response.partnerFlowerName;
                            f.nameInput.text = response.partnerFlowerName;
                            print($"[{gameObject.name}] 파트너의 꽃({f.gameObject.name})에 partner 정보 적용");
                        }

                        f.photonView.RPC("RPC_SyncFlowerState", RpcTarget.AllBuffered,
                            f.flower.curState,
                            f.flower.nickName,
                            f.isRecordComplete,
                            f.isListenComplete,
                            f.flower.evolutionCount,
                            f.flower.voiceClip != null,
                            f.photonView.ViewID);

                        f.flowerEvol.CheckEvolutionCount(isFirstSync);
                    }

                    UpdateUI(flower);
                    UpdateUIText();
                }
                else
                {
                    if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
                    {
                        Debug.LogError($"[GetVoiceStatus] API call failed for ViewID: {photonView.ViewID}");
                    }
                }
            });
    }

    [PunRPC]
    private void RPC_SyncStatesFromServer(string responseJson)
    {
        var response = JsonUtility.FromJson<VoiceStatus>(responseJson);
        UpdateStatesFromResponse(response);
    }

    private void UpdateStatesFromResponse(VoiceStatus response)
    {
        if (photonView.IsMine)
        {
            // 자신의 상태 업데이트
            isRecordComplete = response.myRecordComplete;
            isListenComplete = response.myListenComplete;
            flower.evolutionCount = response.myMoodCount;
            flower.nickName = response.myFlowerName;
            nameInput.text = response.myFlowerName;

            // 파트너의 상태는 파트너 오브젝트가 직접 업데이트하도록 함
            if (partnerFlower != null)
            {
                partnerFlower.photonView.RPC("RPC_SyncFlowerState", RpcTarget.All,
                    partnerFlower.flower.curState,
                    response.partnerFlowerName,
                    response.partnerRecordComplete,
                    response.partnerListenComplete,
                    response.partnerMoodCount,
                    partnerFlower.flower.voiceClip != null,
                    partnerFlower.photonView.ViewID);
            }

            UpdateUI(flower);
            UpdateUIText();
        }
    }


    private IEnumerator GetAndPlayVoiceMessage()
    {
        // 토큰이 없는 경우 처리
        if (string.IsNullOrEmpty(playerToken))
        {
            playerToken = PlayerPrefs.GetString("token");
            if (string.IsNullOrEmpty(playerToken))
            {
                Debug.LogError("No authentication token available");
                buttons[4].SetActive(false);
                yield break;
            }
        }

        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", playerToken);
        bool isPlaying = false;

        // 요청 전 토큰 로깅
        Debug.Log($"Using token for voice request: {playerToken}");

        yield return NetworkManager.Instance.GetWithoutBody("api/flower/voice",
            (success, response) =>
            {
                if (success)
                {
                    try
                    {
                        Debug.Log($"Successful voice response: {response}");
                        recorder.PlayStreamingAudio(response);
                        isPlaying = true;
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error playing voice: {e.Message}");
                        buttons[4].SetActive(false);
                    }
                }
                else
                {
                    Debug.LogError($"Failed to get voice URL. Response code: {response}");
                    if (response.Contains("403"))
                    {
                        Debug.LogError("Authentication failed - please check token validity");
                    }
                    buttons[4].SetActive(false);
                }
            });

        if (isPlaying)
        {
            StartCoroutine(CheckAudioCompletion());
        }
    }

    [System.Serializable]
    private class VoiceUrlResponse
    {
        public string url;
    }

    private IEnumerator CheckAudioCompletion()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        bool isMyFlower = false;
        if (click.checkID != null)
        {
            isMyFlower = click.checkID.IsMine(flower);
        }
        if (!isMyFlower)
        {
            isListenComplete = true;
            SwapButtonUI(2);  // 상대방 꽃은 항상 듣기 버튼으로
            UpdateUIText();
            photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, isRecordComplete, true);

            // 듣기 완료 시 상태 업데이트
            StartCoroutine(GetVoiceStatus());
        }
        buttons[4].SetActive(false);
    }

    [System.Serializable]
    public class NickNamePost
    {
        public string name;
    }
    private IEnumerator PostNickName(NickNamePost name, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", playerToken);

        yield return NetworkManager.Instance.Post($"/api/flower/set-name", name,
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("NickName fix successfully");
                    //nameInput.text = name.name;
                    flower.nickName = name.name;
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to fix NickName: {response}");
                }
            });
    }

    public void UpdateName(Flower flower)
    {
        // 디버깅을 위한 로그 추가
        Debug.Log($"Click: {click}, CheckID: {click?.checkID}, Flower: {flower}");

        if (click?.checkID == null)
        {
            Debug.LogError($"CheckID is null on GameObject: {gameObject.name}");
            Debug.LogError($"PlayerWoman reference exists: {click?.checkID != null}");
            return;
        }

        if (!click.checkID.IsMine(flower)) return;

        var newName = new NickNamePost
        {
            name = nameInput.text
        };

        photonView.RPC("RPC_UpdateFlowerName", RpcTarget.All, nameInput.text, photonView.ViewID);
        StartCoroutine(PostNickName(newName, null));
    }

    public void OnClickNewFlower()
    {
        recordCount = 0;
        isSuccess = false;
        successPanel.SetActive(false);
        
        if (!click.checkID.IsMine(flower)) return;
        sound.PlaySound("smjAudioClopAttay", 0);
        photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, false, false);
        click.checkID.ResetFirst();

        // 모든 클라이언트에서 꽃 초기화 실행
        photonView.RPC("RPC_ResetFlower", RpcTarget.All);

        flower.ResetFlower();
        OnCloseButtonClick();
        StartCoroutine(Delay(0.5f));
    }

    IEnumerator Delay(float sec)
    {
        yield return new WaitForSeconds(sec);
        //Vector3 worldPosition = coinStartVecObj.transform.position;
        Vector3 worldPosition = gameObject.transform.position;
        coinEffect.PlayCoinEffect(worldPosition);
        sound.PlaySound("smjAudioClopAttay", 4);
    }

    [PunRPC]
    private void RPC_ResetFlower()
    {
        flowerEvol.NewFlower();
    }
    public void OnEvolutionComplete(Flower.States state)
    {
        if (flower == null || click == null || click.checkID == null) return;

        if (click.checkID.IsMine(flower))
        {
            if (state == Flower.States.BLOSSOM && isRecordComplete)
            {
                SwapButtonUI(5);  // 새 꽃 심기 버튼으로 변경
            }
        }

        UpdateUI(flower);
        UpdateUIText();
    }

    private void OnDestroy()
    {
        if (photonView.IsMine)
        {
            PlayerPrefs.SetInt($"IsSuccess_{photonView.ViewID}", isSuccess ? 1 : 0);
            PlayerPrefs.Save();
        }

        PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
    }
}