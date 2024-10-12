using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

public class RegistTest : MonoBehaviour
{


    public class UserInfo
    {
        public string userId { get; set; }
        public string userPassword { get; set; }
    }

    public string loadUserInfo;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

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
                UserInfo newUserInfo = new UserInfo
                {
                    userId = LoginTest.instance.idText,
                    userPassword = LoginTest.instance.passText
                };

                userInfoList.Add(newUserInfo);
                print("userInfoList" + userInfoList);

                //신규유저 정보를 Json으로 변경
                string newUserJson = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                print("신규유저정보" + newUserJson);

                // JSON 데이터를 파일에 저장
                File.WriteAllText(path, newUserJson);
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
                    if (userInfo.userId == LoginTest.instance.idText && userInfo.userPassword == LoginTest.instance.passText)
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
                        userId = LoginTest.instance.idText,
                        userPassword = LoginTest.instance.passText
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
                    userId = LoginTest.instance.idText,
                    userPassword = LoginTest.instance.passText
                };

                userInfoList.Add(newUserInfo);
                print("userInfoList" + userInfoList);
                
                //신규유저 정보를 Json으로 변경
                string newUserJson = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                print("신규유저정보" + newUserJson);

                // JSON 데이터를 파일에 저장
                File.WriteAllText(path, newUserJson);
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
                    if (userInfo.userId == LoginTest.instance.idText)
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
                        userId = LoginTest.instance.idText,
                        userPassword = LoginTest.instance.passText
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
