using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController_JSW : MonoBehaviour
{
    public Transform target; // 카메라가 따라다닐 캐릭터
    public float rotationSpeed = 100.0f; // 회전 속도
    public Vector2 sensitivity = new Vector2(0.1f, 0.1f); // 회전 감도

    private Vector2 previousTouchPosition; // 이전 터치 위치
    private bool isDragging = false; // 드래그 상태 확인
    public JSW_VirtualJoyStick virtualJoyStick;


    void Update()
    {
        if (virtualJoyStick.isMovingPlayer) return;
        // 터치 입력 감지
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                // 드래그 시작
                isDragging = true;
                previousTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                // 드래그 중: 터치 이동 거리 계산
                Vector2 deltaPosition = touch.position - previousTouchPosition;

                // 회전 계산
                float horizontalRotation = deltaPosition.x * sensitivity.x;
                float verticalRotation = deltaPosition.y * sensitivity.y;

                // 캐릭터 기준으로 카메라 회전
                transform.RotateAround(target.position, Vector3.up, horizontalRotation * rotationSpeed * Time.deltaTime);

                // 상하 회전 제한
                float currentXAngle = transform.eulerAngles.x - verticalRotation * rotationSpeed * Time.deltaTime;
                currentXAngle = Mathf.Clamp(currentXAngle, 10, 80); // 각도 제한 (예: 10도 ~ 80도)
                transform.eulerAngles = new Vector3(currentXAngle, transform.eulerAngles.y, 0);

                // 터치 위치 갱신
                previousTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                // 드래그 종료
                isDragging = false;
            }
        }
    }
}
