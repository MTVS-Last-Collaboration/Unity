using System.Collections;
using System.Collections.Generic;
//TMP_InputField 사용에 필요 
using TMPro;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Networking;
using File = System.IO.File;
using System;
using System.IO;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Antlr3.Runtime;
using static RegistTest;
using static LoginTest;
using static System.Net.WebRequestMethods;
using static UnityEngine.Rendering.DebugUI;


public class LoginTest : MonoBehaviour
{
    public static LoginTest instance;
    public ConnectionManager connectionManager;

    private void Awake()
    {
        instance = this;
    }

    LoginUI loginUI;

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
    public string myToken;
    public string myCoupleCode;
    public string myCoupleCodeJson;
    public TMP_InputField input_CoupleCode;
    public GameObject loginImage;
    public GameObject allRegistObject;
    public TMP_InputField viewMyCoupleCode;
    public GameObject coupleMenu;
    public GameObject coupleMenu2to1;
    public GameObject coupleMenu3;
    public HoonSoundManagerLogin hoonSoundManagerLogin;
    public HoonChoiceRoom hoonChoiceRoom;
    public GameObject Img_ChoiceRoomError;
    public GameObject Img_CreatingRoom;


    public class UserInfo
    {
        public string email;
        public string password;
       
    }
    public class CoupleCode
    {
        public string coupleCode;
    }
    public class PartnerNickName
    {
        public string partnerNickName;
    }

    public class MyInfoResponse
    {
        public string nickname;
        public string partnerNickName;
        public string gender;
        //public string anniversaryDate;
        public List<int> anniversaryDate; // JSON 배열을 List<int>로 받기
        public string coupleCode;
    }

    // Start is called before the first frame update
    void Start()
    {
        input_Id = input_Id_Object.GetComponent<TMP_InputField>();
        input_Pass = input_Pass_Object.GetComponent<TMP_InputField>();
        placeHole_Id_Text = placeHold_Id_Object.GetComponent<TextMeshProUGUI>();
        placeHole_Pass_Text = placeHold_Psss_Object.GetComponent <TextMeshProUGUI>();
        loginUI = transform.GetComponent<LoginUI>();
        Img_ChoiceRoomError.SetActive(false);

    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
    public void LoginUserInfo()
    {

    }

    public void CreateCoupleCode()
    {
         CoupleCode cpcd = new CoupleCode();
         {
             cpcd.coupleCode = input_CoupleCode.text;
             //cpcd.coupleCode = "AAAAAA";

         }

        string jsonString = JsonConvert.SerializeObject(cpcd);
        //string jsonString = myCoupleCodeJson;
        //string jsonString = input_CoupleCode.text;
        print("커플코드" + jsonString);
        print("내토큰보기" + myToken);

      
        StartCoroutine(PostCreateCoupleCode(jsonString));
        //print("내가보낸 커플코드 " + jsonString);

    }

    IEnumerator PostCreateCoupleCode(string jsonData)
    {
        string url = "http://125.132.216.190:12223/api/couple/join"; //couplecode join
        UnityWebRequest request = new UnityWebRequest(url, "POST");  // HTTP POST 요청 준비

        // JSON 데이터를 담아 요청 생성
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발사

        // 여기에 토큰 추가
        //string token = "your_token_here"; // 실제 토큰 값을 여기에 설정합니다.

        //서버의 응답을 기다리는중~

        yield return request.SendWebRequest();

        // 응답 코드 확인
        long responseCode = request.responseCode;

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("HTTP Status Code: " + responseCode);
            // 추가 응답 메시지
            Debug.LogError("서버 응답 내용: " + request.downloadHandler.text);

            //커플코드 연결 성공처리

            
        }
        else //응답성공
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버 응답: " + responseText);
            //서버가 응답하면 커플정보를 불러오자.



            // 서버 응답과 newUser 정보가 같은지 확인
            if (responseText.Contains(input_Id.text))
            {
                Debug.Log("서버 응답과 신규 유저 정보가 일치합니다.");

                
            }
            else
            {
                Debug.LogWarning("서버 응답과 신규 유저 정보가 일치하지 않습니다.");
            }


            //여기에서 UI Img_CoupleMenu3 꺼야합니다.
            coupleMenu3.SetActive(false);
            CheckUserInfo();


        }

    }

    public void CheckLoginUserInfo() //SeverLogin
    {
        hoonSoundManagerLogin.PlaySound(0);

        UserInfo userInfo = new UserInfo();
        {
            userInfo.email = input_Id.text;
            userInfo.password = input_Pass.text;
        }

        string jsonString = JsonConvert.SerializeObject(userInfo); //구조를 파싱
        print("id,text" + jsonString);

        StartCoroutine(PostCheckUserInfo(jsonString));

    }

    IEnumerator PostCheckUserInfo(string jsonData)
    {
        string url = "http://125.132.216.190:12223/api/auth/login"; // 서버 URL 변경 필요

        UnityWebRequest request = new UnityWebRequest(url, "POST");  // HTTP POST 요청 준비

        // JSON 데이터를 담아 요청 생성
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 여기에 토큰 추가
        //string token = "authorization: Bearer eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiIxQGV4YW1wbGUuY29tIiwidHlwZSI6ImFjY2VzcyIsInVzZXJJZCI6MSwibmlja25hbWUiOiLrgqjsnpAiLCJhdXRoIjoiVVNFUiIsImlhdCI6MTczMDQ0MTI3NCwiZXhwIjoxNzMwNDQ0ODc0fQ.gKuTSUGDDWVHRHQMvzv8rzZTu6JqeSnGZauaTY9B3ZE "; // 실제 토큰 값을 여기에 설정합니다.
        //request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러500
        }
        else //연결이 잘되면.
        {
            string responseText = request.downloadHandler.text;
            print("서버 응답: " + responseText);
            myCoupleCodeJson = responseText;
            //print("커플코드json" + myCoupleCodeJson);
            CoupleCode  cc = JsonConvert.DeserializeObject<CoupleCode>(responseText); //json을 문자열로 파싱하기.
            myCoupleCode = cc.coupleCode; //정보 넣기
            print("커플코드 " + myCoupleCode); //커플코드출력
            LoginInfoManager.instance.coupleCode = myCoupleCode;
            PartnerNickName pnn = JsonConvert.DeserializeObject<PartnerNickName>(responseText);
            LoginInfoManager.instance.partnerNickName = pnn.partnerNickName;
            //print("로그인인포 매니저 커플코드 " + LoginInfoManager.instance.coupleCode);

            string authHeader = request.GetResponseHeader("authorization");
            string accessToken = authHeader.Substring("Bearer ".Length).Trim();
            //Debug.Log("Access Token: " + accessToken);
            myToken = accessToken; //토큰저장변수
            //print("내토큰보기" + myToken);
            PlayerPrefs.SetString("token", myToken); //플레이어 프리펩에 토큰저장
            LoginInfoManager.instance.myToken = myToken;  //로그인인포에 토큰저장
            //print("플레이어 프리팹 내토큰" + PlayerPrefs.GetString("token"));
            //Access Token: eyJhbGciOiJIUzI1NiJ9.eyJzdWIiOiJhYmNAbmF2ZXIuY29tIiwidHlwZSI6ImFjY2VzcyIsInVzZXJJZCI6Mywibmlja25hbWUiOiLtlZzqta3rjIDsnqUiLCJhdXRoIjoiVVNFUiIsImNvdXBsZUlkIjozLCJpYXQiOjvE3MzA3MDgzODQsImV4cCI6MTczMDcxMTk4NH0.gpZys92FhA63oRm_Qxu_7O5oK-GLnUWrv7trmJzrick

            // 서버 응답과 newUser가 같은지 확인
           /* if (responseText == input_Id.text)
            {
                Debug.Log("서버 응답과 신규 유저 정보가 일치합니다.");
            }
            else
            {
                Debug.LogWarning("서버 응답과 신규 유저 정보가 일치하지 않습니다.");
            }*/

            CheckUserInfo(); //내정보가져오기

            /*allRegistObject.SetActive(false);
            loginImage.SetActive(false);*/

        }
    
    }
    
    public void CheckUserInfo()
    {   
        /*List<UserInfo> userInfoList = new List<UserInfo>(); //정보를 담을 리스트

        UserInfo userInfo = new UserInfo();
        {
            userInfo.email = input_Id.text;
            userInfo.password = input_Pass.text;
        }
        userInfoList .Add(userInfo);


        //string jsonString = JsonConvert.SerializeObject(userInfoList); //리스트로 파싱 담기
         string jsonString = JsonConvert.SerializeObject(userInfo); //구조를 파싱
        print("id,text" + jsonString);*/

        StartCoroutine(GetUserInfo());

    }

    IEnumerator GetUserInfo()
    {
        string urlMyInfo = "http://125.132.216.190:12223/api/auth/my-info";
        //string urlTodayMission = "http://125.132.216.190:12223/api/missions/current"; //테스트용

        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlMyInfo); //Get url
        //UnityWebRequest request = UnityWebRequest.Get(urlTodayMission); //Get url 미션가져오기테스트용
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        
        // 여기에 토큰 추가
        //string token = "your_token_here"; // 실제 토큰 값을 여기에 설정합니다.
        //request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러500
        }
        else
        {
            string responseText = request.downloadHandler.text;
            print("유저정보 서버응답: " + responseText); // 내가 받은 정보

            //responseText 응답결과
            /*{
                "nickname":"한국대장",
                "gender":"MALE",
                "anniversaryDate":[2024, 11, 4],
                "coupleCode":"1X4J9"
            }*/

            //받은 정보를 json으로 파싱하고 List에 저장하고, 리스트의 각 항복에 대한 keyvalue값을 저장하자.
            MyInfoResponse myInfo = JsonUtility.FromJson<MyInfoResponse>(responseText);
            LoginInfoManager.instance.nickName = myInfo.nickname;
            print("내닉네임" + LoginInfoManager.instance.nickName);
            //LoginInfoManager.instance.partnerNickName = myInfo.partnerNickName;
            //LoginInfoManager.instance.partnerNickName = myInfo.nickname;

            LoginInfoManager.instance.avataChoice = myInfo.gender;
            print("내아바타" + LoginInfoManager.instance.avataChoice);
            DateTime anniversary = new DateTime(myInfo.anniversaryDate[0], myInfo.anniversaryDate[1], myInfo.anniversaryDate[2]);
            print("날짜0번" + myInfo.anniversaryDate[0] + "날짜1번"+ myInfo.anniversaryDate[1] + "날짜2번" + myInfo.anniversaryDate[2]);
            LoginInfoManager.instance.coupleDay = anniversary.ToString("yyyy-MM-dd");
            print("내기념일 " + LoginInfoManager.instance.coupleDay);
            LoginInfoManager.instance.coupleCode = myInfo.coupleCode;
            print("내커플코드" + LoginInfoManager.instance.coupleCode);

            LoginInfoManager.instance.isLogin = true;


            //print("날짜 파싱 " + anniversary);
            /*  LoginInfoManager.instance.coupleDay = anniversary.ToString("yyyy-MM-dd");
              print("내기념일" + LoginInfoManager.instance.coupleDay);
              LoginInfoManager.instance.coupleCode = myInfo.coupleCode;
              print("내커플코드" + LoginInfoManager.instance.coupleCode);*/

            // 서버 응답과 newUser가 같은지 확인
           /* if (responseText == input_Id.text)
            {
                Debug.Log("서버 응답과 신규 유저 정보가 일치합니다.");
            }
            else
            {
                Debug.LogWarning("서버 응답과 신규 유저 정보가 일치하지 않습니다.");
            }*/

            //여기에서 회원가입 이미지를 꺼야 합니다.
            allRegistObject.SetActive(false);
            //커플코드입력필드에 내코드를 넣어주기
            viewMyCoupleCode.text = myInfo.coupleCode;
            //print(viewMyCoupleCode.text);
            //viewMyCoupleCode.GetComponent<TextMeshPro>().text = myInfo.coupleCode;
            //뭐하는거냐고

            //UI 닫음.
            //CloseLoginUI();
            print("로그인UI닫기");
            StartCoroutine(ChangeDownPositionLoginUI(loginImage));
            
        }

    }
   
    IEnumerator ChangeDownScaleLoglinUI()
    {
        //창의 크기를 줄이자.
        Vector3 max = Vector3.one;
        Vector3 min = Vector3.zero;
        float durationTime = 1f; //변하는시간
        float currentTime = 0f; //시작시간

        print("줄어들게하자");
        while (currentTime < durationTime)
        {
            currentTime += Time.deltaTime; //시간누적
            float t = currentTime / durationTime;

            loginImage.transform.localScale = Vector3.Lerp(max, min, t);
            yield return null;
        }

        allRegistObject.SetActive(false);
        loginImage.SetActive(false);

    }

    IEnumerator ChangeDownPositionLoginUI(GameObject obj)
    {
        
        //창의 위치를 내리자.
        Vector3 max = Vector3.zero; 
        Vector3 min = new Vector3(0,-800,0);
        float durationTime = 0.3f; //변하는시간
        float currentTime = 0f; //시작시간

        print("UI를내리자");
        while (currentTime < durationTime)
        {
            currentTime += Time.deltaTime; //시간누적
            float t = currentTime / durationTime;

            obj.transform.localPosition = Vector3.Lerp(max, min, t);

            
            yield return null;
        }
        allRegistObject.SetActive(false);
        loginImage.SetActive(false);

    }

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

            print("유저리스트 있음, 유저검색시작");
            //리스트파일을 순차검사
            foreach (var userInfo in userInfoList)
            {

                //아이디와 패스워드가 일치하는지 확인
                //if (userInfo["userId"] == idText && userInfo["userPassword"] == passText)
                if (userInfo.email == idText && userInfo.password == passText)
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

    public void Login()
    {
        hoonChoiceRoom = transform.GetComponent<HoonChoiceRoom>();

        //방을 선택한것으로 간주하기
        hoonChoiceRoom.isViewChoiveMark = true;
        if (hoonChoiceRoom.isViewChoiveMark)
        {
            print("방을선택했습니다");
            Img_CreatingRoom.SetActive(true);
        }
        else
        {
            print("방을선택해야합니다.");
            Img_ChoiceRoomError.SetActive(true);
            return;
        }


        hoonSoundManagerLogin.PlaySound(0);
        print("로그인완료, 로비생성하기");
        //SceneManager.LoadScene("");
        connectionManager.StartLobby();
    }

    public void CopyCoupleCode()
    {
        // InputField에 입력된 텍스트를 클립보드로 복사
        GUIUtility.systemCopyBuffer = viewMyCoupleCode.text;
        Debug.Log("텍스트가 클립보드에 복사되었습니다: " + viewMyCoupleCode.text);

        coupleMenu2to1.SetActive(false);
        //coupleMenu.SetActive(true);





    }
}
