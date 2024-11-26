using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Photon.Realtime;

public class RoomShareManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ceiling;
    public Camera RenderingCam;
    public GameObject cri;
    public string myToken;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickShareButton()
    {
        ceiling.SetActive(false);
        RenderingCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

        RawImage ri = cri.GetComponent<RawImage>();

        // 각 모델을 위한 Render Texture 생성
        RenderTexture renderTextures = new RenderTexture(256, 256, 16);
        RenderingCam.targetTexture = renderTextures;

        // 모델 위치 조정 및 렌더링
        RenderingCam.Render();

        // UI에 해당 Render Texture 할당
        ri.texture = renderTextures;
        RenderingCam.targetTexture = renderTextures = new RenderTexture(256, 256, 16);

        ceiling.SetActive(true);

        StartCoroutine(PostRequest());
    }

    public void OnClickImage()
    {
        ceiling.SetActive(false);
        RenderingCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

        RawImage ri = cri.GetComponent<RawImage>();

        // 각 모델을 위한 Render Texture 생성
        RenderTexture renderTextures = new RenderTexture(256, 256, 16);
        RenderingCam.targetTexture = renderTextures;

        // 모델 위치 조정 및 렌더링
        RenderingCam.Render();

        // UI에 해당 Render Texture 할당
        ri.texture = renderTextures;
        RenderingCam.targetTexture = renderTextures = new RenderTexture(256, 256, 16);

        ceiling.SetActive(true);
    }


    private string apiUrl = "http://125.132.216.190:12223/api/rooms/collection/current"; // 이미지에 나온 엔드포인트

    IEnumerator PostRequest()
    {
        // 데이터를 보낼 필요가 없는 경우 빈 JSON 데이터 준비
        string jsonData = "{}";

        string jwtToken = LoginInfoManager.instance.myToken;

        // UnityWebRequest에 POST 요청 초기화
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        print("fads---");

        // 요청 보내기
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST 성공: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("POST 실패: " + request.error);
        }

    }

    public void OnListMyRoom()
    {
        //string jsonData = "";
        //StartCoroutine(PostShareRoomStart(jsonData));
        StartCoroutine(PostShareRoomStart());
    }
    //
    IEnumerator PostShareRoomStart()
    {
        myToken = LoginInfoManager.instance.myToken;
        string urlTrue = "http://125.132.216.190:12223/api/rooms/sharing?isShared=true";

        UnityWebRequest request = new UnityWebRequest(urlTrue, "POST");  // HTTP POST 요청 준비

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); // JSON 데이터를 담아 요청 생성
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발사
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

        }

    }
    //방공유설정 그만하기 1-2
    public void OffListMyRoom()
    {
        string jsonData = "";
        StartCoroutine(PostShareRoomEnd(jsonData));
    }

    IEnumerator PostShareRoomEnd(string jsonData)
    {
        myToken = LoginInfoManager.instance.myToken;

        string urlFalse = "http://125.132.216.190:12223/api/rooms/sharing?isShared=false";

        UnityWebRequest request = new UnityWebRequest(urlFalse, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

        }

    }


}
