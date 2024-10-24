using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSW_UIManager : MonoBehaviour
{
    // 캐싱 값 가지고 있는 객체 (저장 매체)
    // 코드 몰아 넣는 객체 인스턴스 하나 (코드 실행 매체)

    public GameObject album_UI;
    public GameObject Album2;
    public GameObject Album_Loading;
    public GameObject PicUploadingUI;
    public GameObject Calender1;
    public GameObject Calender2;
    public GameObject Mong_1;
    public GameObject Mong_Chat_2;
    public GameObject DecorateShopUI;
    public GameObject DecorateMineUI;


    // Start is called before the first frame update
    void Start()
    {
        album_UI = GameObject.Find("UI_Album");
        Album2 = GameObject.Find("Album2");
        Album_Loading = GameObject.Find("Album_Loading");
        PicUploadingUI = GameObject.Find("PicUploadingUI");

        Calender1 = GameObject.Find("Calender1");
        Calender2 = GameObject.Find("Calender2");

        Mong_1 = GameObject.Find("Mong_1");
        Mong_Chat_2 = GameObject.Find("Mong_Chat_2");

        DecorateShopUI = GameObject.Find("DecorateShopUI_All");
        DecorateMineUI = GameObject.Find("DecorateMineUI_All");

        AllActiveFasle();
    }

    void AllActiveFasle()
    {
        album_UI.SetActive(false);
        Album2.SetActive(false);
        Album_Loading.SetActive(false);
        PicUploadingUI.SetActive(false);
        Calender1.SetActive(false);
        Calender2.SetActive(false);
        Mong_1.SetActive(false);
        Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 0;
        //Mong_Chat_2.SetActive(false);
        DecorateShopUI.SetActive(false);
        DecorateMineUI.SetActive(false);
    }

    // Album과 관련된 코드

    public void OnClickAlbum()
    {
        album_UI.SetActive(true);
    }
    public void OnClickAlbum_Back()
    {
        album_UI.SetActive(false);
    }


    public void OnClickAlbum_Making()
    {
        album_UI.SetActive(false);
        Album2.SetActive(true);
    }
    public void OnClickAlbum_Making_Back()
    {
        Album2.SetActive(false);
        album_UI.SetActive(true);
    }


    public void OnClickAlbum_Loading()
    {
        Album2.SetActive(false);
        Album_Loading.SetActive(true);
    }
    public void OnClickAlbum_Loading_Back()
    {
        Album_Loading.SetActive(false);
        Album2.SetActive(true);
    }

    public void OnClickPicUploadingUI()
    {
        //Album2.SetActive(false);
        PicUploadingUI.SetActive(true);
    }
    public void OnClickPicUploadingUI_Back()
    {
        PicUploadingUI.SetActive(false);
        //Album2.SetActive(true);
    }

    // Calender와 관련된 코드
    public void OnClickCalender()
    {
        Calender1.SetActive(true);
        Calender2.SetActive(true);
    }


    public void OnClickCalender_Back()
    {
        Calender1.SetActive(false);
        Calender2.SetActive(false);
    }

    // Mong과 관련된 코드
    public void OnClickMong()
    {
        Mong_1.SetActive(true);
    }
    public void OnClickMong_Back()
    {
        Mong_1.SetActive(false);
    }


    public void OnClickMongChat()
    {
        Mong_1.SetActive(false);
        //Mong_Chat_2.SetActive(true);
        Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void OnClickMongChat_Back()
    {
        Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 0;
        //Mong_Chat_2.SetActive(false);
        Mong_1.SetActive(true);
    }

    public void OnClickDecorateShopUI()
    {
        DecorateShopUI.SetActive(true);
        DecorateMineUI.SetActive(false);
    }

    public void OnClickDecorateShopUI_Back()
    {
        DecorateShopUI.SetActive(false);
    }

    public void OnClickDecorateMineUI()
    {
        DecorateMineUI.SetActive(true);
        DecorateShopUI.SetActive(false);
    }

    public void OnClickDecorateMineUI_Back()
    {
        DecorateMineUI.SetActive(false);
    }
}
