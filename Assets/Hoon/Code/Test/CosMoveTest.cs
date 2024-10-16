using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CosMoveTest : MonoBehaviour
{
    
    public Transform player;  // 플레이어 또는 기준 오브젝트
    public float radius = 5.0f;  // 오브젝트가 유지할 거리 (반지름)
    public float speed = 2.0f;   // 원운동 속도
    private float angle = 0f;

    public bool isLeftRotaion = false;
    public bool isRightRotaion = false;
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            print("우클릭중");

            

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
        // 매 프레임마다 각도를 증가시킴
        angle += speed * Time.deltaTime;

        // x, z 좌표에서 원운동 경로를 계산
        float x = player.position.x + Mathf.Cos(angle) * radius;
        float z = player.position.z + Mathf.Sin(angle) * radius;

        // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
        transform.position = new Vector3(x, transform.position.y, z);
    }

    public void LeftRotationMainCamera()
    {
        print("왼쪽으로 카메라회전");
        isLeftRotaion = !isLeftRotaion; //버튼을 누를때마다 변수를 반대값으로
        print("isLeftRotaion" + isLeftRotaion);
        // 매 프레임마다 각도를 증가시킴
        angle += speed * Time.deltaTime;

        // x, z 좌표에서 원운동 경로를 계산
        float x = -player.position.x + Mathf.Cos(angle) * radius;
        float z = player.position.z + Mathf.Sin(angle) * radius;

        // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
        transform.position = new Vector3(x, transform.position.y, z);
    }

    public void RightRotationMainCamera()
    {
        print("오른쪽으로 카메라회전");
        isRightRotaion = !isRightRotaion; //버튼을 누를때마다 변수를 반대값으로

        // 매 프레임마다 각도를 증가시킴
        angle += speed * Time.deltaTime;

        // x, z 좌표에서 원운동 경로를 계산
        float x = player.position.x + Mathf.Cos(angle) * radius;
        float z = player.position.z + Mathf.Sin(angle) * radius;

        // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
        transform.position = new Vector3(x, transform.position.y, z);
    }

}


