using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DecoMineManager : MonoBehaviourPun, IOnEventCallback
{
    public GameObject Floors;
    public GameObject Walls;
    public Material[] Floormaterials;
    public Material[] Wallmaterials;
    public int floorNum=0;
    public int wallNum=0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Floors.GetComponent<MeshRenderer>().material = Floormaterials[floorNum];
        Walls.GetComponent<MeshRenderer>().material = Wallmaterials[wallNum];
    }

    public void changeFloor()
    {


        if (floorNum < 4)
        {
            floorNum++;
            //Floors.GetComponent<MeshRenderer>().material = Floormaterials[floorNum++];
        }
        else
        {
            floorNum = 0;
        }

        object[] sendContent = new object[] { floorNum };

        // 송신 옵션
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(5, sendContent, eventOptions, SendOptions.SendUnreliable);

        EventSystem.current.SetSelectedGameObject(null);
    }
    public void changeWalls()
    {

        if (wallNum < 10)
        {
            wallNum++;
            //Walls.GetComponent<MeshRenderer>().material = Wallmaterials[wallNum++];
        }
        else
        {
            wallNum = 0;
        }

        object[] sendContent = new object[] { wallNum };

        // 송신 옵션
        RaiseEventOptions eventOptions = new RaiseEventOptions();
        eventOptions.Receivers = ReceiverGroup.All;
        //eventOptions.CachingOption = EventCaching.DoNotCache;

        // 이벤트 송신 시작
        PhotonNetwork.RaiseEvent(6, sendContent, eventOptions, SendOptions.SendUnreliable);


        EventSystem.current.SetSelectedGameObject(null);

        

    }

    private void OnEnable()
    {
        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;

    }


    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 5)
        {
            // 받은 내용을 "닉네임: 채팅 내용" 형식으로 스크롤뷰의 텍스트에 전달한다.
            object[] receiveObjects = (object[])photonEvent.CustomData;
            floorNum = (int)receiveObjects[0];
        }

        if (photonEvent.Code == 6)
        {
            object[] receiveObjects = (object[])photonEvent.CustomData;
            wallNum = (int)receiveObjects[0];
        }
    }

    private void OnDisable()
    {
        //PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this); // 델리게이트 방식
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
}
