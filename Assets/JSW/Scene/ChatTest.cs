using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;


public class ChatTest : MonoBehaviour
{
    private WebSocket ws;

    // 웹소켓 서버의 주소
    private string serverUrl = "http://125.132.216.190:8080/api/chat/send";

    void Start()
    {
        // 웹소켓 연결
        ws = new WebSocket(serverUrl);

        // 웹소켓 이벤트 핸들러 등록
        ws.OnMessage += (sender, e) => {
            Debug.Log("Received: " + e.Data);
            // 여기서 AI 서버의 응답을 처리할 수 있습니다.
        };

        ws.OnOpen += (sender, e) => {
            Debug.Log("WebSocket opened.");
        };

        ws.OnError += (sender, e) => {
            Debug.LogError("WebSocket error: " + e.Message);
        };

        ws.OnClose += (sender, e) => {
            Debug.Log("WebSocket closed.");
        };

        // 웹소켓 연결 시작
        ws.Connect();
    }

    // 메시지 전송 메소드
    public void SendMessage(string message)
    {
        if (ws != null && ws.ReadyState == WebSocketState.Open)
        {
            ws.Send(message); // 메시지를 웹소켓으로 전송
            Debug.Log("Sent: " + message);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open.");
        }
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close(); // 웹소켓 연결 종료
        }
    }
}
