using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static JSW_ServerDeco;
using static UnityEngine.Rendering.DebugUI;

public class JSW_PetManager : MonoBehaviour
{
    public GameObject Mong;
    public GameObject mongUI;
    public string MongName;
    public int mongLevel = 0;
    public float mongExp = 0;
    public float mongExpTarget = 0;

    public TMP_Text mongLevel_text;
    public TMP_Text nickName_text;
    public TMP_Text mongLevel_Exp;

    public Image expPercent;
    bool isOpenMongUI;

    public TMP_Text mainMongText;
    public GameObject GifticonImage;
    public GameObject mongBackground2;

    public TMP_Text MongName_TMP;
    public GameObject MongName_InputField;

    public GameObject completeAnswer;
    public GameObject NOcompleteAnswer;

    void Start()
    {
        mongExp = 0;
        MongLevelUpGet();
        //MongNickname("동글이");
        if (MongName_InputField != null)
        {
            MongName_InputField.GetComponent<TMP_InputField>().onEndEdit.AddListener(MongNickname);
        }
        StartCoroutine(GetAboutMinssion());
        //nickName_text.text = "어른이 된 " + MongName;
    }

    private void Update()
    {
        mainMongText.text = mongLevel.ToString();
        if (mongUI.activeSelf == true)
        {
            mongExp = Mathf.Lerp(mongExp, mongExpTarget, Time.deltaTime);
            expPercent.fillAmount = mongExp / 100;
            mongLevel_Exp.text = ((int)mongExp).ToString() + "%";
            

            if (mongExpTarget - mongExp <= 0.4f)
            {
                mongExp = mongExpTarget;
            }

            if (mongLevel >= 20 && mongExpTarget!= 100)
            {
                OpenGifticon();
                mongExpTarget = 100;
                return;
            }


            if (mongLevel != 20 && mongExp >= 100)
            {
                mongExp = 0;
                mongExpTarget -= 100;
                mongLevel += 1;
                SetNickNameMong();
            }

            if (isOpenMongUI == false)
            {
                isOpenMongUI = true;
            }
        }
        else
        {
            isOpenMongUI = false;
            mongExp = 0;
        }
    }

    public void MongLevelUp()
    {
        //if (mongLevel < 3)
        //{
        //    transform.GetChild(mongLevel++).gameObject.SetActive(false);
        //    transform.GetChild(mongLevel).gameObject.SetActive(true);
        //}
        StartCoroutine(AddPetExperience());
    }

    private string url = "http://125.132.216.190:12223/api/pet/add-exp";

    IEnumerator AddPetExperience()
    {
        // 요청을 생성합니다.
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        // 인증 토큰이 필요한 경우 헤더에 추가합니다.
        // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN");
        string jwtToken = LoginInfoManager.instance.myToken;

        request.SetRequestHeader("Accept", "application/json");
        //request.SetRequestHeader("accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청을 서버로 전송하고 응답을 기다립니다.
        yield return request.SendWebRequest();

        // 서버의 응답을 확인합니다.
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 성공 응답 처리
            Debug.Log("경험치 추가 성공: " + request.downloadHandler);
        }
        else
        {
            // 실패 응답 처리
            Debug.LogError("경험치 추가 실패: " + request.error);
        }
        mongExpTarget += 500;
    }

    public void MongLevelUpnineteen()
    {
        mongLevel = 19;
    }

    public void MongLevelUpGet()
    {
        StartCoroutine(GetPetStatus());
    }
    private string url2 = "http://125.132.216.190:12223/api/pet";


    [System.Serializable]
    public class PetStatus
    {
        public string name;
        public int level;
        public int experience;
    }


    IEnumerator GetPetStatus()
    {
        // GET 요청을 생성합니다.
        UnityWebRequest request = UnityWebRequest.Get(url2);

        string jwtToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청을 서버로 전송하고 응답을 기다립니다.
        yield return request.SendWebRequest();

        // 서버의 응답을 확인합니다.
        if (request.result == UnityWebRequest.Result.Success)
        {
            //// 성공 응답 처리
            //Debug.Log("펫 상태 조회 성공: " + request.downloadHandler.text);
            PetStatus petStatus = JsonUtility.FromJson<PetStatus>(request.downloadHandler.text);
            Debug.Log("펫 상태 조회 성공: 레벨 = " + petStatus.level + ", 경험치 = " + petStatus.experience);
            MongName = petStatus.name;
            mongLevel = petStatus.level;
            mongExpTarget = petStatus.experience;
            SetNickNameMong();
        }
        else
        {
            // 실패 응답 처리
            Debug.LogError("펫 상태 조회 실패: " + request.error);
        }
    }

    void SetNickNameMong()
    {
        mongLevel_text.text = "레벨 " + mongLevel.ToString();
        if (1 <= mongLevel && mongLevel <= 5)
        {
            nickName_text.text = "갓 태어난 " + MongName;
            Mong.transform.GetChild(0).gameObject.SetActive(true);

        }
        else if (6 <= mongLevel && mongLevel <= 10)
        {
            Mong.transform.GetChild(0).gameObject.SetActive(false);
            nickName_text.text = "사춘기의 " + MongName;
            Mong.transform.GetChild(1).gameObject.SetActive(true);
        }
        else if (11 <= mongLevel && mongLevel <= 15)
        {
            Mong.transform.GetChild(1).gameObject.SetActive(false);
            nickName_text.text = "의젓한 " + MongName;
            Mong.transform.GetChild(2).gameObject.SetActive(true);
        }
        else if (16 <= mongLevel && mongLevel <= 20)
        {
            Mong.transform.GetChild(2).gameObject.SetActive(false);
            nickName_text.text = "어른이 된 " + MongName;
            Mong.transform.GetChild(3).gameObject.SetActive(true);
        }
        MongName_TMP.text = MongName;
    }


    public void MongNickname(string nickName)
    {
        //string nickName = MongName_InputField.GetComponent<TMP_InputField>().text;
        if (nickName == "" || nickName == null) return;
        MongName = nickName;
        MongName_TMP.text = nickName;
        StartCoroutine(ChangeNickname(nickName));
        MongName_InputField.SetActive(false);
        SetNickNameMong();
    }
    public void ClickMongName()
    {
        MongName_InputField.SetActive(true);
    }

    private string Nickurl = "http://125.132.216.190:12223/api/pet/name";

    public class Namess
    {
        public string name;
    }


    IEnumerator ChangeNickname(string nickName)
    {
        // 요청을 생성합니다.
        UnityWebRequest request = new UnityWebRequest(Nickurl, "PUT");

        Namess data = new Namess
        {
            name = nickName
        };

        string json = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        // 인증 토큰이 필요한 경우 헤더에 추가합니다.
        // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        string jwtToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청을 서버로 전송하고 응답을 기다립니다.
        yield return request.SendWebRequest();

        // 서버의 응답을 확인합니다.
        if (request.result == UnityWebRequest.Result.Success)
        {
            // 성공 응답 처리
            Debug.Log("이름 변경 성공: " + request.downloadHandler);
        }
        else
        {
            // 실패 응답 처리
            Debug.LogError("이름 변경 실패: " + request.error);
        }
    }

    public void OpenGifticon()
    {
        GifticonImage.SetActive(true);
        mongBackground2.SetActive(true);
        Vector3 size = GifticonImage.transform.localScale;
        GifticonImage.transform.localScale = Vector3.one * 0.2f;
        iTween.ScaleTo(GifticonImage, iTween.Hash(
        "scale", size, // 최종 크기
        "time", 1.5f,         // 애니메이션 시간
        "easetype", iTween.EaseType.easeOutElastic // 애니메이션 타입
        ));
    }
    public void CloseGifticon()
    {
        GifticonImage.SetActive(false);
        mongBackground2.SetActive(false);
    }


    public void TodayMissonGetServer()//�������� ���� �̼��� ��������.
    {
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

            // 원하는 형식의 문자열로 변환
          
            string completed;

            completed = jsonObj["completed"].ToString();

            if (completed == "True")
            {
                completeAnswer.SetActive(true);
                NOcompleteAnswer.SetActive(false);
                print("답변 True래");
            }
            else
            {
                completeAnswer.SetActive(false);
                NOcompleteAnswer.SetActive(true);
                print("답변 False래");
            }
        }
    }





    [System.Serializable]
    public class MissionStatus
    {
        public int missionNumber;
        public string missionDate;
        public string missionContent;
        public string partnerMood;
        public string partnerAnswer;
        public string partner2Mood;
        public string partner2Answer;
        public bool completed;
    }

    private string missionUrl = "http://125.132.216.190:12223/api/missions/current";

    IEnumerator GetAboutMinssion()
    {
        // GET 요청을 생성합니다.
        UnityWebRequest request = UnityWebRequest.Get(missionUrl);

        string jwtToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청을 서버로 전송하고 응답을 기다립니다.
        yield return request.SendWebRequest();

        // 서버의 응답을 확인합니다.
        if (request.result == UnityWebRequest.Result.Success)
        {
            //// 성공 응답 처리
            //Debug.Log("펫 상태 조회 성공: " + request.downloadHandler.text);
            MissionStatus missionStatus = JsonUtility.FromJson<MissionStatus>(request.downloadHandler.text);
            if (missionStatus.completed)
            {
                completeAnswer.SetActive(true);
                NOcompleteAnswer.SetActive(false);
            }
            else
            {
                completeAnswer.SetActive(false);
                NOcompleteAnswer.SetActive(true);
            }
        }
        else
        {
            // 실패 응답 처리
            Debug.LogError("미션 상태 조회 실패: " + request.error);
        }
    }
}
