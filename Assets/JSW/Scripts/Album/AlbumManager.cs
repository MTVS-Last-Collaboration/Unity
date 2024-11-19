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
using UnityEditor.VersionControl;

public class AlbumManager : MonoBehaviourPun, IOnEventCallback
{
    public GameObject picUploadingUI;
    public GameObject PicFactory;
    public RectTransform trContent;

    public GameObject testOb;

    public GameObject DestroyPic;
    public GameObject DeleteUIPic;

    public GameObject[] AlbumPos123;
    public AlbumPicClass[] albumPicClass;
    public int nowIndex;

    public List<AlbumPicClass> Albumlist = new List<AlbumPicClass>();

    public bool clickRightLeft;
    public float TimeLerp=0;

    private void Start()
    {
        ResetList();
        SetImageIntoUI();
    }

    private void Update()
    {
        if (AlbumPos123[0].activeSelf)
        {
            if (clickRightLeft)
            {
                AlbumPos123[0].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[0].GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 5);
                AlbumPos123[1].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[1].GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 5);
                AlbumPos123[2].GetComponent<CanvasGroup>().alpha = Mathf.Lerp(AlbumPos123[2].GetComponent<CanvasGroup>().alpha, 0, Time.deltaTime * 5);
                TimeLerp += Time.deltaTime;
                if (TimeLerp > 0.4f)
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
        public Texture2D sprite;
        public string title;
        public string day;
        public string content;
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

        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(2,null, eventOptions, SendOptions.SendUnreliable);

        print("Send!");
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void OnEnable()
    {

        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 2)
        {
            //GameObject newPic = Instantiate(PicFactory, trContent);
            //string title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
            //string content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            //Texture2D newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
            //string day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            //newPic.GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            string title;
            string content;
            Texture2D newImage;
            string day;

            if (Albumlist.Count >3 && nowIndex  != 0)
            {
                title = Albumlist[nowIndex -1].title;
                content = Albumlist[nowIndex - 1].content;
                day = Albumlist[nowIndex - 1].day;
                newImage = Albumlist[nowIndex - 1].sprite;

                AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            }

            if (Albumlist.Count < 1)
            {
                title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
                content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
                newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
                day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
                AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
                Albumlist.Insert(0, new AlbumPicClass { sprite = newImage, title = title, content = content, day = day });
                return;
            }
            title = Albumlist[nowIndex + 0].title;
            content = Albumlist[nowIndex + 0].content;
            day = Albumlist[nowIndex + 0].day;
            newImage = Albumlist[nowIndex + 0].sprite;

            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            if (Albumlist.Count < 2)
            {
                title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
                content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
                newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
                day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
                AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
                Albumlist.Insert(0, new AlbumPicClass { sprite = newImage, title = title, content = content, day = day });
                return;
            }

            title = Albumlist[nowIndex + 1].title;
            content = Albumlist[nowIndex + 1].content;
            day = Albumlist[nowIndex + 1].day;
            newImage = Albumlist[nowIndex + 1].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
            content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
            day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            Albumlist.Insert(0,new AlbumPicClass { sprite = newImage, title = title, content = content, day = day });
        }
    }
    private void OnDisable()
    {
        //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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
        for (int i = 0; i < Content.transform.childCount;i++)
        {
            if (Content.transform.GetChild(i).GetChild(1).GetComponent<TMP_Text>().text.Contains(AlbumInputField.text))
            {
                CenterTextHorizontally(Content.transform.GetChild(i).gameObject.GetComponent<RectTransform>());
                break;
            }
        }
    }


    public void CenterTextHorizontally(RectTransform textTransform)
    {
        float contentWidth = scrollRect.content.rect.width;
        float viewportWidth = scrollRect.viewport.rect.width;

        // 텍스트 요소의 World 좌표를 Content 좌표계로 변환
        Vector3 worldPosition = textTransform.position;
        Vector3 localPosition = scrollRect.content.InverseTransformPoint(worldPosition);

        float textXPosition = localPosition.x;
        float textWidth = textTransform.rect.width;

        // 중앙 정렬 계산
        float targetPosition = (textXPosition + (textWidth / 2)) / (contentWidth * 2 - viewportWidth);

        // 비율이 0~1 범위를 벗어나지 않도록 클램프
        targetPosition = Mathf.Clamp(targetPosition, 0f, 1f);

        iTween.ValueTo(gameObject, iTween.Hash(
            "from", scrollRect.horizontalNormalizedPosition,
            "to", targetPosition,
            "time", 1.0f,  // 애니메이션 시간 (초)
            "easetype", iTween.EaseType.easeInOutSine,
            "onupdate", "UpdateScrollPosition"
        ));
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
            if (Albumlist.Count < nowIndex + 2)
            {
                AlbumPos123[0].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                Albumlist.RemoveAt(nowIndex + 0);
                return;
            }
            title = Albumlist[nowIndex + 1].title;
            content = Albumlist[nowIndex + 1].content;
            day = Albumlist[nowIndex + 1].day;
            newImage = Albumlist[nowIndex + 1].sprite;

            AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            if (Albumlist.Count < nowIndex + 3)
            {
                AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                Albumlist.RemoveAt(nowIndex + 0);
                return;
            }

            title = Albumlist[nowIndex + 2].title;
            content = Albumlist[nowIndex + 2].content;
            day = Albumlist[nowIndex + 2].day;
            newImage = Albumlist[nowIndex + 2].sprite;

            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            if (Albumlist.Count < nowIndex + 4)
            {
                AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                Albumlist.RemoveAt(nowIndex + 0);
                return;
            }

            title = Albumlist[nowIndex + 3].title;
            content = Albumlist[nowIndex + 3].content;
            day = Albumlist[nowIndex + 3].day;
            newImage = Albumlist[nowIndex + 3].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            Albumlist.RemoveAt(nowIndex + 0);
        }
        else if (DestroyPic == AlbumPos123[1])
        {
            if (Albumlist.Count < nowIndex + 3)
            {

                AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                Albumlist.RemoveAt(nowIndex + 1);
                return;
            }

            title = Albumlist[nowIndex + 2].title;
            content = Albumlist[nowIndex + 2].content;
            day = Albumlist[nowIndex + 2].day;
            newImage = Albumlist[nowIndex + 2].sprite;

            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

            if (Albumlist.Count < nowIndex + 4)
            {
                AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                Albumlist.RemoveAt(nowIndex + 1);
                return;
            }

            title = Albumlist[nowIndex + 3].title;
            content = Albumlist[nowIndex + 3].content;
            day = Albumlist[nowIndex + 3].day;
            newImage = Albumlist[nowIndex + 3].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
            Albumlist.RemoveAt(nowIndex + 1);
        }
        else
        {
            if (Albumlist.Count < nowIndex + 4)
            {
                AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
                Albumlist.RemoveAt(nowIndex + 2);
                return;
            }

            title = Albumlist[nowIndex + 3].title;
            content = Albumlist[nowIndex + 3].content;
            day = Albumlist[nowIndex + 3].day;
            newImage = Albumlist[nowIndex+ 3].sprite;

            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
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
        //print("제제하ㅏ핳하ㅏ");
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
            return;
        }

        title = Albumlist[nowIndex + 0].title;
        content = Albumlist[nowIndex + 0].content;
        day = Albumlist[nowIndex + 0].day;
        newImage = Albumlist[nowIndex + 0].sprite;

        AlbumPos123[0].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

        if (Albumlist.Count < 2)
        {
            AlbumPos123[1].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            return;
        }

        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

        if (Albumlist.Count < 3)
        {
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            return;
        }

        title = Albumlist[nowIndex+ 2].title;
        content = Albumlist[nowIndex + 2].content;
        day = Albumlist[nowIndex + 2].day;
        newImage = Albumlist[nowIndex + 2].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
    }

    public void ResetList()
    {
        for (int i = 0; i < albumPicClass.Length;i++)
        {
            Albumlist.Add(albumPicClass[i]);
        }
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
            return;
        }
        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

        if (nowIndex + 2 >= Albumlist.Count)
        {
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            return;
        }
        title = Albumlist[nowIndex + 2].title;
        content = Albumlist[nowIndex + 2].content;
        day = Albumlist[nowIndex + 2].day;
        newImage = Albumlist[nowIndex + 2].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
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
            return;
        }
        title = Albumlist[nowIndex + 1].title;
        content = Albumlist[nowIndex + 1].content;
        day = Albumlist[nowIndex + 1].day;
        newImage = Albumlist[nowIndex + 1].sprite;

        AlbumPos123[1].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);

        if (nowIndex + 2 >= Albumlist.Count)
        {
            AlbumPos123[2].GetComponent<AlbumItem>().SetContents(null, null, null, null);
            return;
        }
        title = Albumlist[nowIndex + 2].title;
        content = Albumlist[nowIndex + 2].content;
        day = Albumlist[nowIndex + 2].day;
        newImage = Albumlist[nowIndex + 2].sprite;

        AlbumPos123[2].GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        clickRightLeft = true;
        TimeLerp = 0;
    }

}
