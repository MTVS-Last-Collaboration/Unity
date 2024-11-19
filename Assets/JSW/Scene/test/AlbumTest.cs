using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static JSW_ScheduleManager;
using UnityEngine.Networking;

public class AlbumTest : MonoBehaviour
{
    public string serverUrl = "http://125.132.216.190:12223/api/calendar/event";
    public Texture2D imageTexture; // 이미지 텍스처는 Inspector에서 설정하거나 프로그래밍적으로 할당할 수 있습니다.

    public void UploadImage()
    {
        // Texture2D를 PNG 형식으로 인코딩하여 바이트 배열 가져오기
        byte[] imageBytes = imageTexture.EncodeToPNG(); // JPEG로 인코딩할 경우 EncodeToJPG() 사용

        // 바이트 배열을 base64 문자열로 변환
        string base64String = Convert.ToBase64String(imageBytes);

        // HTTP 요청을 생성하고 base64 문자열을 서버에 보내기
        StartCoroutine(SendImageToServer(base64String));
    }

    private IEnumerator SendImageToServer(string base64Image)
    {
        // 서버에 보낼 JSON 데이터 준비 (예시)
        string jsonData = "{\"image\": \"" + base64Image + "\"}";

        string json = JsonUtility.ToJson(jsonData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        string jwtToken = LoginInfoManager.instance.myToken;

        UnityWebRequest request = new UnityWebRequest(serverUrl, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            EventForPost schedulepost = JsonUtility.FromJson<EventForPost>(request.downloadHandler.text);
        }
    }
}
