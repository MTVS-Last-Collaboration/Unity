using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine.Rendering.LookDev;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Converters;



[System.Serializable]
public class DayComentData
{
    public string date;
    public string user1name;
    public string user1mood;
    public string user1coment;
    public string user2name;
    public string user2mood;
    public string user2coment;
}


public class MailManager : MonoBehaviour
{
    public GameObject mail_IconObject; //메일아이콘오브젝트
    public GameObject mail_ImageObject; //메일미션이미지
    public Button touchButton; //터치버튼
    public TextMeshProUGUI currentDay;
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
    public GameObject mailComentTestObject; //메일코면트
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

    // Start is called before the first frame update
    void Start()
    {
        moodChoiceObject.SetActive(false);

       if (mail_IconObject != null)
        {
            print("오브젝트 있음");
            mail_IconImage = mail_IconObject.GetComponent<Image>();
            mail_IconImage.gameObject.SetActive(false);
        }

       if(mail_ImageObject != null)
        {
            //mail_ImageObject.SetActive(false); //오브젝트를 끄자.
        }

        tmp_InputFieldObject.SetActive(false); //인풋필드 오브젝트 끄기.
        mailComentButtonText = mailComentTestObject.GetComponent<TextMeshProUGUI>(); //메일코멘트버튼텍스트

        StartCoroutine(FindPlayer());
        CheckDate(); //날짜를 계산해줍니다.
        CheckComent(); //코맨드가 있는지 계산합니다.
        //CheckMood(); //무드를 바꾸자.

    }

    // Update is called once per frame
    void Update()
    {
        if (isMailImage)//메일컨텐츠BG
        {
            mail_ImageObject.SetActive(true);
        }
        else
        {
            //mail_ImageObject.SetActive(false); //메일 이미지 오브젝트 끄기
        }

        if(isMoodSwihtch1)//감정표현버튼1
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
        mailComentText2 = Coment2.GetComponent<TextMeshProUGUI>();

        //저장된 데이터가 있는지 확인하기
        //파일이 있으면 각 필드에 저장된 값을 세팅하기
        //경로파일을 불러옵니다.
        string path = Application.dataPath + "/Resources/DayComentTest.json";

        string fakeDate = "2010-10-22";//가짜날짜데이터
        if (File.Exists(path))//파일있니?
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //List로 파싱하기
           
            foreach (var ComentData in loadDayComenList)
            {
                //if (ComentData.date == currentDate) //날짜가 일치하면
                if(ComentData.date == fakeDate) //가짜날짜로 일치확인
                {
                    print("날짜일치");
                    mailComentText1.text = "닉네임" + ":" + ComentData.user1name + "," + "기분" + ":" + ComentData.user1mood + "," + "답변" + ":" + ComentData.user1coment;
                    mailComentText2.text = "즐겁게 개발하기";
                    break;

                }
                else
                {
                    print("일치하는 날짜 없음");
                    mailComentText1.text = "나의 답변을 입력해주세요";
                    mailComentText2.text = "상대방이 답변을 입력하지 않았어요";
                }
            }
        }
    }

    public void CheckDate()
    {
        // 현재 날짜를 yyyy-MM-dd 형식의 문자열로 변환
        startDate = "2024-10-23";
        Debug.Log("시작 날짜: " + currentDate);

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

    public void CheckMood()
    {
        //이미지를 골랐는지 확인하기
        //user1
        if (userNumber == "user1")
        {
            moodSwitch2.gameObject.SetActive(false);
        }
        else
        {
            moodSwitch1.gameObject.SetActive(false);
        }
    }

    public void SaveDayComentJsonTest()
    {
        string nickName = LobbyGameManager.instance.playerNickName; //닉네임 캐싱

        if (isMailComentButton) //답변하기 누른후 저장하기 누르면
        {
            mailComentButtonText.text = "답변하기"; //코멘트버튼 텍스트 변경 답변하기

            if (userNumber == "user1")
            {
                mailComentText1.text = nickName + ":" + tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
                print("1번유저 위치에 답변저장");
            }
            else
            {
                mailComentText2.text = nickName + ":" + tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
                print("2번유저 위치에 답변저장");
            }

            //답변을 저장하자.
            //경로 파일을 불러오자.
            string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";

            if (File.Exists(path))  // 파일이 존재하는지 확인
            {
                string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
                loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo);
                Debug.Log("loadedDataList" + loadDayComenList);

                
                foreach (var ComentData in loadDayComenList)// 날짜 확인 및 로그 출력
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

                        string jsonString;
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

                        if (userNumber == "user1")
                        {
                             dayComentData = new DayComentData
                            {
                                date = currentDate, // 현재 날짜
                                user1name = playerNicknameMgr1.nickNameComp.text,
                                user1mood = "null",
                                user1coment = mailComentText1.text,
                                user2name = "null",
                                user2mood = "null",
                                user2coment = "null"
                            };
                           
                        }
                        else
                        {
                             dayComentData = new DayComentData
                            {
                                date = currentDate, // 현재 날짜
                                user1name = "null",
                                user1mood = "null",
                                user1coment = "null",
                                user2name = playerNicknameMgr2.nickNameComp.text,
                                 user2mood = "null",
                                user2coment = mailComentText2.text,
                             };
                        }

                        // DayComentData를 JSON 문자열 배열로 변수에저장
                        jsonString = JsonConvert.SerializeObject(new[] { dayComentData }, Formatting.Indented);
                        jsonString = matchDayComentinfo + jsonString;

                        // JSON 파일로 저장
                        File.WriteAllText(path, jsonString);
                        Debug.Log("파일 생성 완료: " + path);
                        Debug.Log("저장된 JSON 데이터: " + jsonString);



                    }

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
        string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";

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

            //버튼을 눌렀을때 함수 호출
            touchButton.onClick.AddListener(MailImageControll);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("이미지 끄기");
        mail_IconImage.gameObject.SetActive(false);
    }

    public void MailImageControll()
    {
        isMailImage = !isMailImage;

        if(!isMailImage)
        {
            mail_ImageObject.SetActive(false);
        }
    }

    public void MoodSwitch(int switchNum)

    {
        if(switchNum == 1)
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

        if (mood == "1Good")
        { 
            img1.sprite = moodSprites[0];
        }
        else if(mood == "1Normal")
        {
            img1.sprite = moodSprites[1];
        }
        else if (mood == "1Bad")
        {
            img1.sprite = moodSprites[2];
        }

        if (mood == "2Good")
        {
            img2.sprite = moodSprites[0];
        }
        else if (mood == "2Normal")
        {
            img2.sprite = moodSprites[1];
        }
        else if (mood == "2Bad")
        {
            img2.sprite = moodSprites[2];
        }
    }

    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(0.1f);

        player1 = GameObject.Find("PlayerMale(Clone)"); //플레이어는 남자아바타
        
        if (player1 == null) //남자 아바타가 없으면
        {
            player2 = GameObject.Find("PlayerWoman(Clone)"); //플레이어는 여자아바타
        }
        
        if (player1 != null || player2 != null) //플레이어가 존재하면
        {
           
             

            if (player1 != null && player1.gameObject.name == "PlayerMale(Clone)")
            {

                //여캐가 아니면
                playerNicknameMgr1 = player1.GetComponent<PlayerNicknameManager>(); //플레이어의 컴포넌트 가져와서
                userNumber = "user1";
                print("FindPlayer 유저번호" + userNumber);
            }
            if(player2 != null && player2.gameObject.name == "PlayerWoman(Clone)")
            {   //여캐가 맞으면
                playerNicknameMgr2 = player2.GetComponent<PlayerNicknameManager>();
                userNumber = "user2";
                print("FindPlayer 유저번호" + userNumber);
            }
            
        }
        else
        {
            //Debug.LogError("플레이어 없음");
            print("플레이어 없음");
        }

        CheckMood();
    }

}//클래스 끝
