using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements.Experimental;

public class JSW_UIManager : MonoBehaviour
{
    // 캐싱 값 가지고 있는 객체 (저장 매체)
    // 코드 몰아 넣는 객체 인스턴스 하나 (코드 실행 매체)

    public JSW_CameraControllTest cameraControllTest;

    public GameObject album_UI;
    public GameObject Album2;
    public GameObject Album_Loading;
    public GameObject Album_Loading2;
    public GameObject PicUploadingUI;
    public GameObject Calender;
    public GameObject Calender1;
    public GameObject Calender2;
    public GameObject Mong_1;
    public GameObject Mong_Chat_2;
    public GameObject DecorateShopUI;
    public GameObject DecorateMineUI;

    public GameObject PlayerInfoUI;
    public GameObject heartInfoUI;

    CanvasGroup playerInfo;
    CanvasGroup heartInfo;
    public float time=1;
    public float heartTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        album_UI = GameObject.Find("UI_Album");
        Album2 = GameObject.Find("Album2");
        Album_Loading2 = GameObject.Find("Album_Loading2");
        Album_Loading = GameObject.Find("Album_Loading");
        PicUploadingUI = GameObject.Find("PicUploadingUI");

        Calender = GameObject.Find("Calender");
        Calender1 = GameObject.Find("Calender1");
        Calender2 = GameObject.Find("Calender2");

        Mong_1 = GameObject.Find("Mong_1");
        Mong_Chat_2 = GameObject.Find("Mong_Chat_2");

        DecorateShopUI = GameObject.Find("DecorateShopUI_All");
        DecorateMineUI = GameObject.Find("DecorateMineUI_All");

        PlayerInfoUI = GameObject.Find("PlayerInfoUI");

        playerInfo = PlayerInfoUI.GetComponent<CanvasGroup>();
        heartInfo = heartInfoUI.GetComponent<CanvasGroup>();
        AllActiveFalse();
    }

    private void Update()
    {
        playerInfo.alpha = Mathf.Lerp(playerInfo.alpha, time, Time.deltaTime * 5);
        heartInfo.alpha = Mathf.Lerp(heartInfo.alpha, heartTime, Time.deltaTime * 5);
    }
    void AllActiveFalse()
    {
        album_UI.SetActive(false);
        Album2.SetActive(false);
        Album_Loading2.SetActive(false);
        Album_Loading.SetActive(false);
        PicUploadingUI.SetActive(false);
        Calender1.SetActive(false);
        Calender2.SetActive(false);
        Mong_1.SetActive(false);
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 0;
        Mong_Chat_2.SetActive(false);
        DecorateShopUI.SetActive(false);
        DecorateMineUI.SetActive(false);
    }

    // Album과 관련된 코드

    public void OnClickAlbum()
    {
        album_UI.SetActive(true);
        easingUI(album_UI, 1f);
        easingUIDark(PlayerInfoUI, 0f);
    }
    public void OnClickAlbum_Back()
    {
        album_UI.SetActive(false);
        easingUIDark(PlayerInfoUI, 1f);
    }



    public void OnClickAlbum_Making()
    {
        album_UI.SetActive(false);
        Album2.SetActive(true);

        easingUI(Album2, 0.5f);
    }
    public void OnClickAlbum_Making_Back()
    {
        Album2.SetActive(false);
        album_UI.SetActive(true);

        easingUI(album_UI, 1f);
    }

    public void OnClickAlbum_Loading0()
    {
        //dasdsa
        Album2.SetActive(false);
        Album_Loading2.SetActive(true);
    }
    public void OnClickAlbum_Loading0_Back()
    {
        //dasdsa
        Album2.SetActive(true);
        Album_Loading2.SetActive(false);
    }


    public void OnClickAlbum_Loading()
    {
        //dasdsa
        cameraControllTest.CameraTo3D();
        Album_Loading2.SetActive(false);
        Album_Loading.SetActive(true);
        StartCoroutine(OnClickAlbum_Loading_Back());
    }

    IEnumerator OnClickAlbum_Loading_Back()
    {
        float time = 0;
        while (true)
        {
            if(time >= 5)
            {
                break;
            }
            Album_Loading.GetComponent<CanvasGroup>().alpha= Mathf.Lerp(Album_Loading.GetComponent<CanvasGroup>().alpha,0,Time.deltaTime * 0.5f);
            time += Time.deltaTime;
            yield return null;
        }

        cameraControllTest.CameraToAlbum();
        Album_Loading.GetComponent<CanvasGroup>().alpha = 1f;
        Album_Loading.SetActive(false);
        album_UI.SetActive(true);
        easingUI(Album2, 1f);
    }

    //public void OnClickAlbum_Loading_Back()
    //{
    //    ///fadsa
    //    cameraControllTest.CameraToAlbum();
    //    Album_Loading.GetComponent<CanvasGroup>().alpha = 1f;
    //    Album_Loading.SetActive(false);
    //    Album_Loading2.SetActive(true);
    //    easingUI(Album2, 1f);
    //}


    public void OnClickPicUploadingUI()
    {
        //Album2.SetActive(false);
        PicUploadingUI.SetActive(true);

        easingUI(PicUploadingUI, 0.5f);
    }

    public void OnClickPicUploadingUI_Back()
    {
        PicUploadingUI.SetActive(false);
        //Album2.SetActive(true);
    }

    // Calender와 관련된 코드
    public void OnClickCalender()
    {
        easingUIDark(PlayerInfoUI, 0f);
        easingUI(Calender, 1.0f);
        Calender1.SetActive(true);
        Calender2.SetActive(true);
    }


    public void OnClickCalender_Back()
    {
        easingUIDark(PlayerInfoUI, 1f);
        Calender1.SetActive(false);
        Calender2.SetActive(false);
    }

    // Mong과 관련된 코드
    public void OnClickMong()
    {
        easingUIDark(PlayerInfoUI, 0f);
        Mong_1.SetActive(true);
    }
    public void OnClickMong_Back()
    {
        easingUIDark(PlayerInfoUI, 1f);
        Mong_1.SetActive(false);
    }


    public void OnClickMongChat()
    {
        Mong_1.SetActive(false);
        Mong_Chat_2.SetActive(true);
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void OnClickMongChat_Back()
    {
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 0;
        Mong_Chat_2.SetActive(false);
        Mong_1.SetActive(true);
    }

    public void OnClickDecorateShopUI()
    {
        heartTime = 0;
        DecorateShopUI.SetActive(true);
        DecorateMineUI.SetActive(true);
    }

    public void OnClickDecorateShopUI_Back()
    {
        heartTime = 1;
        DecorateShopUI.SetActive(false);
        DecorateMineUI.SetActive(false);
    }

    public void OnClickDecorateMineUI()
    {
        heartTime = 0;
        DecorateMineUI.SetActive(true);
        DecorateShopUI.SetActive(false);
    }

    public void OnClickDecorateMineUI_Back()
    {
        heartTime = 1;
        DecorateMineUI.SetActive(false);
    }

    public void easingUI(GameObject uiObject, float time)
    {
        Vector3 size = uiObject.transform.localScale;
        uiObject.transform.localScale = Vector3.one * 0.2f;
        iTween.ScaleTo(uiObject, iTween.Hash(
        "scale", size, // 최종 크기
        "time", time,         // 애니메이션 시간
        "easetype", iTween.EaseType.easeOutElastic // 애니메이션 타입
        ));
    }
    public void easingUIDark(GameObject uiObject, float time)
    {
        this.time = time;
    }
}
