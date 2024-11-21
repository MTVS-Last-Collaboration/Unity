using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class HoonUIController : MonoBehaviour
{
    public bool isMyMarkObject = false;
    public bool isGetMarkObject = false;
    public bool isShareMarkObejct = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /* void Update()
     {

     }*/

    public void OpenUI(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void CloseUI(GameObject obj)
    {
        obj.SetActive(false);
    }
    public void ViewMarkObejct(GameObject obj)
    {
        if (obj.name == "MyStorageMark")
        {
            isMyMarkObject = !isMyMarkObject;
            if (isMyMarkObject)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false); //print("false" + obj);

            }
        }
        else if(obj.name == "GetRoomMark")
        {
            isGetMarkObject = !isGetMarkObject;
            if (isGetMarkObject)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false); print("false" + obj);

            }
        }
        else if (obj.name == "MyStorageMark")
        {
            isShareMarkObejct = !isShareMarkObejct;
            if (isShareMarkObejct)
            {
                obj.SetActive(true);
            }
            else
            {
                obj.SetActive(false); print("false" + obj);

            }
        }


         
    }


}// 클래스끝 
