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
    }

    private void Update()
    {
        restTime = $"{dateChanger.timeUntilAvailable.Hours} : {dateChanger.timeUntilAvailable.Minutes}";
        UpdateUIText();
        UpdateAlertEmoji();
    }

    private void UpdateUIText()
    {
        if (dateChanger.UseFeature() == false && isRecordComplete == true)
        {
            completeText.text = "연인에게 따뜻한 한마디 말하기(완료)\n" + restTime;
        }
        else if (dateChanger.UseFeature() == true && isRecordComplete == false)
        {
            completeText.text = "연인에게 따뜻한 한마디 말하기";
        }

        if (dateChanger.UseFeature() == false && isListenComplete == true)
        {
            listenCompleteText.text = "연인의 말한마디 듣기\n" + restTime;
            buttons[2].GetComponent<Button>().interactable = false;
        }
        else
        {
            buttons[2].GetComponent<Button>().interactable = true;
            listenCompleteText.text = "연인의 말한마디 듣기";
            isListenComplete = false;
        }
    }

    private void UpdateAlertEmoji()
    {
        if (dateChanger.UseFeature() == false && isListenComplete == true)
        {
            alertEmoji.SetActive(false);
        }
        else if (flower.voiceClip != null && !click.checkID.IsMine(flower))
        {
            // 자신의 꽃이 아닐 때만(상대방의 꽃일 때만) 이모지 표시
            alertEmoji.SetActive(true);
        }
        else
        {
            alertEmoji.SetActive(false);
        }
    }

    public void OnClickTest()
    {
        if (!photonView.IsMine) return;

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
    }

    [PunRPC]
    private void RPC_ShowAlertEmoji(bool show)
    {
        alertEmoji.SetActive(show);
    }

    [PunRPC]
    private void RPC_UpdateUI(Flower.States state, string statusMsg)
    {
        statusText.text = statusMsg;
        if (state == Flower.States.BLOSSOM)
        {
            SwapButtonUI(5);
        }
    }

    public void ShowFlowerInfo(Flower flower, int idx)
    {
        if (flower == null)
        {
            return;
        }
        UpdateUI(flower);
        uiPanel.SetActive(true);

        if (isRecordComplete == false || isListenComplete == true)
        {
            SwapButtonUI(idx);
        }
        else
        {
            SwapButtonUI(3);
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

        if (photonView.IsMine)
        {
            photonView.RPC("RPC_UpdateUI", RpcTarget.All, flower.curState, statusMsg);
        }
    }

    public void UpdateButtonInteractable(bool isInteractable, int idx)
    {
        if (!photonView.IsMine) return;
        buttons[idx].GetComponent<Button>().interactable = isInteractable;
    }

    public void OnCloseButtonClick()
    {
        HideFlowerInfo();
    }

    public void OnTalkButtonClick()
    {
        if (!photonView.IsMine) return;
        recordPanel.SetActive(true);
    }

    public void OnRecordingButtonClick(float second)
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        if (testRecord == true)
        {
            recordButtons[2].SetActive(false);
            recordButtons[4].SetActive(true);

            photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, true, isListenComplete);

            flower.evolutionCount++;
            flowerEvol.CheckEvolutionCount();
            UpdateUI(flower);
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
                photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, true, isListenComplete);
            }
        }
    }

    public void OnRecordCompleteToday()
    {
        if (!photonView.IsMine) return;

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
        if (!photonView.IsMine) return;

        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnStopRecordingButtonClick()
    {
        if (!photonView.IsMine) return;

        StopCoroutine(recordingCor);
        recorder.StopRecording();
        exitButton.SetActive(true);
        recordButtons[2].SetActive(true);
    }

    public void OnListenVoiceButtonClick()
    {
        if (!photonView.IsMine) return;

        SwapButtonUI(4);
        recorder.PlayRecording();
        photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, isRecordComplete, true);
        StartCoroutine(CheckAudioCompletion());
    }

    private IEnumerator CheckAudioCompletion()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        buttons[4].SetActive(false);
    }

    public void UpdateName(Flower flower)
    {
        if (!photonView.IsMine) return;

        photonView.RPC("RPC_UpdateFlowerName", RpcTarget.All, nameInput.text);
    }

    public void OnClickNewFlower()
    {
        if (!photonView.IsMine) return;

        testRecord = false;
        photonView.RPC("RPC_UpdateRecordStatus", RpcTarget.All, false, false);
        click.checkID.ResetFirst();
        flowerEvol.NewFlower();
        flower.ResetFlower();
        OnCloseButtonClick();
    }
}