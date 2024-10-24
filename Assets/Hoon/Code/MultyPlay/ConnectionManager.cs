//포톤추가
using Photon.Pun;
//MethodInfo 추가
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEditor;

//부모를 MonoBehaviourPunCallbacks로 변경
public class ConnectionManager : MonoBehaviourPunCallbacks
{
    //public MethodInfo methodInfo;
    public static ConnectionManager Instance;
    public GameObject avataName;
    public string createRoomName;
    public string joinRoomName;

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

    }
    //마스터연결 콜백
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        // 마스터 서버에 접속이 완료되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

        // 서버의 로비로 들어간다.
        PhotonNetwork.JoinLobby();

    }
    //로비연결 콜백
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // 서버 로비에 들어갔음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");

    }
    //방생성
    public void CreateRoom()
    {
        //string roomName = "LoobyTestHoon";
        string roomName = createRoomName;
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
    //방참가
    public void JoinRoom()
    {
        //tring roomName = "LoobyTestHoon";
        string roomName = joinRoomName;

        //룸이름 길이가 0보다 크면
        if (roomName.Length > 0)
        {
            PhotonNetwork.JoinRoom(roomName);
        }

    }
    //방만들기 콜백
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        // 성공적으로 방이 개설되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        print("방 만들어짐!");


    }
    //방참가 콜백
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // 성공적으로 방에 입장되었음을 알려준다.
        print(MethodInfo.GetCurrentMethod().Name + " is Call!");
        print("방에 입장 성공");

        // 방에 입장한 친구들은 모두 N번 씬으로 이동하자! //빌드세팅에 추가해야만 이동가능 idx 확인 필수
        PhotonNetwork.LoadLevel(1);

    }
    //방참가 실패 콜백
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        // 룸에 입장이 실패한 이유를 출력한다.
        Debug.LogError(message);
        print("입장 실패..." + message);

    }
    // 룸에 다른 플레이어가 입장했을 때의 콜백 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        string playerMsg = $"{newPlayer.NickName}님이 입장하셨습니다.";
        print(playerMsg);

    }
    // 룸에 있던 다른 플레이어가 퇴장했을 때의 콜백 함수
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        string playerMsg = $"{otherPlayer.NickName}님이 퇴장하셨습니다.";
        print(playerMsg);

    }
    //룸리스트 콜백은 로비에 접속했을때 자동으로 호출
    //현재 로비에서 룸 리스트를 받아보자. 로비에서만 호출가능
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        print("룸리스트 업데이트");

        //print("방이 있으니 참가해야지");
        // 로비에 들어갔으면 방을 생성
        //JoinRoom();
        bool isCreataeRoom = false;
        if (roomList.Count > 0) //방개수가 0보다 크면                                
        {
            print("방개수" + roomList.Count);
            //방을 모두 검색해서 방이 있는지 찾자.
            foreach (RoomInfo roomInfo in roomList)
            {
                //방이름을 포함하고 있으면 참가.
                //if (roomInfo.Name.Contains("LoobyTestHoon"))
                if (roomInfo.Name.Contains(joinRoomName))
                {
                    print("방이있으니까 참가해야지~");
                    isCreataeRoom = false;
                    JoinRoom();
                    break;
                }
                else
                {
                    isCreataeRoom = true;
                }
            }

            if (isCreataeRoom)
            {
                print("원하는 방이 없으니 만들자");
                //없으면 방 만들기
                CreateRoom();

            }

        }
        else if (roomList.Count == 0) //방개수가 0개면
        {
            print("방개수" + roomList.Count + "방이없으니 만들어야지...");
            CreateRoom();
        }

    }

} //클래스 끝
