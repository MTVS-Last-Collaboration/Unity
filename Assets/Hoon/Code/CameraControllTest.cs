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
    public float mianCamZ = -8; // 카메라 거리
    public float radius = 10.0f;  // 오브젝트가 유지할 거리 (반지름)
    public float speed = 1.0f;   // 원운동 속도
    private float angle = 0f;    //각도
    public bool isLeftRotaion = false;
    public bool isRightRotaion = false;

    void Start()
    {
        //메인카메라 캐싱
        mainCam_Object = GameObject.Find("MainCamera");
    }

    void Update()
    {
        if(transform != null)
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
        
        if(isLeftRotaion)
        {
            LeftRotationMainCamera();
        }
        if (isRightRotaion)
        {
            RightRotationMainCamera();
        }


    }

    public void RotaionMainCamera()
    {
        //플레이어를 기준으로 카메를 원운동 시키자.
        angle += speed * Time.deltaTime; // 매 프레임마다 각도를 증가시킴
        float x = player.position.x + Mathf.Cos(angle) * radius;    // x, z 좌표에서 원운동 경로를 계산
        float z = player.position.z + Mathf.Sin(angle) * radius;
        mainCamPos_Object.transform.position = new Vector3(x, mainCamY, z);    // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)

        LineRenderer line = new LineRenderer();
        line.enabled = true;
    }
    
    public void OnclickLeftRotation()
    {
        isLeftRotaion = !isLeftRotaion; //버튼을 누를때마다 변수를 반대값으로
        print("isLeftRotaion" + isLeftRotaion);
    }
    public void OnclickRightRotation()
    {
        isRightRotaion = !isRightRotaion; //버튼을 누를때마다 변수를 반대값으로
        print("isLeftRotaion" + isRightRotaion);
    }


    public void LeftRotationMainCamera()
    {
        print("왼쪽으로 카메라회전");      
        // 매 프레임마다 각도를 증가시킴
        angle += speed * Time.deltaTime;
        // x, z 좌표에서 원운동 경로를 계산
        float x = player.position.x + Mathf.Cos(angle) * radius;
        float z = player.position.z + Mathf.Sin(angle) * radius;

        // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
        mainCamPos_Object.transform.position = new Vector3(-x, mainCamY, z);

    }

    public void RightRotationMainCamera()
    {
        print("오른쪽으로 카메라회전");
        // 매 프레임마다 각도를 증가시킴
        angle += speed * Time.deltaTime;
        // x, z 좌표에서 원운동 경로를 계산
        float x = player.position.x + Mathf.Cos(angle) * radius;
        float z = player.position.z + Mathf.Sin(angle) * radius;

        // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
        mainCamPos_Object.transform.position = new Vector3(x, mainCamY, z);

    }


}
