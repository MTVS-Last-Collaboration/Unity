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
    public void AddPoints(int points)
    {
        ItemIDs itemids = new ItemIDs
        {
            itemId = 3
        };

        StartCoroutine(PostAddPoints(points));
    }

    public class ItemIDs
    {
        public int itemId;
    }

    // Coroutine을 통해 POST 요청을 수행
    private IEnumerator PostAddPoints(int points)
    {
        // 요청 URL 설정 (서버의 URL로 변경해야 합니다)
        string url = "http://125.132.216.190:12223/api/couple/add-points";
        //string url = "http://125.132.216.190:12223/api/shop/purchase";
        string jwtToken = LoginInfoManager.instance.myToken;

        string jsonData = points.ToString();
        //string jsonData = JsonUtility.ToJson(points);

        // UnityWebRequest 생성 및 설정
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("포인트가 성공적으로 추가되었습니다: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("포인트 추가 실패: " + request.error);
        }
    }
}
