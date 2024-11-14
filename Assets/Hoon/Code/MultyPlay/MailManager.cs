using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
//claaback�̺�Ʈ
using ExitGames.Client.Photon;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Net.NetworkInformation;
using UnityEditor;
/*using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine.Rendering.LookDev;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Converters;
using Photon.Pun.Demo.Cockpit;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine.UIElements;*/
/*using System.Text;
using static System.Net.WebRequestMethods;*/

[System.Serializable]
public class DayComentData
{
    public string date;
    public string dateMission;
    public string user1name;
    public string user1mood;
    public string user1coment;
    public string user2name;
    public string user2mood;
    public string user2coment;
}

[System.Serializable]
public class SeverMailData
{
    public string missionNumber;
    public int[] missionDate;
    public string missionContent;
    public string partner1Mood;
    public string partner1Answer;
    public string partner2Mood;
    public string partner2Answer;
    public string completed;

}


public class MailManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject mail_IconObject; // Img_MailIcon
    public GameObject mail_ImageObject; //Img_MailBG
    public Button touchButton; //Btn_TouchEnter
    public TextMeshProUGUI currentDay; //Text_Day
    public GameObject moodChoiceObject; //Img_MoodChoice2
    public GameObject moodSwitch1; //Btn_MoodSwitch1
    public GameObject moodSwitch2;  //Btn_MoodSwitch2
    public GameObject moodChoice1Object; //Btn_MoodSwitch1
    public GameObject moodChoice2Object; //Btn_MoodSwitch2
    public Image moodChice1; //Img_MoodChoice1
    public Image moodChice2; // Img_MoodChoice2
    public Button moodGood; // Btn_Good
    public Button moodNormal; //Btn_Normal
    public Button moodBad; //Btn_Bad
    public Sprite[] moodSprites;
    public GameObject tmp_InputFieldObject; //Input_DayMission
    public Button mailComentButton; //Btn_Mail_Coment
    public GameObject mailComentTestObject; //Text_Mail_Coment
    public GameObject dayMisiionObject; //Text_Mail_DayMission
    public GameObject Coment1; //Text_Mail_DayComent1
    public GameObject Coment2; //Text_Mail_DayComent2
    TextMeshProUGUI mailComentText1; //
    TextMeshProUGUI mailComentText2; //
    public string startDate; //coupleAnivarsary
    public string userNumber;

    GameObject player1;
    GameObject player2;
    Image mail_IconImage;
    bool isMailImage = false; //
    bool isMoodSwihtch1 = false;
    bool isMoodSwihtch2 = false;
    bool isMailComentButton = false;
    TMP_InputField tmp_InputField;
    TextMeshProUGUI mailComentButtonText;
    bool isMailComentSave = false;

    bool isMailComent1 = false;
    bool isMainComent2 = false;

    string currentDate; //���� ��¥�� ������ ����
    string playerNickName; //�г��� ���� ����

    List<DayComentData> loadDayComenList; //�ε��� �����͸� �����ϴ� ����Ʈ
    string matchDayComentinfo; //����Ʈ���� ��ġ�ϴ� �����͸� ������ ���ڿ�

    PlayerNicknameManager playerNicknameMgr1;
    PlayerNicknameManager playerNicknameMgr2;

    // ������ ���ؼ� �̺�Ʈ�� ������.
    // ���濡�� ���� �̺�Ʈ �ڵ� (��: 100)
    private const byte DATA_SYNC_EVENT_CODE = 100;
    // JSON ������ ����� ���� ���
    public string jsonSyncPath;
    //public string jsonSyncPath = Application.persistentDataPath + "/DayComentTest.json";
    public string jsonSyncString;
    public string todayMission;
    public TextMeshProUGUI DataPath;

    public GameObject moodButton1;
    public GameObject moodButton2;
    public GameObject historyScrollview; //�߾��ϱ��ư���� ��ũ�Ѻ並 �״� ��������.
    bool isHistoryScrollview = false; // �߾ｺũ�Ѻ䰡 ���̴��� Ȯ���ϴ� ����
    public Transform historyContent; // �߾��ư�� �������� ��ġ
    public GameObject historyButton; // �߾��ư

    string moodText1;
    string moodText2;

    List<DayComentData> histrotyList = new List<DayComentData>(); //�����丮 ���� ����Ʈ
    List<Button> histroyButtonList = new List<Button>(); //��ư�� ���� ����Ʈ

    public bool isServerComent = false;
    public TextMeshProUGUI testSeverComentButton;

    public List<Image> moodChoice1ButtonImageList;
    public List<Image> moodChoice2ButtonImageList;
    public GameObject historyBGObject; //지난답변보기 오브젝트

    public TextMeshProUGUI btnText_MailServerComent; //button save coment server
    bool isButtonSaveComentSever = false;

    public string textMyMood;

    //Histroty value
    public TextMeshProUGUI text_HistoryDate;
    public TextMeshProUGUI text_HistoryUser1NickName;
    public TextMeshProUGUI text_HistoryUser2NickName;
    public TextMeshProUGUI text_HistoryUser1Coment;
    public TextMeshProUGUI text_HistoryUser2Coment;
    public Image img_HistoryUser1Mood;
    public Image img_HistoryUser2Mood;
    //GetMoodServer
    public string partner1Mood;
    public string partner2Mood;

    //imgMoodChoiceBlackBg
    GameObject imgMoodChoiceBlackBg;
    public HoonSoundManagerLogin hoonSoundManagerLogin;

    void Start()
    {
        StartCoroutine(FindPlayer());

        jsonSyncPath = Application.persistentDataPath + "/DayComentTest.json";
        if (System.IO.File.Exists(jsonSyncPath))
        {
            print("findLocalJsonFile");
        }
        else
        {
            print("jsonfile 없음,Create");
            CreateNewDayComentJsonArray();

        }
        //PhotonNetwork.AddCallbackTarget(this);  // �̺�Ʈ �ݹ� ���
        //Debug.Log(Application.persistentDataPath);
        //DataPath.text = Application.persistentDataPath;

        //��ȯ���� int

        string playerList = "playerCount" + PhotonNetwork.PlayerList.Length; //+ "\n" + // + "\n" +
        string roomName = "";
        string nickName = "";

        if (PhotonNetwork.CurrentRoom != null)
        {
            roomName = "roomName" + PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            roomName = "Default";
        }

        if (PhotonNetwork.LocalPlayer.NickName != null)
        {
            nickName = "myNickName" + PhotonNetwork.LocalPlayer.NickName;
        }

        DataPath.text = playerList + "\n" + roomName + "\n" + nickName;

        moodChoiceObject.SetActive(false);

        if (mail_IconObject != null)
        {
            //print("mail_IconObject ");
            mail_IconImage = mail_IconObject.GetComponent<Image>();
            mail_IconImage.gameObject.SetActive(false);
        }

        if (mail_ImageObject != null) //mailUIObject false
        {
            imgMoodChoiceBlackBg = GameObject.Find("Img_MoodChoiceBlackBG");
            if (imgMoodChoiceBlackBg != null)
            {
                imgMoodChoiceBlackBg.SetActive(false);
            }
            else
            {
                Debug.LogError("myobject" + imgMoodChoiceBlackBg);
            }

            mail_ImageObject.SetActive(false); //offImgMailUIObject
        }

        tmp_InputFieldObject.SetActive(false); //��ǲ�ʵ� ������Ʈ ����.
        mailComentButtonText = mailComentTestObject.GetComponent<TextMeshProUGUI>(); //�����ڸ�Ʈ��ư�ؽ�Ʈ

        DataPath.gameObject.SetActive(false);//����Ÿ �ؽ�Ʈ ����
        //�����丮 ��ũ�Ѻ� ����
        historyScrollview.gameObject.SetActive(false);

        //���ϱ�м���UI����
        moodChoice1Object.SetActive(false); //1�����̽�����
        moodChoice2Object.SetActive(false); //2�����̽�����
        historyBGObject.SetActive(false); //지난답변보기 오브젝트

       


    }

    void Update()
    {
        /* if (isMailImage)//����������BG
         {
             mail_ImageObject.SetActive(true);

         }
         else
         {
             mail_ImageObject.SetActive(false); //���� �̹��� ������Ʈ ����
             print("�����̹��� ����");
         }*/

        /* if (isMoodSwihtch1)//����ǥ����ư1
         {
             moodChice1.gameObject.SetActive(true);
         }
         else
         {
             moodChice1.gameObject.SetActive(false);
         }

         if (isMoodSwihtch2)//����ǥ����ư2
         {
             moodChice2.gameObject.SetActive(true);
         }
         else
         {
             moodChice2.gameObject.SetActive(false);
         }*/

    }

    public void TodayMissonGetServer()//�������� ���� �̼��� ��������.
    {
        hoonSoundManagerLogin.PlaySound(0); //buttonSound
        StartCoroutine(GetTodayMission());
        
    }

    IEnumerator GetTodayMission() //�������� Get �̼��� ��������.
    {
        string urlTodayMission = "http://125.132.216.190:12223/api/missions/current"; //url 

        UnityWebRequest request = UnityWebRequest.Get(urlTodayMission);
        request.SetRequestHeader("Authorization", "Bearer " + LoginInfoManager.instance.myToken); //get mytoken

        yield return request.SendWebRequest(); //������ ��û�� �ö����� ���

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)  //error
        {

            Debug.LogError("Error: " + request.error);
            //error500

        }
        else //respose
        {
            string responseText = request.downloadHandler.text;
            print("서버에 등록된 정보: " + responseText); //뭐냐이거 왜이럼?

            //json Object convert, require token
            JObject jsonObj = JObject.Parse(responseText); //jsonParse 
            string missionNumber = jsonObj["missionNumber"].ToString(); //missionNumber
            int[] missionDateArray = jsonObj["missionDate"].ToObject<int[]>(); //todayDate 
            string missionContent = jsonObj["missionContent"].ToString();  // todayMission
            Debug.Log("Mission Content: " + missionContent);// 미션을 출력하자.

            DateTime missionDate = new DateTime(missionDateArray[0], missionDateArray[1], missionDateArray[2]); //ChangeForm datetime

            // 원하는 형식의 문자열로 변환
            string formattedDate = missionDate.ToString("yyyy-MM-dd");

            string partner1Mood;
            string partner1Answer;
            string partner2Mood;
            string partner2Answer;
            string completed;

            currentDay.text = "No:" + missionNumber + " " + formattedDate;  // +missionDate //numMisson 
            dayMisiionObject.GetComponent<TextMeshProUGUI>().text = missionContent; //change dayMission text
            partner1Mood = jsonObj["partner1Mood"].ToString();
            partner1Answer = jsonObj["partner1Answer"].ToString();
            partner2Mood = jsonObj["partner2Mood"].ToString();
            partner2Answer = jsonObj["partner2Answer"].ToString();
            completed = jsonObj["completed"].ToString();

            print("partner1Mood" + partner1Mood);
            print("partner1Answer" + partner1Answer);
            print("partner2Mood" + partner2Mood);
            print("partner2Answer" + partner2Answer);
            print("completed" + completed);

            // isComplete
            if (completed =="True") //uppdercase true
            {
                print("MissionComplite");
                if(userNumber == "user1")
                {
                    print("userNumber" + userNumber);
                    ChangeMoodImage(partner1Mood);
                    Image img2 = moodSwitch2.GetComponent<Image>();
                    if (partner2Mood == "null")
                    {
                        img2.sprite = moodSprites[0];
                        textMyMood = "null";
                    }
                    else if (partner2Mood == "Good")
                    {
                        img2.sprite = moodSprites[1];
                        textMyMood = "Good";
                    }
                    else if (partner2Mood == "Normal")
                    {
                        img2.sprite = moodSprites[2];
                        textMyMood = "Normal";
                    }
                    else if (partner2Mood == "Bad")
                    {
                        img2.sprite = moodSprites[3];
                        textMyMood = "Bad";
                    }

                }
                else
                {
                    print("userNumber" + userNumber);
                    ChangeMoodImage(partner2Mood);
                    Image img1 = moodSwitch1.GetComponent<Image>();
                    if (partner1Mood == "null")
                    {
                        img1.sprite = moodSprites[0];
                        textMyMood = "null";
                    }
                    else if (partner1Mood == "Good")
                    {
                        img1.sprite = moodSprites[1];
                        textMyMood = "Good";
                    }
                    else if (partner1Mood == "Normal")
                    {
                        img1.sprite = moodSprites[2];
                        textMyMood = "Normal";
                    }
                    else if (partner1Mood == "Bad")
                    {
                        img1.sprite = moodSprites[3];
                        textMyMood = "Bad";
                    }


                }
                
                Coment1.GetComponent<TextMeshProUGUI>().text = jsonObj["partner1Answer"].ToString();
                Coment2.GetComponent<TextMeshProUGUI>().text = jsonObj["partner2Answer"].ToString();
                moodSwitch1.GetComponent<Image>().color = Color.white; //chage switch color
                moodSwitch2.GetComponent<Image>().color = Color.white; //chage switch color
            }
            else
            {
                print("MissionInProgress");

                if(userNumber == "user1") //user1
                {
                    // user1Mood
                    if (partner1Mood == "null")
                    {
                        Debug.LogError("1111");
                        partner1Mood = "null";
                        ChangeMoodImage(partner1Mood);
                        print("partner1Mood" + partner1Mood);
                        //moodSwitch1.GetComponent<Image>().color = Color.white;

                    }
                    else
                    {
                        Debug.LogError("2222");
                        //partner1Mood = "null";
                        ChangeMoodImage(partner1Mood); //user1 showMyMood
                        moodSwitch1.GetComponent<Image>().color = Color.white;
                        //btn_moodswitch1.GetComponent<Image>.color = Color.white;//colorChange
                        //Debug.LogError("ChangeMoodColor" + moodSwitch1.GetComponent<Image>().color.ToString());
                    }
                    // user1coment
                    if (jsonObj["partner1Answer"].ToString() == "null")
                    {
                        Coment1.GetComponent<TextMeshProUGUI>().text = "작성하기를 눌러 답변을 작성하고 제출해보세요.";
                    }
                    else
                    {
                        Coment1.GetComponent<TextMeshProUGUI>().text = jsonObj["partner1Answer"].ToString(); //user1 showMyComent
                    }

                    // user2Mood
                    if (jsonObj["partner2Mood"].ToString() == "null")
                    {
                        partner2Mood = "null";
                    }
                    else
                    {
                        partner2Mood = "null";
                        ChangeMoodImage(partner2Mood);
                        moodSwitch2.GetComponent<Image>().color = Color.white; // hideUser2Mood

                        //btn_moodswitch1.GetComponent<Image>.color = Color.white;//colorChange
                        //Debug.LogError("ChangeMoodColor" + moodSwitch1.GetComponent<Image>().color.ToString());

                    }
                    // user2coment.
                    if (jsonObj["partner2Answer"].ToString() == "null")
                    {
                        Coment2.GetComponent<TextMeshProUGUI>().text = "연인이 답변을 기다리고있습니다.";
                    }
                    else
                    {
                        Coment2.GetComponent<TextMeshProUGUI>().text = "연인의 답변이 등록되었습니다. 답반을 작성하고 저장하면 공개됩니다."; //hideUser2Mood
                    }
                
                }
                else //user2
                {
                    // user1Mood
                    if (partner1Mood == "null")
                    {
                        Debug.LogError("1111");
                        partner1Mood = "null";
                        ChangeMoodImage(partner1Mood);
                        print("partner1Mood" + partner1Mood);
                        //moodSwitch1.GetComponent<Image>().color = Color.white;

                    }
                    else
                    {
                        Debug.LogError("2222");
                        partner1Mood = "null";
                        ChangeMoodImage(partner1Mood);
                        moodSwitch1.GetComponent<Image>().color = Color.white; //hideUser1Mood
                        //btn_moodswitch1.GetComponent<Image>.color = Color.white;//colorChange
                        //Debug.LogError("ChangeMoodColor" + moodSwitch1.GetComponent<Image>().color.ToString());
                    }
                    // user1coment
                    if (jsonObj["partner1Answer"].ToString() == "null")
                    {
                        Coment1.GetComponent<TextMeshProUGUI>().text = "아직 연인의 답변이 입력되지 않았습니다.";
                    }
                    else
                    {
                        Coment1.GetComponent<TextMeshProUGUI>().text = "연인의 답변이 등록되었습니다. 답반을 작성하고 저장하면 공개됩니다.";  //hideUser1Coment
                    }

                    // user2Mood
                    if (jsonObj["partner2Mood"].ToString() == "null")
                    {
                        partner2Mood = "null";
                    }
                    else
                    {
                        //partner2Mood = "null";
                        ChangeMoodImage(partner2Mood);
                        moodSwitch2.GetComponent<Image>().color = Color.white;
                        //btn_moodswitch1.GetComponent<Image>.color = Color.white;//colorChange
                        //Debug.LogError("ChangeMoodColor" + moodSwitch1.GetComponent<Image>().color.ToString());

                    }
                    // user2coment.
                    if (jsonObj["partner2Answer"].ToString() == "null")
                    {
                        Coment2.GetComponent<TextMeshProUGUI>().text = "작성하기를 눌러 답변을 작성하고 저장해보세요.";
                    }
                    else
                    {
                        Coment2.GetComponent<TextMeshProUGUI>().text = jsonObj["partner2Answer"].ToString();  // showUser2coment
                    }
                }
                
            }

        }

    }

    public void ViewComentInputField()//��ư�� ������ �亯 �Է��ʵ带 ��������.
    {
        string nickName;
        if (LobbyGameManager.instance.playerNickName != null)
        {
            nickName = LobbyGameManager.instance.playerNickName;
        }
        else
        {
            nickName = "�׽�Ʈ�г���";
        }

        print("�����亯�ϱ�");
        isServerComent = true; //�����亯 ������

        //���� ��ư�� �ؽ�Ʈ�� ���� �������� ����ϱ�.
        testSeverComentButton.text = "���������ϱ�";




        if (isMailComentButton) //�亯�ϱ� ������ �����ϱ� ������
        {
            mailComentButtonText.text = "�亯�ϱ�"; //�ڸ�Ʈ��ư �ؽ�Ʈ ���� �亯�ϱ�

            if (userNumber == "user1")
            {
                //mailComentText1.text = nickName + ":" + tmp_InputField.text; //������ �ڸ�Ʈ�� ù��°�� ����
                //print("1������ ��ġ�� �亯����");
                mailComentText1.text = nickName + "�亯" + ":" + tmp_InputField.text;
            }
            else
            {
                mailComentText2.text = nickName + "�亯" + ":" + tmp_InputField.text; //������ �ڸ�Ʈ�� ù��°�� ����
                //print("2������ ��ġ�� �亯����");
            }
        }
        else
        {


        }



    }

    public void HistroyView() //int histroyButtonIndex = 0;
    {

    }

    public void CheckHistoty()
    {
        print("Check LocalHistory");
        string path = Application.persistentDataPath + "/DayComentTest.json"; //����ȭ���
        if (System.IO.File.Exists(path))//�����ִ�?
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //���ڿ��� ��������
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //List�� �Ľ��ϱ�

            bool isCurrentDate = false;
            foreach (var ComentData in loadDayComenList)
            {

                //��¥�� ��ġ
                if (ComentData.date != null && ComentData.dateMission != null)
                {
                    histrotyList.Add(ComentData); //�ҷ������� historyList ���

                    string historyDate = ComentData.date; //��¥
                    string historyDateMission = ComentData.dateMission; //�̼�

                    // ��ư ���� �� ����
                    GameObject newButtonObj = Instantiate(historyButton, historyContent); // �������� Content�� �ڽ����� ����
                    Button newButton = newButtonObj.GetComponent<Button>(); //������ ��ư�� ������Ʈ ��������  
                    TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>(); // Button �ڽ�������Ʈ TextMeshProUGUI ��������

                    histroyButtonList.Add(newButton); //�����丮 ��ư ����Ʈ�� �ű� ������ ��ư�� ���

                    int buttonIndex = histroyButtonList.Count - 1; // ���� �ε��� ���� (���� ����Ʈ�� ������ �ε���)

                    //Create newButton onClick Button
                    /*newButton.onClick.AddListener(() =>
                    {
                        newButton.GetComponent<MailHistoryManager>().buttonNumber = buttonIndex;
                        Instantiate(histroyButtonList[buttonIndex], historyContent);

                    });*/

                    newButton.onClick.AddListener(() =>
                    {
                       
                    });

                    if (buttonText != null)
                    {
                        buttonText.text = "질문" + (histroyButtonList.Count) + ":" + " "+ historyDateMission;//+ "\n" + historyDate; // ��ư �ؽ�Ʈ ����

                    }
                }

            }

        }

    }

    public void NewCheckHistory()
    {
        print("Check LocalHistory");
        string path = Application.persistentDataPath + "/DayComentTest.json"; // JSON 파일 경로

        if (System.IO.File.Exists(path))
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); // JSON 파일 읽기
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); // JSON 데이터를 리스트로 변환

            foreach (var comentData in loadDayComenList)
            {
                if (comentData.date != null && comentData.dateMission != null)
                {
                    // 버튼 생성 및 설정
                    GameObject newButtonObj = Instantiate(historyButton, historyContent);
                    Button newButton = newButtonObj.GetComponent<Button>();
                    TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();

                    histroyButtonList.Add(newButton); // 생성된 버튼을 리스트에 추가
                    int buttonIndex = histroyButtonList.Count - 1; // 버튼 인덱스

                    // 버튼 클릭 시 해당 인덱스의 데이터를 반환하도록 설정
                    newButton.onClick.AddListener(() =>
                    {
                        DisplayHistory(buttonIndex); // 버튼 클릭 시 DisplayHistory 호출
                    });

                    // 버튼 텍스트 설정
                    if (buttonText != null)
                    {
                        buttonText.text = "질문 " + (buttonIndex + 1) + ": " + comentData.dateMission;
                    }
                }
            }
        }
    }

    public void DisplayHistory(int index)
    {
        if (index >= 0 && index < loadDayComenList.Count)
        {
            DayComentData data = loadDayComenList[index];
            // 데이터를 반환하거나 원하는 방식으로 사용
            Debug.Log("날짜: " + data.date + ", 미션: " + data.dateMission +", 닉네임1: " + data.user1name + ", 기분1: " + data.user1mood + ", 답변1: " + data.user1coment + ", 닉네임2: " + data.user2name + ", 기분2: " + data.user2mood + ", 답변2: " + data.user2coment);

            text_HistoryDate.text = data.date;
            text_HistoryUser1NickName.text = data.user1name;
            text_HistoryUser2NickName.text = data.user2name;
            text_HistoryUser1Coment.text = data.user1coment;
            text_HistoryUser2Coment.text = data.user2coment;

            if(data.user1mood == "null")
            {
                img_HistoryUser1Mood.sprite = moodSprites[0];
            }
            else if(data.user1mood == "Good")
            {
                img_HistoryUser1Mood.sprite = moodSprites[1];
            }
            else if (data.user1mood == "Normal")
            {
                img_HistoryUser1Mood.sprite = moodSprites[2];
            }
            else if (data.user1mood == "Bad")
            {
                img_HistoryUser1Mood.sprite = moodSprites[3];
            }

            if (data.user2mood == "null")
            {
                img_HistoryUser2Mood.sprite = moodSprites[0];
            }
            else if (data.user1mood == "Good")
            {
                img_HistoryUser2Mood.sprite = moodSprites[1];
            }
            else if (data.user2mood == "Normal")
            {
                img_HistoryUser2Mood.sprite = moodSprites[2];
            }
            else if (data.user2mood == "Bad")
            {
                img_HistoryUser2Mood.sprite = moodSprites[3];
            }


        }
    }


    public void ViewHistoryScrollview()
    {
        hoonSoundManagerLogin.PlaySound(0);
        if (!isHistoryScrollview)
        {
            hoonSoundManagerLogin.PlaySound(0); //buttonSound
            imgMoodChoiceBlackBg.SetActive(true);
            historyBGObject.SetActive(true);
            historyScrollview.gameObject.SetActive(true);
            isHistoryScrollview = true;
        }
        else
        {
            hoonSoundManagerLogin.PlaySound(1);//buttonSound
            imgMoodChoiceBlackBg.SetActive(false);
            historyBGObject.SetActive(false);
            historyScrollview.gameObject.SetActive(false);
            isHistoryScrollview = false;
        }

    }

    public void CheckMission()//���ù̼�Ȯ��
    {
        // ��¥ ���ڿ��� DateTime���� ��ȯ
        DateTime dateTime = DateTime.Parse(currentDate);

        // ����, ��, ���� ���� int�� ��ȯ
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;

        /*  // ��� ���
          Console.WriteLine("Year: " + year);
          Console.WriteLine("Month: " + month);*/
        Console.WriteLine("Day: " + day);

        // ��(Day) �κ��� ���ڿ��� ��ȯ
        string dayString = dateTime.Day.ToString("D2"); // "25" (�� �ڸ� ���� �������� ��ȯ)
        // ù ��° �ڸ��� �� ��° �ڸ��� ���� int�� ��ȯ�Ͽ� ����
        int firstDigit = int.Parse(dayString[0].ToString()); // ù ��° �ڸ� ���ڿ��� ����
        int secondDigit = int.Parse(dayString[1].ToString()); // �� ��° �ڸ� ���ڿ��� ����
        int dayCheck = firstDigit + secondDigit;
        int value = UnityEngine.Random.Range(0, 10); //�����̱�

        //�̼Ǽ����ϱ�
        if (day == 01 || day == 11 || day == 21)
        {
            todayMission = "������ ù�λ��� �˷��ּ���";
        }
        else if (day == 02 || day == 12 || day == 22)
        {
            todayMission = "���� ���ɿ� ������ �Ծ����� �˷��ּ���.";
        }
        else if (day == 03 || day == 13 || day == 23)
        {
            todayMission = "���� ������ ����� �������� �˷��ּ���.";
        }
        else if (day == 04 || day == 14 || day == 24)
        {
            todayMission = "���� ������ ���� ������ �˷��ּ���.";
        }
        else if (day == 05 || day == 15 || day == 25)
        {
            todayMission = "���� �Բ� ���� �������� ��Ҵ� ����ΰ���.";
        }
        else if (day == 06 || day == 16 || day == 26)
        {
            todayMission = "¥�� vs «�� �� ��ȣ�ϴ� ������ �˷��ּ���.";
        }
        else if (day == 07 || day == 17 || day == 27)
        {
            todayMission = "���� ��ܵ�� �뷡�� �˷��ּ���";
        }
        else if (day == 08 || day == 18 || day == 28)
        {
            todayMission = "���ΰ� �Բ����� ���� ��ȭ�� �˷��ּ���";
        }
        else if (day == 09 || day == 19 || day == 29)
        {
            todayMission = "���ΰ� �Բ� �԰����� ������ �˷��ּ���";
        }
        else if (day == 10 || day == 20 || day == 30)
        {
            todayMission = "���ο��� �����ϴ� ���Ḧ �˷��ּ���.";
        }
        else
        {
            todayMission = "���� ���谡 ����Ѵٸ� ���ο��� ����� ���� ���� �����ΰ���.";

        }

        //�̼������ϱ�
        string path = Application.persistentDataPath + "/DayComentTest.json"; //����ȭ���

        if (System.IO.File.Exists(path))  // ������ �����ϸ�
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path);
            bool isMatchDate = false;
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //���ڿ��� Json �迭�� ����

            for (int i = 0; i < loadDayComenList.Count; i++) //for������ ����Ʈ idx�� �����ϱ�.
            {


                var ComentData = loadDayComenList[i];
                if (ComentData.date == currentDate) // ��¥�� ��ġ�ϸ�
                {
                    // ��ġ�ϴ� ������ �α׸� ������ ����.
                    matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                    //Debug.Log("��¥�� ��ġ�ϴ� ������: " + matchDayComentinfo);
                    //���������� ����
                    ComentData.dateMission = todayMission;
                    // ������ �����͸� ����Ʈ�� �ݿ�
                    loadDayComenList[i] = ComentData;
                    // ����Ʈ�� JSON ���ڿ��� ��ȯ
                    string jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);
                    //Json ���·� ������ ������.
                    jsonSyncString = jsonString;

                    PhotonNetwork.RaiseEvent(DATA_SYNC_EVENT_CODE, jsonSyncString, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    Debug.Log("Photon �̺�Ʈ�� �߻��߽��ϴ�.");

                    TextMeshProUGUI dayMissionText = dayMisiionObject.GetComponent<TextMeshProUGUI>();
                    dayMissionText.text = todayMission;
                    isMatchDate = true;
                    break;

                }

            }
            if (!isMatchDate) //��ġ�ϴ� ��¥ ������ �̼ǻ����ϱ�
            {
                DayComentData dayComentData = new DayComentData //Ŭ���� ���� ������
                {
                    date = currentDate, // ���� ��¥
                    dateMission = todayMission,
                    user1name = "null",
                    user1mood = "null",
                    user1coment = "null",
                    user2name = "null",
                    user2mood = "null",
                    user2coment = "null"
                };

                loadDayComenList.Add(dayComentData); //������ �ҷ��� List�� dayComentData �� usertype �� �´� ������ �����ϱ�

                // ����Ʈ�� JSON ���ڿ��� ��ȯ
                string jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);
                // JSON ���Ϸ� ����
                System.IO.File.WriteAllText(path, jsonString);
                //Debug.Log("���� ���� �Ϸ�: " + path);
                //Debug.Log("����� JSON ������: " + jsonString);

                //UI�� �̼��ؽ�Ʈ ����
                TextMeshProUGUI dayMissionText = dayMisiionObject.GetComponent<TextMeshProUGUI>();
                dayMissionText.text = todayMission;


            }

        }
        else
        {
            DataPath.text = "������ �����ϴ�";
            //��¥�� �����ϱ� ���� ������ְ� ��������.


        }
    }

    public void CheckDate()//���ó�¥Ȯ��
    {
        // ���� ��¥�� yyyy-MM-dd ������ ���ڿ��� ��ȯ
        startDate = "2024-10-28";
        Debug.Log("coupleAnivarsary: " + startDate);

        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log("todayDate: " + currentDate);
        // ���ڿ��� DateTime �������� ��ȯ
        DateTime startDay = DateTime.Parse(startDate);
        // ���� ��¥ ��������
        DateTime currDay = DateTime.Now;

        // ��¥ ���� ���
        TimeSpan difference = currDay - startDay;
        //��¥����
        int totalDays = difference.Days + 1;
        // ������ �ϼ��� ���ڿ��� ��ȯ�Ͽ� sumDay�� ����
        string sumDay = totalDays.ToString();

        string today = "Day" + sumDay + ":" + currentDate;
        //currentDay.text = today; //���ó�¥�� ǥ��������.

        string path = Application.persistentDataPath + "/DayComentTest.json";
        if (System.IO.File.Exists(path))  // findFile
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //filePath
                                                                         //print("loadDayComentInfo" + loadDayComentInfo); //���ڿ� ����ϱ�
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);

            bool isCurrentDateMatch = false;
            for (int i = 0; i < loadDayComenList.Count; i++) //
            {
                var ComentData = loadDayComenList[i];

                if (ComentData.date == currentDate) //find MatchDate
                {
                    print("Find Match Date");
                    isCurrentDateMatch = true;
                    break;
                }

            }

            if (!isCurrentDateMatch) //UnMatchDate
            {
                print("not Find Match Date");
                DayComentData dayComentData = new DayComentData();
                {
                    dayComentData.date = currentDate;
                    dayComentData.dateMission = "null";
                    dayComentData.user1name = "null";
                    dayComentData.user1mood = "null";
                    dayComentData.user1coment = "null";
                    dayComentData.user2name = "null";
                    dayComentData.user2mood = "null";
                    dayComentData.user2coment = "null";

                }

                loadDayComenList.Add(dayComentData);

                // ����Ʈ�� JSON ���ڿ��� ��ȯ
                string jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);

                // JSON ���Ϸ� ����
                System.IO.File.WriteAllText(path, jsonString);
                Debug.Log("SaveResult" + jsonString);
               
            }

        }
    }

    public void CheckComent() //������ ������ ������ �ε����ֱ�.
    {
        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // �ڸ�Ʈ ��ǲ�ʵ� ������Ʈ
        mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>(); //�ڸ�Ʈ1 �� �ؽ�Ʈ
        mailComentText2 = Coment2.GetComponent<TextMeshProUGUI>(); //�ڸ�Ʈ2�� �ؽ�Ʈ

        //����� �����Ͱ� �ִ��� Ȯ���ϱ�
        //������ ������ �� �ʵ忡 ����� ���� �����ϱ�
        //��������� �ҷ��ɴϴ�.
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComentTest.json"; //���ð��
        string path = Application.persistentDataPath + "/DayComentTest.json"; //����ȭ���

        string fakeDate = "2010-10-22";//��¥��¥������
        if (System.IO.File.Exists(path))//�����ִ�?
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //���ڿ��� ��������
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //List�� �Ľ��ϱ�

            bool isCurrentDate = false;
            foreach (var ComentData in loadDayComenList)
            {
                print("����ȳ�¥" + ComentData.date + "���ҳ�¥" + currentDate);
                if (ComentData.date == currentDate) //��¥�� ��ġ�ϸ�
                //f(ComentData.date == fakeDate) //��¥��¥�� ��ġȮ��
                {
                    print("CheckComent, ��¥��ġ");
                    //mailComentText1.text = "�г���" + ":" + ComentData.user1name + "," + "���" + ":" + ComentData.user1mood + "," + "�亯" + ":" + ComentData.user1coment;
                    //mailComentText2.text = "�г���" + ":" + ComentData.user2name + "," + "���" + ":" + ComentData.user2mood + "," + "�亯" + ":" + ComentData.user2coment;
                    //mailComentText1.text = ComentData.user1name + "�亯" + ":" + ComentData.user1coment;
                    //mailComentText2.text = ComentData.user2name + "�亯" + ":" + ComentData.user2coment;

                    //�Ѵٴ亯�� ���
                    if (ComentData.user1coment != "null" && ComentData.user2coment != "null")
                    {
                        //�Ѵ� �亯 �������� ����ó��
                        mailComentText1.text = ComentData.user1coment;
                        mailComentText2.text = ComentData.user2coment;
                        print("�޽��� �����ع�����~");
                        isCurrentDate = true;
                        break;
                    }


                    if (userNumber == "user1" && mailComentText1.text == null) //����1, �ؽ�Ʈ1�����;���.
                    {
                        mailComentText1.text = "���� �亯�� �Է����ּ���";
                        //print("111");
                    }
                    else if (userNumber == "user1" && ComentData.user1coment == "null") //����1, �ؽ�Ʈ1null
                    {
                        mailComentText1.text = "���� �亯�� �Է����ּ���";
                        //print("112");
                    }
                    else if (userNumber == "user1" && ComentData.user1coment != "null") //����1, �ؽ�Ʈ1!null
                    {
                        //�� �亯 ���� ���� ����.
                        mailComentText1.text = mailComentText1.text = ComentData.user1coment; ;
                        //print("113" + ComentData.user1coment);
                    }

                    if (userNumber == "user1" && mailComentText2.text == null)
                    {
                        mailComentText2.text = "���� ������ �亯���� �ʾҾ��"; //����1, �ؽ�Ʈ2�����;���.
                        //print("121");
                    }
                    else if (userNumber == "user1" && ComentData.user2coment == "null")//����1, �ؽ�Ʈ2null
                    {
                        mailComentText2.text = "���� ������ �亯���� �ʾҾ��";
                        //print("122");
                    }
                    else if (userNumber == "user1" && ComentData.user2coment != "null") //����1, �ؽ�Ʈ2!null
                    {
                        mailComentText2.text = mailComentText2.text = ComentData.user2coment;
                        //print("123" + ComentData.user2coment);
                    }

                    //����2�϶�
                    if (userNumber == "user2" && mailComentText1.text == null) //����1, �ؽ�Ʈ1�����;���
                    {
                        mailComentText1.text = "���� ������ �亯���� �ʾҾ��.";
                    }
                    else if (userNumber == "user2" && ComentData.user1coment == "null") //�ؽ�Ʈ�� ������
                    {
                        mailComentText1.text = "���� ������ �亯���� �ʾҾ��.";
                    }
                    else if (userNumber == "user2" && ComentData.user1coment != "null")
                    {
                        mailComentText1.text = "�亯�� �����߽��ϴ�. ������ �亯�� �Ϸ��ϰ� �亯���⸦ ����������.";
                    }

                    if (userNumber == "user2" && mailComentText2.text == null)
                    {
                        mailComentText2.text = "���� �亯�� �Է����ּ���";
                    }
                    else if (userNumber == "user2" && ComentData.user2coment == "null")
                    {
                        mailComentText2.text = "���� �亯�� �Է����ּ���";
                    }
                    else if (userNumber == "user2" && ComentData.user2coment != "null") //����1, �ؽ�Ʈ2!null
                    {
                        mailComentText2.text = "�亯�� �����߽��ϴ�. ������ �亯�� �Ϸ��ϰ� �亯���⸦ ����������.";
                        //print("123" + ComentData.user2coment);
                    }

                    isCurrentDate = true;
                    break;

                }

            }
            if (!isCurrentDate) //��ġ�ϴ°� ������
            {
                print("��¥����ġ, �����Ͼ���");

                if (userNumber == "user1")
                {
                    print("user1 ��¥����");
                    mailComentText1.text = "���� �亯�� �Է����ּ���";
                    mailComentText2.text = "������ �亯�� �Է����� �ʾҾ��";
                }
                else if (userNumber == "user2")
                {
                    print("user2 ��¥����");
                    mailComentText2.text = "���� �亯�� �Է����ּ���";
                    mailComentText1.text = "������ �亯�� �Է����� �ʾҾ��";
                }

            }
        }
    }


    public void CheckMood() //��л���Ȯ��
    {

        //�̹����� ������� Ȯ���ϱ�
        foreach (var ComentData in loadDayComenList)
        {
            if (ComentData.date == currentDate) //��¥�� ��ġ�ϸ�
            //if(ComentData.date == fakeDate) //��¥��¥�� ��ġȮ��
            {
                print("��¥��ġ, ����������");
                Image img1 = moodSwitch1.GetComponent<Image>();
                Image img2 = moodSwitch2.GetComponent<Image>();

                if (ComentData.user1coment != "null" && ComentData.user2coment != "null")
                {
                    print("둘다값이 있으니 보여주자.");
     
                }

                if (ComentData.user1mood == "null")
                {
                    img1.sprite = moodSprites[0];
                    textMyMood = "null";
                    print("textMyMood" + textMyMood);
                }
                else if (ComentData.user1mood == "Good")
                {
                    //chage switch mood
                    img1.sprite = moodSprites[1];
                    textMyMood = "Good";
                    print("textMyMood" + textMyMood);
                }
                else if (ComentData.user1mood == "Normal")
                {
                    img1.sprite = moodSprites[2];
                    textMyMood = "Normal";
                    print("textMyMood" + textMyMood);
                }
                else if (ComentData.user1mood == "Bad")
                {
                    img1.sprite = moodSprites[3];
                    textMyMood = "Bad";
                    print("textMyMood" + textMyMood);
                }

                print("1�� �̹��� ������� Ȯ���ϱ�" + ComentData.user1mood);


                if (ComentData.user2mood == "null")
                {
                    img2.sprite = moodSprites[0];
                    textMyMood = "null";
                }
                else if (ComentData.user2mood == "Good")
                {
                    img2.sprite = moodSprites[1];
                    textMyMood = "Good";
                }
                else if (ComentData.user2mood == "Normal")
                {
                    img2.sprite = moodSprites[2];
                    textMyMood = "Normal";
                }
                else if (ComentData.user2mood == "Bad")
                {
                    img2.sprite = moodSprites[3];
                    textMyMood = "Bad";
                }

                print("user2 �̹��� ������� Ȯ���ϱ�");
                break;

            }
            else
            {
                print("CheckMood, ��ġ�ϴ� ��¥ ����, ��ġ�ϴ� �̹����� ����.");
            }
        }
    }

    public void CheckMail()//�̼�, ���, �亯�� �ִ��� Ȯ��
    {
        CheckMission();
        CheckMood();
        CheckComent();
    }

    public void GetCheckMission() //�������� �̼��� �޾ƿ���. 
    {
        //findplayer
        StartCoroutine(GetTodayMission());

    }
    
    public void SaveDayComentJsonTest()
    {
        string nickName = LobbyGameManager.instance.playerNickName; //�г��� ĳ��
        string jsonString;
        DayComentData dayComentData; //Ŭ���� ���� ����

        if (isMailComentButton) //�亯�ϱ� ������ �����ϱ� ������
        {
            mailComentButtonText.text = "저장하기"; //�ڸ�Ʈ��ư �ؽ�Ʈ ���� �亯�ϱ�

            if (userNumber == "user1")
            {
                //mailComentText1.text = nickName + ":" + tmp_InputField.text; //������ �ڸ�Ʈ�� ù��°�� ����
                //print("1������ ��ġ�� �亯����");
                mailComentText1.text = nickName + "�亯" + ":" + tmp_InputField.text;
            }
            else
            {
                mailComentText2.text = nickName + "�亯" + ":" + tmp_InputField.text; //������ �ڸ�Ʈ�� ù��°�� ����
                //print("2������ ��ġ�� �亯����");
            }

            //�亯�� ��������.
            //��� ������ �ҷ�����.
            //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComentTest.json"; //���ð��
            string path = Application.persistentDataPath + "/DayComentTest.json"; //����ȭ���
            if (System.IO.File.Exists(path))  // ������ �����ϸ�
            {
                string loadDayComentInfo = System.IO.File.ReadAllText(path); //���ڿ��� ��������
                //print("loadDayComentInfo" + loadDayComentInfo); //���ڿ� ����ϱ�
                loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
                //Debug.Log("loadedDataList" + loadDayComenList);

                bool isMatched = false; // ��ȿ���˻� ����

                for (int i = 0; i < loadDayComenList.Count; i++) //for������ ����Ʈ idx�� �����ϱ�.
                {
                    var ComentData = loadDayComenList[i];

                    if (ComentData.date == currentDate) // ��¥�� ��ġ�ϸ�
                    {
                        // ��ġ�ϴ� ������ �α׸� ������ ����.
                        matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                        //Debug.Log("��¥�� ��ġ�ϴ� ������: " + matchDayComentinfo);
                        isMatched = true; //��ȿ���˻�

                        // ���� �����͸� ����
                        if (userNumber == "user1")
                        {
                            //Debug.Log("��ġ�ϴ� ������ ����, user1 ���� ������Ʈ");
                            ComentData.user1name = playerNicknameMgr1.nickNameComp.text;
                            ComentData.user1mood = moodText1;
                            ComentData.user1coment = mailComentText1.text;
                        }
                        else
                        {
                            //Debug.Log("��ġ�ϴ� ������ ����, user2 ���� ������Ʈ");
                            ComentData.user2name = playerNicknameMgr2.nickNameComp.text;
                            ComentData.user2mood = moodText2;
                            ComentData.user2coment = mailComentText2.text;
                        }

                        // ������ �����͸� ����Ʈ�� �ݿ�
                        loadDayComenList[i] = ComentData;

                        // ����Ʈ�� JSON ���ڿ��� ��ȯ
                        jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);

                        //Json ���·� ������ ������.
                        jsonSyncString = jsonString;

                        // JSON ���Ϸ� ����
                        //File.WriteAllText(path, jsonString); //��������
                        //Debug.Log("���� ���� �Ϸ�: " + path);
                        //Debug.Log("����� JSON ������: " + jsonString);

                        PhotonNetwork.RaiseEvent(DATA_SYNC_EVENT_CODE, jsonSyncString, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                        Debug.Log("Photon �̺�Ʈ�� �߻��߽��ϴ�.");
                        break; //for�� ����

                    }


                }

                if (!isMatched) //��ġ�ϴ� �����Ͱ� ������ ���� �����ؼ� �ֱ�
                {

                    if (userNumber == "user1") //����1 �̸�
                    {
                        dayComentData = new DayComentData //Ŭ���� ���� ������
                        {
                            date = currentDate, // ���� ��¥
                            dateMission = todayMission,
                            user1name = playerNicknameMgr1.nickNameComp.text,
                            user1mood = "Good",
                            user1coment = mailComentText1.text,
                            user2name = "null",
                            user2mood = "null",
                            user2coment = "null"
                        };

                    }
                    else //����2 �̸�
                    {
                        dayComentData = new DayComentData //Ŭ���� ���� ������
                        {
                            date = currentDate, // ���� ��¥
                            dateMission = todayMission,
                            user1name = "null",
                            user1mood = "null",
                            user1coment = "null",
                            user2name = playerNicknameMgr2.nickNameComp.text,
                            user2mood = "Good",
                            user2coment = mailComentText2.text,
                        };
                    }
                    loadDayComenList.Add(dayComentData); //������ �ҷ��� List�� dayComentData �� usertype �� �´� ������ �����ϱ�

                    // ����Ʈ�� JSON ���ڿ��� ��ȯ
                    jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);

                    // JSON ���Ϸ� ����
                    System.IO.File.WriteAllText(path, jsonString);
                    Debug.Log("���� ���� �Ϸ�: " + path);
                    Debug.Log("����� JSON ������: " + jsonString);

                }

            }

            tmp_InputFieldObject.SetActive(false);  //��ǲ�ʵ� ����
            isMailComentButton = false;


        }
        else //�亯�ϱ⸦ ������
        {

            tmp_InputFieldObject.SetActive(true); //��ǲ�ʵ� ����.
            if (userNumber == "user1")
            {
                mailComentText1.text = nickName + ":" + "�亯";
                print("1������ ��ġ�� �亯����");
            }
            else
            {
                mailComentText2.text = nickName + ":" + "�亯";
                print("2������ ��ġ�� �亯����");
            }

            mailComentButtonText.text = "�����ϱ�"; //�ڸ�Ʈ��ư �ؽ�Ʈ�� �����ϱ�� ����
            isMailComentButton = true;

        }
    }

    public void SaveServerDayComentQuaryParameter()
    {
        // jsonData
        //string jsonData = "{\"missionNumber\":1,\"missionDate\":[2024,11,11],\"missionContent\":\"���ο��� ���� ������ ���� �����ΰ���? �� ������ �����ΰ���?\",\"partner1Mood\":\"Good\",\"partner1Answer\":\"����\",\"partner2Mood\":\"Good\",\"partner2Answer\":\"ŭ\",\"completed\":true}";
        // jsonObjectParse
        //JObject jsonObject = JObject.Parse(jsonData);

        if (!isButtonSaveComentSever)
        {
            hoonSoundManagerLogin.PlaySound(0);
            if (userNumber =="user1")
            {
                TouchMoodSwitchButton(1);
            }
            else
            {
                TouchMoodSwitchButton(2);
            }

            btnText_MailServerComent.text = "제출하기"; //btn text change save
            tmp_InputFieldObject.SetActive(true); //Active inputField
            isButtonSaveComentSever = !isButtonSaveComentSever; //chage true
            //print("isButtonSaveComentSever" + isButtonSaveComentSever);

        }
        else
        {
            //string coment1 = tmp_InputFieldObject.GetComponent<TextMeshPro>().text;
            hoonSoundManagerLogin.PlaySound(1);
            if (userNumber == "user1")
            {
                /* if(Coment1 != null)
                 {
                     print("coment" + Coment1.GetComponent<TextMeshProUGUI>().text);
                     print("playerNickName" + LoginInfoManager.instance.nickName);
                     print("답변" + tmp_InputFieldObject.GetComponent<TextMeshPro>().text);
                 }*/

                Coment1.GetComponent<TextMeshProUGUI>().text = LoginInfoManager.instance.nickName + "답변" + ":" + tmp_InputFieldObject.GetComponent<TMP_InputField>().text;

            }
            else
            {
                /* print("coment" + Coment2.GetComponent<TextMeshProUGUI>().text);
                 //print("playerNickName" + LoginInfoManager.instance.nickName);
                 if(tmp_InputFieldObject != null)
                 {
                     print("답변" + tmp_InputFieldObject.GetComponent<TMP_InputField>().text);
                 }*/

                Coment2.GetComponent<TextMeshProUGUI>().text = LoginInfoManager.instance.nickName + "답변" + ":" + tmp_InputFieldObject.GetComponent<TMP_InputField>().text;

            }


            btnText_MailServerComent.text = "답변하기"; //btn text change Coment
            tmp_InputFieldObject.SetActive(false); //Active inputField
            isButtonSaveComentSever = !isButtonSaveComentSever; //chage false
            //print("isButtonSaveComentSever" + isButtonSaveComentSever);

            //PostSever
            string myMood = "null";
            string myComent = "null";
            if (userNumber =="user1")
            {
                 myComent = Coment1.GetComponent<TextMeshProUGUI>().text; //user1coment
            }
            else
            {
                myComent = Coment2.GetComponent<TextMeshProUGUI>().text; //user2coment
            }
            
            if (textMyMood != "null")
            {
                myMood = textMyMood;
                print("myComent" + myComent + "myMood" + myMood);
                if(myMood == "")
                {
                    myMood = "null";
                }

                if(myMood != "null")
                {
                    // 쿼리 파라미터를 포함한 URL 생성
                    string baseUrl = "http://125.132.216.190:12223/api/missions/answer";
                    string url = $"{baseUrl}?mood={UnityWebRequest.EscapeURL(myMood)}&answer={UnityWebRequest.EscapeURL(myComent)}";
                    string url1 = $"{baseUrl}?mood={UnityWebRequest.EscapeURL(myMood)}&answer={UnityWebRequest.EscapeURL(myComent)}";
                    StartCoroutine(PostDayComentQuaryParameter(url1));
                }
                else
                {
                    print("감정표현을 선택해주세요.");
                    if(userNumber == "user1")
                    {
                        //Img_MoodChoice1 오브젝트를 보여주자.
                        moodChoice1Object.SetActive(true);
                    }
                    else
                    {
                        //Img_MoodChoice2 오브젝트를 보여주자.
                        moodChoice2Object.SetActive(true);
                    }
                    return;
                }
                              
            }
            else 
            {
                print("감정표현을 선택해주세요.");
                return;

            }
           
        }

    }

   
    IEnumerator PostDayComentQuaryParameter(string url)
    {
        print("서버에 요청시작");
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Authorization", "Bearer " + LoginInfoManager.instance.myToken); //Bearer에 공백 있어야함. 서버로 토큰 발사
        //print("내토큰" + LoginInfoManager.instance.myToken);
        print("서버에 요청중");

        yield return request.SendWebRequest();

        // 요청 결과 처리
        if (request.result == UnityWebRequest.Result.Success) //성공이니?
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            Debug.Log("Response Code: " + request.responseCode);
            Debug.Log("Response Body: " + request.downloadHandler.text);
        }
        else //응 아니야~
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("Response Code: " + request.responseCode);
            Debug.LogError("Response Body: " + request.downloadHandler.text);

            if (request.responseCode == 409)
            {
                //put하는 
            }

        }
    }

    public void RestDayComent()
    {
        string myMood = "null";
        string myComent = "null";
        // 쿼리 파라미터를 포함한 URL 생성
        string baseUrl = "http://125.132.216.190:12223/api/missions/answer";
        string url = $"{baseUrl}?mood={UnityWebRequest.EscapeURL(myMood)}&answer={UnityWebRequest.EscapeURL(myComent)}";
        string url1 = $"{baseUrl}?mood={UnityWebRequest.EscapeURL(myMood)}&answer={UnityWebRequest.EscapeURL(myComent)}";
        StartCoroutine(PostDayComentQuaryParameter(url));
        StartCoroutine(PostDayComentQuaryParameter(url1));

    }

    public void LoadDayComentJson()
    {
        //��� ������ �ҷ�����.
        string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";

        if (System.IO.File.Exists(path))  // ������ �����ϴ��� Ȯ��
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //���ڿ��� ��������
            print("�ε����̽�" + loadDayComentInfo);
            print("���ó�¥" + currentDate);

            // JSON ���ڿ��� DayCommentData ��ü�� ����Ʈ�� �Ľ�
            //List<DayComentData> loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
            Debug.Log("loadedDataList" + loadDayComenList);

            CheckDayComentJsonDate(); //��¥������ ��ġ�ϴ��� Ȯ���ϰ� ������ �����ؼ� ������ ����

        }

        else
        {
            CreateNewDayComentJsonArray(); //���Ͼ����� ���� ����

        }

    }

    public void CheckDayComentJsonDate()
    {
        // ��¥ Ȯ�� �� �α� ���
        foreach (var ComentData in loadDayComenList)
        {
            if (ComentData.date == currentDate) //��¥�� ��ġ�ϸ�
            {
                // ��ġ�ϴ� ������ �α׸� ����������.
                //string matchDayComentinfo = JsonConvert.SerializeObject(dayComentData, Formatting.Indented);
                matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                Debug.Log("��¥�� ��ġ�ϴ� ������: " + matchDayComentinfo);
                break;

            }
            else //��ġ�ϴ� �����Ͱ� ������ ���� �����ؼ� �ֱ�
            {


                DayComentData dayComentData = new DayComentData
                {
                    date = currentDate, // ���� ��¥
                    user1name = "null",
                    user1mood = "null",
                    user1coment = "null",
                    user2name = "null",
                    user2mood = "null",
                    user2coment = "null"
                };

            }

        }
    }

    public void CreateNewDayComentJsonArray() //���̽� �迭�� �����ϱ�
    {
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";//���ð��
        string path = Application.persistentDataPath + "/DayComentTest.json"; //����ȭ���

        // DayComentData ��ü ����
        DayComentData dayComentData = new DayComentData
        {
            date = currentDate, // ���� ��¥
            dateMission = "null",
            user1name = "null",
            user1mood = "null",
            user1coment = "null",
            user2name = "null",
            user2mood = "null",
            user2coment = "null"
        };

        // DayComentData�� JSON ���ڿ� �迭�� ����������
        string jsonString = JsonConvert.SerializeObject(new[] { dayComentData }, Formatting.Indented);

        // JSON ���Ϸ� ����
        System.IO.File.WriteAllText(path, jsonString);
        Debug.Log("���� ���� �Ϸ�: " + path);
        Debug.Log("����� JSON ������: " + jsonString);


    }

    public void CreateNewDayComentJsonArray(string mood) //���̽� �迭�� �����ϱ�
    {
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";//���ð��
        string path = Application.persistentDataPath + "/DayComentTest.json"; //����ȭ���

        string jsonString = "";
        if (System.IO.File.Exists(path)) //���� �ִ�?
        {
            //���������� ������ �ҷ�����.
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //���ڿ��� ��������

            // JSON ���ڿ��� DayCommentData ��ü�� ����Ʈ�� �Ľ�
            //List<DayComentData> loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
            Debug.Log("loadedDataList" + loadDayComenList);

            DayComentData dayComentData;
            if (userNumber == "user1")
            {
                print("ChangeMoodImage userNumber" + userNumber);
                // DayComentData ��ü ����
                dayComentData = new DayComentData
                {
                    date = currentDate, // ���� ��¥
                    dateMission = "null",
                    user1name = "null",
                    user1mood = mood,
                    user1coment = "null",
                    user2name = "null",
                    user2mood = "null",
                    user2coment = "null"
                };
            }
            else
            {
                print("ChangeMoodImage �����ѹ�2" + userNumber);
                // DayComentData ��ü ����
                dayComentData = new DayComentData
                {
                    date = currentDate, // ���� ��¥
                    dateMission = "null",
                    user1name = "null",
                    user1mood = "null",
                    user1coment = "null",
                    user2name = "null",
                    user2mood = "null",
                    user2coment = mood
                };
            }
            //�����迭�� �ű� �迭�߰��Ͽ� ����
            loadDayComenList.Add(dayComentData);

            // DayComentData�� JSON ���ڿ� �迭�� ����������
            jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);


        }
        else //���Ͼ���
        {
            DayComentData dayComentData;
            if (userNumber == "user1")
            {
                // DayComentData ��ü ����
                dayComentData = new DayComentData
                {
                    date = currentDate, // ���� ��¥
                    dateMission = "null",
                    user1name = "null",
                    user1mood = mood,
                    user1coment = "null",
                    user2name = "null",
                    user2mood = "null",
                    user2coment = "null"
                };
            }
            else
            {
                // DayComentData ��ü ����
                dayComentData = new DayComentData
                {
                    date = currentDate, // ���� ��¥
                    dateMission = "null",
                    user1name = "null",
                    user1mood = mood,
                    user1coment = "null",
                    user2name = "null",
                    user2mood = "null",
                    user2coment = mood
                };

                // DayComentData�� JSON ���ڿ� �迭�� ����������
                jsonString = JsonConvert.SerializeObject(new[] { dayComentData }, Formatting.Indented);

            }
        }




        // JSON ���Ϸ� ����
        System.IO.File.WriteAllText(path, jsonString);
        Debug.Log("SavePath: " + path);
        Debug.Log("SaveJsonString: " + jsonString);


    }

    public void CreateNewDayComentJson()
    {
        string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";

        // ������ ������ �� ���� ����
        Debug.Log("������ �������� �ʾ� ���� �����մϴ�.");

        // ���� ��¥�� yyyy-MM - dd �������� ��������
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // �⺻ ������ ��ü ����
        DayComentData dayComent = new DayComentData
        {
            date = "null",
            user1name = "null",
            user1mood = "null",
            user1coment = "null",
            user2name = "null",
            user2mood = "null",
            user2coment = "null"
        };

        // Ŭ������ JSON ���ڿ��� ��ȯ (����ȭ)
        string jsonString = JsonUtility.ToJson(dayComent, true);

        // ���� ���� �� �⺻ ���� ����
        System.IO.File.WriteAllText(path, jsonString);
        Debug.Log("���� ���� �Ϸ�: " + path);
    }

    public void ViewInputMailComent()
    {

        //print("ViewInputMailComent");
        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // �ڸ�Ʈ ��ǲ�ʵ� ������Ʈ
        mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>(); //�ڸ�Ʈ1 �� �ؽ�Ʈ
        if (isMailComentButton) //�亯�ϱ� ������ �������� ������
        {
            mailComentButtonText.text = "�亯�ϱ�"; //�ڸ�Ʈ��ư �ؽ�Ʈ�� ����
            //print("�Է¸� �ڸ�Ʈ" +tmp_InputField.text); //�ؽ�Ʈ ����غ���
            mailComentText1.text = currentDate + "\n" + "���Ǵ亯:" + tmp_InputField.text; //������ �ڸ�Ʈ�� ù��°�� ����
            tmp_InputFieldObject.SetActive(false);  //��ǲ�ʵ� ����
            isMailComentButton = false;

        }
        else //�亯�ϱ⸦ ������
        {
            tmp_InputFieldObject.SetActive(true); //��ǲ�ʵ� ����.
            mailComentText1.text = "���Ǵ亯:";
            mailComentButtonText.text = "�����ϱ�"; //�����ϱ� ��ư���� ����
            isMailComentButton = true;

        }

    }

    private void OnTriggerEnter(Collider other)
    {
        print("FindplayertoRange");
        if (other.gameObject.name.Contains("Player")) //���ӿ�����Ʈ�� �÷��̾ �����ϰ� �ִٸ�
        {
            print("showMailImage");
            mail_IconImage.gameObject.SetActive(enabled);

            //��ư�� �������� �Լ� ȣ��, �ѹ��� ȣ���ϰ� ����.
            touchButton.onClick.AddListener(WithInRangeViewMailImageControll);

        }
    }

    /*private void OnTriggerStay(Collider other)
    {

    }*/

    private void OnTriggerExit(Collider other)
    {
        //��ư�� �������� �Լ� ȣ�� 
        touchButton.onClick.RemoveListener(WithInRangeViewMailImageControll);
        mail_IconImage.gameObject.SetActive(false);
        print("�̹��� ����");
    }

    public void OpenMailUI(GameObject obj)//������Ʈ ����
    {
        obj.SetActive(true);
    }

    public void CloseMailUI(GameObject obj)//closeMailUIObject
    {
        obj.SetActive(false);
       
        hoonSoundManagerLogin.PlaySound(1); //buttonSound
        isMailImage = !isMailImage; // mailRangeReset

        if (obj.name == "Img_MoodChoice1")
        {
            imgMoodChoiceBlackBg.SetActive(false);
        }

        if (obj.name == "Img_MoodChoice2")
        {
            imgMoodChoiceBlackBg.SetActive(false);

        }
    }

    public void WithInRangeViewMailImageControll()
    {
        isMailImage = !isMailImage;

        if (!isMailImage)
        {
            
            hoonSoundManagerLogin.PlaySound(1); //buttonSound
            mail_ImageObject.SetActive(false);
        }
        else
        {
            hoonSoundManagerLogin.PlaySound(0); //buttonSound
            mail_ImageObject.SetActive(true);
        }
    }

    public void TouchMoodSwitchButton(int switchNum)
    {
        
        if (switchNum == 1 && userNumber == "user1")
        {
            hoonSoundManagerLogin.PlaySound(0); //buttonSound
            imgMoodChoiceBlackBg.SetActive(true);
            isMoodSwihtch1 = !isMoodSwihtch1;
            OpenMailUI(moodChoice1Object);
            print("maleMoodSwitch");
            moodSwitch1.GetComponent<Image>().color = Color.white; //chage switch color



        }
        else if (switchNum == 2 && userNumber == "user2")
        {
            hoonSoundManagerLogin.PlaySound(0); //buttonSound
            imgMoodChoiceBlackBg.SetActive(true);
            isMoodSwihtch2 = !isMoodSwihtch2;
            OpenMailUI(moodChoice2Object);
            print("femaleMoodSwitch");
            moodSwitch2.GetComponent<Image>().color = Color.white; //chage switch color
        }
        else
        {
            hoonSoundManagerLogin.PlaySound(1); //buttonSound
        }

    }

    public void ChangeMoodImage(string mood) // when choice moodBtoon change moodImage
    {
        Image img1 = moodSwitch1.GetComponent<Image>();
        Image img2 = moodSwitch2.GetComponent<Image>();

        //print("switch image cashing");

        hoonSoundManagerLogin.PlaySound(0); //buttonSound
        if (userNumber == "user1")
        {
            print("checkUserNumber" + userNumber);
            //��й迭���ִ¸�� �׸��� �����ϰ� ����.
            for (int i = 1; i < moodChoice1ButtonImageList.Count; i++)
            {
                moodChoice1ButtonImageList[i].color = new Color(1, 1, 1, 0.4f);
            }

            //1���̹��� ����
            if (mood == "Good")
            {
                img1.sprite = moodSprites[1];
                moodChoice1ButtonImageList[1].color = new Color(1, 1, 1, 1);
                textMyMood = "Good";
                print("textMyMood" + textMyMood);
            }
            else if (mood == "Normal")
            {
                img1.sprite = moodSprites[2];
                moodChoice1ButtonImageList[2].color = new Color(1, 1, 1, 1);
                textMyMood = "Normal";
                print("textMyMood" + textMyMood);
            }
            else if (mood == "Bad")
            {
                img1.sprite = moodSprites[3];
                moodChoice1ButtonImageList[3].color = new Color(1, 1, 1, 1);
                textMyMood = "Bad";
                print("textMyMood" + textMyMood);
            }

        }
        else
        {

            print("checkUserNumber" + userNumber);
            //��й迭���ִ¸�� �׸��� �����ϰ� ����.
            for (int i = 1; i < moodChoice2ButtonImageList.Count; i++)
            {
                moodChoice2ButtonImageList[i].color = new Color(1, 1, 1, 0.4f);
            }

            //2���̹��� ����
            if (mood == "Good")
            {
                textMyMood = "Good";
                img2.sprite = moodSprites[1];
                moodChoice2ButtonImageList[1].color = new Color(1, 1, 1, 1);
            }
            else if (mood == "Normal")
            {
                textMyMood = "Normal";
                img2.sprite = moodSprites[2];
                moodChoice2ButtonImageList[2].color = new Color(1, 1, 1, 1);
            }
            else if (mood == "Bad")
            {
                textMyMood = "Bad";
                img2.sprite = moodSprites[3];
                moodChoice2ButtonImageList[3].color = new Color(1, 1, 1, 1);

            }
            
        }
        print("myChoiceMood" + mood);

        //내껄먼저 설정하고 상대방기분을 설정하기
        
        
        
        
        
        //��� ������ �ҷ�����.
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComentTest.json"; //���ð��
       /* string path = Application.persistentDataPath + "/DayComentTest.json"; //��ũ���
        bool isCurrentDate = false;
        for (int i = 0; i < loadDayComenList.Count; i++) // ��¥ Ȯ�� �� �α� ���
        {
            var ComentData = loadDayComenList[i];


            if (ComentData.date == currentDate) // ��¥�� ��ġ�ϸ�
            {
                // check match date.
                matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                Debug.Log("checkMatchDay: " + matchDayComentinfo);

                if (userNumber == "user1")
                {
                    print("ChangeMoodImage user1" + userNumber);
                    //1���̹�������
                    if (mood == "Good")
                    {
                        ComentData.user1mood = "Good";
                        textMyMood = "Good";
                    }
                    else if (mood == "Normal")
                    {
                        ComentData.user1mood = "Normal";
                        textMyMood = "Normal";
                    }
                    else if (mood == "Bad")
                    {
                        ComentData.user1mood = "Bad";
                        textMyMood = "Bad";
                    }

                    moodText1 = mood;
                }
                else
                {
                    print("ChangeMoodImage userNumer" + userNumber);
                    //2���̹��� ����
                    if (mood == "Good")
                    {
                        ComentData.user2mood = "Good";
                    }
                    else if (mood == "Normal")
                    {
                        ComentData.user2mood = "Normal";
                    }
                    else if (mood == "Bad")
                    {
                        ComentData.user2mood = "Bad";
                    }
                    moodText2 = mood;
                }

            }


            // ������ �����͸� ����Ʈ�� �ݿ�
            loadDayComenList[i] = ComentData;

            // ����Ʈ�� JSON ���ڿ��� ��ȯ
            //string jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented); ��������
            // JSON ���Ϸ� ����
            //File.WriteAllText(path, jsonString); //��������
            //Debug.Log("���� ���� �Ϸ�: " + path);
            //Debug.Log("����� JSON ������: " + jsonString);

            jsonSyncString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented); //��ũ����
            PhotonNetwork.RaiseEvent(DATA_SYNC_EVENT_CODE, jsonSyncString, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
            //Debug.Log("Photon ?");
            isCurrentDate = true;
            break;
        }
        if (!isCurrentDate)
        {
            print("noneMatchDate");
            CreateNewDayComentJsonArray(mood);

        }*/
    
    }//함수끝

    IEnumerator FindPlayer()//playerFind afterSpawn
    {
        yield return new WaitForSeconds(0.1f);

        player1 = GameObject.Find("PlayerMale(Clone)"); //�÷��̾�� ���ھƹ�Ÿ
        player2 = GameObject.Find("PlayerWoman(Clone)"); //�÷��̾�� ���ھƹ�Ÿ

        //only user1, only user2,
        if (player1 != null)
        {
            playerNicknameMgr1 = player1.GetComponent<PlayerNicknameManager>();
        }

        if (player2 != null)
        {
            playerNicknameMgr2 = player2.GetComponent<PlayerNicknameManager>();
        }

        if (player1 != null && player2 == null) // user1
        {
            playerNicknameMgr1 = player1.GetComponent<PlayerNicknameManager>();
            userNumber = "user1";  // male user1
            print("FindPlayer male: " + userNumber);

        }
        else if (player2 != null && player1 == null) // user2
        {
            playerNicknameMgr2 = player2.GetComponent<PlayerNicknameManager>();
            userNumber = "user2";  // female user2
            print("FindPlayer female " + userNumber);

        }
        else if (player1 != null && player2 != null) // both user1 user2
        {

            if (PhotonNetwork.LocalPlayer.ActorNumber == player1.GetComponent<PhotonView>().Owner.ActorNumber)
            {
                // ���ڰ� ���� ���� �÷��̾��� ��
                userNumber = "user1";  // ���ڴ� �׻� user1
                print("FindPlayer ���� ���� ����, ������ȣ: user1");
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == player2.GetComponent<PhotonView>().Owner.ActorNumber)
            {
                // ���ڰ� ���� ���� �÷��̾��� ��
                userNumber = "user2";  // ���ڴ� �׻� user2
                print("FindPlayer ���� ���� ����, ������ȣ: user2");
            }

        }

        if (!player1 && !player2)//���ڿ��� �Ѵپ���
        {
            //Debug.LogError("�÷��̾� ����");
            print("�÷��̾� ����");
        }

        //�����ư����
        /* if (userNumber == "user1")
         {
             moodButton2.SetActive(false);
             print("�����ѹ�1 2�������ư����");
         }
         else
         {
             moodButton1.SetActive(false);
             print("�����ѹ�2 1�������ư����");
         }*/

        //local only
        CheckDate(); //load date local
        //CheckMission(); //load mission local
        //CheckComent(); //load coment local
        //CheckMood(); //load mood local
        GetCheckMission();
        //CheckHistoty();
        NewCheckHistory();
       
       
    }

    private void SaveStringAsJson(string data) // ���ڿ��� JSON �������� ���� ��ο� �����ϴ� �޼���
    {
        // JSON ����ȭ (�ܼ� string�� JSON ������ ��ȯ�ϴ� ���)
        //var jsonData = new { syncedData = data };

        // ����ȭ�� JSON �����͸� ���ڿ��� ��ȯ
        //string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

        // ���� ���Ͽ� ����
        //File.WriteAllText(jsonSyncPath, jsonString);
        System.IO.File.WriteAllText(jsonSyncPath, data);

        //Debug.Log("Data saved as JSON at: " + jsonSyncPath);
    }

    private void OnEnable()  // �̺�Ʈ�� �޴� �޼���
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)  // Photon ��Ʈ��ũ���� �ݹ� Ÿ�� ���
    {
        // �츮�� ������ �̺�Ʈ���� Ȯ��
        if (photonEvent.Code == DATA_SYNC_EVENT_CODE)
        {
            // �̺�Ʈ ������ �ޱ� (string ���¶�� ����)
            string jsonString = (string)photonEvent.CustomData;

            //Debug.Log("Received data: " + jsonString);

            // ���� �����͸� JSON���� ����
            SaveStringAsJson(jsonString);
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);  // �̺�Ʈ �ݹ� ����
    }


}//Ŭ���� ��
