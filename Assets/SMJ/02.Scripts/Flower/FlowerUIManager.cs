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
using static UnityEngine.CullingGroup;
using Unity.VisualScripting;

public class FlowerUIManager : MonoBehaviourPun
{
    public MidnightChecker dateChanger;

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

    [SerializeField] private bool testRecord = false;
    [SerializeField] private GameObject hoonUI;

    private UIPopupAnimation uiPopup;

    private VoiceRecorder recorder;
    private FlowerEvolution flowerEvol;
    private Flower flower;
    private ClickFlower click;

    private int recordCount = 0;

    public bool isRecordComplete = false;
    public bool isListenComplete = false;

    private string restTime = string.Empty;
    Coroutine recordingCor;
    private const int CHUNK_SIZE = 5000;
    private List<byte[]> voiceDataChunks = new List<byte[]>();

    private HoonSoundManagerLogin sound;

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 30;
        PhotonNetwork.SerializationRate = 15;

        // 타임아웃 값을 더 길게 설정
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.DisconnectTimeout = 120000; // 120초
        PhotonNetwork.NetworkingClient.LoadBalancingPeer.TimePingInterval = 2000;
    }
    private void Start()
    {
        sound = GameObject.Find("HoonLobyCanvas").GetComponent<HoonSoundManagerLogin>();
        SendOptions sendOptions = new SendOptions();
        sendOptions.Reliability = true; // 신뢰성 있는 전송
        sendOptions.Channel = 0; // 채널 설정
        flower = GetComponent<Flower>();
        recorder = GetComponent<VoiceRecorder>();
        flowerEvol = GetComponent<FlowerEvolution>();
        click = GetComponent<ClickFlower>();
        uiPopup = GetComponent<UIPopupAnimation>();
        uiPopup.SetTarget(uiPanel.GetComponent<RectTransform>());

        hoonUI = GameObject.Find("HoonLoobyCanvas");

        // 초기 상태 텍스트 설정
        UpdateStateText(flower.curState);
        StartCoroutine(InitialStateSync());

        PhotonNetwork.NetworkingClient.StateChanged += OnStateChanged;
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
        if (PhotonNetwork.IsMessageQueueRunning)
        {
            photonView.RPC("RPC_RequestInitialState", RpcTarget.All);
        }
    }

    private void Update()
    {
        restTime = $"{dateChanger.timeUntilAvailable.Hours} : {dateChanger.timeUntilAvailable.Minutes}";
        UpdateUIText();
        UpdateAlertEmoji();
    }

    private void UpdateUIText()
    {
        if (dateChanger.UseFeature() == false && isListenComplete == true)
        {
            listenCompleteText.text = "연인의 말한마디 듣기\n" + restTime;
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
    public void OnClickTest()
    {
        if (!click.checkID.IsMine(flower)) return;

        if (testRecord == true)
        {
            testRecord = false;
            print("실패!");
        }
        else
        {
            testRecord = true;
            print("성공!");
        }
    }

    [PunRPC]
    private void RPC_RequestInitialState()
    {
        if (photonView.IsMine && flower != null)
        {
            photonView.RPC("RPC_SyncFlowerState", RpcTarget.All,
                flower.curState,
                flower.nickName,
                isRecordComplete,
                isListenComplete,
                flower.evolutionCount,
                flower.voiceClip != null);

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
    private void RPC_SyncFlowerState(Flower.States state, string name, bool recordComplete, bool listenComplete, int evolutionCount, bool hasRecording)
    {
        if (flower == null || flowerEvol == null) return;

        flower.curState = state;
        flower.nickName = name;
        isRecordComplete = recordComplete;
        isListenComplete = listenComplete;
        flower.evolutionCount = evolutionCount;

        if (click.checkID != null)
        {
            // 상태 동기화 후 UI 업데이트
            if (flower.curState == Flower.States.BLOSSOM && isRecordComplete && click.checkID.IsMine(flower))
            {
                SwapButtonUI(5);
            }
        }

        flowerEvol.CheckEvolutionCount();
        UpdateUI(flower);
        UpdateUIText();
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
        flowerEvol.CheckEvolutionCount();

        // 진화 후 상태 확인하여 UI 업데이트
        if (flower.curState == Flower.States.BLOSSOM && isRecordComplete && click.checkID.IsMine(flower))
        {
            SwapButtonUI(5);
        }
    }


    [PunRPC]
    private void RPC_UpdateFlowerName(string newName)
    {
        flower.nickName = newName;
        nameInput.text = newName;
    }

    [PunRPC]
    private void RPC_UpdateRecordStatus(bool recordComplete, bool listenComplete)
    {
        isRecordComplete = recordComplete;
        isListenComplete = listenComplete;

        // 녹음 완료 시 상태 다시 체크
        if (isRecordComplete && flower.curState == Flower.States.BLOSSOM && click.checkID.IsMine(flower))
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
        if (click.checkID != null) {
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
        hoonUI.SetActive(false);
        if (click.isFirstClick == true)
        {
            Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Player_CheckFlower"));
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
            sound.PlaySound("smjAudioClopAttay", 1);
            SwapButtonUI(5);  // 새 꽃 심기 버튼
            return;  // 여기서 종료
        }
        
        // 다른 상태들 처리
        if (isMyFlower)
        {
            if (isRecordComplete == false || isListenComplete == true)
            {
                sound.PlaySound("smjAudioClopAttay", 1);
                SwapButtonUI(idx);
            }
            else
            {
                sound.PlaySound("smjAudioClopAttay", 1);
                SwapButtonUI(3);
            }
        }
        else
        {
            if (targetFlower.voiceClip != null && !isListenComplete)
            {
                sound.PlaySound("smjAudioClopAttay", 1);
                SwapButtonUI(2);
            }
            else
            {
                sound.PlaySound("smjAudioClopAttay", 1);
                SwapButtonUI(idx);
            }
        }
    }

    public void HideFlowerInfo()
    {
        Camera.main.cullingMask |= (1 << LayerMask.NameToLayer("Player_CheckFlower"));
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
        sound.PlaySound("smjAudioClopAttay", 2);
        HideFlowerInfo();
        click.ReturnCamera();
    }

    public void OnTalkButtonClick()
    {
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
    public void SubmitRecord()
    {
        if (!click.checkID.IsMine(flower)) return;

        if (testRecord == true)
        {
            OffPanel();
            recordButtons[2].SetActive(false);
            recordButtons[4].SetActive(true);

            flower.evolutionCount++;
            photonView.RPC("RPC_UpdateEvolutionCount", RpcTarget.All, flower.evolutionCount);

            // 음성 데이터를 청크로 나눠서 전송
            byte[] voiceData = recorder.GetRecordedData();
            StartCoroutine(WaitForConnectionAndSendVoice(voiceData));
        }
        else
        {
            recordCount++;
            if (recordCount < 3)
            {
                OffPanel();
                recordButtons[2].SetActive(false);
                recordButtons[3].SetActive(true);
                //photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, false, isListenComplete);
            }
            else
            {
                OffPanel();
                recordButtons[2].SetActive(false);
                recordButtons[5].SetActive(true);
                recordCount = 0;

                //// 진화 카운트 증가
                //flower.evolutionCount++;
                //photonView.RPC("RPC_UpdateEvolutionCount", RpcTarget.All, flower.evolutionCount);

                //// 음성 데이터를 청크로 나눠서 전송
                //byte[] voiceData = recorder.GetRecordedData();
                //StartCoroutine(SendVoiceDataInChunks(voiceData));
            }
        }
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
    private IEnumerator SendVoiceDataInChunks(byte[] voiceData)
    {
        // 최대 재시도 횟수 설정
        int maxRetries = 3;
        int currentRetry = 0;

        // 초기 연결 대기
        float connectionTimeout = 5f;  // 5초 타임아웃
        float timer = 0f;

        while (!PhotonNetwork.IsConnectedAndReady && timer < connectionTimeout)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom)
        {
            Debug.LogError("Failed to connect to network!");
            yield break;
        }

        int chunks = Mathf.CeilToInt(voiceData.Length / (float)CHUNK_SIZE);

        // 초기화 RPC
        bool initSuccess = false;
        try
        {
            photonView.RPC("RPC_InitializeVoiceTransfer", RpcTarget.All, chunks);
            initSuccess = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize transfer: {e.Message}");
        }

        if (!initSuccess)
        {
            yield break;
        }

        yield return new WaitForSeconds(0.2f);

        // 청크 전송
        for (int i = 0; i < chunks; i++)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                Debug.LogError("Lost connection during transfer!");
                yield break;
            }

            int size = Mathf.Min(CHUNK_SIZE, voiceData.Length - i * CHUNK_SIZE);
            byte[] chunk = new byte[size];
            Array.Copy(voiceData, i * CHUNK_SIZE, chunk, 0, size);

            bool chunkSent = false;
            while (!chunkSent && currentRetry < maxRetries)
            {
                try
                {
                    photonView.RPC("RPC_ReceiveVoiceChunk", RpcTarget.All, chunk, i);
                    chunkSent = true;
                }
                catch (Exception e)
                {
                    currentRetry++;
                    Debug.LogWarning($"Retry {currentRetry}/{maxRetries} for chunk {i}: {e.Message}");
                }

                if (!chunkSent)
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }

            if (!chunkSent)
            {
                Debug.LogError($"Failed to send chunk {i} after {maxRetries} retries");
                yield break;
            }

            yield return new WaitForSeconds(0.2f);
        }

        // 전송 완료 처리
        if (PhotonNetwork.IsConnectedAndReady)
        {
            try
            {
                photonView.RPC("RPC_FinalizeVoiceTransfer", RpcTarget.All);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to finalize transfer: {e.Message}");
            }
        }
    }

    [PunRPC]
    private void RPC_InitializeVoiceTransfer(int totalChunks)
    {
        // 새로운 보이스 데이터를 받기 위한 초기화
        voiceDataChunks = new List<byte[]>(totalChunks);
    }

    [PunRPC]
    private void RPC_ReceiveVoiceChunk(byte[] chunk, int index)
    {
        // 청크를 순서대로 저장
        while (voiceDataChunks.Count <= index)
        {
            voiceDataChunks.Add(null);
        }
        voiceDataChunks[index] = chunk;
    }

    [PunRPC]
    private void RPC_FinalizeVoiceTransfer()
    {
        // 모든 청크를 하나의 배열로 합치기
        int totalSize = voiceDataChunks.Sum(chunk => chunk.Length);
        byte[] completeVoiceData = new byte[totalSize];

        int currentPosition = 0;
        foreach (byte[] chunk in voiceDataChunks)
        {
            Array.Copy(chunk, 0, completeVoiceData, currentPosition, chunk.Length);
            currentPosition += chunk.Length;
        }

        // 완성된 음성 데이터 처리
        recorder.SetRecordedData(completeVoiceData);
        flower.voiceClip = recorder.GetAudioClip();

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
        if (!click.checkID.IsMine(flower)) return;

        OffPanel();
    }

    public void OnStopRecordingButtonClick()
    {
        if (!click.checkID.IsMine(flower)) return;

        StopCoroutine(recordingCor);
        recorder.StopRecording();
        exitButton.SetActive(true);
        OffPanel();
        recordButtons[2].SetActive(true);
    }

    public void OnListenVoiceButtonClick()
    {
        if (click == null || click.checkID == null || flower == null) return;
        //if (click.checkID.IsMine(flower)) return;

        SwapButtonUI(4);  // 재생 중 UI
        recorder.PlayRecording();
        StartCoroutine(CheckAudioCompletion());
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
        }
        buttons[4].SetActive(false);
    }

    public void UpdateName(Flower flower)
    {
        if (!click.checkID.IsMine(flower)) return;

        photonView.RPC("RPC_UpdateFlowerName", RpcTarget.All, nameInput.text);
    }

    public void OnClickNewFlower()
    {
        if (!click.checkID.IsMine(flower)) return;

        testRecord = false;
        photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, false, false);
        click.checkID.ResetFirst();

        // 모든 클라이언트에서 꽃 초기화 실행
        photonView.RPC("RPC_ResetFlower", RpcTarget.All);

        flower.ResetFlower();
        OnCloseButtonClick();
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

    public void JigglingUI()
    {
        //iTween.mo
    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
    }
}