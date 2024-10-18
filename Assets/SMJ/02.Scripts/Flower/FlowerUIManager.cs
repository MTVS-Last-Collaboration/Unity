using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUIManager : MonoBehaviour
{
    public MidnightChecker dateChanger;

    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text completeText;
    [SerializeField] private TMP_Text listenCompleteText;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Image flowerImg;

    [SerializeField] private GameObject exitButton;

    [SerializeField] private GameObject recordPanel;
    [SerializeField] private GameObject[] recordButtons;

    [SerializeField] private AudioSource audioSource;

    private VoiceRecorder recorder;
    private FlowerEvolution flowerEvol;
    private Flower flower;

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
    }

    private void Update()
    {
        restTime = $"{dateChanger.timeUntilAvailable.Hours} : {dateChanger.timeUntilAvailable.Minutes}";
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

    public void OnClickTest()
    {
        if (testRecord == true)
        {
            testRecord = false;
            print("실패!");
        }
        else
        {
            testRecord= true;
            print("성공!");
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
            buttons[idx].SetActive(true);
        }
        else
        {
            buttons[3].SetActive(true);
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
        // 추가적인 UI 업데이트 로직

        //상태 문구 테이블 받을 때 수정
        switch (flower.curState)
        {
            case Flower.States.SPROUT:
                statusText.text = "상태: 자라나는 중...";
                break;
            case Flower.States.BUD:
                statusText.text = "상태: 피기 직전.";
                break;
            case Flower.States.BLOSSOM:
                statusText.text = "상태: 활짝 피었어요!";
                break;
        }
        //이미지도 받고 수정
    }

    public void UpdateButtonInteractable(bool isInteractable, int idx)
    {
        buttons[idx].GetComponent<Button>().interactable = isInteractable;
    }

    public void OnCloseButtonClick()
    {
        HideFlowerInfo();
    }

    public void OnTalkButtonClick()
    {
        recordPanel.SetActive(true);
    }

    public void OnRecordingButtonClick(float second)
    {
        exitButton.SetActive(false);
        recordButtons[1].SetActive(true);
        recordingCor = StartCoroutine(RecordingVoice(second));
    }
    bool testRecord = false;
    IEnumerator RecordingVoice(float second)
    {
        recorder.StartRecording();
        yield return new WaitForSeconds(second);
        recordButtons[2].SetActive(true);
    }

    public void SubmitRecord()
    {
        //추후에 성공 실패 여부 get 후에 변경
        //4 : 성공 //3 : 실패
        if (testRecord == true)
        {
            recordButtons[2].SetActive(false);
            recordButtons[4].SetActive(true);
            isRecordComplete = true;
            flower.evolutionCount++;
            flowerEvol.CheckEvolutionCount();
            UpdateUI(flower);
            //recorder.SaveRecording();
        }
        else
        {
            recordCount++;
            if (recordCount < 3)
            {
                recordButtons[2].SetActive(false);
                recordButtons[3].SetActive(true);
                //다시말하기 해야됨
                isRecordComplete = false;
            }
            else
            {
                //찐 실패
                recordButtons[2].SetActive(false);
                recordButtons[5].SetActive(true);
                recordCount = 0;
                isRecordComplete = true;
            }
        }
    }

    public void OnRecordCompleteToday()
    {
        recordPanel.SetActive(false);
        exitButton.SetActive(true);
        buttons[3].SetActive(true);
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnReRecordingClick()
    {
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnStopRecordingButtonClick()
    {
        StopCoroutine(recordingCor);
        recorder.StopRecording();
        exitButton.SetActive(true);
        //for (int i = 1; i < recordButtons.Length; i++)
        //{
        //    recordButtons[i].SetActive(false);
        //}
        recordButtons[2].SetActive(true);
        //OnTalkButtonClick();
    }

    public void OnListenVoiceButtonClick()
    {
        buttons[4].SetActive(true);
        recorder.PlayRecording();
        isListenComplete = true;
        StartCoroutine(CheckAudioCompletion());
    }

    private IEnumerator CheckAudioCompletion()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        //소리 재생 후 false
        buttons[4].SetActive(false);
        //추후 자정 지났을때 초기화 기능 추가
    }

    public void UpdateName(Flower flower)
    {
        flower.nickName = nameInput.text;
        //추후 네트워크 포스트
    }
}