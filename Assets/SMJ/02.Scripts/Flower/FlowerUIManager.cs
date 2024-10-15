using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Image flowerImg;

    [SerializeField] private GameObject exitButton;

    [SerializeField] private GameObject recordPanel;
    [SerializeField] private GameObject[] recordButtons;

    public bool isRecordComplete = false;

    Coroutine recordingCor;

    public void ShowFlowerInfo(Flower flower, int idx)
    {
        if (flower == null)
        {
            return;
        }
        UpdateUI(flower);
        uiPanel.SetActive(true);
        if (isRecordComplete == false)
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
        for (int i = 0; i < recordButtons.Length; i++)
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
        nameText.text = flower.nickName;
        //상태 문구 테이블 받을 때 수정
        switch (flower.curState)
        {
            case Flower.States.SPROUT:
                statusText.text = "1";
                break;
            case Flower.States.BUD:
                statusText.text = "2";
                break;
            case Flower.States.BLOSSOM:
                statusText.text = "3";
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

    IEnumerator RecordingVoice(float second)
    {
        yield return new WaitForSeconds(second);
        recordButtons[2].SetActive(true);
        yield return new WaitForSeconds(2f);
        //추후에 성공 실패 여부 get 후에 변경
        recordButtons[4].SetActive(true);
        isRecordComplete = true;
        yield return new WaitForSeconds(2f);
        recordPanel.SetActive(false);
        exitButton.SetActive(true);
        buttons[3].SetActive(true);
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
    }

    public void OnStopRecordingButtonClick()
    {
        StopCoroutine(recordingCor);
        exitButton.SetActive(true);
        for (int i = 1; i < recordButtons.Length; i++)
        {
            recordButtons[i].SetActive(false);
        }
        OnTalkButtonClick();
    }
}