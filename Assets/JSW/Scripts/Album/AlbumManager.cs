using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ExitGames.Client.Photon;
using Photon.Pun.Demo.Cockpit;
using static JSW_InitRoom;
using static JSW_ServerDeco;
using System.Text;
using UnityEngine.Networking;
using System.IO;
using System;

public class AlbumManager : MonoBehaviourPun
{
    public GameObject picUploadingUI;
    public GameObject PicFactory;
    public RectTransform trContent;

    public GameObject testOb;

    public GameObject DestroyPic;
    public GameObject DeleteUIPic;

    public GameObject[] AlbumPos123;
    public GameObject[] AlbumPos123Button;

    public AlbumPicClass[] albumPicClass;
    public int nowIndex;
    public GameObject picWriteUI;

    public List<AlbumPicClass> Albumlist = new List<AlbumPicClass>();

    public bool clickRightLeft;
    public float TimeLerp=0;

    private string apiUrl = "http://125.132.216.190:12223/api/photo-album"; // Replace with the actual API endpoint
    


    private void Start()
    {
        ResetList();
    }

    private void Update()
    {
        if (AlbumPos123[0].activeSelf)
        {
            if (clickRightLeft)
            {
                AlbumPos123[0].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[0].GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 30);
                AlbumPos123[1].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[1].GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 30);
                AlbumPos123[2].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[2].GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 30);
                TimeLerp += Time.deltaTime;
                if (TimeLerp > 0.2f)
                {
                    TimeLerp = 0;
                    clickRightLeft = false;
                }
            }
            else
            {
                AlbumPos123[0].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[0].GetComponent<CanvasGroup>().alpha, 1, Time.deltaTime * 2);
                AlbumPos123[1].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[1].GetComponent<CanvasGroup>().alpha, 1, Time.deltaTime * 2);
                AlbumPos123[2].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[2].GetComponent<CanvasGroup>().alpha, 1, Time.deltaTime * 2);
            }
        }
        
    }

    //    // 값 추가
    //    list.Add(4);

    //// 값 삭제
    //list.Remove(2);

    //// 값 삽입
    //list.Insert(1, 10);

    [System.Serializable]
    public class AlbumPicClass
    {
        public int id;
        public Texture2D sprite;
        public string title;
        public string day;
        public string content;
        public string ObjURL;
        public string TextureURL;
    }

    [System.Serializable]
    public class PostPhoto_album
    {
        public string title;
        public string content;
        public string photoDate;
        public byte[] photo;
    }


    // Start is called before the first frame update
    void Awake()
    {
        picUploadingUI = GameObject.Find("PicUploadingUI");
        //trContent = GameObject.Find("AlbumContentBody").GetComponent<RectTransform>();
    }


    public void SettingPic()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ImageSound);

        //RaiseEventOptions eventOptions = new RaiseEventOptions();
        //eventOptions.Receivers = ReceiverGroup.All;

        //PhotonNetwork.RaiseEvent(2,null, eventOptions, SendOptions.SendUnreliable);

        //EventSystem.current.SetSelectedGameObject(null);

        print("Dfadsa");
        string title;
        string content;
        Texture2D newImage;
        string day;
        byte[] imageBytes;
        PostPhoto_album postpic;

        if (picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture == null)
        {
            return;
        }

        if (Albumlist.Count > 3 && nowIndex != 0)
        {
            title = Albumlist[nowIndex - 1].title;
            content = Albumlist[nowIndex - 1].content;
            day = Albumlist[nowIndex - 1].day;
            newImage = Albumlist[nowIndex - 1].sprite;

            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[0].SetActive(true);
        }


        if (Albumlist.Count < 1)
        {
            title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
            content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
            day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[0].SetActive(true);

            imageBytes = newImage.EncodeToPNG();

            postpic = new PostPhoto_album { title = title, content = content, photoDate = day, photo = imageBytes };


            StartCoroutine(PostPhotoEvent(apiUrl, postpic));

            Albumlist.Insert(0, new AlbumPicClass { sprite = newImage, title = title, content = content, day = day });
            return;
        }

        title = Albumlist[nowIndex + 0].title;
        content = Albumlist[nowIndex + 0].content;
        day = Albumlist[nowIndex + 0].day;
        newImage = Albumlist[nowIndex + 0].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[1].SetActive(true);

        if (Albumlist.Count < 2)
        {
            title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
            content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
            day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[0].SetActive(true);

            imageBytes = newImage.EncodeToPNG();



            postpic = new PostPhoto_album { title = title, content = content, photoDate = day, photo = imageBytes };


            StartCoroutine(PostPhotoEvent(apiUrl, postpic));

            Albumlist.Insert(0, new AlbumPicClass { sprite = newImage, title = title, content = content, day = day });
            return;
        }

        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[2].SetActive(true);

        title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
        content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
        newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
        day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
        AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[0].SetActive(true);



        imageBytes = newImage.EncodeToPNG();


        postpic = new PostPhoto_album { title = title, content = content, photoDate = day, photo = imageBytes };


        StartCoroutine(PostPhotoEvent(apiUrl, postpic));


        Albumlist.Insert(0, new AlbumPicClass { sprite = newImage, title = title, content = content, day = day });
    }

    //private void OnEnable()
    //{
    //    //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
    //    PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == 2)
    //    {
    //        //
    //    }
    //}
    //private void OnDisable()
    //{
    //    //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
    //    PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    //}


    IEnumerator PostPhotoEvent(string url, PostPhoto_album photoAlbum)
    {
        // JWT 토큰 가져오기
        string jwtToken = LoginInfoManager.instance.myToken;

        WWWForm form = new WWWForm();
        form.AddField("title", photoAlbum.title);           // 제목
        form.AddField("content", photoAlbum.content);       // 내용
        form.AddField("photoDate", photoAlbum.photoDate);   // 날짜
        form.AddBinaryData("photo", photoAlbum.photo, "photo.png", "image/png");

        //WWWForm form = new WWWForm();
        //form.AddField("photoId",11);           // 제목
        //form.AddField("positionX", 470);       // 내용
        //form.AddField("positionY", 546);   // 날짜

        // UnityWebRequest 생성
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        Debug.Log("Send!");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            print("사진 잘 올라가지 않은");
            Debug.LogError("Error: " + request.error);
            print(request.downloadHandler.text);
        }
        else
        {
            print("사진 잘 올라감");
            AlbumStatus0 wrapper = JsonUtility.FromJson<AlbumStatus0>(request.downloadHandler.text);
            Albumlist[0].id = wrapper.data.id;
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    public void testObject()
    {
        testOb.SetActive(true);
    }



    public ScrollRect scrollRect;
    public GameObject Content;
    public TMP_InputField AlbumInputField;
    public void FindAlbumPicTitle()
    {
        string title;
        string content;
        string day;
        Texture2D newImage;
        for (int i = nowIndex; i < Albumlist.Count; i++)
        {
            if (Albumlist[i].content.Contains(AlbumInputField.text))
            {
               if(i % 3 == 0)
               {
                    nowIndex = i;
                    for (int j = 0; j <3;j++)
                    {
                        if(i + j >= Albumlist.Count )
                        {
                            AlbumPos123[j].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                            AlbumPos123Button[j].SetActive(false);
                            continue;
                        }
                        title = Albumlist[i + j].title;
                        content = Albumlist[i + j].content;
                        day = Albumlist[i+j].day;
                        newImage = Albumlist[i+j].sprite;
                        AlbumPos123[j].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
                        AlbumPos123Button[j].SetActive(true);
                    }
               }
               else if (i % 3 == 1)
                {
                    nowIndex = i - 1;
                    int l = 1;
                    for (int j = -1; j < 2; j++)
                    {
                        if (i + j >= Albumlist.Count)
                        {
                            AlbumPos123[j+l].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                            AlbumPos123Button[j + l].SetActive(false);
                            l += 1;
                            continue;
                        }
                        title = Albumlist[i + j].title;
                        content = Albumlist[i + j].content;
                        day = Albumlist[i + j].day;
                        newImage = Albumlist[i + j].sprite;
                        AlbumPos123[j + l].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
                        AlbumPos123Button[j + l].SetActive(true);
                        l += 1;
                    }
                }
                else if (i % 3 == 2)
                {
                    nowIndex = i - 2;
                    int l = 1;
                    for (int j = -2; j < 1; j++)
                    {
                        if (i + j >= Albumlist.Count)
                        {
                            AlbumPos123[j+ l].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                            AlbumPos123Button[j + l].SetActive(false);
                            l += 1;
                            continue;
                        }
                        title = Albumlist[i + j].title;
                        content = Albumlist[i + j].content;
                        day = Albumlist[i + j].day;
                        newImage = Albumlist[i + j].sprite;
                        AlbumPos123[j+ l].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
                        AlbumPos123Button[j + l].SetActive(true);
                        l += 1;
                    }
                }
                clickRightLeft = true;
                break;
            }
        }
    }

    private void UpdateScrollPosition(float newValue)
    {
        scrollRect.horizontalNormalizedPosition = newValue;
    }

    public void OpenDeleteUI()
    {
        DeleteUIPic.SetActive(true);
    }

    public void OkayDeleteUI()
    {
        string title;
        string content;
        string day;
        Texture2D newImage;
        //Destroy(DestroyPic);
        DeleteUIPic.SetActive(false);
        if (DestroyPic == AlbumPos123[0])
        {
            if (Albumlist[nowIndex].id == GetComponent<Making3DObject>().exhibitionPicId) return;
            DeletePicEvent(Albumlist[nowIndex].id);
            if (Albumlist.Count < nowIndex + 2)
            {
                AlbumPos123[0].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                AlbumPos123Button[0].SetActive(false);
                Albumlist.RemoveAt(nowIndex + 0);
                return;
            }
            title = Albumlist[nowIndex + 1].title;
            content = Albumlist[nowIndex + 1].content;
            day = Albumlist[nowIndex + 1].day;
            newImage = Albumlist[nowIndex + 1].sprite;

            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[0].SetActive(true);

            if (Albumlist.Count < nowIndex + 3)
            {
                AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                AlbumPos123Button[1].SetActive(false);
                Albumlist.RemoveAt(nowIndex + 0);
                return;
            }

            title = Albumlist[nowIndex + 2].title;
            content = Albumlist[nowIndex + 2].content;
            day = Albumlist[nowIndex + 2].day;
            newImage = Albumlist[nowIndex + 2].sprite;

            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[1].SetActive(true);

            if (Albumlist.Count < nowIndex + 4)
            {
                AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                AlbumPos123Button[2].SetActive(false);
                Albumlist.RemoveAt(nowIndex + 0);
                return;
            }

            title = Albumlist[nowIndex + 3].title;
            content = Albumlist[nowIndex + 3].content;
            day = Albumlist[nowIndex + 3].day;
            newImage = Albumlist[nowIndex + 3].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[2].SetActive(false);

            Albumlist.RemoveAt(nowIndex + 0);
        }
        else if (DestroyPic == AlbumPos123[1])
        {
            if (Albumlist[nowIndex + 1].id == GetComponent<Making3DObject>().exhibitionPicId) return;
            DeletePicEvent(Albumlist[nowIndex + 1].id);
            if (Albumlist.Count < nowIndex + 3)
            {
                AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                AlbumPos123Button[1].SetActive(false);
                Albumlist.RemoveAt(nowIndex + 1);
                return;
            }

            title = Albumlist[nowIndex + 2].title;
            content = Albumlist[nowIndex + 2].content;
            day = Albumlist[nowIndex + 2].day;
            newImage = Albumlist[nowIndex + 2].sprite;

            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[1].SetActive(true);

            if (Albumlist.Count < nowIndex + 4)
            {
                AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                AlbumPos123Button[2].SetActive(false);
                Albumlist.RemoveAt(nowIndex + 1);
                return;
            }

            title = Albumlist[nowIndex + 3].title;
            content = Albumlist[nowIndex + 3].content;
            day = Albumlist[nowIndex + 3].day;
            newImage = Albumlist[nowIndex + 3].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[2].SetActive(true);
            Albumlist.RemoveAt(nowIndex + 1);
        }
        else
        {
            if (Albumlist[nowIndex + 2].id == GetComponent<Making3DObject>().exhibitionPicId) return;
            DeletePicEvent(Albumlist[nowIndex + 2].id);
            if (Albumlist.Count < nowIndex + 4)
            {
                AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                AlbumPos123Button[2].SetActive(false);
                Albumlist.RemoveAt(nowIndex + 2);
                return;
            }

            title = Albumlist[nowIndex + 3].title;
            content = Albumlist[nowIndex + 3].content;
            day = Albumlist[nowIndex + 3].day;
            newImage = Albumlist[nowIndex+ 3].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            AlbumPos123Button[2].SetActive(true);
            Albumlist.RemoveAt(nowIndex + 2);
        }
    }
    public void NoDeleteUI()
    {
        DestroyPic = null;
        DeleteUIPic.SetActive(false);
    }

    public void SetImageIntoUI()
    {
        //GameObject newPic = Instantiate(PicFactory, AlbumPos[0].transform);
        //newPic.transform.position = AlbumPos[0].transform.position;
        nowIndex = 0;
        string title;
        string content;
        string day;
        Texture2D newImage;


        if (Albumlist.Count < 1)
        {
            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[0].SetActive(false);
            AlbumPos123Button[1].SetActive(false);
            AlbumPos123Button[2].SetActive(false);
            return;
        }

        title = Albumlist[nowIndex + 0].title;
        content = Albumlist[nowIndex + 0].content;
        day = Albumlist[nowIndex + 0].day;
        newImage = Albumlist[nowIndex + 0].sprite;

        AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[0].SetActive(true);

        if (Albumlist.Count < 2)
        {
            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[1].SetActive(false);
            AlbumPos123Button[2].SetActive(false);
            return;
        }

        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[1].SetActive(true);

        if (Albumlist.Count < 3)
        {
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[2].SetActive(false);
            return;
        }

        title = Albumlist[nowIndex+ 2].title;
        content = Albumlist[nowIndex + 2].content;
        day = Albumlist[nowIndex + 2].day;
        newImage = Albumlist[nowIndex + 2].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[2].SetActive(true);
    }


    public void RightMoveButton()
    {
        string title;
        string content;
        string day;
        Texture2D newImage;

        if (nowIndex + 3 >= Albumlist.Count) return;
        nowIndex += 3;
        title = Albumlist[nowIndex + 0].title;
        content = Albumlist[nowIndex + 0].content;
        day = Albumlist[nowIndex + 0].day;
        newImage = Albumlist[nowIndex + 0].sprite;

        AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        if (nowIndex + 1 >= Albumlist.Count)
        {
            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[1].SetActive(false);
            AlbumPos123Button[2].SetActive(false);
            clickRightLeft = true;
            return;
        }
        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[1].SetActive(true);

        if (nowIndex + 2 >= Albumlist.Count)
        {
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[2].SetActive(false);
            clickRightLeft = true;
            return;
        }
        title = Albumlist[nowIndex + 2].title;
        content = Albumlist[nowIndex + 2].content;
        day = Albumlist[nowIndex + 2].day;
        newImage = Albumlist[nowIndex + 2].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[2].SetActive(true);
        clickRightLeft = true;
        TimeLerp = 0;

    }
    public void LeftMoveButton()
    {
        string title;
        string content;
        string day;
        Texture2D newImage;

        if (nowIndex == 0) return;
        if (Albumlist.Count <= 3) return;
        nowIndex -= 3;

        title = Albumlist[nowIndex + 0].title;
        content = Albumlist[nowIndex + 0].content;
        day = Albumlist[nowIndex + 0].day;
        newImage = Albumlist[nowIndex + 0].sprite;

        AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        if (nowIndex + 1 >= Albumlist.Count)
        {
            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[1].SetActive(false);
            AlbumPos123Button[2].SetActive(false);
            clickRightLeft = true;
            return;
        }
        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[1].SetActive(true);

        if (nowIndex + 2 >= Albumlist.Count)
        {
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123Button[2].SetActive(false);
            clickRightLeft = true;
            return;
        }
        title = Albumlist[nowIndex + 2].title;
        content = Albumlist[nowIndex + 2].content;
        day = Albumlist[nowIndex + 2].day;
        newImage = Albumlist[nowIndex + 2].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        AlbumPos123Button[2].SetActive(true);
        clickRightLeft = true;
        TimeLerp = 0;
    }


    public void ResetList()
    {
        //for (int i = 0; i < albumPicClass.Length; i++)
        //{
        //    Albumlist.Add(albumPicClass[i]);
        //}
        StartCoroutine(GetPhotoStatusCoroutine());
    }

    public void GetRoomStatus()
    {
        StartCoroutine(GetPhotoStatusCoroutine());

    }

    private IEnumerator GetPhotoStatusCoroutine()
    {
        Albumlist.Clear();
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {

            //request.SetRequestHeader("Accept", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response: " + request.downloadHandler.text);
                AlbumStatus wrapper = JsonUtility.FromJson<AlbumStatus>(request.downloadHandler.text);
                for (int i=0; i < wrapper.data.Length;i++)
                {
                    Texture2D texture2 = new Texture2D(2, 2);
                    yield return StartCoroutine(DownloadImageFromURL(wrapper.data[i].imageUrl, downloadedTexture =>
                    {
                        texture2 = downloadedTexture;
                    }));
                    //texture2.LoadImage(wrapper.data[i].)
                    AlbumPicClass albumPicClass = new AlbumPicClass
                    {
                        id = wrapper.data[i].id,
                        title = wrapper.data[i].title,
                        day = wrapper.data[i].photoDate[0].ToString() + '-' + wrapper.data[i].photoDate[1].ToString() + '-' + wrapper.data[i].photoDate[2].ToString(),
                        content = wrapper.data[i].content,
                        sprite = texture2,
                        ObjURL = wrapper.data[i].pngUrl,
                        TextureURL = wrapper.data[i].materialUrl
                    };
                    Albumlist.Add(albumPicClass);
                }

                //for (int i =0; i < Albumlist.Count;i++)
                //{
                //    if (Albumlist[i].ObjURL != null)
                //    {
                //        GetComponent<Making3DObject>().make3DObjectInit(Albumlist[i].ObjURL, Albumlist[i].TextureURL);
                //        break;
                //    }
                //}
                SetImageIntoUI();
            }
            else
            {
                print("안나왓어요!!!!!!!!!!");
            }

        }
    }


    private IEnumerator DownloadImageFromURL(string url, System.Action<Texture2D> callback)
    {
        using (UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return textureRequest.SendWebRequest();

            if (textureRequest.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(textureRequest);
                callback(texture); // 다운로드 성공 시 Texture2D 반환
            }
            else
            {
                Debug.LogError($"이미지 다운로드 실패: {textureRequest.error}");
                callback(null);
            }
        }
    }

    private string deleteApiUrl = "http://125.132.216.190:12223/api/photo-album/";

    public void DeletePicEvent(int Id)
    {
        if (GetComponent<Making3DObject>().exhibitionPicId == Id)
        {
            return;
        }
        StartCoroutine(DeletePic_CO(Id));
    }

    IEnumerator DeletePic_CO(int Id)
    {
        // UnityWebRequest를 사용하여 GET 요청 전송
        using (UnityWebRequest request = UnityWebRequest.Delete(deleteApiUrl + Id.ToString()))
        {
            // 헤더 설정
            request.SetRequestHeader("Accept", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

            // 요청을 보내고 응답을 기다림
            yield return request.SendWebRequest();

            // 응답 코드 확인
            if (request.result == UnityWebRequest.Result.Success)
            {
                // 성공적으로 데이터를 받아온 경우
                Debug.Log("삭제");

            }
            else
            {
                print("삭제시도2");
                print(Id);
                Debug.LogError("에러 발생: " + request.error);
            }
        }
    }

    [System.Serializable]
    public class AlbumStatus
    {
        public string message;
        public AlbumPic[] data;
    }

    [System.Serializable]
    public class AlbumPic
    {
        public int id;
        public string title;
        public string content;
        public int[] photoDate;
        public string imageUrl;     // obj
        public string objectUrl;
        public string pngUrl;       // texture
        public string materialUrl;
        public int positionX;
        public int positionY;
    }

    [System.Serializable]
    public class AlbumStatus0
    {
        public string message;
        public AlbumPic data;
    }

    [System.Serializable]
    public class AlbumPic0
    {
        public int id;
        public string title;
        public string content;
        public int[] photoDate;
        public string imageUrl;
        public string objectUrl;
        public string pngUrl;
        public string materialUrl;
        public int positionX;
        public int positionY;
    }
}
