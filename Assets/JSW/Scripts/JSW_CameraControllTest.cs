using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSW_CameraControllTest : MonoBehaviour
{
    //기본카메라 Vector3(0,8,-4)
    public GameObject mainCam_Object;//메인카메라 오브젝트
    public GameObject mainCamPos_Object;  //메인카메라의 위치
    public Transform player;   // 플레이어 또는 기준 오브젝트
    public float mainCamY = 8; //카메라의 높이
    PhotonView playerPhotonview;


    void Start()
    {
        //메인카메라 캐싱
        mainCam_Object = GameObject.Find("MainCamera");
        
    }

    void Update()
    {


        if (transform != null)
        {
            Vector3 playerDir = transform.position - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
            float mainCamPosX = mainCamPos_Object.transform.position.x; //x방향
            float mainCamPosZ = mainCamPos_Object.transform.position.z; //z방향
            mainCam_Object.transform.position = new Vector3(mainCamPosX, mainCamY, mainCamPosZ); //플레이어의 움직임 따라가기
            mainCam_Object.transform.forward = playerDir; //카메라가 플레이어 방향을 계속 보게함
        }


        if (Input.GetMouseButton(1))//우클릭하는동안
        {

            print("우클릭 하는중");

        }




    }

}
