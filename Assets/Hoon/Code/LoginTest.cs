using System.Collections;
using System.Collections.Generic;
//TMP_InputField 사용에 필요 
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Newtonsoft.Json;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using static RegistTest;
using UnityEngine.SceneManagement;

public class LoginTest : MonoBehaviour
{
    public static LoginTest instance;

    private void Awake()
    {
        instance = this;
    }


    public GameObject input_Id_Object;
    public GameObject input_Pass_Object;

    public GameObject placeHold_Id_Object;
    public GameObject placeHold_Psss_Object;


    //tmp inputfield
    public TMP_InputField input_Id;
    public TMP_InputField input_Pass;
    public TextMeshProUGUI placeHole_Id_Text;
    public TextMeshProUGUI placeHole_Pass_Text;

    public string idText;
    public string passText;
    public string loadUserInfo;


    // Start is called before the first frame update
    void Start()
    {
        input_Id = input_Id_Object.GetComponent<TMP_InputField>();
        input_Pass = input_Pass_Object.GetComponent<TMP_InputField>();
        placeHole_Id_Text = placeHold_Id_Object.GetComponent<TextMeshProUGUI>();
        placeHole_Pass_Text = placeHold_Psss_Object.GetComponent <TextMeshProUGUI>();

    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
    public void LocalRegistJson()
    {
        print("회원가입 버튼 클릭");

        //입력된 아이디 가져오기
        idText = input_Id.text;
        print("아이디는 " + idText);
        //입력된 패스워드 가져오기
        passText = input_Pass.text;
        print("비밀번호는 " + passText);

        // 파일명과 경로 설정 (JSON 파일)
        string path = Application.dataPath + "/StreamingAssets/LocalLoginJson.json";

        // 파일이 존재하는지, 그리고 동일한 내용이 있는지 확인
        if (System.IO.File.Exists(path))
        {
            //pah의 모든 택스트를 가져오자.
            loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + loadUserInfo);
            
            // 유저 정보가 있는지 확인하는 변수
            bool isUserFound = false;

            if (loadUserInfo != null)
            {
                // JSON 파일을 Dictionary 리스트로 변환
                List<Dictionary<string, string>> userInfoList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(loadUserInfo);

                //파일이 비지 않을때
                if (userInfoList != null)
                {
                    foreach (var userInfo in userInfoList)
                    {

                        //아이디와 패스워드가 일치하는지 확인
                        if (userInfo["userId"] == idText && userInfo["userPassword"] == passText)
                        {
                            //정보표시해주기
                            print("id 일치" + idText + "pass 일치" + passText);
                            isUserFound = true;
                        }

                    }
                }

            }
            
            //파일이 비었거나 일치하는 내용이 없다면
            if(!isUserFound)
            {
                //사용자 정보를 Dictionary로 변경
                 Dictionary<string, string> newUserInfo = new Dictionary<string, string>
                {
                    { "userId", idText },
                    { "userPassword", passText },
            
                };
   
                //신규유저 정보를 Json 으로 변경
                string newUser = JsonConvert.SerializeObject(newUserInfo, Formatting.Indented);
                print("신규유저정보" + newUser);

                // JSON 데이터를 파일에 저장
                File.WriteAllText(path, newUser);
                print("SaveComplete" + newUser);


            }

        }
        else
        {
            // 파일이 없으면 새로 생성 (빈 파일)
            System.IO.File.Create(path).Dispose();
            print("JSON 파일 생성됨");
        }
    }


    public void LocalLoginJson()
    {
        
        print("로컬 로그인 버튼 클릭");

        //입력된 아이디 가져오기
        idText = input_Id.text;
        print("아이디는 " + idText);
        //입력된 패스워드 가져오기
        passText = input_Pass.text;
        print("비밀번호는 " + passText);

        // 파일명과 경로 설정 (JSON 파일)
        string path = Application.dataPath + "/StreamingAssets/LocalLoginJson.json";
        
        // 파일이 존재하는지, 그리고 동일한 내용이 있는지 확인
        if (System.IO.File.Exists(path))
        {
            //pah의 모든 택스트를 가져오자.
            loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + loadUserInfo);

            // JSON 파일을 Dictionary 리스트로 변환
            //List<Dictionary<string, string>> userInfoList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(loadUserInfo);
            //print("유저정보량" + userInfoList.Count);

            List<UserInfo> userInfoList = JsonConvert.DeserializeObject<List<UserInfo>>(LoginTest.instance.loadUserInfo);
            Debug.Log("JSON 파싱 성공");



            if (userInfoList == null)
            {
                print("파일이 비었습니다.");
                return;
            }
            // 유저 정보가 있는지 확인하는 변수
            bool isUserFound = false;

            //리스트파일을 순차검사
            foreach (var userInfo in userInfoList)
            {

                //아이디와 패스워드가 일치하는지 확인
                //if (userInfo["userId"] == idText && userInfo["userPassword"] == passText)
                if (userInfo.userId == idText && userInfo.userPassword == passText)
                {
                    //정보표시해주기
                    print("id, pass 일치");
                    //userId 저장하기
                    //saveUserId = idText;

                    //이름변수에 이름을 저장
                    
                    //userNameText = userInfo["userNickName"];
                    
                    //print("내이름" + userNameText);
                    //idText = userInfo["userId"];
                    //print("내id" + idText);
                    //MyInfo UserName을 갱신
                    
                    //mainUiObject.nameTextComp.text = userNameText;

                    //유저찾음
                    isUserFound = true;
                    //로그인처리하기
                    Login();
                    print("로그인 시작");
                    //루틴 나가기
                    break;

                }
                //유저가 없으면
                else if (!isUserFound)
                {
                    //print("아이디가 틀림");
                    input_Id.text = "";
                    placeHole_Id_Text.text = "아이디가 틀림";
                    placeHole_Id_Text.color = Color.red;

                    //print("비밀번호가 틀림");
                    input_Pass.text = "";
                    //mainUiObject.phPass_Text.text = "비밀번호 틀림";
                    //mainUiObject.phPass_Text.color = Color.red;

                }

            }

        }
        //파일 없으면 생성하기
        else
        {
            // 파일이 없으면 새로 생성 (빈 파일)
            System.IO.File.Create(path).Dispose();
            print("JSON 파일 생성됨");
        }

    }//TestLocalLoginJson end

    void Login()
    {
        print("로그인완료");
        SceneManager.LoadScene("");
    }

}
