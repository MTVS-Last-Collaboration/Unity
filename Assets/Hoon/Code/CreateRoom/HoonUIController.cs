using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class HoonUIController : MonoBehaviour
{
    public bool isMyMarkObject = false;
    public bool isGetMarkObject = false;
    public bool isShareMarkObejct = false;
    public bool isOptionButton = false;
    public RectTransform imgOptionPanelObject; //옵션패널
    public Image Img_OptionButton;
    public Sprite[] LobbySprites;
    

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

    public void OptionPanelControll()
    {
        isOptionButton = !isOptionButton;

        if (isOptionButton)
        {
            Img_OptionButton.sprite = LobbySprites[0];
            StartCoroutine(OpenOptionPanel());           
        }
        else
        {
            Img_OptionButton.sprite = LobbySprites[1];
            StartCoroutine(CloseOptionPanel());
        }
    }

    IEnumerator OpenOptionPanel()
    {
        Vector3 startPos;
        Vector3 targetPos = new Vector3(975, 165, 0);
        float duration = 1f;
        float currentTime = 0f;

        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            //패널열기
            imgOptionPanelObject.anchoredPosition = Vector3.Lerp(imgOptionPanelObject.anchoredPosition, targetPos, currentTime / duration);
            yield return null;

        }
        imgOptionPanelObject.anchoredPosition = targetPos;

    }

    IEnumerator CloseOptionPanel()
    {
        Vector3 startPos;
        Vector3 targetPos = new Vector3(1305, 165, 0);
        float duration = 1f;
        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            //패널열기
            imgOptionPanelObject.anchoredPosition = Vector3.Lerp(imgOptionPanelObject.anchoredPosition, targetPos, currentTime / duration);
            yield return null;

        }
        imgOptionPanelObject.anchoredPosition = targetPos;

    }


}// 클래스끝 
