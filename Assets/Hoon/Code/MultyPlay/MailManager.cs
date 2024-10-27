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
//claaback이벤트
using ExitGames.Client.Photon;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine.Rendering.LookDev;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Converters;
using Photon.Pun.Demo.Cockpit;
using System.Reflection;

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

public class MailManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject mail_IconObject; //메일아이콘오브젝트
    public GameObject mail_ImageObject; //메일미션이미지
    public Button touchButton; //터치버튼
    public TextMeshProUGUI currentDay; //날짜표시
    public GameObject moodChoiceObject; //오늘의기분 변경 오브젝트
    //public Button moodSwitch; //버튼기분변경
    public Button moodSwitch1;
    public Button moodSwitch2;
    public Image moodChice1; // 기분변경 BG
    public Image moodChice2;
    public Button moodGood; // 버튼 기분좋음
    public Button moodNormal; //버튼 기분중간
    public Button moodBad; //버튼 기분나쁨
    public Sprite[] moodSprites; //기분이미지 배열
    public GameObject tmp_InputFieldObject; //코멘트 인풋
    public Button mailComentButton; //코멘트 인풋 열기
    public GameObject mailComentTestObject; //메일코멘트
    public GameObject dayMisiionObject;
    public GameObject Coment1;
    public GameObject Coment2;
    public string startDate; //시작일 지정변수
    public string userNumber;

    GameObject player1;
    GameObject player2;
    Image mail_IconImage;
    bool isMailImage = false;
    bool isMoodSwihtch1 = false;
    bool isMoodSwihtch2 = false;
    bool isMailComentButton = false;
    TMP_InputField tmp_InputField;
    TextMeshProUGUI mailComentButtonText;
    bool isMailComentSave = false;
    TextMeshProUGUI mailComentText1;
    TextMeshProUGUI mailComentText2;
    bool isMailComent1 = false;
    bool isMainComent2 = false;

    string currentDate; //오늘 날짜를 저장할 변수
    string playerNickName; //닉네임 저장 변수

    List<DayComentData> loadDayComenList; //로드한 데이터를 저장하는 리스트
    string matchDayComentinfo; //리스트에서 일치하는 데이터를 저장할 문자열

    PlayerNicknameManager playerNicknameMgr1;
    PlayerNicknameManager playerNicknameMgr2;

    // 포톤을 통해서 이벤트롤 보내자.
    // 포톤에서 받을 이벤트 코드 (예: 100)
    private const byte DATA_SYNC_EVENT_CODE = 100;
    // JSON 파일이 저장될 로컬 경로
    public string jsonSyncPath;
    //public string jsonSyncPath = Application.persistentDataPath + "/DayComentTest.json";
    public string jsonSyncString;
    public string todayMission;
    public TextMeshProUGUI DataPath;

    void Start()
    {
        jsonSyncPath = Application.persistentDataPath + "/DayComentTest.json";
        if (File.Exists(jsonSyncPath))
        {
            print("json파일있음");
        }
        else
        {
            print("json파일없음");
            CreateNewDayComentJsonArray();

        }
        //PhotonNetwork.AddCallbackTarget(this);  // 이벤트 콜백 등록
        //Debug.Log(Application.persistentDataPath);
        //DataPath.text = Application.persistentDataPath;
        
       //반환값이 int
        
        string playerList = "참가한 플레이어 " + PhotonNetwork.PlayerList.Length; //+ "\n" + // + "\n" +
        string roomName= "";
        string nickName = "";

        if (PhotonNetwork.CurrentRoom != null)
        {
             roomName = "방이름 " + PhotonNetwork.CurrentRoom.Name;
        }
        else
        {
            roomName = "방이름없음" ;
        }
        
        if (PhotonNetwork.LocalPlayer.NickName != null)
        {
            nickName = "닉네임" + PhotonNetwork.LocalPlayer.NickName;
        }

        DataPath.text = playerList + "\n" + roomName + "\n" + nickName;

        moodChoiceObject.SetActive(false);

        if (mail_IconObject != null)
        {
            //print("오브젝트 있음");
            mail_IconImage = mail_IconObject.GetComponent<Image>();
            mail_IconImage.gameObject.SetActive(false);
        }

        if (mail_ImageObject != null)
        {
            //mail_ImageObject.SetActive(false); //오브젝트를 끄자.
        }

        tmp_InputFieldObject.SetActive(false); //인풋필드 오브젝트 끄기.
        mailComentButtonText = mailComentTestObject.GetComponent<TextMeshProUGUI>(); //메일코멘트버튼텍스트

        StartCoroutine(FindPlayer());

        CheckDate(); //날짜를 계산해줍니다.
        CheckMission(); //날짜에 맞는 미션을 생성합니다.
        CheckComent(); //코맨드가 있는지 계산합니다.
        CheckMood(); //무드를 바꾸자. FindPlayer 를 줌.

        DataPath.gameObject.SetActive(false);//데이타 텍스트 끄기
    }


    void Update()
    {
        if (isMailImage)//메일컨텐츠BG
        {
            mail_ImageObject.SetActive(true);
        }
        else
        {
            mail_ImageObject.SetActive(false); //메일 이미지 오브젝트 끄기
        }

        if (isMoodSwihtch1)//감정표현버튼1
        {
            moodChice1.gameObject.SetActive(true);
        }
        else
        {
            moodChice1.gameObject.SetActive(false);
        }

        if (isMoodSwihtch2)//감정표현버튼2
        {
            moodChice2.gameObject.SetActive(true);
        }
        else
        {
            moodChice2.gameObject.SetActive(false);
        }

       


    }

  

    public void CheckComent() //저정된 내용이 있으면 로드해주기.
    {
        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // 코멘트 인풋필드 컴포넌트
        mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>(); //코멘트1 의 텍스트
        mailComentText2 = Coment2.GetComponent<TextMeshProUGUI>(); //코멘트2의 텍스트

        //저장된 데이터가 있는지 확인하기
        //파일이 있으면 각 필드에 저장된 값을 세팅하기
        //경로파일을 불러옵니다.
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComentTest.json"; //로컬경로
        string path = Application.persistentDataPath + "/DayComentTest.json"; //동기화경로

        string fakeDate = "2010-10-22";//가짜날짜데이터
        if (File.Exists(path))//파일있니?
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //List로 파싱하기

            foreach (var ComentData in loadDayComenList)
            {
                if (ComentData.date == currentDate) //날짜가 일치하면
                //f(ComentData.date == fakeDate) //가짜날짜로 일치확인
                {
                    print("날짜일치");
                    //mailComentText1.text = "닉네임" + ":" + ComentData.user1name + "," + "기분" + ":" + ComentData.user1mood + "," + "답변" + ":" + ComentData.user1coment;
                    //mailComentText2.text = "닉네임" + ":" + ComentData.user2name + "," + "기분" + ":" + ComentData.user2mood + "," + "답변" + ":" + ComentData.user2coment;
                    mailComentText1.text = ComentData.user1name + "답변" + ":" + ComentData.user1coment;
                    mailComentText2.text = ComentData.user2name + "답변" + ":" + ComentData.user2coment;
                    break;

                }
                else
                {
                    print("날짜일치 없음, 이전저장 없음");
                    mailComentText1.text = "나의 답변을 입력해주세요";
                    mailComentText2.text = "상대방이 답변을 입력하지 않았어요";
                }
            }
        }
    }
  
    public void CheckMission()
    {
        // 날짜 문자열을 DateTime으로 변환
        DateTime dateTime = DateTime.Parse(currentDate);
        
        // 연도, 월, 일을 각각 int로 변환
        int year = dateTime.Year;
        int month = dateTime.Month;
        int day = dateTime.Day;

       /*  // 결과 출력
         Console.WriteLine("Year: " + year);
         Console.WriteLine("Month: " + month);*/
         Console.WriteLine("Day: " + day);

        // 일(Day) 부분을 문자열로 변환
        string dayString = dateTime.Day.ToString("D2"); // "25" (두 자리 숫자 형식으로 변환)
        // 첫 번째 자리와 두 번째 자리를 각각 int로 변환하여 저장
        int firstDigit = int.Parse(dayString[0].ToString()); // 첫 번째 자리 문자열로 저장
        int secondDigit = int.Parse(dayString[1].ToString()); // 두 번째 자리 문자열로 저장
        int dayCheck = firstDigit + secondDigit;
        int value = UnityEngine.Random.Range(0, 10); //랜덤뽑기
        
        //미션설정하기
        if (day == 27)
        {
            todayMission = "서로의 첫인상을 알려주세요";
        }
        else if(day == 28)
        {
            todayMission = "오늘 점심에 무엇을 먹었는지 알려주세요.";
        }
        else if (day == 29)
        {
            todayMission = "가장 소중한 사람은 누구인지 알려주세요.";
        }
        else if (day == 30)
        {
            todayMission = "오늘 날씨에 대한 느낌을 알려주세요.";
        }
        else if (day == 31)
        {
            todayMission = "둘이 함께 가보고 싶은곳을 알려주세요.";
        }
        else if (day == 01)
        {
            todayMission = "짜장 vs 짬뽕 더 선호하는 음식을 알려주세요.";
        }
        else if (day == 02)
        {
            todayMission = "요즘 즐겨듣는 노래를 알려주세요";
        }

        //미션저장하기
        string path = Application.persistentDataPath + "/DayComentTest.json"; //동기화경로
       
        if (File.Exists(path))  // 파일이 존재하면
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path);
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //문자열을 Json 배열로 변경
            for (int i = 0; i < loadDayComenList.Count; i++) //for문으로 데이트 idx로 설정하기.
            {
                var ComentData = loadDayComenList[i];

                if (ComentData.date == currentDate) // 날짜가 일치하면
                {
                    // 일치하는 데이터 로그를 변수에 저장.
                    matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                    //Debug.Log("날짜가 일치하는 데이터: " + matchDayComentinfo);
                    //기존데이터 수정
                    ComentData.dateMission = todayMission;
                    // 수정된 데이터를 리스트에 반영
                    loadDayComenList[i] = ComentData;
                    // 리스트를 JSON 문자열로 변환
                    string jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);
                    //Json 형태로 파일을 보내자.
                    jsonSyncString = jsonString;

                    PhotonNetwork.RaiseEvent(DATA_SYNC_EVENT_CODE, jsonSyncString, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                    Debug.Log("Photon 이벤트가 발생했습니다.");

                    TextMeshProUGUI dayMissionText = dayMisiionObject.GetComponent<TextMeshProUGUI>();
                    dayMissionText.text = todayMission;
                    break;
            
                }
           
            }        
        }
        else
        {
            DataPath.text = "파일이 없습니다";
        }
    }

    public void CheckDate()
    {
        // 현재 날짜를 yyyy-MM-dd 형식의 문자열로 변환
        startDate = "2024-10-23";
        Debug.Log("시작 날짜: " + startDate);

        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log("현재 날짜: " + currentDate);
        // 문자열을 DateTime 형식으로 변환
        DateTime startDay = DateTime.Parse(startDate);
        // 오늘 날짜 가져오기
        DateTime currDay = DateTime.Now;

        // 날짜 차이 계산
        TimeSpan difference = currDay - startDay;
        //날짜보정
        int totalDays = difference.Days + 1;
        // 차이의 일수를 문자열로 변환하여 sumDay에 저장
        string sumDay = totalDays.ToString();

        string today = "Day" + sumDay + ":" + currentDate;
        currentDay.text = today;
    }

    public void CheckMood() //변경한 무드 불러오기
    {
        //이미지를 골랐는지 확인하기

        foreach (var ComentData in loadDayComenList)
        {
            if (ComentData.date == currentDate) //날짜가 일치하면
            //if(ComentData.date == fakeDate) //가짜날짜로 일치확인
            {
                print("날짜일치, 이전에 저장한 기록이 있음");
                Image img1 = moodSwitch1.GetComponent<Image>();
                Image img2 = moodSwitch2.GetComponent<Image>();

                if (ComentData.user1mood == "null")
                {
                    img1.sprite = moodSprites[0];
                }
                else if (ComentData.user1mood == "Good")
                {
                    img1.sprite = moodSprites[1];
                }
                else if (ComentData.user1mood == "Normal")
                {
                    img1.sprite = moodSprites[2];
                }
                else if (ComentData.user1mood == "Bad")
                {
                    img1.sprite = moodSprites[3];
                }
                print("1번 이미지 변경사항 확인하기");


                if (ComentData.user2mood == "null")
                {
                    img2.sprite = moodSprites[0];
                }
                else if (ComentData.user2mood == "Good")
                {
                    img2.sprite = moodSprites[1];
                }
                else if (ComentData.user2mood == "Normal")
                {
                    img2.sprite = moodSprites[2];
                }
                else if (ComentData.user2mood == "Bad")
                {
                    img2.sprite = moodSprites[3];
                }
                print("user2 이미지 변경사항 확인하기");
                break;

            }
            else
            {
                print("CheckMood, 일치하는 날짜 없음, 일치하는 이미지가 없음.");
            }
        }
    }

    public void CheckMail()
    {
        CheckMission();
        CheckMood();
        CheckComent();
    }

    public void SaveDayComentJsonTest()
    {
        string nickName = LobbyGameManager.instance.playerNickName; //닉네임 캐싱
        string jsonString;
        DayComentData dayComentData; //클래스 변수 선언

        if (isMailComentButton) //답변하기 누른후 저장하기 누르면
        {
            mailComentButtonText.text = "답변하기"; //코멘트버튼 텍스트 변경 답변하기

            if (userNumber == "user1")
            {
                mailComentText1.text = nickName + ":" + tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
                //print("1번유저 위치에 답변저장");
            }
            else
            {
                mailComentText2.text = nickName + ":" + tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
                //print("2번유저 위치에 답변저장");
            }

            //답변을 저장하자.
            //경로 파일을 불러오자.
            //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComentTest.json"; //로컬경로
            string path = Application.persistentDataPath + "/DayComentTest.json"; //동기화경로
            if (File.Exists(path))  // 파일이 존재하면
            {
                string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
                //print("loadDayComentInfo" + loadDayComentInfo); //문자열 출력하기
                loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
                //Debug.Log("loadedDataList" + loadDayComenList);

                bool isMatched = false; // 유효성검사 변수

                for (int i = 0; i < loadDayComenList.Count; i++) //for문으로 데이트 idx로 설정하기.
                {
                    var ComentData = loadDayComenList[i];

                    if (ComentData.date == currentDate) // 날짜가 일치하면
                    {
                        // 일치하는 데이터 로그를 변수에 저장.
                        matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                        //Debug.Log("날짜가 일치하는 데이터: " + matchDayComentinfo);
                        isMatched = true; //유효성검사

                        // 기존 데이터를 수정
                        if (userNumber == "user1")
                        {
                            //Debug.Log("일치하는 데이터 있음, user1 정보 업데이트");
                            ComentData.user1name = playerNicknameMgr1.nickNameComp.text;
                            ComentData.user1mood = "Good";
                            ComentData.user1coment = mailComentText1.text;
                        }
                        else
                        {
                            //Debug.Log("일치하는 데이터 있음, user2 정보 업데이트");
                            ComentData.user2name = playerNicknameMgr2.nickNameComp.text;
                            ComentData.user2mood = "Normal";
                            ComentData.user2coment = mailComentText2.text;
                        }

                        // 수정된 데이터를 리스트에 반영
                        loadDayComenList[i] = ComentData;

                        // 리스트를 JSON 문자열로 변환
                        jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);

                        //Json 형태로 파일을 보내자.
                        jsonSyncString = jsonString;

                        // JSON 파일로 저장
                        //File.WriteAllText(path, jsonString); //로컬저장
                        //Debug.Log("파일 저장 완료: " + path);
                        //Debug.Log("저장된 JSON 데이터: " + jsonString);

                        PhotonNetwork.RaiseEvent(DATA_SYNC_EVENT_CODE, jsonSyncString, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                        Debug.Log("Photon 이벤트가 발생했습니다.");
                        break; //for문 중지

                    }


                }

                if (!isMatched) //일치하는 데이터가 없으면 새로 생성해서 넣기
                {

                    if (userNumber == "user1") //유저1 이면
                    {
                        dayComentData = new DayComentData //클래스 정보 재정의
                        {
                            date = currentDate, // 현재 날짜
                            user1name = playerNicknameMgr1.nickNameComp.text,
                            user1mood = "Good",
                            user1coment = mailComentText1.text,
                            user2name = "null",
                            user2mood = "null",
                            user2coment = "null"
                        };

                    }
                    else //유저2 이면
                    {
                        dayComentData = new DayComentData //클래스 정보 재정의
                        {
                            date = currentDate, // 현재 날짜
                            user1name = "null",
                            user1mood = "null",
                            user1coment = "null",
                            user2name = playerNicknameMgr2.nickNameComp.text,
                            user2mood = "Normal",
                            user2coment = mailComentText2.text,
                        };
                    }
                    loadDayComenList.Add(dayComentData); //기존에 불러온 List에 dayComentData 에 usertype 에 맞는 정보를 저장하기

                    // 리스트를 JSON 문자열로 변환
                    jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented);
                    
                    // JSON 파일로 저장
                    File.WriteAllText(path, jsonString);
                    Debug.Log("파일 생성 완료: " + path);
                    Debug.Log("저장된 JSON 데이터: " + jsonString);
                  
                }

            }

            tmp_InputFieldObject.SetActive(false);  //인풋필드 끄자
            isMailComentButton = false;
           

        }
        else //답변하기를 누르면
        {

            tmp_InputFieldObject.SetActive(true); //인풋필드 켜자.
            if (userNumber == "user1")
            {
                mailComentText1.text = nickName + ":" + "답변";
                print("1번유저 위치에 답변저장");
            }
            else
            {
                mailComentText2.text = nickName + ":" + "답변";
                print("2번유저 위치에 답변저장");
            }

            mailComentButtonText.text = "저장하기"; //코멘트버튼 텍스트를 저장하기로 변경
            isMailComentButton = true;

        }
    }

    public void LoadDayComentJson()
    {
        //경로 파일을 불러오자.
        string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";

        if (File.Exists(path))  // 파일이 존재하는지 확인
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
            print("로드제이슨" + loadDayComentInfo);
            print("오늘날짜" + currentDate);

            // JSON 문자열을 DayCommentData 객체의 리스트로 파싱
            //List<DayComentData> loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
            Debug.Log("loadedDataList" + loadDayComenList);

            CheckDayComentJsonDate(); //날짜정보가 일치하는지 확인하고 없으면 생성해서 정보를 갱신

        }

        else
        {
            CreateNewDayComentJsonArray(); //파일없으면 새로 생성

        }

    }

    public void CheckDayComentJsonDate()
    {
        // 날짜 확인 및 로그 출력
        foreach (var ComentData in loadDayComenList)
        {
            if (ComentData.date == currentDate) //날짜가 일치하면
            {
                // 일치하는 데이터 로그를 변수에저장.
                //string matchDayComentinfo = JsonConvert.SerializeObject(dayComentData, Formatting.Indented);
                matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                Debug.Log("날짜가 일치하는 데이터: " + matchDayComentinfo);
                break;

            }
            else //일치하는 데이터가 없으면 새로 생성해서 넣기
            {


                DayComentData dayComentData = new DayComentData
                {
                    date = currentDate, // 현재 날짜
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

    public void CreateNewDayComentJsonArray() //제이슨 배열로 저장하기
    {
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";//로컬경로
        string path = Application.persistentDataPath + "/DayComentTest.json"; //동기화경로

        // DayComentData 객체 생성
        DayComentData dayComentData = new DayComentData
        {
            date = currentDate, // 현재 날짜
            user1name = "null",
            user1mood = "null",
            user1coment = "null",
            user2name = "null",
            user2mood = "null",
            user2coment = "null"
        };

        // DayComentData를 JSON 문자열 배열로 변수에저장
        string jsonString = JsonConvert.SerializeObject(new[] { dayComentData }, Formatting.Indented);

        // JSON 파일로 저장
        File.WriteAllText(path, jsonString);
        Debug.Log("파일 생성 완료: " + path);
        Debug.Log("저장된 JSON 데이터: " + jsonString);


    }

    public void CreateNewDayComentJson()
    {
        string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";

        // 파일이 없으면 새 파일 생성
        Debug.Log("파일이 존재하지 않아 새로 생성합니다.");

        // 현재 날짜를 yyyy-MM - dd 형식으로 가져오기
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");

        // 기본 구조의 객체 생성
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

        // 클래스를 JSON 문자열로 변환 (직렬화)
        string jsonString = JsonUtility.ToJson(dayComent, true);

        // 파일 생성 및 기본 내용 쓰기
        File.WriteAllText(path, jsonString);
        Debug.Log("파일 생성 완료: " + path);
    }

    public void ViewInputMailComent()
    {
        //print("ViewInputMailComent");
        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // 코멘트 인풋필드 컴포넌트
        mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>(); //코멘트1 의 텍스트
        if (isMailComentButton) //답변하기 누른후 저장히기 누르면
        {
            mailComentButtonText.text = "답변하기"; //코멘트버튼 텍스트를 변경
            //print("입력만 코멘트" +tmp_InputField.text); //텍스트 출력해보기
            mailComentText1.text = currentDate + "\n" + "나의답변:" + tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
            tmp_InputFieldObject.SetActive(false);  //인풋필드 끄자
            isMailComentButton = false;

        }
        else //답변하기를 누르면
        {
            tmp_InputFieldObject.SetActive(true); //인풋필드 켜자.
            mailComentText1.text = "나의답변:";
            mailComentButtonText.text = "저장하기"; //저장하기 버튼으로 변경
            isMailComentButton = true;

        }

    }
    
    private void OnTriggerEnter(Collider other)
    {
        print("플레이어가 근처에 있음");
        if (other.gameObject.name.Contains("Player")) //게임오브젝트가 플레이어를 포함하고 있다면
        {
            print("이미지 보여주기");
            mail_IconImage.gameObject.SetActive(enabled);
            
            //버튼을 눌렀을때 함수 호출, 한번만 호출하게 하자.
            touchButton.onClick.AddListener(MailImageControll);

        }
    }

    /*private void OnTriggerStay(Collider other)
    {

    }*/
    private void OnTriggerExit(Collider other)
    {
        //버튼을 눌렀을때 함수 호출 
        touchButton.onClick.RemoveListener(MailImageControll);
        mail_IconImage.gameObject.SetActive(false);
        print("이미지 끄기");
    }

    public void MailImageControll()
    {
        isMailImage = !isMailImage;

        if (!isMailImage)
        {
            mail_ImageObject.SetActive(false);
        }
    }

    public void MoodSwitch(int switchNum)

    {
        if (switchNum == 1)
        {
            isMoodSwihtch1 = !isMoodSwihtch1;
        }
        else
        {
            isMoodSwihtch2 = !isMoodSwihtch2;
        }

    }

    public void ChangeMoodImage(string mood)
    {
        Image img1 = moodSwitch1.GetComponent<Image>();
        Image img2 = moodSwitch2.GetComponent<Image>();

        print("이미지를 바꾸면 그 값을 json파일에 저장합니다.");
        //1번이미지 변경
        if (mood == "1Good")
        {
            img1.sprite = moodSprites[1];
        }
        else if (mood == "1Normal")
        {
            img1.sprite = moodSprites[2];
        }
        else if (mood == "1Bad")
        {
            img1.sprite = moodSprites[3];
        }
        //2번이미지 변경
        if (mood == "2Good")
        {
            img2.sprite = moodSprites[1];
        }
        else if (mood == "2Normal")
        {
            img2.sprite = moodSprites[2];
        }
        else if (mood == "2Bad")
        {
            img2.sprite = moodSprites[3];
        }

        //경로 파일을 불러오자.
        //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComentTest.json"; //로컬경로
        string path = Application.persistentDataPath + "/DayComentTest.json"; //싱크경로

        for (int i = 0; i < loadDayComenList.Count; i++) // 날짜 확인 및 로그 출력
        {
            var ComentData = loadDayComenList[i];

            if (ComentData.date == currentDate) // 날짜가 일치하면
            {
                // 일치하는 데이터 로그를 변수에 저장.
                matchDayComentinfo = JsonConvert.SerializeObject(ComentData, Formatting.Indented);
                Debug.Log("날짜가 일치하는 데이터: " + matchDayComentinfo);

                if (mood == "1Good")
                {
                    ComentData.user1mood = "Good";
                }
                else if (mood == "1Normal")
                {
                    ComentData.user1mood = "Normal";
                }
                else if (mood == "1Bad")
                {
                    ComentData.user1mood = "Bad";
                }
                //2번이미지 변경
                if (mood == "2Good")
                {
                    ComentData.user2mood = "Good";
                }
                else if (mood == "2Normal")
                {
                    ComentData.user2mood = "Normal";
                }
                else if (mood == "2Bad")
                {
                    ComentData.user2mood = "Bad";
                }

                // 수정된 데이터를 리스트에 반영
                loadDayComenList[i] = ComentData;

                // 리스트를 JSON 문자열로 변환
                //string jsonString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented); 로컬저장
                // JSON 파일로 저장
                //File.WriteAllText(path, jsonString); //로컬저장
                //Debug.Log("파일 저장 완료: " + path);
                //Debug.Log("저장된 JSON 데이터: " + jsonString);

                jsonSyncString = JsonConvert.SerializeObject(loadDayComenList, Formatting.Indented); //싱크저장
                PhotonNetwork.RaiseEvent(DATA_SYNC_EVENT_CODE, jsonSyncString, new RaiseEventOptions { Receivers = ReceiverGroup.All }, SendOptions.SendReliable);
                Debug.Log("Photon 이벤트가 발생했습니다.");

                break;
            }

        }

    }
    //레벨에 있는 플레이어 찾기
    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        player1 = GameObject.Find("PlayerMale(Clone)"); //플레이어는 남자아바타
        player2 = GameObject.Find("PlayerWoman(Clone)"); //플레이어는 여자아바타

        // 남자가 먼저 입장한 경우 user1, 여자가 나중에 입장하면 user2, 닉네임 저장
       if(player1 != null)
        {
            playerNicknameMgr1 = player1.GetComponent<PlayerNicknameManager>();
        }
            
        if(player2 != null)
        {
            playerNicknameMgr2 = player2.GetComponent<PlayerNicknameManager>();
        }
     
        if (player1 != null && player2 == null) // 남자 혼자 입장
        {
            playerNicknameMgr1 = player1.GetComponent<PlayerNicknameManager>();
            userNumber = "user1";  // 남자는 user1
            print("FindPlayer 남자 혼자 입장, 유저번호: " + userNumber);
            
        }
        else if (player2 != null && player1 == null) // 여자 혼자 입장
        {
            playerNicknameMgr2 = player2.GetComponent<PlayerNicknameManager>();
            userNumber = "user2";  // 여자는 user2
            print("FindPlayer 여자 혼자 입장, 유저번호: " + userNumber);
            
        }
        else if (player1 != null && player2 != null) // 남자와 여자가 모두 입장한 경우
        {

            if (PhotonNetwork.LocalPlayer.ActorNumber == player1.GetComponent<PhotonView>().Owner.ActorNumber)
            {
                // 남자가 현재 로컬 플레이어일 때
                userNumber = "user1";  // 남자는 항상 user1
                print("FindPlayer 남자 유저 입장, 유저번호: user1");
            }
            else if (PhotonNetwork.LocalPlayer.ActorNumber == player2.GetComponent<PhotonView>().Owner.ActorNumber)
            {
                // 여자가 현재 로컬 플레이어일 때
                userNumber = "user2";  // 여자는 항상 user2
                print("FindPlayer 여자 유저 입장, 유저번호: user2");
            }

        }

        if (!player1 && !player2)//남자여자 둘다없음
        {
            //Debug.LogError("플레이어 없음");
            print("플레이어 없음");

            

        }

    }

   
    // 문자열을 JSON 형식으로 로컬 경로에 저장하는 메서드
    private void SaveStringAsJson(string data)
    {
        // JSON 직렬화 (단순 string을 JSON 구조로 변환하는 경우)
        //var jsonData = new { syncedData = data };

        // 직렬화된 JSON 데이터를 문자열로 변환
        //string jsonString = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

        // 로컬 파일에 저장
        //File.WriteAllText(jsonSyncPath, jsonString);
        File.WriteAllText(jsonSyncPath, data);

        Debug.Log("Data saved as JSON at: " + jsonSyncPath);
    }
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    // 이벤트를 받는 메서드
    public void OnEvent(EventData photonEvent)
    {
        // 우리가 정의한 이벤트인지 확인
        if (photonEvent.Code == DATA_SYNC_EVENT_CODE)
        {
            // 이벤트 데이터 받기 (string 형태라고 가정)
            string jsonString = (string)photonEvent.CustomData;

            Debug.Log("Received data: " + jsonString);

            // 받은 데이터를 JSON으로 저장
            SaveStringAsJson(jsonString);
        }
    }
    // Photon 네트워크에서 콜백 타겟 등록
   

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);  // 이벤트 콜백 해제
    }

}//클래스 끝
