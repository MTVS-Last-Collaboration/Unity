using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject[] buttons;
    [SerializeField] private Image flowerImg;

    public void ShowFlowerInfo(Flower flower, int idx)
    {
        if (flower == null)
        {
            return;
        }
        //UpdateUI(flower);
        uiPanel.SetActive(true);
        buttons[idx].SetActive(true);
    }

    public void HideFlowerInfo()
    {
        uiPanel.SetActive(false);
        //UpdateButtonInteractable(true, idx);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetActive(false);
        }
    }

    public void UpdateUI(Flower flower)
    {
        //nameText.text = flower.nickName;
        // 추가적인 UI 업데이트 로직
    }

    public void UpdateButtonInteractable(bool isInteractable, int idx)
    {
        buttons[idx].GetComponent<Button>().interactable = isInteractable;
    }

    public void OnCloseButtonClick()
    {
        HideFlowerInfo();
    }
}