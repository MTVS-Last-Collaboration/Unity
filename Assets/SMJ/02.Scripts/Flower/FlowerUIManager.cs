using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlowerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image flowerImg;

    private void Awake() => HideFlowerInfo();

    public void ShowFlowerInfo(Flower flower)
    {
        if (flower == null)
        {
            Debug.LogError("Attempted to show info for null flower");
            return;
        }
        UpdateUI(flower);
        uiPanel.SetActive(true);
    }

    public void HideFlowerInfo() => uiPanel.SetActive(false);

    private void UpdateUI(Flower flower)
    {
        nameText.text = flower.nickName;
        // 추가적인 UI 업데이트 로직
    }

    public void OnCloseButtonClick() => HideFlowerInfo();
}