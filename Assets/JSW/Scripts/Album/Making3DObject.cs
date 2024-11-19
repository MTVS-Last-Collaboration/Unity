using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using static JSW_InitRoom;

public class Making3DObject : MonoBehaviour
{
    public GameObject picPrefabItem;
    public Transform picTr;
    public AlbumManager albumManager;

    public Texture modelImage;
    public GameObject isMaking3DUI;
    public GameObject isMaking3DUIImage;

    public GameObject[] MakingButtons;

    public int posX;
    public int posY;

    public TMP_Text Changetext;
    

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
            item.GetComponent<RawImage>().texture = albumManager.Albumlist[i].sprite;
        }
    }

    public void ClickPic(Texture texture2d)
    {
        isMaking3DUI.SetActive(true);
        modelImage = texture2d;
        isMaking3DUIImage.GetComponent<RawImage>().texture = texture2d;
    }

    public void OnTouchImage()
    {
        MakingButtons[0].SetActive(false);
        MakingButtons[1].SetActive(false);
        MakingButtons[2].SetActive(true);
        MakingButtons[3].SetActive(true);
        Changetext.text = "선택한 부분을 변환할까요?";
    }

    public void OnTouchImageMakingNo()
    {
        MakingButtons[0].SetActive(true);
        MakingButtons[1].SetActive(true);
        MakingButtons[2].SetActive(false);
        MakingButtons[3].SetActive(false);
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
        StartCoroutine(PostPhotoEvent(apiUrl2 , -30,-100,-100));
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
            print("사진 잘 올라감");
            //FurnitureData schedulepost = JsonUtility.FromJson<FurnitureData>(request.downloadHandler.text);
            Debug.Log("Response: " + request.downloadHandler.text);
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
