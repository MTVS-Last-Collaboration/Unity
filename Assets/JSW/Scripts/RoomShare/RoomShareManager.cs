using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class RoomShareManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ceiling;
    public Camera RenderingCam;
    public GameObject cri;
    public GameObject cri2;
    public string myToken;
    public byte[] nowImage;

    public GameObject ShareOnButton;
    public GameObject NoShareOnButton;

    public Scrollbar volumeScrollbar1;
    public Scrollbar volumeScrollbar2;

    public Text text1;
    public Text text2;

    public AudioSource audio1;
    public AudioSource audio2;

    public GameObject scrollvalue1;
    public GameObject scrollvalue2;

    private void Start()
    {
        volumeScrollbar1.onValueChanged.AddListener(OnVolumeChange1);
        volumeScrollbar2.onValueChanged.AddListener(OnVolumeChange2);
    }
    void OnVolumeChange1(float value)
    {
        // Scrollbar 값(0~1)을 dB 값으로 변환하여 AudioMixser에 전달
        float volume = value;
        audio1.volume = volume;
        scrollvalue2.GetComponent<Image>().fillAmount = volume;
        if (text1 == null) return;
        text1.text = volume.ToString();

    }
    void OnVolumeChange2(float value)
    {
        // Scrollbar 값(0~1)을 dB 값으로 변환하여 AudioMixser에 전달
        float volume = value;
        audio2.volume = volume;
        scrollvalue1.GetComponent<Image>().fillAmount = volume;
        if (text2 == null) return;
        text2.text = volume.ToString();
        
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
        //RenderingCam.targetTexture = renderTextures = new RenderTexture(256, 256, 16);



        Texture2D texture2D = ConvertTextureToTexture2D(RenderingCam.targetTexture);
        cri2.GetComponent<RawImage>().texture = texture2D;
        //cri.GetComponent<RawImage>().texture = cri2.GetComponent<RawImage>().texture;
        nowImage = texture2D.EncodeToPNG();
        print(nowImage);


        ceiling.SetActive(true);

        StartCoroutine(PostRequest());
    }

    public void OnClickImage()
    {
        ceiling.SetActive(false);
        RenderingCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

        RawImage ri = cri.GetComponent<RawImage>();

        // 각 모델을 위한 Render Texture 생성
        RenderTexture renderTextures = new RenderTexture(1024, 1024, 16);
        RenderingCam.targetTexture = renderTextures;

        // 모델 위치 조정 및 렌더링
        RenderingCam.Render();

        // UI에 해당 Render Texture 할당
        ri.texture = renderTextures;
        //RenderingCam.targetTexture = renderTextures = new RenderTexture(256, 256, 16);
       
        Texture2D texture2D = ConvertTextureToTexture2D(RenderingCam.targetTexture);
        cri2.GetComponent<RawImage>().texture = texture2D;
        //cri.GetComponent<RawImage>().texture = cri2.GetComponent<RawImage>().texture;
        nowImage = texture2D.EncodeToPNG();
        print(nowImage);
        ceiling.SetActive(true);
    }

    private Texture2D ConvertTextureToTexture2D(RenderTexture texture)
    {
        var oldRen = RenderTexture.active;
        RenderTexture.active = texture;
        var texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false, false);
        texture2D.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = oldRen;
        return texture2D;
    }


    private string apiUrl = "http://125.132.216.190:12223/api/rooms/collection/current"; // 이미지에 나온 엔드포인트

    IEnumerator PostRequest()
    {
        //// 데이터를 보낼 필요가 없는 경우 빈 JSON 데이터 준비
        //string jsonData = "{}";

        print(nowImage);
        string jwtToken = LoginInfoManager.instance.myToken;
        WWWForm form = new WWWForm();
        form.AddBinaryData("thumbnail", nowImage, "photo.png", "image/png");

        // UnityWebRequest에 POST 요청 초기화
        UnityWebRequest request = UnityWebRequest.Post(apiUrl, form);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

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
        StartCoroutine(PostShareRoomStart());
        ShareOnButton.SetActive(false);
        NoShareOnButton.SetActive(true);
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
        ShareOnButton.SetActive(true);
        NoShareOnButton.SetActive(false);
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
