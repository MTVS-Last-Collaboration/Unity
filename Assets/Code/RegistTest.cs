using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Newtonsoft.Json.Linq;

public class RegistTest : MonoBehaviour
{
    



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/


    public class UserInfo
    {
        public string userId { get; set; }
        public string userPassword { get; set; }
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

        // 파일이 존재하는지, 그리고 동일한 내용이 있는지 확인
        if (System.IO.File.Exists(path))
        {
            //pah의 모든 택스트를 가져오자.
            LoginTest.instance.loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + LoginTest.instance.loadUserInfo);


            // loadUserInfo에 내용이 있는지 확인
            if (!string.IsNullOrEmpty(LoginTest.instance.loadUserInfo))
            {

                
                // JSON 파일을 Dictionary 리스트로 변환
                List<Dictionary<string, string>> userInfoList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(LoginTest.instance.loadUserInfo);

                // 유저 정보가 있는지 확인하는 변수
                bool isUserFound = false;


                //유효성검사하기
                foreach (var userInfo in userInfoList)
                {

                    //아이디와 패스워드가 일치하는지 확인
                    if (userInfo["userId"] == LoginTest.instance.idText && userInfo["userPassword"] == LoginTest.instance.passText)
                    {
                        //정보표시해주기
                        print("id 일치" + LoginTest.instance.idText + "pass 일치" + LoginTest.instance.passText);
                        isUserFound = true;
                    }

                }

                if (!isUserFound)
                {
                    //사용자 정보를 Dictionary로 저장
                    Dictionary<string, string> newUserInfo = new Dictionary<string, string>
                    {
                        { "userId", LoginTest.instance.idText },
                        { "userPassword", LoginTest.instance.passText },

                    };

                    // 새로운 유저 정보를 리스트에 추가
                    userInfoList.Add(newUserInfo);
                    print("회원가입정보" + newUserInfo);

                    //신규유저 정보를 Json 으로 변경
                    string newUser = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                    print("신규유저정보" + newUser);

                    // JSON 데이터를 파일에 저장
                    File.WriteAllText(path, LoginTest.instance.loadUserInfo);
                    print("SaveComplete" + newUser);


                }
            }
            else
            {
                print("유저정보가 없어요");
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

      
        // 파일명과 경로 설정 (JSON 파일)
        string path = Application.dataPath + "/StreamingAssets/LocalLoginJson.json";

        // 파일이 존재하는지, 그리고 동일한 내용이 있는지 확인
        if (System.IO.File.Exists(path))
        {
            //path의 모든 텍스트를 가져옴.
            LoginTest.instance.loadUserInfo = System.IO.File.ReadAllText(path);
            print("JSON 파일 읽기 완료" + LoginTest.instance.loadUserInfo);

            if(string.IsNullOrEmpty(LoginTest.instance.loadUserInfo))
            {
                print("문자열이 비었습니다.");

                // 새로운 사용자 정보를 담을 리스트 생성
                List<Dictionary<string, string>> userInfoList  = new List<Dictionary<string, string>>();

                //사용자 정보를 Dictionary로 저장
                UserInfo  newUserInfo = new UserInfo
                {
                        { "userId", LoginTest.instance.idText },
                        { "userPassword", LoginTest.instance.passText }

                };

                userInfoList.Add(newUserInfo);
                print("userInfoList" + userInfoList);
                

                //신규유저 정보를 Json으로 변경
                string newUserJson = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                print("신규유저정보" + newUserJson);

                /*// JSON 데이터를 파일에 저장
                File.WriteAllText(path, newUserJson);
                print("SaveComplete" + newUserJson);*/



            }
            //문자열이 있으면
            else
            {
                //문자열을 딕셔너리 구조로 파싱하기
                List < Dictionary<string, string> > userInfoList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(LoginTest.instance.loadUserInfo);

                // 유저 정보가 있는지 확인하는 변수
                bool isUserFound = false;

                foreach (var userInfo in userInfoList)
                {
                    //아이디와 패스워드가 일치하는지 확인
                    if (userInfo["userId"] == LoginTest.instance.idText && userInfo["userPassword"] == LoginTest.instance.passText)
                    {
                        //정보 표시
                        print("id 일치" + LoginTest.instance.idText + "pass 일치" + LoginTest.instance.passText);
                        isUserFound = true;
                    }
                }

                if (!isUserFound)
                {
                    //사용자 정보를 Dictionary로 저장 (바디 추가)
                    Dictionary<string, string> newUserInfo = new Dictionary<string, string>
                    {
                        { "userId", LoginTest.instance.idText },
                        { "userPassword", LoginTest.instance.passText }

                    };

                    userInfoList.Add(newUserInfo);

                    //신규유저 정보를 Json으로 변경
                    string newUser = JsonConvert.SerializeObject(userInfoList, Formatting.Indented);
                    print("신규유저정보" + newUser);

                    // JSON 데이터를 파일에 저장
                    File.WriteAllText(path, newUser);
                    print("SaveComplete" + newUser);
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
