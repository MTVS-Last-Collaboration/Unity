using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml;
using Unity.VisualScripting;

public class JSW_ScheduleItem : MonoBehaviourPun, IOnEventCallback
{
    public TMP_Text scheduleText;
    public GameObject scrollView;
    public Image mainImage;
    public Sprite[] iconSprite;
    public int iconNum;
    public string eventID;
    public JSW_ScheduleManager scheduleManager;

    void Start()
    {
        scheduleManager = GameObject.Find("ScheduleManager").GetComponent<JSW_ScheduleManager>();
        scheduleText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetText(string text, int Num, string eventid)
    {
        scheduleText = transform.GetChild(0).GetComponent<TMP_Text>();
        scheduleText.text = text;
        mainImage.sprite = iconSprite[Num];
        iconNum = Num;
        eventID = eventid;
    }

    public void OnClickImage()
    {
        scrollView.SetActive(!scrollView.activeSelf);
    }

    public void IconImageSetting()
    {
        mainImage.sprite = iconSprite[iconNum];
    }


    public void OnClickDelete()
    {
        DeleteCalendarEvent(eventID);
    }

    public void DeleteCalendarEvent(string eventId)
    {
        StartCoroutine(DeleteRequest(eventId));
    }


    
    private IEnumerator DeleteRequest(string eventId)
    {
        print(eventId);
        ScheduleSubmit2(eventId);
        string apiUrl = "http://125.132.216.190:12223/api/calendar/event/" + eventId; 

        using (UnityWebRequest request = UnityWebRequest.Delete(apiUrl))
        {
            // 헤더 설정
            request.SetRequestHeader("Content-Type", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

            // 요청을 보내고 응답을 기다림
            yield return request.SendWebRequest();

            // 응답 코드 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 성공적으로 데이터를 받아온 경우
                Debug.Log("이벤트 목록: 굿굿");
            }
            else
            {
                Debug.LogError("에러 발생: " + request.error);
            }
        }
    }

    public void ScheduleSubmit2(string eventnum)
    {
        object[] sendContent = new object[] {eventnum };

        // 송신 옵션
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(8, sendContent, eventOptions, SendOptions.SendUnreliable);

        EventSystem.current.SetSelectedGameObject(null);

        //string chat = input_Field.text;
        //string dayString = "" + calenderManager.nowYear.ToString() + calenderManager.nowMonth.ToString("D2") + calenderManager.nowDay.ToString("D2");
        //JSW_Schedule newSchedule = new JSW_Schedule(1, chat);
        //AddSchedule(dayString, newSchedule);
        //CreateScheduleItem(chat, Color.black);
    }

    private void OnEnable()
    {

        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 8)
        {
            print("dd");
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string evenId = receiveObjects[0].ToString();
            foreach (KeyValuePair<string, List<JSW_Schedule>> kvp in scheduleManager.scheduleDictionary)
            {
                for(int i = 0;i < kvp.Value.Count;i++)
                {
                    if (kvp.Value[i].EventID == evenId)
                    {
                        kvp.Value.RemoveAt(i);
                        scheduleManager.InitCalenderforUpdate();
                        break;
                    }
                }
            }
            if (eventID == evenId)
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnDisable()
    {
        //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
