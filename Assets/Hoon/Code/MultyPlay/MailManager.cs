using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine.Rendering.LookDev;
using System.Net.NetworkInformation;
using Newtonsoft.Json;
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
    public Button moodSwitch; //버튼기분변경
    public Image moodChice; // 기분변경 BG
    public Button moodGood; // 버튼 기분좋음
    public Button moodNormal; //버튼 기분중간
    public Button moodBad; //버튼 기분나쁨
    public Sprite[] moodSprites; //기분이미지 배열
    public GameObject tmp_InputFieldObject; //코멘트 인풋
    public Button mailComentButton; //코멘트 인풋 열기
    public GameObject mailComentTestObject; //메일코면트
    public GameObject Coment1;
    public GameObject Coment2;

    Image mail_IconImage;
    bool isMailImage = false;
    bool isMoodSwihtch = false;
    bool isMailComentButton = false;
    TMP_InputField tmp_InputField;
    TextMeshProUGUI mailComentButtonText;
    bool isMailComentSave = false;
    TextMeshProUGUI mailComentText1;
    TextMeshProUGUI mailComentText2;
    bool isMailComent1 = false;
    bool isMainComent2 = false;

    string startDate; //시작일 지정변수
    string currentDate; //오늘 날짜를 저장할 변수
    string playerNickName; //닉네임 저장 변수
    

    List<DayComentData> loadDayComenList; //로드한 데이터를 저장하는 리스트
    string matchDayComentinfo; //리스트에서 일치하는 데이터를 저장할 문자열

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

        // 현재 날짜를 yyyy-MM-dd 형식의 문자열로 변환
        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        Debug.Log("현재 날짜: " + currentDate);
        string today = "Day" + 1 + currentDate;
        currentDay.text = today;

        CheckComent();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMailImage)
        {
            mail_ImageObject.SetActive(true);
        }
        else
        {
            mail_ImageObject.SetActive(false); //메일 이미지 오브젝트 끄기
        }

        if(isMoodSwihtch)
        {
            moodChice.gameObject.SetActive(true);
        }
        else
        {
            moodChice.gameObject.SetActive(false);
        }
    
    }

    public void CheckComent()
    {
        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // 코멘트 인풋필드 컴포넌트
        mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>(); //코멘트1 의 텍스트
        mailComentText2 = Coment2.GetComponent<TextMeshProUGUI>();

        //저장된 데이터가 있는지 확인하기
        //파일이 있으면 각 필드에 저장된 값을 세팅하기
        //경로파일을 불러옵니다.
        string path = Application.dataPath + "/Resources/DayComentTest.json";

        if (File.Exists(path))//파일있니?
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //List로 파싱하기
           
            foreach (var ComentData in loadDayComenList)
            {
                if (ComentData.date == currentDate) //날짜가 일치하면
                {
                    print("날짜일치");
                    mailComentText1.text = "닉네임" + ":" + ComentData.user1name + "," + "기분" + ":" + ComentData.user1mood + "," + "답변" + ":" + ComentData.user1coment;
                    mailComentText2.text = "즐겁게 개발하기";
                    break;

                }
                else
                {
                    print("일치하는 날짜 없음");
                }
            }

        }
    }

    public void LoadDayComentJsonTest()
    {
        //ViewInputMailComent();

        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // 코멘트 인풋필드 컴포넌트
        mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>(); //코멘트1 의 텍스트

        //저장된 데이터가 있는지 확인하기
        //파일이 있으면 각 필드에 저장된 값을 세팅하기
        //경로파일을 불러옵니다.
        string path = Application.dataPath + "/Resources/DayComentTest.json";

        if (File.Exists(path))//파일있니?
        {
            string loadDayComentInfo = System.IO.File.ReadAllText(path); //문자열로 가져오기
            loadDayComenList = JsonConvert.DeserializeObject<List<DayComentData>>(loadDayComentInfo); //List로 파싱하기
           

            foreach (var ComentData in loadDayComenList)
            {
                if (ComentData.date == currentDate) //날짜가 일치하면
                {
                    print("날짜일치");
                    mailComentText1.text = ComentData.user1name + "기분" + ComentData.user1mood + ComentData.user1coment;

                    break;

                }
            }

        }
        else //없으면생성하기
        {
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

        }



        if (isMailComentButton) //답변하기 누른후 저장히기 누르면
        {
            mailComentButtonText.text = "답변하기"; //코멘트버튼 텍스트를 변경
            //print("입력만 코멘트" +tmp_InputField.text); //텍스트 출력해보기
            mailComentText1.text = "나의답변:" + tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
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





    public void LoadDayComentJson()
    {
        //경로 파일을 불러오자.
        string path = Application.dataPath + "/Resources/DayComent.json";

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
        string path = Application.dataPath + "/Resources/DayComent.json";

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
        string path = Application.dataPath + "/Resources/DayComent.json";

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

       /*if (mailComentText1.text == "") //아무것도 없나?
        {
            mailComentText1.text = "나의답변";
        
        }
        else //내용이 있으면
        {
            print("내용있어");
            if (!isMailComent1)
            {
                mailComentText1.text = "나의답변:" + tmp_InputField.text;
                isMailComent1 = true;
            }
            else             
            {
                mailComentText2 = Coment2.GetComponent<TextMeshProUGUI>();
                mailComentText2.text = "나의답변:"+ tmp_InputField.text; 
                print("너의답변" + mailComentText2.text);

                //mailComentText.text = "너의답변:" + tmp_InputField.text;
            }

        }*/
   
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
    }

    public void MoodSwitch()
    {
        isMoodSwihtch = !isMoodSwihtch;
    }
    public void ChangeMoodImage(string mood)
    {
        Image img = moodSwitch.GetComponent<Image>();

        if (mood == "Good")
        { 
            img.sprite = moodSprites[0];
        }
        else if(mood == "Normal")
        {
            img.sprite = moodSprites[1];
        }
        else if (mood == "Bad")
        {
            img.sprite = moodSprites[2];
        }

    }

}//클래스 끝
