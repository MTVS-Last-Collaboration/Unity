using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JSW_PlayerDecorate : MonoBehaviour
{
    public Vector3 playerDir;
    public Vector3 playerPos;
    public GameObject funitureObject;
    public JSW_DecorateRoomManager DRM;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        playerDir = new Vector3(Mathf.Round(transform.forward.x), Mathf.Round(transform.forward.y), Mathf.Round(transform.forward.z));
        playerPos = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));

    }
    
    public void SetFuniture()
    {
        int dir = 0;

        GameObject funitureOb = Instantiate(funitureObject);
        
        if (Mathf.Abs(playerDir.x) == Mathf.Abs(playerDir.z))
        {
            if (Mathf.Abs(transform.forward.x) >= Mathf.Abs(transform.forward.z))
            {
                funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 1);
                playerDir.z = 0;
            }
            else
            {
                funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 1, playerPos.z + playerDir.z * 2);
                playerDir.x = 0;
            }
        }
        else
        {
            funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 2);
        }

        funitureOb.transform.forward = playerDir;

        if (playerDir.z == 1)
        {
            dir = 0;
        }
        else if (playerDir.x == 1)
        {
            dir = 1;
        }
        else if (playerDir.z == -1)
        {
            dir = 2;
        }
        else if (playerDir.x == -1)
        {
            dir = 3;
        }

        print(playerPos.x + " " + playerPos.z + " z");
        if (DRM.IsCanAddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, 1, 2, dir))
        {
            DRM.AddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, 1, 2, dir);
        }
        else
        {
            Destroy(funitureOb);
            print("no");
        }
    }

    
    public void PushFunitureSetting()
    {
        print("k");
        // 플레이어의 위치와 방향 설정 (정면 방향)
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // Ray 생성 (카메라 앞에서 앞으로 쏘는 Ray)
        Ray ray = new Ray(transform.position, forward);

        // Ray를 시각적으로 확인하기 위해 그립니다
        Debug.DrawRay(transform.position + Vector3.right*2, forward * 5, Color.red);

        // Ray가 물체와 충돌하는지 검사
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1))
        {
            // 충돌한 물체가 있다면 물체의 이름 출력
            Debug.Log("Ray hit: " + hit.collider.gameObject.name);

        }
        int dir = 0;

        //GameObject funitureOb = Instantiate(funitureObject);

        //if (Mathf.Abs(playerDir.x) == Mathf.Abs(playerDir.z))
        //{
        //    if (Mathf.Abs(transform.forward.x) >= Mathf.Abs(transform.forward.z))
        //    {
        //        funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 1);
        //        playerDir.z = 0;
        //    }
        //    else
        //    {
        //        funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 1, playerPos.z + playerDir.z * 2);
        //        playerDir.x = 0;
        //    }
        //}
        //else
        //{
        //    funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 2);
        //}

        //funitureOb.transform.forward = playerDir;

        //if (playerDir.z == 1)
        //{
        //    dir = 0;
        //}
        //else if (playerDir.x == 1)
        //{
        //    dir = 1;
        //}
        //else if (playerDir.z == -1)
        //{
        //    dir = 2;
        //}
        //else if (playerDir.x == -1)
        //{
        //    dir = 3;
        //}
    }
}
