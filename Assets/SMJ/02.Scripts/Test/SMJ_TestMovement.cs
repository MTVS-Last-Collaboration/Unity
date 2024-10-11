using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMJ_TestMovement : MonoBehaviour
{
    CharacterController cc;
    public float moveSpeed = 0;
    private void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = h * transform.right + v * transform.forward;

        if (dir.magnitude > 1)
        {
            dir.Normalize();
        }

        transform.position += dir * moveSpeed * Time.deltaTime;

        //온트리거 내의 첫 클릭이냐? >> isFirst = true >> 닉네임 입력 ui 켜기 >> parterID 자동 입력
        //꽃 상태 ui 켜기 >> 백엔드에서 닉네임, 녹음파일 유무 받기 Get
    }
}
