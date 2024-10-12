using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControllTest : MonoBehaviour
{
    //기본카메라 Vector3(0,8,-4)


    public GameObject mainCam_Object;//메인카메라 오브젝트
    public GameObject mainCamPos_Object;  //메인카메라의 위치
    public Transform player;   // 플레이어 또는 기준 오브젝트
    public float mainCamY = 8; //카메라의 높이
    public float radius = 20.0f;  // 오브젝트가 유지할 거리 (반지름)
    public float speed = 2.0f;   // 원운동 속도
    private float angle = 0f;    //각도

    void Start()
    {
        
    }

    void Update()
    {
       
        Vector3 playerDir = transform.position - mainCam_Object.transform.position;  //플레이어 방향을 구합니다.
        float mainCamX = mainCamPos_Object.transform.position.x; //x방향
        float mainCamZ = mainCamPos_Object.transform.transform.position.z; //z방향
        mainCam_Object.transform.position = new Vector3(mainCamX, mainCamY, mainCamZ); //플레이어의 움직임
        mainCam_Object.transform.forward = playerDir; //카메라가 플레이어 방향을 계속 보게함

        if (Input.GetMouseButton(1))//우클릭하는동안
        {
            //플레이어를 기준으로 카메를 원운동 시키자.
            angle += speed * Time.deltaTime; // 매 프레임마다 각도를 증가시킴
            float x = player.position.x + Mathf.Cos(angle) * radius;    // x, z 좌표에서 원운동 경로를 계산
            float z = player.position.z + Mathf.Sin(angle) * radius;
            mainCamPos_Object.transform.position = new Vector3(x, mainCamY, z);    // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)


        }
    

    }
}
