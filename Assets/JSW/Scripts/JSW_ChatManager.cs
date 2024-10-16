using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_ChatManager : MonoBehaviour
{
    // Input ChatInputField
    public TMP_InputField inputChat;

    // ChatItem Prefab
    public GameObject chatItemFactory;
    // ChatItem의 부모 Transform
    public RectTransform trContent;

    // Start is called before the first frame update
    void Start()
    {
        inputChat = GameObject.Find("AIChatInput").GetComponent<TMP_InputField>();
        trContent = GameObject.Find("AIChatContent").GetComponent<RectTransform>();

        // 엔터쳤을 때 호출되는 함수 등록
        inputChat.onSubmit.AddListener(OnSubmit);
    }

    public Color color;
    public string nickName;

    void OnSubmit(string s)
    {

        // 닉네임의 색을 변경 color로
        // <color=#ffffff> 닉네임 </color>
        string nick = "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + nickName + "</color>";


        // 귓속말인지 판단
        // /w 아이디 메시지
 
        //if (text[0] == "/w")
        //{
        //    // 전체 채팅 구성을 만들자.
        //    string chat = nick + " : " + text[2];
        //    // 귓속말을 보내자
        //    chatClient.SendPrivateMessage(text[1], chat);
        //}
        //else
        //{
            // 전체 채팅 구성을 만들자.
            string chat = nick + " : " + s;
        // 일반 채팅을 보내자.
        //chatClient.PublishMessage(currChannel, chat);
        CreateChatItem(chat, Color.black);
        //}
    }

    void CreateChatItem(string chat, Color chatColor)
    {
        // s의 내용으로 ChatItem을 만들자.
        GameObject go = Instantiate(chatItemFactory, trContent);
        // 만들어진 go에서 ChatItem 컴포넌트 가져오자.
        JSW_ChatItem chatItem = go.GetComponent<JSW_ChatItem>();


        // 가져온 컴포넌트의 SetText 함수 실행
        chatItem.SetText(chat, chatColor);
    }


}
