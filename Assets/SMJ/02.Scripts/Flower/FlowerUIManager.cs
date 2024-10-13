using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] uiPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button btn;
    [SerializeField] private Image flowerImg;

    public void ShowFlowerInfo(Flower flower, int idx)
    {
        if (flower == null)
        {
            return;
        }
        //UpdateUI(flower);
        uiPanel[idx].SetActive(true);
    }

    public void HideFlowerInfo(int idx)
    {
        uiPanel[idx].SetActive(false);
    }

    public void UpdateUI(Flower flower)
    {
        //nameText.text = flower.nickName;
        // 추가적인 UI 업데이트 로직
    }

    public void UpdateButtonText(string text)
    {
        buttonText.text = text;
    }
    public void UpdateButtonInteractable(bool isInteractable)
    {
        btn.interactable = isInteractable;
    }

    public void OnCloseButtonClick(int idx)
    {
        HideFlowerInfo(idx);
    }
}