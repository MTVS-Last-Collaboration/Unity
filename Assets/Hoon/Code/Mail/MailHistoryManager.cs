using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Button =  UnityEngine.UI.Button;


/*[System.Serializable]
public class MissionHistoryInfo
{
    public string missionNumber;
    public string missionDate;
    public string missionContent;
    //public string user1name;
    public string partner1Mood;
    public string partner1Answer;
    //public string user2name;
    public string partner2Mood;
    public string partner2Answer;
    public string completed;
}*/

[System.Serializable]
public class MissionHistoryInfo
{
    //public int missionNumber;
    public string missionNumber;
    //public string missionDate;
    public int[] missionDate;
    public string missionContent;
    public string partner1Name;
    public string partner1Mood;
    public string partner1Answer;
    public string partner2Name;
    public string partner2Mood;
    public string partner2Answer;
    // public bool completed;
    public string completed;

    // 날짜를 DateTime으로 파싱하고 문자열로 반환
    public string GetMissionDate()
    {
        if (missionDate.Length == 3)
        {
            DateTime date = new DateTime(missionDate[0], missionDate[1], missionDate[2]);
            return date.ToString("yyyy-MM-dd");
        }
        return string.Empty; // 날짜가 유효하지 않으면 빈 문자열 반환
    }

}


public class MailHistoryManager : MonoBehaviour
{
    GameObject mailBox;
    MailManager mailManager;
    List<MissionHistoryInfo> loadHistoryList = new List<MissionHistoryInfo>();
    List<Button> missionHistroyButtonList = new List<Button>();

    //공개되는 변수
    public int buttonNumber = 0;
    public GameObject historyButton;
    public Transform historyContent;
    public Sprite[] moodSprites;
    public TextMeshProUGUI text_HistoryDate;
    public TextMeshProUGUI Text_HistoryMission;
    public TextMeshProUGUI text_HistoryUser1NickName;
    public TextMeshProUGUI text_HistoryUser2NickName;
    public TextMeshProUGUI text_HistoryUser1Coment;
    public TextMeshProUGUI text_HistoryUser2Coment;
    public Image img_HistoryUser1Mood;
    public Image img_HistoryUser2Mood;



    // Start is called before the first frame update
    void Start()
    {
        mailBox = GameObject.Find("MailBoxHoon");
        if (mailBox != null )
        {
            //print("find MailBoxObjectManager");
            mailManager = mailBox.GetComponent<MailManager>();
            if( mailManager != null )
            {
                //Debug.LogError("find MailManager");
            }
   
        }

        //기존미션조회하기
        ViewMissionHistroyComplete();
 

    }

    // Update is called once per frame
   /* void Update()
    {
        
    }*/

    public void ViewMissionHistroy()
    {
        StartCoroutine(GetViewMissionHistory());
    }

    IEnumerator GetViewMissionHistory()
    {
        string urlMissionHistory = "http://125.132.216.190:12223/api/missions/history"; //url 

        UnityWebRequest request = UnityWebRequest.Get(urlMissionHistory);
        request.SetRequestHeader("Authorization", "Bearer " + LoginInfoManager.instance.myToken); //get mytoken

        yield return request.SendWebRequest(); //Wait Server Request

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)  //error
        {
            Debug.LogError("request downloadHandler" + request.downloadHandler);
            Debug.LogError("Error: " + request.error);

            //error500

        }
        else //respose
        {
            string responseText = request.downloadHandler.text;
            print("서버에 등록된 정보: " + responseText); //뭐냐이거 왜이럼?

            List<MissionHistoryInfo> missionHistroyList = JsonConvert.DeserializeObject<List<MissionHistoryInfo>>(responseText); //
            //List<MissionHistoryInfo > loadHistoryList = new List<MissionHistoryInfo> ();
            //List<Button> missionHistroyButtonList = new List<Button>();
            
            bool isMissionHistroy = false;
            foreach (var missionHistroy in missionHistroyList)
            {
                string historyDate = missionHistroy.GetMissionDate(); //
                string historyDateMission = missionHistroy.missionContent; //

                loadHistoryList.Add(missionHistroy);

                GameObject historyButtonObj = Instantiate(historyButton, historyContent); //Instantiate(GameObject preset, Transform location)
                Button newhistoryButton = historyButtonObj.GetComponent<Button>(); //
                TextMeshProUGUI buttonText = newhistoryButton.GetComponentInChildren<TextMeshProUGUI>(); //
                missionHistroyButtonList.Add(newhistoryButton); //히스토리 리스트

                int buttonIndex = missionHistroyButtonList.Count - 1; //

                // 가장 최근 데이터일수록 위에 추가되도록 설정
                historyButtonObj.transform.SetSiblingIndex(0);

                newhistoryButton.onClick.AddListener(() =>
                {
                    print("checkMisision");
                    DisplayHistory(buttonIndex, historyDate);
                });

                if (buttonText != null)
                {
                    //buttonText.text = "질문" + (missionHistroyButtonList.Count) + ":" + " " + historyDateMission;//+ "\n" + historyDate; 
                    buttonText.text = "질문" + (buttonIndex) + ":" + " " + historyDateMission;//+ "\n" + historyDate; 

                }

            }

        }
    
    }

    public void ViewMissionHistroyComplete()
    {
        StartCoroutine(GetViewMissionHistoryComplete());
    }

    IEnumerator GetViewMissionHistoryComplete()
    {
        string urlMissionHistory = "http://125.132.216.190:12223/api/missions/history"; //url 

        UnityWebRequest request = UnityWebRequest.Get(urlMissionHistory);
        request.SetRequestHeader("Authorization", "Bearer " + LoginInfoManager.instance.myToken); //get mytoken

        yield return request.SendWebRequest(); //Wait Server Request

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)  //error
        {
            Debug.LogError("request downloadHandler" + request.downloadHandler);
            Debug.LogError("Error: " + request.error);

            //error500

        }
        else //respose
        {
            string responseText = request.downloadHandler.text;
            print("서버에 등록된 정보: " + responseText); //뭐냐이거 왜이럼?

            List<MissionHistoryInfo> missionHistroyList = JsonConvert.DeserializeObject<List<MissionHistoryInfo>>(responseText); //
                                                                                                                                 //List<MissionHistoryInfo > loadHistoryList = new List<MissionHistoryInfo> ();
                                                                                                                                 //List<Button> missionHistroyButtonList = new List<Button>()
            bool isMissionHistroy = false;
            foreach (var missionHistroy in missionHistroyList)
            {
                if(missionHistroy.completed != "false")
                {
                    string historyDate = missionHistroy.GetMissionDate(); //
                    string historyDateMission = missionHistroy.missionContent; //

                    loadHistoryList.Add(missionHistroy);


                    GameObject historyButtonObj = Instantiate(historyButton, historyContent); //Instantiate(GameObject preset, Transform location)
                    Button newhistoryButton = historyButtonObj.GetComponent<Button>(); //
                    TextMeshProUGUI buttonText = newhistoryButton.GetComponentInChildren<TextMeshProUGUI>(); //
                    missionHistroyButtonList.Add(newhistoryButton); //히스토리 리스트

                    int buttonIndex = missionHistroyButtonList.Count - 1; //

                    // 가장 최근 데이터일수록 위에 추가되도록 설정
                    historyButtonObj.transform.SetSiblingIndex(0);

                    newhistoryButton.onClick.AddListener(() =>
                    {
                        print("checkMisision");
                        DisplayHistory(buttonIndex, historyDate);
                    });

                    if (buttonText != null)
                    {
                        //buttonText.text = "질문" + (missionHistroyButtonList.Count) + ":" + " " + historyDateMission;//+ "\n" + historyDate; 
                        buttonText.text = "질문" + (missionHistroy.missionNumber) + ":" + " " + historyDateMission;//+ "\n" + historyDate; 

                    }
                
                }

                

            }

        }

    }


    public void DisplayHistory(int index, string date)
    {
        if (index >= 0 && index < loadHistoryList.Count)
        {
            MissionHistoryInfo data = loadHistoryList[index];
            // 데이터를 반환하거나 원하는 방식으로 사용
            //Debug.Log("날짜: " + data.date + ", 미션: " + data.dateMission + ", 닉네임1: " + data.user1name + ", 기분1: " + data.user1mood + ", 답변1: " + data.user1coment + ", 닉네임2: " + data.user2name + ", 기분2: " + data.user2mood + ", 답변2: " + data.user2coment);
            Debug.Log("날짜: " + date + ", 미션: " + data.missionContent + ", 닉네임1: " + data.partner1Name + ", 기분1: " + data.partner1Mood + ", 답변1: " + data.partner1Answer + ", 닉네임2: " + data.partner2Name + ", 기분2: " + data.partner2Mood + ", 답변2: " + data.partner2Answer);

            text_HistoryDate.text = date;
            Text_HistoryMission.text = data.missionContent;
            text_HistoryUser1NickName.text = data.partner1Name;
            text_HistoryUser2NickName.text = data.partner2Name;
            // 원래 텍스트
            string originalText1 = data.partner1Answer;
            string originalText2 = data.partner2Answer;

            // ':'의 위치를 찾음
            int colonIndex1 = originalText1.IndexOf(":");
            int colonIndex2 = originalText2.IndexOf(":");

            // ':' 뒤의 텍스트만 가져옴
            string displayText1 = "";
            string displayText2 = "";
            if (colonIndex1 != -1)
            {
                displayText1 = originalText1.Substring(colonIndex1 + 1).Trim(); // ':' 이후 텍스트를 추출하고 Trim()양끝 공백 제거
            }
            if (colonIndex2 != -1) 
            {
                displayText2 = originalText2.Substring(colonIndex2 + 1).Trim(); // ':' 이후 텍스트를 추출하고  Trim()양끝 공백 제거
            }


            text_HistoryUser1Coment.text = displayText1;
            text_HistoryUser2Coment.text = displayText2;


            if (data.partner1Mood == "null")
            {
                img_HistoryUser1Mood.sprite = moodSprites[0];
            }
            else if (data.partner1Mood == "Good")
            {
                img_HistoryUser1Mood.sprite = moodSprites[1];
            }
            else if (data.partner1Mood == "Normal")
            {
                img_HistoryUser1Mood.sprite = moodSprites[2];
            }
            else if (data.partner1Mood == "Bad")
            {
                img_HistoryUser1Mood.sprite = moodSprites[3];
            }
            else if (data.partner1Mood == "Dizzy")
            {
                img_HistoryUser1Mood.sprite = moodSprites[4];
            }
            else if (data.partner1Mood == "Cry")
            {
                img_HistoryUser1Mood.sprite = moodSprites[5];
            }
            else if (data.partner1Mood == "Angry")
            {
                img_HistoryUser1Mood.sprite = moodSprites[6];
            }
            else if (data.partner1Mood == "Confuse")
            {
                img_HistoryUser1Mood.sprite = moodSprites[7];
            }
            else if (data.partner1Mood == "Sleep")
            {
                img_HistoryUser1Mood.sprite = moodSprites[8];
            }

            if (data.partner2Mood == "null")
            {
                img_HistoryUser2Mood.sprite = moodSprites[0];
            }
            else if (data.partner2Mood == "Good")
            {
                img_HistoryUser2Mood.sprite = moodSprites[1];
            }
            else if (data.partner2Mood == "Normal")
            {
                img_HistoryUser2Mood.sprite = moodSprites[2];
            }
            else if (data.partner2Mood == "Bad")
            {
                img_HistoryUser2Mood.sprite = moodSprites[3];
            }
            else if (data.partner2Mood == "Dizzy")
            {
                img_HistoryUser2Mood.sprite = moodSprites[4];
            }
            else if (data.partner2Mood == "Cry")
            {
                img_HistoryUser2Mood.sprite = moodSprites[5];
            }
            else if (data.partner2Mood == "Angry")
            {
                img_HistoryUser2Mood.sprite = moodSprites[6];
            }
            else if (data.partner2Mood == "Confuse")
            {
                img_HistoryUser2Mood.sprite = moodSprites[7];
            }
            else if (data.partner2Mood == "Sleep")
            {
                img_HistoryUser2Mood.sprite = moodSprites[8];
            }

        }

    }




}
