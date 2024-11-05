using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class JSW_ScheduleManager : MonoBehaviourPun, IOnEventCallback
{
    // 추후 아이콘도 추가할 예정

    Dictionary<string, List<JSW_Schedule>> scheduleDictionary = new Dictionary<string, List<JSW_Schedule>>();
    public TMP_InputField input_Field;
    public RectTransform trcontent;
    public GameObject scheduleFactory;
    public JSW_CalenderManager calenderManager;

    private void Awake()
    {
        input_Field = GameObject.Find("Schedule_Input").GetComponent<TMP_InputField>();
        trcontent = GameObject.Find("ScheduleContentBody").GetComponent<RectTransform>();
        calenderManager = GameObject.Find("CalenderManager").GetComponent<JSW_CalenderManager>();

        JSW_Schedule newSchedule = new JSW_Schedule(1, "test1");
        JSW_Schedule newSchedule1 = new JSW_Schedule(1, "test2");
        JSW_Schedule newSchedule2 = new JSW_Schedule(1, "test3");
        JSW_Schedule newSchedule3 = new JSW_Schedule(1, "test3");
        AddSchedule("20241021", newSchedule);
        AddSchedule("20241022", newSchedule1);
        AddSchedule("20241023", newSchedule2);
        AddSchedule("20241101", newSchedule3);
    }

    // Start is called before the first frame update
    void Start()
    {

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
            CreateScheduleItem(schedule.Description, Color.black);
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

    void CreateScheduleItem(string chat, Color chatColor)
    {
        // s의 내용으로 ChatItem을 만들자.
        GameObject go = Instantiate(scheduleFactory, trcontent);
        // 만들어진 go에서 ChatItem 컴포넌트 가져오자.
        JSW_ScheduleItem scheduleItem = go.GetComponent<JSW_ScheduleItem>();

        // 가져온 컴포넌트의 SetText 함수 실행
        scheduleItem.SetText(chat,0);
    }

    private void OnEnable()
    {

        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

    }

    public void ScheduleSubmit()
    {
        string chat = input_Field.text;
        string dayString = "" + calenderManager.nowYear.ToString() + calenderManager.nowMonth.ToString("D2") + calenderManager.nowDay.ToString("D2");

        object[] sendContent = new object[] { dayString, PhotonNetwork.NickName, chat };

        // 송신 옵션
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(1, sendContent, eventOptions, SendOptions.SendUnreliable);

        print("Send!");
        EventSystem.current.SetSelectedGameObject(null);

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
            string chat = receiveObjects[2].ToString();
            

            JSW_Schedule newSchedule = new JSW_Schedule(0, chat);
            AddSchedule(dayString, newSchedule);
            CreateScheduleItem(chat, Color.black);
        }
    }

    private void OnDisable()
    {
        //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
