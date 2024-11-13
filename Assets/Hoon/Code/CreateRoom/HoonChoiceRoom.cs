using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoonChoiceRoom : MonoBehaviour
{
    public GameObject img_CreatingRoom;
    public GameObject roomChoiceMark;
    public bool isViewChoiveMark = false;
    

    

    void Start()
    {
        img_CreatingRoom.SetActive(false);
        roomChoiceMark.SetActive(false); //closeRoomChoiceMark
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void ViewRoomChoiceMarkControll()
    {
        if(!isViewChoiveMark)
        {
            roomChoiceMark.SetActive(true);
            isViewChoiveMark = true;
        }
        else
        {
            roomChoiceMark.SetActive(false);
            isViewChoiveMark = false;
        }

    }

    public void ViewCreatingUIControll()
    {
        img_CreatingRoom.SetActive(true);
    }


}
