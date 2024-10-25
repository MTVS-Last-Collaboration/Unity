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

    public string cameraPos = "Original";

    public float mainCamY = 8; //카메라의 높이
    PhotonView playerPhotonview;

    void Start()
    {
        //메인카메라 캐싱
        mainCam_Object = GameObject.Find("MainCamera");
        lobbyGameManager = GameObject.Find("LobbyGameManager");
    }

    void Update()
    {

        if (cameraPos == "Original")    
        {
            Vector3 playerDir = transform.position - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
            float mainCamPosX = mainCamPos_Object.transform.position.x; //x방향
            float mainCamPosY = mainCamPos_Object.transform.position.y; //x방향
            float mainCamPosZ = mainCamPos_Object.transform.position.z; //z방향
            Camera.main.fieldOfView = 60;
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position,new Vector3(mainCamPosX, mainCamPosY, mainCamPosZ), Time.deltaTime * 10); //플레이어의 움직임 따라가기
            mainCam_Object.transform.forward = playerDir; //카메라가 플레이어 방향을 계속 보게함
        }
        else if (cameraPos == "Mong")
        {
            Camera.main.fieldOfView = 100;
            playerTransform = lobbyGameManager.GetComponent<JSW_LobbyGameManager>().player.transform;
            mainCam_Object.transform.position = Vector3.Lerp(mainCam_Object.transform.position, playerTransform.position + ((mong.position - Vector3.up * 0.5f) - playerTransform.position).normalized * 0.3f + Vector3.up * 0.4f, Time.deltaTime * 10);
            mainCam_Object.transform.forward = ((mong.position - Vector3.up) - playerTransform.position).normalized;
            mong.forward = ((mong.position) - playerTransform.position).normalized * -1;
        }
        else if (cameraPos == "3D")
        {
            mainCam_Object.transform.position = CamPos_3D.position;
            mainCam_Object.transform.forward = CamPos_3D.forward;
        }
        // 여기에 선반도 추가하면 될 듯

    }

    public void CameraToMong()
    {
        cameraPos = "Mong";
    }
    public void ResetCamera()
    {
        cameraPos = "Original";
        mong.forward = playerTransform.forward * -1;
    }
    public void CameraTo3D()
    {
        cameraPos = "3D";
    }
    //0.13 0.15 -1
}
