using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSW_CoupleSceneManager : MonoBehaviour
{
    private const string apiUrl = "http://125.132.216.190:12223/api/couple";

    public static JSW_CoupleSceneManager instance;
    public int CoupleId;
    public string CoupleCode;
    public int Points;
    public string AnniversaryDate;
    public DecoShopManager decoShopManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        GetCalenderEvent();
    }


    public class CoupleEventTest
    {
        public int coupleId;
        public string coupleCode;
        public int points;
        public string anniversaryDate;
    }


    public void GetCalenderEvent()
    {
        StartCoroutine(GetEvents());
    }

    IEnumerator GetEvents()
    {
        // UnityWebRequest를 사용하여 GET 요청 전송
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            // 헤더 설정
            request.SetRequestHeader("Accept", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

            // 요청을 보내고 응답을 기다림
            yield return request.SendWebRequest();

            // 응답 코드 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 성공적으로 데이터를 받아온 경우
                CoupleEventTest couple = JsonUtility.FromJson<CoupleEventTest>(request.downloadHandler.text);
                CoupleId = couple.coupleId;
                CoupleCode = couple.coupleCode;
                Points = couple.points;
                AnniversaryDate = couple.anniversaryDate;
                decoShopManager.point = Points;

                // JSON 데이터 파싱
                //ProcessEvents(request.downloadHandler.text);
            }
            else if (request.responseCode == 404)
            {
                Debug.LogError("이벤트를 찾을 수 없습니다.");
            }
            else
            {
                Debug.LogError("에러 발생: " + request.error);
            }
        }
    }
}
