using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HoonCreateRoom : MonoBehaviour
{
    public GameObject imgMyStorageMenuObject;
    public GameObject imgGetRoomObject;
    public GameObject imgShowRoomListObject;
    public HoonUIController hoonUIController;
    public GameObject choiceRoomErr;
    public GameObject choiceRoomOk;
    public OnMoveTrigger onMoveTrigger;

    void Start()
    {
        
    }

    // Update is called once per frame
    /* void Update()
    {       
    
    }*/
    
    public void VeiwRoomStorage(GameObject obj)
    {
        print(obj.name);
        if(obj.name == "Btn_MyStorageMenu")
        {
            imgMyStorageMenuObject.SetActive(true);
            imgGetRoomObject.SetActive(false);
            imgShowRoomListObject.SetActive(false);

        }
        else if(obj.name == "Btn_GetRoom")
        {
            imgMyStorageMenuObject.SetActive(false);
            imgGetRoomObject.SetActive(true);
            imgShowRoomListObject.SetActive(false);
        }
        else if (obj.name == "Btn_ShowRoomList")
        {
            imgMyStorageMenuObject.SetActive(false);
            imgGetRoomObject.SetActive(false);
            imgShowRoomListObject.SetActive(true);
        }

    }

    public void ViewRoomMark(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void MoveRoom()
    {
        //hoonUIController.isMyMarkObject , hoonUIController.isGetMarkObject, hoonUIController.isShareMarkObejct

        if(!hoonUIController.isMyMarkObject && !hoonUIController.isGetMarkObject)
        {
            print("방을선택해라");
            choiceRoomErr.SetActive(true);
            return;
        }


        if (hoonUIController.isMyMarkObject)
        {
            print("내방마크됨" + hoonUIController.isMyMarkObject);
            choiceRoomOk.SetActive(true);
            // 내방으로 가는 코드를 넣자.
            onMoveTrigger.GoOtherRoom();



        }
        else
        {
            print("내방마크되지않음" + hoonUIController.isMyMarkObject);

        }


    }

}//클래스끝
