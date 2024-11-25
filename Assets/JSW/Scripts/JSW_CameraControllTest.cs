using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSW_CameraControllTest : MonoBehaviour
{
    //기본카메라 Vector3(0,8,-4)
    public GameObject mainCam_Object;//메인카메라 오브젝트
    public GameObject mainCamPos_Object;  //메인카메라의 위치
    public GameObject lobbyGameManager;

    public Transform playerTransform;   // 플레이어 또는 기준 오브젝트
    public Transform mong;
    public Transform CamPos_3D;
    public Transform CalenderPos;
    public Transform AlbumPos;

    //public GameObject LobbyGameManager;

    public float aa = 1.5f;
    public float bb = -2;

    public string cameraPos = "Original";

    public float mainCamY = 8; //카메라의 높이
    PhotonView playerPhotonview;

    void Start()
    {
        //메인카메라 캐싱
        mainCam_Object = GameObject.Find("MainCamera");
        //lobbyGameManager = GameObject.Find("LobbyGameManager");
    }


    void LateUpdate()
    {

        if (lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player == null)
        {
            return;
        }
        if (cameraPos == "Original")
        {
            mong.forward = (lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player.transform.position + Vector3.up * 0.5f - (mong.position)).normalized;
            Vector3 playerDir = transform.position - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
            if (lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player == null)
            {
                return;
            }
            Vector3 cameraMoveDir = lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player.transform.position +Vector3.up - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
            float mainCamPosX = mainCamPos_Object.transform.position.x; //x방향
            float mainCamPosY = mainCamPos_Object.transform.position.y; //x방향
            float mainCamPosZ = mainCamPos_Object.transform.position.z; //z방향
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position, new Vector3(mainCamPosX, mainCamPosY, mainCamPosZ) + cameraMoveDir.normalized * 2.5f + Vector3.up * -2.3f, Time.deltaTime); //플레이어의 움직임 따라가기
            mainCam_Object.transform.forward = Vector3.Lerp(mainCam_Object.transform.forward, cameraMoveDir, Time.deltaTime * 0.5f); //카메라가 플레이어 방향을 계속 보게함
        }
        else if (cameraPos == "Funiture")    
        {
            Vector3 playerDir = transform.position - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
            if (lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player == null)
            {
                return;
            }
            Vector3 cameraMoveDir = lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player.transform.position + mainCam_Object.transform.right * 1.6f - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
            float mainCamPosX = mainCamPos_Object.transform.position.x; //x방향
            float mainCamPosY = mainCamPos_Object.transform.position.y; //x방향
            float mainCamPosZ = mainCamPos_Object.transform.position.z ; //z방향
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position,new Vector3(mainCamPosX, mainCamPosY, mainCamPosZ) + cameraMoveDir.normalized*2.5f, Time.deltaTime); //플레이어의 움직임 따라가기
            mainCam_Object.transform.forward = Vector3.Lerp(mainCam_Object.transform.forward,cameraMoveDir,Time.deltaTime * 0.5f); //카메라가 플레이어 방향을 계속 보게함
        }
        else if (cameraPos == "Mong")
        {
            int layerMask = 1 << LayerMask.NameToLayer("Player");
            // 현재 카메라의 cullingMask에서 지정된 레이어를 제외시킴
            mainCam_Object.GetComponent<Camera>().cullingMask &= ~layerMask;
            playerTransform = lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player.transform;
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position, playerTransform.position + -1*((mong.position - Vector3.up * 0.5f) - playerTransform.position).normalized * 3f + Vector3.up * 1.1f, Time.deltaTime * 5);
            mainCam_Object.transform.forward = Vector3.Lerp(mainCam_Object.transform.forward,((mong.position - Vector3.up* 0.8f) - playerTransform.position).normalized,Time.deltaTime * 5);
            mong.forward = ((mong.position) - mainCam_Object.transform.position).normalized * -1;
        }
        else if (cameraPos == "Calender")
        {
            int layerMask = 1 << LayerMask.NameToLayer("Player");
            // 현재 카메라의 cullingMask에서 지정된 레이어를 제외시킴
            mainCam_Object.GetComponent<Camera>().cullingMask &= ~layerMask;
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position, CalenderPos.position + Vector3.left, Time.deltaTime * 5);
            mainCam_Object.transform.forward = CalenderPos.position - mainCam_Object.transform.position;
        }
        else if (cameraPos == "Album")
        {
            int layerMask = 1 << LayerMask.NameToLayer("Player");
            // 현재 카메라의 cullingMask에서 지정된 레이어를 제외시킴
            mainCam_Object.GetComponent<Camera>().cullingMask &= ~layerMask;
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position, AlbumPos.position + Vector3.forward * -1, Time.deltaTime * 5);
            mainCam_Object.transform.forward = AlbumPos.position - mainCam_Object.transform.position;
        }
        else if (cameraPos == "3D")
        {
            int layerMask = 1 << LayerMask.NameToLayer("Player");
            // 현재 카메라의 cullingMask에서 지정된 레이어를 제외시킴
            mainCam_Object.GetComponent<Camera>().cullingMask &= ~layerMask;
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position, CamPos_3D.position, Time.deltaTime*5);
            mainCam_Object.transform.forward = Vector3.Lerp(mainCam_Object.transform.forward,CamPos_3D.forward,Time.deltaTime*5);
        }
        // 여기에 선반도 추가하면 될 듯
    }

    public void CameraToMong()
    {
        cameraPos = "Mong";
    }
    public void CameraToCalender()
    {
        cameraPos = "Calender";
    }
    public void CameraToAlbum()
    {
        cameraPos = "Album";
    }
    public void CameraToFuniture()
    {
        cameraPos = "Funiture";
    }

    public void ResetCamera()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Player");
        // 현재 카메라의 cullingMask에 지정된 레이어를 추가
        Camera.main.cullingMask |= layerMask;

        cameraPos = "Original";
        playerTransform = lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player.transform;
        mong.forward = playerTransform.forward * -1;
    }
    public void CameraTo3D()
    {
        cameraPos = "3D";
    }
    //0.13 0.15 -1
}
