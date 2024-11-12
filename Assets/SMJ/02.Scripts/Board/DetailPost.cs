using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetailPost : MonoBehaviour
{
    public int answerId = 0;
    public string title;
    public string nickName;
    public DateTime date;               //MM/dd HH:mm
    public int likeCount = 0;
    public int commentCount = 0;

    private UIPopupAnimation uiPopup;

    public GameObject DetailObj;
    public GameObject inputCommentObj;
    public GameObject boardViewObj;

    private void Start()
    {
        uiPopup = GetComponent<UIPopupAnimation>();
        inputCommentObj.transform.SetAsLastSibling();
    }

    public DetailPost(int answerId, string title, string nickName, DateTime date, int likeCount, int commentCount)
    {
        this.answerId = answerId;
        this.title = title;
        this.nickName = nickName;
        this.date = date;
        this.likeCount = likeCount;
        this.commentCount = commentCount;
        ActivateDetail();
    }
    
    public void ActivateDetail()
    {
        //boardViewObj.SetActive(false);
        uiPopup.PlayPopupAnimation(DetailObj.GetComponent<RectTransform>());
        uiPopup.PlayPopupAnimation(inputCommentObj.GetComponent<RectTransform>());
    }

    public void HideDetail()
    {
        //boardViewObj.SetActive(true);
        uiPopup.Hide(inputCommentObj.GetComponent<RectTransform>());
        uiPopup.Hide(DetailObj.GetComponent<RectTransform>());
    }
}
