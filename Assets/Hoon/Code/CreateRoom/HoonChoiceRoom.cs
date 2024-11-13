using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoonChoiceRoom : MonoBehaviour
{
    public GameObject roomChoiceMark; //
    bool isViewChoiveMark = false;
    // Start is called before the first frame update
    void Start()
    {
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

}
