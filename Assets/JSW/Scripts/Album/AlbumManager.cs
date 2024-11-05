using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using ExitGames.Client.Photon;

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
}
