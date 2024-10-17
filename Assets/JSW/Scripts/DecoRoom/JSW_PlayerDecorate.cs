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
    
    public void setFuniture()
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
}
