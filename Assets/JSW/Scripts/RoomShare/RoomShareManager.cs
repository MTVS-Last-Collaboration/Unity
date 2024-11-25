using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

public class RoomShareManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ceiling;
    public Camera RenderingCam;
    public GameObject cri;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickShareButton()
    {
        //ceiling.SetActive(false);
        //RenderingCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

        //RawImage ri = cri.GetComponent<RawImage>();

        //// 각 모델을 위한 Render Texture 생성
        //RenderTexture renderTextures = new RenderTexture(256, 256, 16);
        //RenderingCam.targetTexture = renderTextures;

        //// 모델 위치 조정 및 렌더링
        //RenderingCam.Render();

        //// UI에 해당 Render Texture 할당
        //ri.texture = renderTextures;
        //RenderingCam.targetTexture = renderTextures = new RenderTexture(256, 256, 16);
        ////models[i].SetActive(false);
        ////Destroy(models[i]);

        ////if (i == models.Length - 1)
        ////{
        ////    RenderingCam.targetTexture = new RenderTexture(256, 256, 16);
        ////}

        ////if (i % 22 == 0 && i != 0)
        ////{
        ////    RenderingCam.targetTexture = new RenderTexture(256, 256, 16);

        ////}
        //ceiling.SetActive(true);


        StartCoroutine(PostRequest());
        
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
}
