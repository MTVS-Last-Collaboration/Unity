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

public class AlbumManager : MonoBehaviourPun, IOnEventCallback
{
    public GameObject picUploadingUI;
    public GameObject PicFactory;
    public RectTransform trContent;

    public GameObject testOb;

    // Start is called before the first frame update
    void Awake()
    {
        picUploadingUI = GameObject.Find("PicUploadingUI");
        trContent = GameObject.Find("AlbumContentBody").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingPic()
    {
        //GameObject newPic = Instantiate(PicFactory, trContent);
        //string title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
        //string content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
        //Texture2D newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
        //string day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
        //newPic.GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        //print("jj");

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
            GameObject newPic = Instantiate(PicFactory, trContent);
            string title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
            string content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
            Texture2D newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
            string day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
            newPic.GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
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
}
