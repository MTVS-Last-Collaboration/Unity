using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using File = System.IO.File;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Analytics;
using System.Reflection;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;
using UnityEngine.UI;

public class RegistTest : MonoBehaviour
{


    /*public class UserInfo
    {
        public string userId { get; set; }
        public string userPassword { get; set; }
    }*/

    public class UserInfo
    {
        public string email;
        public string username;
        public string password;
        public string nickname;
        public string gender;
        public string coupleDay;

    }

    public string loadUserInfo;

    public TMP_InputField inputEmail;
    public TMP_InputField inputPassword;
    public TMP_InputField intputUsername;
    public TMP_InputField inputNickname;
    public TMP_Dropdown dropgender;
    public TMP_InputField inputCoupleDay;
    TMP_Text dropgenderTextComp;



    // Start is called before the first frame update
    void Start()
    {
        
        if(inputEmail != null)
        {
            //print("있음");
            print(inputEmail.text);
        }
        else
        {
            print("없음");
        }

        if(dropgender != null)
        {
            //print("gdender있음");
            TMP_Text dg = dropgender.captionText;
            

            if (dg != null)
            {
                print("dg있음" + dg.text);
            }

        }
        else
        {
            print("gdender있음없음");
        }

    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void ResistNewUserInfoJSon() //유저정보를 요청함.
    {
        //데이터를 저장할 경로
        string path = Application.persistentDataPath + "UserInfo.json";

        // 유저 정보를 저장할 리스트 선언
        List<UserInfo> userInfoList = new List<UserInfo>();

       if (File.Exists(path)) //파일있으면
        {
            print("파일있어영");
            //path의 모든 텍스트를 가져옴.
            loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + loadUserInfo);
            List<UserInfo> loadDayComenList = JsonConvert.DeserializeObject<List<UserInfo>>(loadUserInfo); //List로 파싱하기

        }
        else //파일없으면
        {
            print("파일없어용");
            if (inputEmail.text != "") //이메일이 빈칸이 아니면
            {
                System.IO.File.Create(path).Dispose();   // 파일이 없으면 새로 생성 (빈 파일)
                print("JSON 파일 생성됨");

                userInfoList = new List<UserInfo>(); //유저인포 리스트 만들기

                UserInfo newUserInfo = new UserInfo
                {
                    email = inputEmail.text,
                    username = intputUsername.text,
                    password = inputPassword.text,
                    nickname = inputNickname.text,
                    gender = dropgenderTextComp.text,
                    coupleDay = inputCoupleDay.text

                };

                userInfoList.Add(newUserInfo);
                string jsonString = JsonConvert.SerializeObject(userInfoList , Formatting.Indented);

                File.WriteAllText(path, jsonString); //로컬저장
                print("저장된 문자열" + jsonString);

            }
           
        }



        //StartCoroutine(PostNewUserInfoJSon(""));
    }

    IEnumerator PostNewUserInfoJSon(string jsonString)
    {
        string url = "http://125.132.216.190:12223/api/auth/signup"; // 서버 URL 변경 필요

        UnityWebRequest request = new UnityWebRequest(url, "POST");  // HTTP POST 요청 준비

        // JSON 데이터를 담아 요청 생성
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 여기에 토큰 추가
        string token = "your_token_here"; // 실제 토큰 값을 여기에 설정합니다.
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            print("서버 응답: " + responseText);


            // 서버 응답과 newUser가 같은지 확인
            if (responseText == jsonString)
            {
                Debug.Log("서버 응답과 신규 유저 정보가 일치합니다.");
            }
            else
            {
                Debug.LogWarning("서버 응답과 신규 유저 정보가 일치하지 않습니다.");
            }

        }

    }

    public void LocalRegistJson()
    {
        print("회원가입 버튼 클릭");
        //입력된 아이디 가져오기
        LoginTest.instance.idText = LoginTest.instance.input_Id.text;
        print("아이디는 " + LoginTest.instance.idText);
        //입력된 패스워드 가져오기
        LoginTest.instance.passText = LoginTest.instance.input_Pass.text;
        print("비밀번호는 " + LoginTest.instance.passText);

        // 파일명과 경로 설정 (JSON 파일)
        string path = Application.dataPath + "/StreamingAssets/LocalLoginJson.json";

        // 유저 정보를 저장할 리스트 선언
        List<UserInfo> userInfoList = new List<UserInfo>();

        // 파일이 존재하는지, 그리고 동일한 내용이 있는지 확인
        if (System.IO.File.Exists(path))
        {
            //path의 모든 텍스트를 가져옴.
            LoginTest.instance.loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + LoginTest.instance.loadUserInfo);
            //loadUserInfo = System.IO.File.ReadAllText(path);

            //문자열이 없으면
            if (string.IsNullOrEmpty(LoginTest.instance.loadUserInfo))
            {
                print("문자열이 비었습니다.");

                // 새로운 사용자 정보를 생성
                UserInfo newUserInfos = new UserInfo
                {
                    email = LoginTest.instance.idText,
                    username = "null",
                    password = LoginTest.instance.passText,
                    nickname = "null",
                    gender = "MALE"

                };

                userInfoList.Add(newUserInfos);
                print("userInfoList" + userInfoList);

                //신규유저 정보를 Json으로 변경
                string newUserJson = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                print("신규유저정보" + newUserJson);

                // JSON 데이터를 파일에 저장
                System.IO.File.WriteAllText(path, newUserJson);
                print("SaveComplete" + newUserJson);



            }
            //문자열이 있으면
            else
            {
                print("문자열이 있습니다");

                //문자열을 딕셔너리 구조로 파싱하기
                //List<Dictionary<string, string>> userInfoLists = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(LoginTest.instance.loadUserInfo);

                //Json을 파상하여 List<Userinfo> 형태로 저장
                List<UserInfo> userInfoLists = JsonConvert.DeserializeObject<List<UserInfo>>(LoginTest.instance.loadUserInfo);

                // 유저 정보가 있는지 확인하는 변수
                bool isUserFound = false;

                //리스트 순환
                foreach (var userInfo in userInfoLists)
                {
                    //아이디와 패스워드가 일치하는지 확인
                    if (userInfo.email == LoginTest.instance.idText && userInfo.password == LoginTest.instance.passText)
                    {
                        //정보 표시
                        print("id 일치" + LoginTest.instance.idText + "pass 일치" + LoginTest.instance.passText);
                        isUserFound = true;
                    }
                }

                if (!isUserFound)
                {
                    print("같은 문자열이 없어요");

                    // 새로운 사용자 정보를 생성
                    UserInfo newUserInfo = new UserInfo
                    {
                        email = LoginTest.instance.idText,
                        username = "null",
                        password = LoginTest.instance.passText,
                        nickname = "null",
                        gender = "MALE"


                    };

                    userInfoList.Add(newUserInfo);
                    print("userInfoList" + userInfoList);


                    //신규유저 정보를 Json으로 변경
                    string newUserJson = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                    print("신규유저정보" + newUserJson);
                }

            }

        }
        else
        {
            // 파일이 없으면 새로 생성 (빈 파일)
            System.IO.File.Create(path).Dispose();
            print("JSON 파일 생성됨");
        }

    }

   
    public void LocalRegistJsonTest()
    {
        print("회원가입 버튼 클릭");
        //입력된 아이디 가져오기
        LoginTest.instance.idText = LoginTest.instance.input_Id.text;
        print("아이디는 " + LoginTest.instance.idText);
        //입력된 패스워드 가져오기
        LoginTest.instance.passText = LoginTest.instance.input_Pass.text;
        print("비밀번호는 " + LoginTest.instance.passText);

        //정규식, @ 기호가 존재하는지, @ 앞뒤로 공백이 없는지, 도메인에 .이 포함되어 있는지
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        //ID가 이메일 형식인지 테스트
        if (!Regex.IsMatch(LoginTest.instance.idText, emailPattern))
        {
            print("이메일 형식이 아님");
            LoginTest.instance.input_Id.text = "";
            LoginTest.instance.input_Pass.text = "";
            LoginTest.instance.placeHole_Id_Text.text = "이메일형식으로입력";
            LoginTest.instance.placeHole_Pass_Text.text = "비밀번호4자이상";
            LoginTest.instance.placeHole_Id_Text.color = Color.red;//new Color(1, 0, 0, 1);
            return;
        }

        // 비밀번호 최소 조건: 대문자, 소문자, 숫자, 특수기호 포함
        string pattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{4,}$";
        // 비밀번호 최소 조건: 대문자, 소문자, 숫자, 특수기호 포함 + 공백 없는지 확인
        string passPattern = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!%*?&])[\S]{4,}$";

        /*코드 설명:
        (?=.* [A - Z]): 적어도 하나의 대문자가 있는지 확인
        (?=.* [a - z]): 적어도 하나의 소문자가 있는지 확인
        (?=.*\d): 적어도 하나의 숫자가 있는지 확인
        (?=.* [@$! % *? &]): 적어도 하나의 특수문자가 있는지 확인
        { 8,}: 최소 8자 이상*/

        //비밀번호 형식 확인
        if (!Regex.IsMatch(LoginTest.instance.passText, passPattern))
        {
            print("패스워드형식 불일치");
            LoginTest.instance.input_Id.text = "";
            LoginTest.instance.input_Pass.text = "";
            LoginTest.instance.placeHole_Pass_Text.text = "대문자,소문자,숫자,특수기호(포함)";
            LoginTest.instance.placeHole_Pass_Text.color = Color.red;//new Color(1, 0, 0, 1);
            return;
        }

        //패스워드의 길이가 3자 이하인경우
        if (LoginTest.instance.passText.Length <= 3)
        {
            print("패스워드가 짧습니다.");
            LoginTest.instance.input_Id.text = "";
            LoginTest.instance.input_Pass.text = "";
            LoginTest.instance.placeHole_Pass_Text.text = "비밀번호4자이상";
            LoginTest.instance.placeHole_Pass_Text.color = Color.red;//new Color(1, 0, 0, 1);
            return;
        }


        // 파일명과 경로 설정 (JSON 파일)
        string path = Application.dataPath + "/StreamingAssets/LocalLoginJson.json";

        // 파일이 존재하는지, 그리고 동일한 내용이 있는지 확인
        if (System.IO.File.Exists(path))
        {
            //path의 모든 텍스트를 가져옴.
            LoginTest.instance.loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + LoginTest.instance.loadUserInfo);
            //loadUserInfo = System.IO.File.ReadAllText(path);

            //문자열이 없으면
            if (string.IsNullOrEmpty(LoginTest.instance.loadUserInfo))
            {
                print("문자열이 비었습니다.");


                // 유저 정보를 저장할 리스트 선언
                List<UserInfo> userInfoList = new List<UserInfo>();

                // 새로운 사용자 정보를 생성
                UserInfo newUserInfo = new UserInfo
                {
                    email = LoginTest.instance.idText,
                    username = "null",
                    password = LoginTest.instance.passText,
                    nickname = "null",
                    gender = "MALE"
                };

                userInfoList.Add(newUserInfo);
                print("userInfoList" + userInfoList);
                
                //신규유저 정보를 Json으로 변경
                string newUserJson = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                print("신규유저정보" + newUserJson);

                // JSON 데이터를 파일에 저장
                System.IO.File.WriteAllText(path, newUserJson);
                print("SaveComplete" + newUserJson);


            }
            //문자열이 있으면
            else
            {
                print("문자열이 있습니다" + LoginTest.instance.loadUserInfo);

                //문자열을 딕셔너리 구조로 파싱하기
                //List<Dictionary<string, string>> userInfoLists = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(LoginTest.instance.loadUserInfo);

                // 앞뒤 공백 제거
                //LoginTest.instance.loadUserInfo = LoginTest.instance.loadUserInfo.Trim();
                //print("공백제거");

                List<UserInfo> userInfoLists = JsonConvert.DeserializeObject<List<UserInfo>>(LoginTest.instance.loadUserInfo);
                Debug.Log("JSON 파싱 성공");

                // 유저 정보가 있는지 확인하는 변수
                bool isUserFound = false;

                //리스트 순환
                foreach (var userInfo in userInfoLists)
                {
                    //아이디와 패스워드가 일치하는지 확인
                    //if (userInfo.userId == LoginTest.instance.idText && userInfo.userPassword == LoginTest.instance.passText)
                    //일치하는 아이디가 있다면
                    if (userInfo.email == LoginTest.instance.idText)
                    {
                        //정보 표시
                        print("id 일치" + LoginTest.instance.idText + "pass 일치" + LoginTest.instance.passText);
                        isUserFound = true;

                        LoginTest.instance.input_Id.text = "";
                        LoginTest.instance.input_Pass.text = "";
                        LoginTest.instance.placeHole_Id_Text.text = "아이디중복";
                        LoginTest.instance.placeHole_Id_Text.color = Color.red;//new Color(1, 0, 0, 1);
                        break;
                    }

                }
                // 중복된 아이디가 없으면 새로운 유저 추가
                if (!isUserFound)
                {
                    print("같은 문자열이 없어요");

                    // 새로운 사용자 정보를 생성
                    UserInfo newUserInfo = new UserInfo
                    {
                        email = LoginTest.instance.idText,
                        username = "null",
                        password = LoginTest.instance.passText,
                        nickname = "null",
                        gender = "null"
                    };

                    userInfoLists.Add(newUserInfo);
                    print("userInfoList" + userInfoLists);

                    //신규유저 정보를 Json으로 변경
                    string newUserJson = JsonConvert.SerializeObject(userInfoLists, Formatting.Indented);
                    print("신규유저정보" + newUserJson);

                    // JSON 데이터를 파일에 저장
                    File.WriteAllText(path, newUserJson);
                    print("SaveComplete" + newUserJson);

                }                
                    
            }        
             
        }
        else
        {
            // 파일이 없으면 새로 생성 (빈 파일)
            System.IO.File.Create(path).Dispose();
            print("JSON 파일 생성됨");
        }
    
    }


}
