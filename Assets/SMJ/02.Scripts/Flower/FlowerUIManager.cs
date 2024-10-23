using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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

    private VoiceRecorder recorder;
    private FlowerEvolution flowerEvol;
    private Flower flower;
    private ClickFlower click;

    private int recordCount = 0;

    public bool isRecordComplete = false;
    public bool isListenComplete = false;

    private string restTime = string.Empty;
    Coroutine recordingCor;

    private void Start()
    {
        flower = GetComponent<Flower>();
        recorder = GetComponent<VoiceRecorder>();
        flowerEvol = GetComponent<FlowerEvolution>();
        click = GetComponent<ClickFlower>();

        // 초기 상태 텍스트 설정
        UpdateStateText(flower.curState);

        StartCoroutine(InitialStateSync());
    }
    private void UpdateStateText(Flower.States state)
    {
        string statusMsg = "";
        switch (state)
        {
            case Flower.States.SPROUT:
                statusMsg = "상태: 자라나는 중...";
                break;
            case Flower.States.BUD:
                statusMsg = "상태: 피기 직전.";
                break;
            case Flower.States.BLOSSOM:
                statusMsg = "상태: 활짝 피었어요!";
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

        bool isMyFlower = click.checkID.IsMine(flower);
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
                case Flower.States.SPROUT:
                    statusMsg = "상태: 자라나는 중...";
                    break;
                case Flower.States.BUD:
                    statusMsg = "상태: 피기 직전.";
                    break;
                case Flower.States.BLOSSOM:
                    statusMsg = "상태: 활짝 피었어요!";
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

        // 상태 동기화 후 UI 업데이트
        if (flower.curState == Flower.States.BLOSSOM && isRecordComplete && click.checkID.IsMine(flower))
        {
            SwapButtonUI(5);
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

        bool isMyFlower = click.checkID.IsMine(flower);

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
        if (targetFlower == null)
        {
            return;
        }

        bool isMyFlower = click.checkID.IsMine(targetFlower);
        UpdateUI(targetFlower);
        uiPanel.SetActive(true);

        // 먼저 진화 상태와 녹음 상태를 확인
        if (isMyFlower && targetFlower.curState == Flower.States.BLOSSOM && isRecordComplete)
        {
            SwapButtonUI(5);  // 새 꽃 심기 버튼
            return;  // 여기서 종료
        }

        // 다른 상태들 처리
        if (isMyFlower)
        {
            if (isRecordComplete == false || isListenComplete == true)
            {
                SwapButtonUI(idx);
            }
            else
            {
                SwapButtonUI(3);
            }
        }
        else
        {
            if (targetFlower.voiceClip != null && !isListenComplete)
            {
                SwapButtonUI(2);
            }
            else
            {
                SwapButtonUI(idx);
            }
        }
    }

    public void HideFlowerInfo()
    {
        uiPanel.SetActive(false);
        recordPanel.SetActive(false);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
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
        HideFlowerInfo();
    }

    public void OnTalkButtonClick()
    {
        if (click.checkID.IsMine(flower))
        {
            recordPanel.SetActive(true);
        }
    }

    public void OnRecordingButtonClick(float second)
    {
        if (!click.checkID.IsMine(flower)) return;

        exitButton.SetActive(false);
        recordButtons[1].SetActive(true);
        recordingCor = StartCoroutine(RecordingVoice(second));
    }

    IEnumerator RecordingVoice(float second)
    {
        recorder.StartRecording();
        yield return new WaitForSeconds(second);
        recordButtons[2].SetActive(true);
    }
    public void SubmitRecord()
    {
        if (!click.checkID.IsMine(flower)) return;

        if (testRecord == true)
        {
            recordButtons[2].SetActive(false);
            recordButtons[4].SetActive(true);

            // 진화 카운트 증가
            flower.evolutionCount++;
            // 모든 클라이언트에 진화 상태 동기화
            photonView.RPC("RPC_UpdateEvolutionCount", RpcTarget.All, flower.evolutionCount);

            byte[] voiceData = recorder.GetRecordedData();
            photonView.RPC("RPC_SyncVoiceClip", RpcTarget.All, voiceData);
            photonView.RPC("RPC_NotifyRecordComplete", RpcTarget.Others, voiceData);
        }
        else
        {
            recordCount++;
            if (recordCount < 3)
            {
                recordButtons[2].SetActive(false);
                recordButtons[3].SetActive(true);
                photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, false, isListenComplete);
            }
            else
            {
                recordButtons[2].SetActive(false);
                recordButtons[5].SetActive(true);
                recordCount = 0;

                // 진화 카운트 증가
                flower.evolutionCount++;
                // 모든 클라이언트에 진화 상태 동기화
                photonView.RPC("RPC_UpdateEvolutionCount", RpcTarget.All, flower.evolutionCount);

                byte[] voiceData = recorder.GetRecordedData();
                photonView.RPC("RPC_SyncVoiceClip", RpcTarget.All, voiceData);
                photonView.RPC("RPC_NotifyRecordComplete", RpcTarget.Others, voiceData);
            }
        }
    }

    public void OnRecordCompleteToday()
    {
        if (!click.checkID.IsMine(flower)) return;

        byte[] voiceData = recorder.GetRecordedData();
        photonView.RPC("RPC_SyncVoiceClip", RpcTarget.All, voiceData);

        recordPanel.SetActive(false);
        exitButton.SetActive(true);
        SwapButtonUI(3);
        UpdateUI(flower);
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnReRecordingClick()
    {
        if (!click.checkID.IsMine(flower)) return;

        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnStopRecordingButtonClick()
    {
        if (!click.checkID.IsMine(flower)) return;

        StopCoroutine(recordingCor);
        recorder.StopRecording();
        exitButton.SetActive(true);
        recordButtons[2].SetActive(true);
    }

    public void OnListenVoiceButtonClick()
    {
        if (click == null || click.checkID == null || flower == null) return;
        if (click.checkID.IsMine(flower)) return;

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

        bool isMyFlower = click.checkID.IsMine(flower);
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
}