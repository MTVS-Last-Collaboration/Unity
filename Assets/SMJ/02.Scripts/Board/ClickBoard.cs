using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBoard : MonoBehaviour
{
    [SerializeField] private GameObject boardUIObject;
    [SerializeField] private GameObject playerUIObject;

    private UIPopupAnimation uiPopup;

    private void Start()
    {
        playerUIObject = GameObject.Find("HoonLoobyCanvas");
        uiPopup = GetComponent<UIPopupAnimation>();
        uiPopup.SetTarget(boardUIObject.GetComponent<RectTransform>());
        boardUIObject.SetActive(false);
    }

    public void HandleInteraction()
    {
        playerUIObject.SetActive(false);
        boardUIObject.SetActive(true);
        uiPopup.PlayPopupAnimation(boardUIObject.GetComponent<RectTransform>());
    }

    public void ExitBoard()
    {
        playerUIObject.SetActive(true);
        uiPopup.Hide(boardUIObject.GetComponent<RectTransform>());
    }
}
