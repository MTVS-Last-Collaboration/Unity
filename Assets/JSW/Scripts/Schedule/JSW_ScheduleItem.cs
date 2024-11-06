using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_ScheduleItem : MonoBehaviour
{
    public TMP_Text scheduleText;
    public GameObject scrollView;
    public Image mainImage;
    public Sprite[] iconSprite;
    public int iconNum;
    

    void Start()
    {
        scheduleText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetText(string text, int Num)
    {
        scheduleText = transform.GetChild(0).GetComponent<TMP_Text>();
        scheduleText.text = text;
        mainImage.sprite = iconSprite[Num];
        iconNum = Num;
    }

    public void OnClickImage()
    {
        scrollView.SetActive(!scrollView.activeSelf);
    }

    public void IconImageSetting()
    {
        mainImage.sprite = iconSprite[iconNum];
    }
}
