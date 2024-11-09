using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class JSW_ServerDeco : MonoBehaviour
{


    // 가구 배치를 위한 데이터 구조
    [System.Serializable]
    public class FurnitureData
    {
        public int coupleId;
        public int furnitureId;
        public int positionX;
        public int positionY;
        public int rotation;
        public int width;
        public int height;
    }

    public class FurnitureDataPut
    {
        public int positionX;
        public int positionY;
        public int rotation;
    }


    public void PostforBackSchedule(JSW_DecoObject decoobject)
    {
        FurnitureData data = new FurnitureData
        {
            coupleId = JSW_CoupleSceneManager.instance.CoupleId,
            furnitureId = 1,
            positionX = 100,
            positionY = 200,
            rotation = 90,
            width = 50,
            height = 30
        };

        StartCoroutine(PostEvent("http://125.132.216.190:12223/api/rooms/decorate", data, decoobject));
    }

    IEnumerator PostEvent(string url, FurnitureData eventData, JSW_DecoObject decoobject)
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
            print("ServerDeco인데 물건 설치할 때 설치했다고 알려줌");
            Debug.LogError("Error: " + request.error);
            decoobject.funitureLayoutId = 11;

        }
        else
        {
            // 되면 바로 여기에 넣으면 될 듯
            decoobject.funitureLayoutId = 10;

            //FurnitureData schedulepost = JsonUtility.FromJson<FurnitureData>(request.downloadHandler.text);
            Debug.Log("Response: " + request.downloadHandler.text);
            //print(schedulepost.iconNumber + " + " + schedulepost.eventDate + " + " + schedulepost.description + " + " + schedulepost.eventId.ToString());
            //ScheduleSubmit2(schedulepost.iconNumber, schedulepost.eventDate[0].ToString("D4") + schedulepost.eventDate[1].ToString("D2") + schedulepost.eventDate[2].ToString("D2"), schedulepost.description, schedulepost.eventId.ToString());
        }
    }

    //public List<Event> eventList = new List<Event>();

    //void ParseJsonToList(string jsonData)
    //{
    //    // JSON 데이터를 JArray로 변환
    //    JArray jsonArray = JArray.Parse(jsonData);

    //    // JSON 배열 내 각 객체를 EventData로 변환하여 리스트에 추가
    //    foreach (var item in jsonArray)
    //    {
    //        Event eventData = new Event
    //        {
    //            eventId = (int)item["eventId"],
    //            eventName = (string)item["eventName"],
    //            iconNumber = (int)item["iconNumber"],
    //            eventDate = $"{(int)item["eventDate"][0]}{(int)item["eventDate"][1]:D2}{(int)item["eventDate"][2]:D2}",
    //            description = (string)item["description"]
    //        };
    //        AddSchedule(eventData.eventDate, new JSW_Schedule(eventData.iconNumber, eventData.description, eventData.eventId.ToString()));
    //    }
    //}


    private const string apiDeleteUrl = "http://125.132.216.190:12223/api/rooms/furniture/";

    public void DeleteCalenderEvent(int layoutId)
    {
        StartCoroutine(DeleteEvents_CO(layoutId));
    }

    IEnumerator DeleteEvents_CO(int layoutId)
    {
        // UnityWebRequest를 사용하여 GET 요청 전송
        using (UnityWebRequest request = UnityWebRequest.Delete(apiDeleteUrl + layoutId.ToString()))
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

            }
            else if (request.responseCode == 404)
            {
                print("삭제시도");
                print(layoutId);
                Debug.LogError("이벤트를 찾을 수 없습니다.");
            }
            else
            {
                print("삭제시도2");
                print(layoutId);
                Debug.LogError("에러 발생: " + request.error);
            }
        }
    }


    private const string apiPutUrl = "http://125.132.216.190:12223/api/rooms/furniture/";

    public void PutCalenderEvent(int layoutId, int positionX1, int positionY2, int rotation3)
    {
        FurnitureDataPut data = new FurnitureDataPut
        {
            positionX = positionX1,
            positionY = positionY2,
            rotation = rotation3
        };

        StartCoroutine(PutEvents_CO(layoutId, data));
    }

    IEnumerator PutEvents_CO(int layoutId, FurnitureDataPut eventData)
    {
        // UnityWebRequest를 사용하여 GET 요청 전송
        using (UnityWebRequest request = new UnityWebRequest(apiPutUrl + layoutId.ToString() + "/move", "PUT"))
        {
            print(apiPutUrl + layoutId.ToString() + "/move");


            // JSON 직렬화
            string jsonBody = JsonUtility.ToJson(eventData);

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

            }
            else if (request.responseCode == 404)
            {
                print("삭제시도");
                print(layoutId);
                Debug.LogError("이벤트를 찾을 수 없습니다.");
            }
            else
            {
                print("삭제시도2");
                print(layoutId);
                Debug.LogError("에러 발생: " + request.error);
            }
        }
    }
}