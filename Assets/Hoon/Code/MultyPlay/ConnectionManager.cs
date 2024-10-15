using Photon.Pun;
//MethodInfo 추가
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

//부모를 MonoBehaviourPunCallbacks로 변경
public class ConnectionManager : MonoBehaviourPunCallbacks
{
    //public MethodInfo methodInfo;
    public static ConnectionManager Instance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /* void Update()
     {

     }*/

    public void StartLobby()
    {
        //만약 사용자 이름이 있다면, 이름의 길이가 0이 아니어야한ㅁ
        print("로그인하는중...");
        //gameVersion projectSetting -> player -> version 과 일치하게 설정
        PhotonNetwork.GameVersion = "1.0.0";
        //닉네임
        PhotonNetwork.NickName = "LoveForestAvata";
        //PhotonNetwork.NickName = MainUI.Instance.userNameText;
        //화면동기화
        PhotonNetwork.AutomaticallySyncScene = true;
        // 접속을 서버에 요청하기
        PhotonNetwork.ConnectUsingSettings();
        

    }
    //서버연결콜백
    public override void OnConnected()
    {
        base.OnConnected();

        // 네임 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

    }
    //서버끊김콜백
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        // 실패 원인을 출력한다.

        print(MethodInfo.GetCurrentMethod().Name + " is call");
        //MainUI.Instance.mainUiObject.move_Lobby_Btn.interactable = true;

    }
    public void CreateRoom()
    {
        string roomName = "LoobyTest";
        int playerCount = 10;

        //룸 네임 길이가 0보다 길고 플레이 카운트가 1보다 크다면
        if (roomName.Length > 0 && playerCount > 1)
        {
            // 나의 룸 옵션 만든다.
            RoomOptions roomOpt = new RoomOptions();
            //최대인원
            roomOpt.MaxPlayers = playerCount;
            //룸에 사람이 들어오게 하자.
            roomOpt.IsOpen = true;
            //룸을 검색할 수 있게 해주자. 
            roomOpt.IsVisible = true;


            //방을생성하자.
            PhotonNetwork.CreateRoom(roomName, roomOpt, TypedLobby.Default);


        }

        //방생성이 완료되면
        //JoinRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 성공적으로 방에 입장되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        print("방에 입장 성공");
        //LobbyUIController.lobbyUI.PrintLog("방에 입장 성공!");

        // 방에 입장한 친구들은 모두 N번 씬으로 이동하자! //빌드세팅에 추가해야만 이동가능 idx 확인 필수
        //PhotonNetwork.LoadLevel(1);

    }










} //클래스 끝
