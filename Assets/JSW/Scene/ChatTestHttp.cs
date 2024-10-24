using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;


public class ChatTestHttp : MonoBehaviour
{
    public string chatUrl = "http://localhost:8080/api/chat/send";

    public string message = "병진님 데이트코스 추천해줘";  // 사용자로부터 입력받은 메시지

    public void SendMessageToChat()
    {
        StartCoroutine(SendChatCoroutine());
    }

    IEnumerator SendChatCoroutine()
    {
        // 메시지 정보 JSON 포맷으로 변환
        string jsonData = "{\"messages\": \"" + message + "\"}";

        // HTTP 요청 생성
        UnityWebRequest request = new UnityWebRequest(chatUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("accept", "application/json");

        yield return request.SendWebRequest();

        // 요청 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Message sent successfully: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Message sending failed: " + request.error);
        }
    }
}
