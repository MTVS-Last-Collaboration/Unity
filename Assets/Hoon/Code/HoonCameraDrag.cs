using JetBrains.Annotations;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class HoonCameraDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
    }

    //카메라 캐싱----------------
    public GameObject playerObject;
    public GameObject cameraObject;
    public GameObject cameraFocusObject;
    //터치테스트------------------
    float cameraMoveValue = 0.2f;
    float camPosX;
    float camPosY = 3.0f;
    float camPosZ = -5.0f;
    //터치유효성 검사----------
    bool isMouseTouch = false;
    //회전에 필요한 변수
    public float radius = 7.0f;  // 기준점으로부터 거리 (반지름)
    public float speed = 5.0f;   // 원운동 속도
    public float mainCamY = 5.0f; //카메라의 높이
    float angle = 0f;    //각도
    //카메라 위치 값---------
    Vector3 beforePos;
    Vector3 afterPos;
    //플레이어 변수-----------
    Vector3 playerDir;
    Vector3 playerPos;
    //카메라가 플레이어 추적
    Vector3 offset;
    bool isOffsetInitialized = false;
    public float rotateSpeed = 1.0f;
    void Start()
    {
        //카메라 위치 초기값
        Camera.main.transform.position = playerObject.transform.position + new Vector3(0, 4, -7);
        
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = playerObject.transform.position;
        PlayerMove(); //플레이어를 움직이게
        CameraViewTest(); //카메라 보는 방향
        //CameraPosTest(); //카메라 위치를 고정
        FixTouchInputCameraMove(); //터치하면 카메라가 움직이게
        //MouseDragTest();
        //TouchInputTest();
        //DrawMouseLineTest();
    }

    void PlayerMove()
    {
        float MoveX = Input.GetAxisRaw("Horizontal");
        float MoveY = Input.GetAxisRaw("Vertical");
        
        //방향, 속도, 시간
        //내위치 + 변화량
        transform.position += new Vector3(MoveX, 0, MoveY) * 3 * Time.deltaTime;

    }

    void CameraViewTest()
    {
        playerDir =  playerObject.transform.position - cameraObject.transform.position;
        cameraObject.transform.forward = playerDir;
        
    }

    public void FixTouchInputCameraMove()
    {
       
        if (Input.GetMouseButton(0))
        {
            isMouseTouch = true;

            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            // 카메라 회전 각도 계산
            if (mouseX != 0)
            {
                angle += (mouseX > 0 ? 1 : -1) * speed * Time.deltaTime;
            }

            // 플레이어를 중심으로 원운동 경로 계산
            float x = playerObject.transform.position.x + Mathf.Cos(angle) * radius;
            float z = playerObject.transform.position.z + Mathf.Sin(angle) * radius;

            // Y축 변경 (위/아래 이동)
            float y = Camera.main.transform.position.y + mouseY * 1f;

            Vector3 cameraDir = new Vector3(x, y, z);
           
            // 카메라 위치 업데이트
            Camera.main.transform.position = Vector3.Lerp(beforePos, cameraDir, rotateSpeed) ;

            // 디버그 선 그리기
            Debug.DrawLine(beforePos, Camera.main.transform.position, Color.red, 6.0f);
            beforePos = Camera.main.transform.position;
        }
        else
        {
            isMouseTouch = false;
        }

        

    }

    void HoonInputMouseRotationScreen()
    {

        if (Input.GetMouseButton(0))
        {
            isMouseTouch = true;
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");


           

            //마우스가 오른쪽으로 이동하면 출력하기
            if (mouseX > 0)
            {
                //print("오른쪽으로 카메라회전");
                // 매 프레임마다 각도를 증가시킴
                angle += speed * Time.deltaTime;

                // 플레이어를 중심으로 x, z 좌표 원운동 경로 계산
                float x = playerObject.transform.position.x + Mathf.Cos(angle) * radius;
                float z = playerObject.transform.position.z + Mathf.Sin(angle) * radius;

                float y;
                if (mouseY > 0)
                {
                    y = Camera.main.transform.position.y + 0.1f;
                }
                else if (mouseY < 0)
                {
                    y = Camera.main.transform.position.y - 0.1f;
                }
                else
                {
                    y = Camera.main.transform.position.y;
                }
                Camera.main.transform.position = new Vector3(x, y, z);

                Debug.DrawLine(beforePos, Camera.main.transform.position, Color.red, 6.0f);
                beforePos = Camera.main.transform.position;
            }
            else if (mouseX < 0)
            {

                //print("왼쪽으로 카메라회전");
                // 매 프레임마다 각도를 증가시킴
                angle += speed * Time.deltaTime;
                // x, z 좌표에서 원운동 경로를 계산
                float x = playerObject.transform.position.x - Mathf.Cos(angle) * radius;
                float z = playerObject.transform.position.z + Mathf.Sin(angle) * radius;

                // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
                //Camera.main.transform.position = new Vector3(x, mainCamY, z);
                //Camera.main.transform.position = new Vector3(x, y, z);

                if (mouseY > 0)
                {
                    float y = Camera.main.transform.position.y + 0.1f;
                    // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
                    //Camera.main.transform.position = new Vector3(x, mainCamY, z);
                    Camera.main.transform.position = new Vector3(x, y, z);
                    print(111);
                }
                else if (mouseY < 0)
                {
                    float y = Camera.main.transform.position.y - 0.1f;
                    // 새로운 위치로 오브젝트 이동 (y는 고정되거나 원하는 값으로 설정)
                    //Camera.main.transform.position = new Vector3(x, mainCamY, z);
                    Camera.main.transform.position = new Vector3(x, y, z);
                    print(222);
                }
                else
                {
                    float y = Camera.main.transform.position.y;
                    Camera.main.transform.position = new Vector3(x, y, z);
                    print(333);
                }

                Debug.DrawLine(beforePos, Camera.main.transform.position, Color.red, 6.0f);
                beforePos = Camera.main.transform.position;

            }

        }
        else
        {
            isMouseTouch = false;
            //print("회전정지");
        }



    }           
            
    void CameraPosTest()
    {
        if (!isMouseTouch)
        {
            //print("isMouseTouch: " + isMouseTouch);
            float smoothSpeed = 3.0f; // 카메라 이동 속도 (값이 클수록 빠름)

            // 플레이어의 위치 가져오기
            playerPos = playerObject.transform.position;

            // 오프셋 초기화 (한 번만 실행)
            if (!isOffsetInitialized)
            {
                offset = Camera.main.transform.position - playerPos; // 현재 카메라와 플레이어 간 상대 위치 계산
                isOffsetInitialized = true;
            }

            // 목표 위치: 플레이어 위치 + 오프셋
            Vector3 targetPosition = playerPos + offset;

            // 카메라의 현재 위치에서 목표 위치로 부드럽게 이동
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * smoothSpeed);

            // 디버그: 거리 출력
            float playerDist = Vector3.Distance(playerPos, Camera.main.transform.position);
            //print("Distance: " + playerDist);



        }
        else
        {
            // 마우스 터치 상태에서 오프셋 초기화 비활성화
            isOffsetInitialized = false;
        }

    }

    public void MouseDragTest()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        print("마우스 x값" + mouseX);
        print("마우스 y값" + mouseY);

    }

    public void TouchInputTest()
    {
       

        //마우스가 내려 갔는지 검증하기
        if (Input.GetMouseButton(0))
        {
            isMouseTouch = true;
            float mouseX = Input.GetAxisRaw("Mouse X");
            float mouseY = Input.GetAxisRaw("Mouse Y");

            //마우스가 오른쪽으로 이동하면 출력하기
            if (mouseX >0)
            {
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(0.1f, 0, 0); //x값을 증가시키자
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(-0.1f, 0, 0); //x값을 감소시키자
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.1f); //z값을 증가시키자
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, -0.1f); //z값을 증가시키자
                
                if(Camera.main.transform.position.z < -5 && Camera.main.transform.position.x < 5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraMoveValue, 0, 0); //x값을 증가시키자
                    camPosX = Camera.main.transform.localPosition.x +cameraMoveValue;
                    print(1111);
                }
                else if (Camera.main.transform.position.x < -5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, -cameraMoveValue); //z값을 증가시키자
                    camPosZ = Camera.main.transform.localPosition.x - cameraMoveValue;
                    print(2222);
                }
                else if (Camera.main.transform.position.z > 5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraMoveValue, 0, 0); //x값을 감소시키자
                    camPosX = Camera.main.transform.localPosition.x - cameraMoveValue;
                    print(3333);
                }
                else if(Camera.main.transform.position.x > 5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, cameraMoveValue); //z값을 증가시키자
                    camPosZ = Camera.main.transform.localPosition.x + cameraMoveValue;
                    print(4444);
                }
                else
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraMoveValue, 0, 0); //x값을 증가시키자
                    camPosX = Camera.main.transform.localPosition.x + cameraMoveValue;
                    print(5555);
                }
                               
            }
            else if(mouseX < 0)
            {
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(0.1f, 0, 0); //x값을 증가시키자
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(-0.1f, 0, 0); //x값을 감소시키자
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.1f); //z값을 증가시키자
                //Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, -0.1f); //z값을 감소시키자

                if (Camera.main.transform.position.z < -5 && Camera.main.transform.position.x > 5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraMoveValue, 0, 0); //x값을 감소시키자
                    camPosX = Camera.main.transform.localPosition.x - cameraMoveValue;
                    print(1111);
                }
                else if (Camera.main.transform.position.x > 5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, -cameraMoveValue); //z값을 감소시키자
                    camPosZ = Camera.main.transform.localPosition.z - cameraMoveValue;
                    print(2222);
                }
                else if (Camera.main.transform.position.z > 5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(cameraMoveValue, 0, 0); //x값을 증가시키자
                    camPosX = Camera.main.transform.localPosition.x + cameraMoveValue;
                    print(3333);
                }
                else if (Camera.main.transform.position.x < -5)
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0, cameraMoveValue); //z값을 증가시키자
                    camPosZ = Camera.main.transform.localPosition.z + cameraMoveValue;
                    print(4444);
                }
                else
                {
                    Camera.main.transform.position = Camera.main.transform.position + new Vector3(-cameraMoveValue, 0, 0); //x값을 감소시키자
                    camPosX = Camera.main.transform.localPosition.x - cameraMoveValue;
                    print(5555);
                }

            }

            //마우스가 왼쪽으로 이동하면 출력하기
            if (mouseY > 0)
            {
                //print("위로 이동" + mouseY);
                Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, 0.1f, 0);
            }
            else if (mouseY < 0)
            {
                //print("아래로 이동" + mouseY);
                Camera.main.transform.position = Camera.main.transform.position + new Vector3(0, -0.1f, 0);
            }

          
        }
        else
        {
            isMouseTouch = false;
        }
    

    }

    void DrawMouseLineTest()
    {
        Vector3 beforeMousePosition = GetMouseWorldPosition();
        // 현재 마우스 위치 계산
        Vector3 currentMousePosition = GetMouseWorldPosition();

        // 이전 위치와 현재 위치를 연결하는 선 그리기
        Debug.DrawLine(beforeMousePosition, currentMousePosition, Color.red, 0.01f);

    }

    // 화면상의 마우스 좌표를 월드 좌표로 변환
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // 카메라와의 거리 (Camera.main 기준)
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

}//클래스 끝
