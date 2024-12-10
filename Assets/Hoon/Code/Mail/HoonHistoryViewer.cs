using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoonHistoryViewer : MonoBehaviour
{
    public GameObject Img_HistoryViewerBG;
    public TextMeshProUGUI Text_HistoryDate;
    public TextMeshProUGUI Text_HistroyMission;
    public TextMeshProUGUI Text_HistoryUser1NickName;
    public TextMeshProUGUI Text_HistoryUser2NickName;
    public TextMeshProUGUI Text_HistoryUser1Coment;
    public TextMeshProUGUI Text_HistoryUser2Coment;
    public Image Img_HistoryUser1Mood;
    public Image Img_HistoryUser2Mood;

    public TextMeshProUGUI Text_HistoryViewerDate;
    public TextMeshProUGUI Text_HistroyViewerMission;
    public TextMeshProUGUI Text_HistoryViewerNickName1;
    public TextMeshProUGUI Text_HistoryViewerNickName2;
    public TextMeshProUGUI Img_HistoryViwerComent1;
    public TextMeshProUGUI Img_HistoryViwerComent2;
    public Image Img_HistroryViewerMood1;
    public Image Img_HistroryViewerMood2;

    bool isHistoryViwer = false;

    void Start()
    {
        
    }

    
    /*void Update()
    {
        
    }*/

    public void ViewerOpen()
    {
        isHistoryViwer = !isHistoryViwer;

        if(isHistoryViwer)
        {
            Img_HistoryViewerBG.SetActive(true);

            Text_HistoryViewerDate.text = Text_HistoryDate.text;
            Text_HistroyViewerMission.text = Text_HistroyMission.text;
            Text_HistoryViewerNickName1.text = Text_HistoryUser1NickName.text;
            Text_HistoryViewerNickName2.text = Text_HistoryUser2NickName.text;
            Img_HistoryViwerComent1.text = Text_HistoryUser1Coment.text;
            Img_HistoryViwerComent2.text = Text_HistoryUser2Coment.text;
            Img_HistroryViewerMood1.sprite = Img_HistoryUser1Mood.sprite;
            Img_HistroryViewerMood2.sprite = Img_HistoryUser2Mood.sprite;

        }
        else
        {
            Img_HistoryViewerBG.SetActive(false);
        }


    }


}//클래스 끝
