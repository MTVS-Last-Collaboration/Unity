using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static JSW_PetManager;
using UnityEngine.Networking;
using TMPro;
using static LoginTest;

public class HoonPointInfo : MonoBehaviour
{
    private const string apiUrl = "http://125.132.216.190:12223/api/couple";

    public int CoupleId;
    public string CoupleCode;
    public int Points;
    public string AnniversaryDate;
    //public DecoShopManager decoShopManager;
    //커플포인트
    public TextMeshProUGUI textPrice;

    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    //jswcouplescenmanager getevent 에서 가져오자.]

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
                //decoShopManager.point = Points;

                textPrice.text = Points.ToString(); //포인트를 문자열로 변경하여 표시
                //Debug.LogError("커플포인트" + Points.ToString());

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




}//클래스끝
