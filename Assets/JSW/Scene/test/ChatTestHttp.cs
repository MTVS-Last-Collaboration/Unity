using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using Newtonsoft.Json.Linq;


public class ChatTestHttp : MonoBehaviour
{
    [System.Serializable]
    public class EventData
    {
        public int coupleId;
        public string eventName;
        public int iconNumber;
        public string eventDate;
        public string description;
    }

    void Start()
    {
        print("dd4");
        EventData eventData = new EventData
        {
            coupleId = 1,
            eventName = "±‚≥‰¿œ",
            iconNumber = 1,
            eventDate = "2024-12-25",
            description = "√π ±‚≥‰¿œ"
        };
        StartCoroutine(PostEvent("https://125.132.216.190:12223/api/calendar/event", eventData));
    }

    public void OnClickCalenderButton()
    {
        print("dd3");
        EventData eventData = new EventData
        {
            coupleId = 1,
            eventName = "±‚≥‰¿œ",
            iconNumber = 1,
            eventDate = "2024-12-25",
            description = "√π ±‚≥‰¿œ"
        };

        StartCoroutine(PostEvent("https://125.132.216.190:12223/api/calendar/event", eventData));
    }

    IEnumerator PostEvent(string url, EventData eventData)
    {
        string json = JsonUtility.ToJson(eventData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "application/json");

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
}
