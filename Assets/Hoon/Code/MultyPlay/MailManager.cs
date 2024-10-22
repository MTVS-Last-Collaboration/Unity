using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.UI;

public class MailManager : MonoBehaviour
{
    public GameObject mail_IconObject; //메일아이콘오브젝트
    public GameObject mail_ImageObject; //메일미션이미지
    public Button touchButton; //터치버튼
    public GameObject moodChoiceObject; //오늘의기분 변경 오브젝트
    public Button moodSwitch; //버튼기분변경
    public Image moodChice; // 기분변경 BG
    public Button moodGood; // 버튼 기분좋음
    public Button moodNormal; //버튼 기분중간
    public Button moodBad; //버튼 기분나쁨
    public Sprite[] moodSprites; //배열 기분이미지
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
            mail_ImageObject.SetActive(false); //오브젝트를 끄자.
        }

        tmp_InputFieldObject.SetActive(false); //인풋필드 오브젝트 끄기.
        mailComentButtonText = mailComentTestObject.GetComponent<TextMeshProUGUI>(); //메일코멘트버튼텍스트

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
            mail_ImageObject.SetActive(false);
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

    public void ViewInputMailComent()
    {
        //print("ViewInputMailComent");
        tmp_InputField = tmp_InputFieldObject.GetComponent<TMP_InputField>(); // 인풋오브젝트 인풋필드 컴포넌트
        if (isMailComentButton) //메일작성이 저장됬다면
        {
            tmp_InputFieldObject.SetActive(false); //인풋필드 끄자
            mailComentButtonText.text = "답변하기"; //코멘트버튼 텍스트를 변경
            isMailComentButton = false;

        }
        else
        {
            tmp_InputFieldObject.SetActive(true); //인풋필드 켜자.
            mailComentButtonText.text = "저장하기"; //저장하기 버튼으로 변경
            //print("입력만 코멘트" +tmp_InputField.text); //텍스트 출력해보기
            mailComentText1 = Coment1.GetComponent<TextMeshProUGUI>();
            mailComentText1.text = tmp_InputField.text; //쓰여진 코멘트를 첫번째로 변경
            isMailComentButton = true;
       
        }

        if (mailComentText1.text == "") //아무것도 없나?
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
