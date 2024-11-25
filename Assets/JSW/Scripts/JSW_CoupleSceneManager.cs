using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JSW_CoupleSceneManager : MonoBehaviour
{
    private const string apiUrl = "http://125.132.216.190:12223/api/couple";

    public static JSW_CoupleSceneManager instance;
    public int CoupleId;
    public string CoupleCode;
    public int Points;
    public string AnniversaryDate;
    public DecoShopManager decoShopManager;
    public Text coupleDays;
    public int coupleDays_Num;
    public TMP_Text coupleMongDays;
    public TMP_Text coupleNickName;

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

                DateTime givenDate = DateTime.Parse(AnniversaryDate);
                // 오늘 날짜 가져오기
                DateTime today = DateTime.Today;

                // 두 날짜의 차이를 계산
                int difference = (today - givenDate).Days;
                coupleDays_Num = difference;
                coupleDays.text = "<<color=\"#FF5733\"> D - " + difference.ToString() + "</color> >";
                coupleMongDays.text = "D - " + difference.ToString();

                string CoupleCompleteNick = "";

                if (difference < 30)
                {
                    CoupleCompleteNick = "이제 막 알아가는";
                }
                else if (difference <= 99)
                {
                    CoupleCompleteNick = "매일 새롭고 설레는";
                }
                else if (difference <= 199)
                {
                    CoupleCompleteNick = "서로의 일상을 함께 하는";
                }
                else if (difference <= 299)
                {
                    CoupleCompleteNick = "서로에게 익숙해진";
                }
                else if (difference <= 365)
                {
                    CoupleCompleteNick = "함께 1년을 보낸";
                }
                else if (difference <= 499)
                {
                    CoupleCompleteNick = "함께 웃고 울었던";
                }
                else if (difference <= 699)
                {
                    CoupleCompleteNick = "나만의 베프이자 사랑스러운";
                }
                else if (difference <= 999)
                {
                    CoupleCompleteNick = "천 일의 시간을 함께 한";
                }
                else 
                {
                    CoupleCompleteNick = "평생을 함께 하고픈 특별한";
                }
                coupleNickName.text = CoupleCompleteNick;
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
