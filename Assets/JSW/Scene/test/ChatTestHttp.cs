using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using static ChatTestHttp;


public class ChatTestHttp : MonoBehaviour
{
    [System.Serializable]
    public class EventData
    {
        public string eventName;
        public int iconNumber;
        public string eventDate;
        public string description;
    }

    void Start()
    {
        //EventData eventData = new EventData
        //{
        //    coupleId = 6,
        //    eventName = "기념일",
        //    iconNumber = 1,
        //    eventDate = "2024-12-25",
        //    description = "첫 기념일"
        //};
        //StartCoroutine(PostEvent("https://125.132.216.190:12223/api/calendar/event", eventData));
    }

    public void OnClickCalenderButton()
    {

        EventData eventData = new EventData
        {
            eventName = "데이트하는 날",
            iconNumber = 2,
            eventDate = "2024-11-01",
            description = "fd하하하"
        };

        StartCoroutine(PostEvent("http://125.132.216.190:12223/api/calendar/event", eventData));
    }

    IEnumerator PostEvent(string url, EventData eventData)
    {
        string json = JsonUtility.ToJson(eventData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        string jwtToken = LoginInfoManager.instance.myToken;
        print(jwtToken);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }



    private const string apiUrl = "http://125.132.216.190:12223/api/calendar/events";


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
                Debug.Log("이벤트 목록: " + request.downloadHandler.text);

                // JSON 데이터 파싱
                ProcessEvents(request.downloadHandler.text);
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

    // JSON 데이터 처리 메서드
    void ProcessEvents(string jsonData)
    {
        // JSON 데이터를 원하는 형식으로 파싱하고 처리할 수 있음
        // 예시로 단순히 로그에 출력
        Debug.Log("받아온 이벤트 데이터: " + jsonData);


        // 필요에 따라 JSON 데이터 파싱 예시:
        // Event[] events = JsonUtility.FromJson<EventList>(jsonData).events;
        // foreach (var e in events) {
        //     Debug.Log("이벤트 이름: " + e.eventName);
        // }
        ParseJsonToList(jsonData);
    }

    public List<Event> eventList = new List<Event>();

    void ParseJsonToList(string jsonData)
    {
        // JSON 데이터를 JArray로 변환
        JArray jsonArray = JArray.Parse(jsonData);

        // JSON 배열 내 각 객체를 EventData로 변환하여 리스트에 추가
        foreach (var item in jsonArray)
        {
            Event eventData = new Event
            {
                eventId = (int)item["eventId"],
                eventName = (string)item["eventName"],
                iconNumber = (int)item["iconNumber"],
                eventDate = $"{(int)item["eventDate"][0]}-{(int)item["eventDate"][1]:D2}-{(int)item["eventDate"][2]:D2}",
                description = (string)item["description"]
            };
            eventList.Add(eventData);
        }
    }
    // JSON 데이터 파싱을 위한 클래스 정의 예시
    [System.Serializable]
    public class Event
    {
        public int eventId;
        public string eventName;
        public int iconNumber;
        public string eventDate;
        public string description;
    }

    [System.Serializable]
    public class EventList
    {
        public Event[] events;
    }
}
