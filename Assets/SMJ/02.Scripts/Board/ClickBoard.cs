using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBoard : MonoBehaviour
{
    [SerializeField] private GameObject boardUIObject;
    [SerializeField] private GameObject boardPartition;
    [SerializeField] private GameObject playerUIObject;

    private UIPopupAnimation uiPopup;
    private HoonSoundManagerLogin sound;
    private void Start()
    {
        sound = GameObject.Find("HoonLoobyCanvas").GetComponent<HoonSoundManagerLogin>();
        playerUIObject = GameObject.Find("HoonLoobyCanvas");
        uiPopup = GetComponent<UIPopupAnimation>();
        uiPopup.SetTarget(boardUIObject.GetComponent<RectTransform>());
        boardPartition.SetActive(false);
        boardUIObject.SetActive(false);
    }

    public void HandleInteraction()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        playerUIObject.SetActive(false);
        boardUIObject.SetActive(true);
        boardPartition.SetActive(true);
        uiPopup.PlayPopupAnimation(boardUIObject.GetComponent<RectTransform>());
    }

    public void ExitBoard()
    {
        boardPartition.SetActive(false);
        playerUIObject.SetActive(true);
        uiPopup.Hide(boardUIObject.GetComponent<RectTransform>());
    }
}
