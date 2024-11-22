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

    //[SerializeField] public bool testRecord = false;
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

    private string flowerId;

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
        flowerId = photonView.ViewID.ToString();
        if (photonView.IsMine)
        {
            StartCoroutine(GetVoiceStatus());
        }
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

        hoonUI = GameObject.Find("HoonLoobyCanvas");

        // 초기 상태 텍스트 설정
        UpdateStateText(flower.curState);
        if (photonView.IsMine)
        {
            StartCoroutine(InitialStateSync());
        }

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
        if (photonView.IsMine && flower != null)
        {
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

            flowerEvol.CheckEvolutionCount();
            UpdateUI(flower);
            UpdateUIText();
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
        flowerEvol.CheckEvolutionCount();

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
        sound.PlaySound("smjAudioClopAttay", 0);
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
    //public void SubmitRecord()
    //{
    //    if (!click.checkID.IsMine(flower)) return;

    //    // 기존의 테스트 모드일 경우
    //    if (testRecord == true)
    //    {
    //        OffPanel();
    //        recordButtons[2].SetActive(false);

    //        // 서버 검증 시작
    //        StartCoroutine(ValidateAndTransferVoice());
    //    }
    //    else
    //    {
    //        recordCount++;
    //        if (recordCount < 3)
    //        {
    //            OffPanel();
    //            recordButtons[2].SetActive(false);
    //            recordButtons[3].SetActive(true);
    //        }
    //        else
    //        {
    //            OffPanel();
    //            recordButtons[2].SetActive(false);
    //            recordButtons[5].SetActive(true);
    //            recordCount = 0;
    //        }
    //    }
    //}
    public void SubmitRecord()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        if (!click.checkID.IsMine(flower)) return;

        OffPanel();
        recordButtons[2].SetActive(false);
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

            NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
            yield return NetworkManager.Instance.PostMultipartData("/api/flower/analyze-mood", formData,
                (success, response) =>
                {
                    if (success)
                    {
                        try
                        {
                            var validationResponse = JsonUtility.FromJson<VoiceValidationResponse>(response);
                            if (validationResponse.mood != null)
                            {
                                if (validationResponse.mood == "부정")
                                {
                                    recordCount++;
                                    if (recordCount < 3)
                                    {
                                        OffPanel();
                                        recordButtons[3].SetActive(true); // 재녹음 버튼
                                    }
                                    else
                                    {
                                        OffPanel();
                                        recordButtons[5].SetActive(true); // 최종 실패 UI
                                        recordCount = 0;
                                    }
                                }
                                else // "긍정" 또는 "중립"
                                {
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

    [SerializeField] private TMP_Text progressText;
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
    private class VoiceStatus
    {
        public bool recordComplete;
        public bool listenComplete;
        public DateTime savedAt;
        public DateTime listenedAt;
        public int moodCount;
        public string flowerName;
    }

    private IEnumerator GetVoiceStatus()
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.Get<VoiceStatus>("api/flower/voice/status",
            (success, response) =>
            {
                if (success && response != null)
                {
                    isRecordComplete = response.recordComplete;
                    isListenComplete = response.listenComplete;
                    flower.evolutionCount = response.moodCount;
                    flower.nickName = response.flowerName;
                    nameInput.text = response.flowerName; ;
                    UpdateUI(flower);
                    UpdateUIText();
                }
            });
    }

    private IEnumerator GetAndPlayVoiceMessage()
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        bool isPlaying = false;

        yield return NetworkManager.Instance.GetWithoutBody("api/flower/voice",
            (success, response) =>
            {
                if (success)
                {
                    try
                    {
                        // response는 URL 문자열이므로 JsonUtility.FromJson 필요없이 바로 사용
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
                    Debug.LogError($"Failed to get voice URL: {response}");
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
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

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
        sound.PlaySound("smjAudioClopAttay", 0);
        if (!click.checkID.IsMine(flower)) return;

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

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.StateChanged -= OnStateChanged;
    }
}