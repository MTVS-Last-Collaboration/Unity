using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class JSW_ServerDeco : MonoBehaviour
{
    // 서버 엔드포인트 URL
    private string url = "http://your-server-url/api/rooms/decorate";

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

    public void PostServerNewFuni(GameObject funi, int dir, string name,int funiLayoutId)
    {

    }

    // 가구 배치 함수
    public void DecorateRoom()
    {
        FurnitureData data = new FurnitureData
        {
            coupleId = 1,
            furnitureId = 1,
            positionX = 100,
            positionY = 200,
            rotation = 90,
            width = 50,
            height = 30
        };

        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(PostRequest(url, jsonData));
    }

    // POST 요청 코루틴
    IEnumerator PostRequest(string url, string jsonData)
    {
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 요청 전송 및 응답 대기
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("서버 응답: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("서버 요청 실패: " + request.error);
            }
        }
    }
}
