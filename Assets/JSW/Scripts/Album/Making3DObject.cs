using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using static JSW_InitRoom;
using Dummiesman;
using System.IO;
using System.Text;
using static AlbumManager;
using UnityEngine.Timeline;
using Unity.Loading;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class Making3DObject : MonoBehaviour, IOnEventCallback
{
    public GameObject picPrefabItem;
    public Transform picTr;
    public AlbumManager albumManager;

    public Texture modelImage;
    public int To3DId;
    public GameObject isMaking3DUI;
    public GameObject isMaking3DUIImage;

    public GameObject[] MakingButtons;

    
    public int posX;
    public int posY;

    public TMP_Text Changetext;
    public ImageClickPixelPosition imageclickpixelposition;

    public GameObject Object3DPos;
    public GameObject Marker;

    public int exhibitionId;
    public int exhibitionPicId;

    public GameObject loadingImage;


    private void Start()
    {
        Get3DPhotoRoomStatus();
    }
    public void OnClickButtonPicUITo3D()
    {
        int childCound = picTr.childCount;
        for (int j = 0; j < childCound; j++)
        {
            Destroy(picTr.GetChild(j).gameObject);
        }

        for (int i =0;i < albumManager.Albumlist.Count;i++)
        {
            GameObject item = Instantiate(picPrefabItem, picTr);
            item.GetComponent<Item2DTo3D>().id = albumManager.Albumlist[i].id;
            item.GetComponent<RawImage>().texture = albumManager.Albumlist[i].sprite;
        }
    }

    private void Update()
    {
        if (loadingImage.activeSelf == true)
        {
            loadingImage.transform.GetChild(0).Rotate(0, 0, 60f * Time.deltaTime);
        }
    }

    public void ClickPic(Texture texture2d, int id)
    {
        imageclickpixelposition.fixPos = false;
        isMaking3DUI.SetActive(true);
        To3DId = id;
        modelImage = texture2d;
        isMaking3DUIImage.GetComponent<RawImage>().texture = texture2d;
    }

    public void OnTouchImage()
    {
        if(posX ==  0 && posY == 0)
        {
            return;
        }
        MakingButtons[0].SetActive(false);
        MakingButtons[1].SetActive(false);
        MakingButtons[2].SetActive(true);
        MakingButtons[3].SetActive(true);
        imageclickpixelposition.fixPos = true;
        Changetext.text = "선택한 부분을 변환할까요?";
    }

    public void OnTouchImageMakingNo()
    {
        MakingButtons[0].SetActive(true);
        MakingButtons[1].SetActive(true);
        MakingButtons[2].SetActive(false);
        MakingButtons[3].SetActive(false);
        imageclickpixelposition.fixPos = false;
        Marker.SetActive(false);
        Changetext.text = "변환할 부분을 터치해주세요";
    }

    public void SetTouchPos(int posX1, int posY1)
    {
        posX = posX1;
        posY = posY1;
    }

    private string apiUrl1 = "http://125.132.216.190:12223/api/photo-album/convert/";
    public void OnTouchImageMakingYes()
    {
        if (Object3DPos.transform.childCount != 0)
        {
            print("이미 전시된 게 있습니다.");
            return;
        }
        StartCoroutine(PostPhotoEvent1(apiUrl1, To3DId, posX, posY));
    }


   

    IEnumerator PostPhotoEvent1(string url, int Id, int positionx, int positiony)
    {
        // JWT 토큰 가져오기
        string jwtToken = LoginInfoManager.instance.myToken;

        WWWForm form = new WWWForm();
        form.AddField("photoId", Id);           // 제목
        form.AddField("positionX", posX);       // 내용
        form.AddField("positionY", posY);   // 날짜

        print("PosX +" + posX + " PosY " + posY +"ID: " + Id);
        apiUrl1 = apiUrl1 + Id;
        // UnityWebRequest 생성
        UnityWebRequest request = UnityWebRequest.Post(apiUrl1, form);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
        loadingImage.SetActive(true);

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent(13, null, eventOptions, SendOptions.SendUnreliable);

        EventSystem.current.SetSelectedGameObject(null);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("사진 잘 올라가지 않은");
            Debug.LogError("Error: " + request.error);
            print(request.downloadHandler.text);
            loadingImage.SetActive(false);

           
            eventOptions.Receivers = ReceiverGroup.All;

            PhotonNetwork.RaiseEvent(14, null, eventOptions, SendOptions.SendUnreliable);

            EventSystem.current.SetSelectedGameObject(null);

        }
        else
        {
            print("3D사진 잘 올라감");
            StartCoroutine(PostPhotoEvent2(apiUrl2, To3DId, posX, posY));
        }
    }



    // 데이터 클래스 정의
    [System.Serializable]
    public class ExhibitionData
    {
        public int photoId;
        public int positionX;
        public int positionY;
    }

    private string apiUrl2 = "http://125.132.216.190:12223/api/exhibition"; // Replace with the actual API endpoint

    IEnumerator PostPhotoEvent2(string url, int Id, int positionx, int positiony)
    {
        // JWT 토큰 가져오기
        string jwtToken = LoginInfoManager.instance.myToken;

        ExhibitionData data = new ExhibitionData
        {
            photoId = Id,
            positionX = posX,
            positionY = posY
        };

        string jsonData = JsonUtility.ToJson(data);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        print("PosX +" + data.photoId + " PosY " + data.positionX+ "dsa" + data.positionY);


        // UnityWebRequest로 POST 요청 생성
        using (UnityWebRequest request = new UnityWebRequest(apiUrl2, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();


            // Content-Type 헤더 설정
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                print("사진 잘 올라가지 않은2");
                Debug.LogError("Error: " + request.error);
                print(request.downloadHandler.text);
            }
            else
            {
                print("3D사진 잘 올라감2");
                Debug.Log("Response: " + request.downloadHandler.text);
                Photo3DFirst wrapper = JsonUtility.FromJson<Photo3DFirst>(request.downloadHandler.text);
                //StartCoroutine(LoadOBJWithTexture(wrapper.textureUrl, wrapper.materialUrl));


                object[] sendContent = new object[] { wrapper.textureUrl, wrapper.materialUrl};

                RaiseEventOptions eventOptions = new RaiseEventOptions();
                eventOptions.Receivers = ReceiverGroup.All;

                PhotonNetwork.RaiseEvent(12, sendContent, eventOptions, SendOptions.SendUnreliable);

                EventSystem.current.SetSelectedGameObject(null);


                exhibitionId = wrapper.exhibitionId;
                exhibitionPicId = wrapper.photo.photoId;

            }
        }
    }


    private void OnEnable()
    {
        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 12) //가구 생성
        {
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string receiveString1 = receiveObjects[0].ToString();
            string receiveString2 = receiveObjects[1].ToString();
            StartCoroutine(LoadOBJWithTexture(receiveString1, receiveString2));
        }
        if (photonEvent.Code == 13) // 생성 시작
        {
            loadingImage.SetActive(true);
        }
        if (photonEvent.Code == 14) // 생성 실패 1
        {
            loadingImage.SetActive(false);
        }
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }




    public void make3DObjectInit(string obj, string png)
    {
        StartCoroutine(LoadOBJWithTexture(obj, png));
    }

    IEnumerator LoadOBJWithTexture(string obj, string png)
    {
        // Step 1: Download OBJ file
        print("Ddfa");
        string objUrl = obj;
        UnityWebRequest objRequest = UnityWebRequest.Get(objUrl);
        yield return objRequest.SendWebRequest();

        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download OBJ file: " + objRequest.error);
            yield break;
        }

        // Create a stream from OBJ file content
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(objRequest.downloadHandler.text));

        // Step 2: Load the OBJ file
        GameObject loadedObj = new OBJLoader().Load(textStream);

        // Step 3: Download Texture
        string textureUrl = png;
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(textureUrl);

        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            print("ddd3");
            Debug.LogError("Failed to download texture: " + textureRequest.error);
            yield break;
        }


        Texture2D texture = DownloadHandlerTexture.GetContent(textureRequest);
       
        // Step 4: Create a Material and assign the texture
       // Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        Material material = new Material(Shader.Find("SimpleURPToonLitExample(With Outline)"));
        material.mainTexture = texture;

        // Step 5: Apply the Material to the loaded object
        var renderer = loadedObj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        // Optional: Adjust object's position or scale if needed
        //loadedObj.transform.position = Object3DPos.transform.position;
        loadedObj.transform.SetParent(Object3DPos.transform);
        loadedObj.transform.localPosition = Vector3.zero;
        loadingImage.SetActive(false);
    }


    [System.Serializable]
    public class Photo3DFirst
    {
       public int exhibitionId;
       public string objectUrl;
       public string textureUrl;
       public string materialUrl;
       public int positionX;
       public int positionY;
       public string exhibitedAt;
       public Photo3D photo;
    }

    [System.Serializable]
    public class Photo3D
    {
        public int photoId;
        public string title;
        public string imageUrl;
        public string photoDate;
        public string description;
    }

    private string apiUrlDelete = "http://125.132.216.190:12223/api/exhibition/"; // Replace with the actual API endpoint

    public GameObject exhibitionPos;

    public void apiUrlDeleteEvent()
    {
        if (exhibitionPos.transform.childCount != 0)
        {
            Destroy(exhibitionPos.transform.GetChild(0).gameObject);
            StartCoroutine(apiUrlDeleteEvent_CO(exhibitionId));
        }

    }

    IEnumerator apiUrlDeleteEvent_CO(int Id)
    {
        // JWT 토큰 가져오기
        string jwtToken = LoginInfoManager.instance.myToken;

        apiUrlDelete = apiUrlDelete + Id.ToString();
        // UnityWebRequest 생성
        UnityWebRequest request = UnityWebRequest.Delete(apiUrlDelete);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("사진 잘 삭제되지 않은");
            Debug.LogError("Error: " + request.error);
            print(request.downloadHandler.text);
        }
        else
        {
            print("3D사진 잘 삭제됨");
            exhibitionId = 0;
            exhibitionPicId = -1;
            Debug.Log("Response: " + request.downloadHandler.text);
            Photo3DFirst wrapper = JsonUtility.FromJson<Photo3DFirst>(request.downloadHandler.text);
            //StartCoroutine(LoadOBJWithTexture(wrapper.objectUrl, wrapper.textureUrl));
            //exhibitionId = wrapper.exhibitionId;
        }
    }


    public class ExhibitionResponse
    {
        public Photo3DFirst1[] exhibitions;
    }

    [System.Serializable]
    public class Photo3DFirst1
    {
        public int exhibitionId;
        public string objectUrl;
        public string textureUrl;
        public string materialUrl;
        public int positionX;
        public int positionY;
        public string exhibitedAt;
        public Photo3D photo;
    }

    public void Get3DPhotoRoomStatus()
    {
        StartCoroutine(Get3DPhotoStatusCoroutine());
        print("dsadsa");
    }

    private string apiUrl3Dex = "http://125.132.216.190:12223/api/exhibition/list"; 

    private IEnumerator Get3DPhotoStatusCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl3Dex))
        {
            print("3D 포토다 냥");
            //request.SetRequestHeader("Accept", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("ResponsexZ: " + request.downloadHandler.text);
                ExhibitionResponse wrapper = JsonUtility.FromJson<ExhibitionResponse>("{\"exhibitions\":" + request.downloadHandler.text + "}");
                if ((wrapper.exhibitions.Length) != 0)
                {
                    exhibitionId = wrapper.exhibitions[0].exhibitionId;
                    exhibitionPicId = wrapper.exhibitions[0].photo.photoId;
                    make3DObjectInit(wrapper.exhibitions[0].textureUrl, wrapper.exhibitions[0].materialUrl);
                }
            }
            else
            {
                print("안나왓어요!!!!!!!!!!");
            }

        }
    }





    //private string initAlbumUrl = "http://125.132.216.190:12223/api/photo-album";

    //public void GetAlbumStatus()
    //{
    //    StartCoroutine(GetAlbumStatusCoroutine());
    //}

    //private IEnumerator GetAlbumStatusCoroutine()
    //{
    //    using (UnityWebRequest request = UnityWebRequest.Get(initAlbumUrl))
    //    {

    //        request.SetRequestHeader("Accept", "application/json");
    //        string jwtToken = LoginInfoManager.instance.myToken;
    //        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
    //        yield return request.SendWebRequest();
    //        if (request.result == UnityWebRequest.Result.Success)
    //        {
    //            // Process JSON response
    //            Debug.Log("Responsesssss: " + request.downloadHandler.text);

    //            string jsonResponse = "{\"items\":" + request.downloadHandler.text + "}";
    //            Debug.Log("Response: " + jsonResponse);

    //            // JSON 데이터를 ShoplistItemWrapper로 파싱
    //            // ShoplistItemWrapper wrapper = JsonUtility.FromJson<ShoplistItemWrapper>(jsonResponse);
    //        }
    //        else
    //        {
    //            print("JSW_InitRoom인데 처음 가구들 설치할 때 호출하는 것임");
    //            Debug.LogError("Error: " + request.error);
    //        }
    //    }
    //}


}
