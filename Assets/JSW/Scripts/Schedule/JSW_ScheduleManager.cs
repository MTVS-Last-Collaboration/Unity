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

        JSW_Schedule newSchedule = new JSW_Schedule(1, "test1321132");
        JSW_Schedule newSchedule1 = new JSW_Schedule(2, "test2321321");
        JSW_Schedule newSchedule2 = new JSW_Schedule(3, "test332131321");
        JSW_Schedule newSchedule3 = new JSW_Schedule(4, "test3321321312312");
        AddSchedule("20241121", newSchedule);
        AddSchedule("20241121", newSchedule);
        AddSchedule("20241122", newSchedule1);
        AddSchedule("20241123", newSchedule2);
        AddSchedule("20241101", newSchedule3);
    }

    // Start is called before the first frame update
    void Start()
    {

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
        StartCoroutine(InitCalenderforUpdate());
    }
    IEnumerator InitCalenderforUpdate()
    {
        yield return new WaitForSeconds(0.5f);
        calenderManager.InitCalender();
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

        object[] sendContent = new object[] { dayString, iconNumInput, chat };

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
}
