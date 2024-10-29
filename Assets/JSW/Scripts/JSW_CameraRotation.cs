using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JSW_CameraRotation : MonoBehaviour
{
    public Camera mainCamera; //메인카메라 가져오기
    
    public GameObject player;
    public GameObject mainCamPos_Object;
    public bool isLeftRotaion = false;
    public bool isRightRotaion = false;
    public float mainCamY = 8; //카메라의 높이
    public float mianCamZ = -8; // 카메라 거리
    public float radius = 10.0f;  // 기준점으로부터 거리 (반지름)
    public float speed = 1.0f;   // 원운동 속도
    private float angle = 180f;    //각도
    Vector3 playerPos;
    public float playerPosX;
    public float playerPosZ;
    Vector3 beforePos;
    Vector3 mainCamObjectPos;
    
    

    void Start()
    {
        player = GameObject.Find("PlayerMale(Clone)");
        mainCamPos_Object = GameObject.Find("MainCamPos");
    }

    void LateUpdate()
    {
        if(player == null)
        {
            player = GameObject.Find("PlayerPos");

        }
        if(mainCamPos_Object == null)
        {
            mainCamPos_Object = GameObject.Find("PlayerMainCamPos");
            //print("mainCamPos_Object 찾는중 ");

        }
        else
        {
            //playerPos = player.transform.position; //print("플레이어 현재위치" + playerPos);
            //playerPosX = player.transform.position.x; //플레이어의 위치값 X
            //playerPosZ = player.transform.position.z; //플에이어의 위치값 Y
            //mainCamObjectPos = mainCamPos_Object.transform.position; //카메라 위치
        }
       
        if (isLeftRotaion)
        {
            LeftRotationMainCamera();
        }
        if (isRightRotaion)
        {
            RightRotationMainCamera();
        }
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
        //플레이어가 있다면?
        if(player != null)
        {

            // 매 프레임마다 각도를 증가시킴
            angle += speed * Time.deltaTime;
            // x, z 좌표에서 원운동 경로를 계산
            float x = player.transform.position.x - Mathf.Cos(angle) * radius;
            float z = player.transform.position.z + Mathf.Sin(angle) * radius;

            // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
            mainCamPos_Object.transform.position = new Vector3(x, mainCamY, z);

            //Debug.DrawLine(beforePos, mainCamPos_Object.transform.position, Color.red, 6.0f);
            beforePos = mainCamPos_Object.transform.position;
        }
        else
        {

        }
       
    }

    public void RightRotationMainCamera()
    {
        // 매 프레임마다 각도를 증가시킴
        angle += speed * Time.deltaTime;
        // x, z 좌표에서 원운동 경로를 계산
        float x = player.transform.position.x + Mathf.Cos(angle) * radius;
        float z = player.transform.position.z + Mathf.Sin(angle) * radius;

        // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
        mainCamPos_Object.transform.position = new Vector3(x, mainCamY, z);


        //Debug.DrawLine(beforePos, mainCamPos_Object.transform.position, Color.red, 6.0f);
        beforePos = mainCamPos_Object.transform.position;
    }


} //클래스 끝
