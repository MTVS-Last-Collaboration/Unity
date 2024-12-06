using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HoonTutorialUI : MonoBehaviour
{
    //변경되지 않음
    public HoonSoundManagerLogin hoonSoundManager;
    public TextMeshProUGUI text_LobbyTutorialNumber;
    public GameObject[] allTutotialImageObject;

    //변경되는값
    int tutorialCount = 1;
    void Start()
    {
        
    }

    /* void Update()
     {

     }*/

    public void OpenUI(GameObject obj)
    {
        hoonSoundManager.PlaySound(0);
        obj.SetActive(true);

    }

    public void CloseUI(GameObject obj)
    {
        hoonSoundManager.PlaySound(1);
        obj.SetActive(false);
    }


    //오른쪽 버튼 누르면 폰트, 이미지 바꾸기

    public void IncreaseTotutialNumberFont()
    {
        //print("숫자증가하기");
        //캐싱해오기
        if (tutorialCount == 5)
        {
            print(1111);
            allTutotialImageObject[tutorialCount-1].SetActive(false);
            tutorialCount = 1;
            text_LobbyTutorialNumber.text = tutorialCount.ToString() + " " + "/" + " " + "5";
            allTutotialImageObject[tutorialCount-1].SetActive(true);
        }
        else if(tutorialCount > 1)
        {
            print(2222);
            allTutotialImageObject[tutorialCount - 1].SetActive(false);
            tutorialCount++;
            text_LobbyTutorialNumber.text = tutorialCount.ToString() + " " + "/" + " " + "5";
            allTutotialImageObject[tutorialCount - 1].SetActive(true);
        }
        else //1일때 들어옴
        {
            print(3333);
            allTutotialImageObject[tutorialCount - 1].SetActive(false);
            tutorialCount++;
            text_LobbyTutorialNumber.text = tutorialCount.ToString() + " "+"/" + " "+ "5";
            allTutotialImageObject[tutorialCount - 1].SetActive(true);
        }
        
        
    }

    public void DecreaseTotutialNumberFont()
    {
        //print("숫자증가하기");
        //캐싱해오기
        if (tutorialCount == 1)
        {
            print(1111);
            allTutotialImageObject[tutorialCount -1].SetActive(false);
            tutorialCount = 5;
            text_LobbyTutorialNumber.text = tutorialCount.ToString() + " " + "/" + " " + "5";
            allTutotialImageObject[tutorialCount -1].SetActive(true);
        }
        else if (tutorialCount > 1)
        {
            print(2222);
            allTutotialImageObject[tutorialCount - 1].SetActive(false);
            tutorialCount--;
            text_LobbyTutorialNumber.text = tutorialCount.ToString() + " " + "/" + " " + "5";
            allTutotialImageObject[tutorialCount - 1].SetActive(true);
        }
        /*else
        {
            print(3333);
            allTutotialImageObject[tutorialCount - 1].SetActive(false);
            
            tutorialCount--;
            text_LobbyTutorialNumber.text = tutorialCount.ToString() + " " + "/" + " " + "5";
            allTutotialImageObject[tutorialCount + 1].SetActive(true);
        }*/


    }

}
