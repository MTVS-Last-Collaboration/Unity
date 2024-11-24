using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoonChoiceRoom : MonoBehaviour
{
    public GameObject img_CreatingRoom;
    public GameObject roomChoiceMark;
    public bool isViewChoiveMark = false;
    HoonSoundManagerLogin hoonSoundManager;
    

    

    void Start()
    {
        //img_CreatingRoom.SetActive(false); //closeCreateRoom
        roomChoiceMark.SetActive(false); //closeRoomChoiceMark
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void ViewRoomChoiceMarkControll()
    {
        hoonSoundManager = transform.GetComponent<HoonSoundManagerLogin>();

        if (!isViewChoiveMark)
        {
            hoonSoundManager.PlaySound(0);
            roomChoiceMark.SetActive(true);
            isViewChoiveMark = true;
        }
        else
        {
            hoonSoundManager.PlaySound(1);
            roomChoiceMark.SetActive(false);
            isViewChoiveMark = false;
        }

    }

    public void ViewCreatingUIControll()
    {
        img_CreatingRoom.SetActive(true);
    }


}
