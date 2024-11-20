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

public class Making3DObject : MonoBehaviour
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

    private string apiUrl2 = "http://125.132.216.190:12223/api/photo-album/convert/"; // Replace with the actual API endpoint

    public void OnTouchImageMakingYes()
    {
        StartCoroutine(PostPhotoEvent(apiUrl2, To3DId, posX, posY));
    }

    IEnumerator PostPhotoEvent(string url, int Id, int positionx, int positiony)
    {
        // JWT 토큰 가져오기
        string jwtToken = LoginInfoManager.instance.myToken;

        WWWForm form = new WWWForm();
        form.AddField("photoId", Id);           // 제목
        form.AddField("positionX", posX);       // 내용
        form.AddField("positionY", posY);   // 날짜

        apiUrl2 = apiUrl2 + Id.ToString();

        print("PosX +" + posX + " PosY " + posY);

        // UnityWebRequest 생성
        UnityWebRequest request = UnityWebRequest.Post(apiUrl2, form);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        Debug.Log("Send!");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("사진 잘 올라가지 않은");
            Debug.LogError("Error: " + request.error);
            print(request.downloadHandler.text);
            //decoobject.funitureLayoutId = 11;

        }
        else
        {
            print("3D사진 잘 올라감"); 
            Debug.Log("Response: " + request.downloadHandler.text);
            AlbumPic3D wrapper = JsonUtility.FromJson<AlbumPic3D>(request.downloadHandler.text);
            StartCoroutine(LoadOBJWithTexture(wrapper.data[1], wrapper.data[2]));
            //FurnitureData schedulepost = JsonUtility.FromJson<FurnitureData>(request.downloadHandler.text);
           
        }
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
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
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
    }

    [System.Serializable]
    public class AlbumPic3D
    {
        public string message;
        public string[] data;
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
