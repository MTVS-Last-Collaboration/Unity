using ExitGames.Client.Photon;
using Newtonsoft.Json.Linq;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JSW_ChatManager : MonoBehaviourPun, IOnEventCallback
{
    // Input ChatInputField
    public TMP_InputField inputChat;

    // ChatItem Prefab
    public GameObject chatItemFactory;
    public GameObject MongchatItemFactory;
    // ChatItem의 부모 Transform
    public RectTransform trContent;


    private void Awake()
    {
        inputChat = GameObject.Find("AIChatInput").GetComponent<TMP_InputField>();
        trContent = GameObject.Find("AIChatContent").GetComponent<RectTransform>();

        // 엔터쳤을 때 호출되는 함수 등록
        inputChat.onSubmit.AddListener(OnSubmit);
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public Color color;
    public string nickName;

    public void SubmitChat()
    {
        OnSubmit(inputChat.text);
    }

    void OnSubmit(string s)
    {
        inputChat.text = "";
        inputChat.ActivateInputField();
        nickName = LoginInfoManager.instance.nickName;
        string nick = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + nickName + "</color>";

        //string chat = nick + " : " + s;
        string chat = s;
        SendMessageToChat(chat);

        CreateChatItem(chat, "ME");
    }

    void CreateChatItem(string chat, string isMong)
    {


        object[] sendContent = new object[] { chat, isMong };

        // 송신 옵션
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(4, sendContent, eventOptions, SendOptions.SendUnreliable);

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
        if (photonEvent.Code == 4)
        {
            // 받은 내용을 "닉네임: 채팅 내용" 형식으로 스크롤뷰의 텍스트에 전달한다.
            object[] receiveObjects = (object[])photonEvent.CustomData;
            string chat = receiveObjects[0].ToString();
            string isMong = receiveObjects[1].ToString();

            if (isMong == "Mong")
            {
                // s의 내용으로 ChatItem을 만들자.
                GameObject go = Instantiate(MongchatItemFactory, trContent);
                // 만들어진 go에서 ChatItem 컴포넌트 가져오자.
                JSW_ChatItem chatItem = go.GetComponent<JSW_ChatItem>();


                // 가져온 컴포넌트의 SetText 함수 실행
                chatItem.SetText(chat, isMong, DateTime.Now.ToString("HH:mm"));
            }
            else
            {
                // s의 내용으로 ChatItem을 만들자.
                GameObject go = Instantiate(chatItemFactory, trContent);
                // 만들어진 go에서 ChatItem 컴포넌트 가져오자.
                JSW_ChatItem chatItem = go.GetComponent<JSW_ChatItem>();


                // 가져온 컴포넌트의 SetText 함수 실행
                chatItem.SetText(chat, isMong, DateTime.Now.ToString("HH:mm"));
            }

        }
    }

    private void OnDisable()
    {
        //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }


    public string chatUrl = "http://125.132.216.190:12223/api/chat/send";

    //public string message = "병진님 데이트코스 추천해줘";  // 사용자로부터 입력받은 메시지


    public void SendMessageToChat(string message)
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound2);
        StartCoroutine(SendChatCoroutine(message));
    }

    public class MongChat
    {
        public string messages;
    }


    IEnumerator SendChatCoroutine(string message)
    {
        MongChat chat = new MongChat { messages = message };
        // 메시지 정보 JSON 포맷으로 변환
        string jsonData = JsonUtility.ToJson(chat);

        // HTTP 요청 생성
        UnityWebRequest request = new UnityWebRequest(chatUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        string jwtToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        yield return request.SendWebRequest();

        // 요청 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Message sent successfully: " + request.downloadHandler.text);

            JObject responseObject = JObject.Parse(request.downloadHandler.text);
            string aiResponse = responseObject["aiResponse"].ToString();
            string extractedText = aiResponse;
            Debug.Log("Extracted AI Response: " + extractedText);

            CreateChatItem(extractedText, "Mong");
        }
        else
        {
            Debug.LogError("Message sending failed: " + request.error);
            string extractedText = "생각이 많아서 답변이 어려워...ㅠ";
            CreateChatItem(extractedText,"Mong");
        }
    }
}
