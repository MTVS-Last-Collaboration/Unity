using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using static ChatTestHttp;

using TMPro;


public class JSW_DecoMineWall : JSW_DecoMineObject
{
    public int ResourceName;
    public DecoMineManager decoMineManager;
    //public bool isPurchased;
    //public TMP_Text isMineText;


    private void Awake()
    {
        decoMineManager = GameObject.Find("DecoMineManager").GetComponent<DecoMineManager>();
    }

    private void Start()
    {
        isMineText = transform.transform.GetChild(2).GetComponent<TMP_Text>();
        initRoom = GameObject.Find("DecorateRoomManager").GetComponent<JSW_InitRoom>();
        if (initRoom.initShopId[shopid])
        {
            isPurchased = true;
            //transform.GetChild(1).GetComponent<TMP_Text>().text = "소유중";
            transform.GetChild(2).GetComponent<TMP_Text>().text = "소유중";
        }
        else
        {
            isPurchased = false;
            transform.GetChild(2).GetComponent<TMP_Text>().text = "";
        }
    }
    public void OnClickMineDeco()
    {
        if (isPurchased)
        {
            decoMineManager.changeWalls(ResourceName);
            AddWall(ResourceName);
        }
    }


    public void AddWall(int ResourcesName)
    {
        StartCoroutine(PostFloor(1 + ResourcesName));
    }



    // Coroutine을 통해 POST 요청을 수행
    private IEnumerator PostFloor(int ResourcesName)
    {
        // 요청 URL 설정 (서버의 URL로 변경해야 합니다)
        string url = "http://125.132.216.190:12223/api/rooms/wallpaper/";

        string jwtToken = LoginInfoManager.instance.myToken;

        string jsonData = ResourcesName.ToString();
        //string jsonData = JsonUtility.ToJson(points);

        // UnityWebRequest 생성 및 설정
        UnityWebRequest request = new UnityWebRequest(url + ResourcesName.ToString(), "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        print("fdfdfd" + ResourcesName);
        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();


        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("벽이 성공적으로 추가되었습니다: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("벽 추가 실패: " + request.error);
        }
    }


}
