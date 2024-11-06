using ExitGames.Client.Photon;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class JSW_ScheduleManager : MonoBehaviourPun, IOnEventCallback
{
    // 추후 아이콘도 추가할 예정

    public Dictionary<string, List<JSW_Schedule>> scheduleDictionary = new Dictionary<string, List<JSW_Schedule>>();
    public TMP_InputField input_Field;
    public RectTransform trcontent;
    public GameObject scheduleFactory;
    public JSW_CalenderManager calenderManager;
    public TMP_Text scheduleNowDay;
    public GameObject inputSchedule;
    public GameObject inputIcon;
    public int iconNumInput;

    private void Awake()
    {
        //input_Field = GameObject.Find("Schedule_Input").GetComponent<TMP_InputField>();
        trcontent = GameObject.Find("ScheduleContentBody").GetComponent<RectTransform>();
        calenderManager = GameObject.Find("CalenderManager").GetComponent<JSW_CalenderManager>();
        GetCalenderEvent();
        //JSW_Schedule newSchedule = new JSW_Schedule(1, "test1321132");
        //JSW_Schedule newSchedule1 = new JSW_Schedule(2, "test2321321");
        //JSW_Schedule newSchedule2 = new JSW_Schedule(3, "test332131321");
        //JSW_Schedule newSchedule3 = new JSW_Schedule(4, "test3321321312312");
        //AddSchedule("20241121", newSchedule);
        //AddSchedule("20241121", newSchedule);
        //AddSchedule("20241122", newSchedule1);
        //AddSchedule("20241123", newSchedule2);
        //AddSchedule("20241101", newSchedule3);
    }

    // Start is called before the first frame update
    void Start()
    {
        //GetCalenderEvent();
    }

    public void OnClickCreateSchedule()
    {
        if(inputSchedule.activeSelf == false) inputSchedule.SetActive(true);
        else
        {
            iconNumInput = 0;
            inputSchedule.SetActive(false);
        }
    }
    public void OnClickCreateIcon()
    {
        if (inputIcon.activeSelf == false) inputIcon.SetActive(true);
        else
        {
            inputIcon.SetActive(false);
        }
    }

    public void ResetSchedule(string day)
    {
        foreach (RectTransform child in trcontent.transform)
        {
            Destroy(child.gameObject);
        }
        List<JSW_Schedule> schedules = GetSchedules(day);
        foreach (JSW_Schedule schedule in schedules)
        {
            CreateScheduleItem(schedule.Description, schedule.iconCode);
        }
    }


    public void AddSchedule(string date, JSW_Schedule schedule)
    {
        if (!scheduleDictionary.ContainsKey(date))
        {
            scheduleDictionary[date] = new List<JSW_Schedule>();
        }
        scheduleDictionary[date].Add(schedule);
    }

    public List<JSW_Schedule> GetSchedules(string date)
    {
        if (scheduleDictionary.ContainsKey(date))
        {
            return scheduleDictionary[date];
        }
        return new List<JSW_Schedule>();
    }

    void CreateScheduleItem(string chat, int iconNum)
    {
        // s의 내용으로 ChatItem을 만들자.
        GameObject go = Instantiate(scheduleFactory, trcontent);
        // 만들어진 go에서 ChatItem 컴포넌트 가져오자.
        JSW_ScheduleItem scheduleItem = go.GetComponent<JSW_ScheduleItem>();
        // 가져온 컴포넌트의 SetText 함수 실행
        scheduleItem.SetText(chat, iconNum);
        //StartCoroutine(InitCalenderforUpdate());
    }

    //public void InitCalenderforUpdate()
    //{
    //    StartCoroutine(InitCalenderforUpdate_CO());
    //}
    //IEnumerator InitCalenderforUpdate_CO()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    calenderManager.InitCalender();
    //}



    private void OnEnable()
    {

        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

    }

    public void ScheduleSubmit()
    {
        string chat = input_Field.text;
        string dayString = "" + calenderManager.nowYear.ToString() +"-" + calenderManager.nowMonth.ToString("D2") +"-" + calenderManager.nowDay.ToString("D2");

        object[] sendContent = new object[] { dayString, iconNumInput, chat };
        PostforBackSchedule(iconNumInput, dayString, chat);
        print("sss");

        // 송신 옵션
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(1, sendContent, eventOptions, SendOptions.SendUnreliable);

        print("Send!");
        EventSystem.current.SetSelectedGameObject(null);
        inputSchedule.SetActive(false);

        //string chat = input_Field.text;
        //string dayString = "" + calenderManager.nowYear.ToString() + calenderManager.nowMonth.ToString("D2") + calenderManager.nowDay.ToString("D2");
        //JSW_Schedule newSchedule = new JSW_Schedule(1, chat);
        //AddSchedule(dayString, newSchedule);
        //CreateScheduleItem(chat, Color.black);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 1)
        {
            // 받은 내용을 "닉네임: 채팅 내용" 형식으로 스크롤뷰의 텍스트에 전달한다.
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string dayString = receiveObjects[0].ToString();
            int INum = (int)receiveObjects[1];
            string chat = receiveObjects[2].ToString();
            

            JSW_Schedule newSchedule = new JSW_Schedule(INum, chat);
            AddSchedule(dayString, newSchedule);
            string nowdate = "" + calenderManager.nowYear.ToString() + calenderManager.nowMonth.ToString("D2") + calenderManager.nowDay.ToString("D2");
            if (dayString == nowdate) CreateScheduleItem(chat, INum);
        }
    }

    private void OnDisable()
    {
        //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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
                eventDate = $"{(int)item["eventDate"][0]}{(int)item["eventDate"][1]:D2}{(int)item["eventDate"][2]:D2}",
                description = (string)item["description"]
            };
            AddSchedule(eventData.eventDate, new JSW_Schedule(eventData.iconNumber, eventData.description));
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

    [System.Serializable]
    public class EventClenderData
    {
        public string eventName;
        public int iconNumber;
        public string eventDate;
        public string description;
    }

    public void PostforBackSchedule(int iconNum, string date, string discript)
    {
        print("sss11111");

        EventClenderData eventData = new EventClenderData
        {
            eventName = "데이트하는 날",
            iconNumber = iconNum,
            eventDate = date,
            description = discript
        };
        print(eventData.eventDate);
        StartCoroutine(PostEvent("http://125.132.216.190:12223/api/calendar/event", eventData));
    }

    IEnumerator PostEvent(string url, EventClenderData eventData)
    {

        string json = JsonUtility.ToJson(eventData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        string jwtToken = LoginInfoManager.instance.myToken;

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");


        yield return request.SendWebRequest();
        print(url);        

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }


    }

}
