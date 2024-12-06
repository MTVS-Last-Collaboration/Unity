using ExitGames.Client.Photon.StructWrapping;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
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
    public GameObject playerContorlUI;
    public GameObject delete3D;

    public GameObject mongBackground1;
    public GameObject mongBackground2;

    public GameObject AlbumBackground1;
    public GameObject AlbumBackground2;
    public GameObject AlbumBackground3;

    public GameObject CalenderBackground1;



    CanvasGroup playerInfo;
    CanvasGroup heartInfo;
    public float time = 1;
    public float heartTime = 1;

    public bool isOpening;

    // Start is called before the first frame update
    void Start()
    {
        album_UI = GameObject.Find("UI_Album");
        Album2 = GameObject.Find("Album2");
        playerContorlUI = GameObject.Find("PlayControlCanvas");
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
        delete3D = GameObject.Find("3DDeleteButton");

        playerInfo = PlayerInfoUI.GetComponent<CanvasGroup>();
        heartInfo = heartInfoUI.GetComponent<CanvasGroup>();

        StartCoroutine(Opening());
        AllActiveFalse();
    }

    public GameObject openingObject;

    public IEnumerator Opening(){
        openingObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        openingObject.transform.GetChild(4).gameObject.SetActive(false);

        iTween.ScaleTo(openingObject, iTween.Hash(
            "scale", Vector3.one*80,        // 목표 스케일 (1, 1, 1)
            "time", 1f,                // 애니메이션 시간 (조정 가능)
            "easeType", "easeInCirc", // 통통 튀는 느낌의 easeType
            "oncomplete", "OnCompleteOpening", // 애니메이션 완료 시 호출할 함수
            "oncompletetarget", gameObject
        ));
    }

    public void OnCompleteOpening()
    {
        isOpening = true;
        openingObject.SetActive(false);
    }

    public IEnumerator Closing()
    {
        yield return new WaitForSeconds(1f);
    }





    private void Update()
    {
        if (isOpening)
        {
            playerInfo.alpha = Mathf.Lerp(playerInfo.alpha, time, Time.deltaTime * 5);
            heartInfo.alpha = Mathf.Lerp(heartInfo.alpha, heartTime, Time.deltaTime * 5);
        }
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
        delete3D.SetActive(false);
        mongBackground1.SetActive(false);
        mongBackground2.SetActive(false);
        AlbumBackground1.SetActive(false);
        AlbumBackground2.SetActive(false);
        //      AlbumBackground3.SetActive(false); //  직접 자식으로 넣음
        CalenderBackground1.SetActive(false);
    }

    // Album과 관련된 코드

    public void OnClickAlbum()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ImageSound);
        album_UI.SetActive(true);
        AlbumBackground1.SetActive(true);
        delete3D.SetActive(false);
        easingUI(album_UI, 1f);
        easingUIDark(PlayerInfoUI, 0f);
    }
    public void OnClickAlbum_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        album_UI.SetActive(false);
        AlbumBackground1.SetActive(false);
        easingUIDark(PlayerInfoUI, 1f);
    }



    public void OnClickAlbum_Making()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        album_UI.SetActive(false);
        Album2.SetActive(true);

        easingUI(Album2, 0.5f);

        //AlbumBackground1.SetActive(false);
        //AlbumBackground2.SetActive(true);
    }
    public void OnClickAlbum_Making_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        Album2.SetActive(false);
        album_UI.SetActive(true);

        easingUI(album_UI, 1f);

        //AlbumBackground1.SetActive(true);
        //AlbumBackground2.SetActive(false);
    }

    public void OnClickAlbum_Loading0()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //dasdsa
        Album2.SetActive(false);
        Album_Loading2.SetActive(true);

        AlbumBackground1.SetActive(false);
        AlbumBackground2.SetActive(true);
    }
    public void OnClickAlbum_Loading0_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //dasdsa
        Album2.SetActive(true);
        Album_Loading2.SetActive(false);

        AlbumBackground1.SetActive(true);
        AlbumBackground2.SetActive(false);
    }


    public void OnClickAlbum_Loading()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //dasdsa
        cameraControllTest.CameraTo3D();
        Album_Loading2.SetActive(false);
        Album_Loading.SetActive(true);
        StartCoroutine(OnClickAlbum_Loading_Back());

        AlbumBackground1.SetActive(false);
        AlbumBackground2.SetActive(true);
    }

    IEnumerator OnClickAlbum_Loading_Back()
    {
        yield return new WaitForSeconds(2f);
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_To3D);
        float time = 0;
        while (true)
        {
            if(time >= 2f)
            {
                break;
            }
            Album_Loading.GetComponent<CanvasGroup>().alpha= Mathf.Lerp(Album_Loading.GetComponent<CanvasGroup>().alpha,0,Time.deltaTime * 1.5f);
            time += Time.deltaTime;
            yield return null;
        }

        cameraControllTest.CameraToAlbum();
        Album_Loading.GetComponent<CanvasGroup>().alpha = 1f;
        Album_Loading.SetActive(false);
        album_UI.SetActive(true);

        OnClickAlbum_Making_Back();

        AlbumBackground1.SetActive(true);
        AlbumBackground2.SetActive(false);
        //easingUI(Album2, 1f);
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
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //Album2.SetActive(false);
        PicUploadingUI.SetActive(true);

        easingUI(PicUploadingUI, 0.5f);
    }

    public void OnClickPicUploadingUI_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        PicUploadingUI.SetActive(false);
        //Album2.SetActive(true);
    }

    // Calender와 관련된 코드
    public void OnClickCalender()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ImageSound);
        easingUIDark(PlayerInfoUI, 0f);
        easingUI(Calender, 1.0f);
        Calender1.SetActive(true);
        Calender2.SetActive(true);
        CalenderBackground1.SetActive(true);
    }


    public void OnClickCalender_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        easingUIDark(PlayerInfoUI, 1f);
        Calender1.SetActive(false);
        Calender2.SetActive(false);
        CalenderBackground1.SetActive(false);
    }

    // Mong과 관련된 코드
    public void OnClickMong()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_PetSound);
        easingUIDark(PlayerInfoUI, 0f);
        playerContorlUI.SetActive(false);
        Mong_1.SetActive(true);
    }
    public void OnClickMong_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        easingUIDark(PlayerInfoUI, 1f);
        Mong_1.SetActive(false);
        playerContorlUI.SetActive(true);
    }


    public void OnClickMongChat()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        Mong_1.SetActive(false);
        Mong_Chat_2.SetActive(true);
        easingUI(Mong_Chat_2, 2.0f);
        mongBackground1.SetActive(true);
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void OnClickMongChat_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 0;
        Mong_Chat_2.SetActive(false);
        Mong_1.SetActive(true);
        mongBackground1.SetActive(false);
    }

    public void OnClickDecorateShopUI()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        heartTime = 0;
        DecorateShopUI.SetActive(true);
        DecorateMineUI.SetActive(true);
    }

    public void OnClickDecorateShopUI_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        heartTime = 1;
        DecorateShopUI.SetActive(false);
        DecorateMineUI.SetActive(false);
    }

    public void OnClickDecorateMineUI()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        heartTime = 0;
        DecorateMineUI.SetActive(true);
        DecorateShopUI.SetActive(false);
    }

    public void OnClickDecorateMineUI_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
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



    public bool isOptionButton = false;
    public RectTransform imgOptionPanelObject; //옵션패널
    public Image Img_OptionButton;
    public Sprite[] LobbySprites;
    public RectTransform Img_SoundSwitchObject;
    public RectTransform Img_BgmSwitchObejct;


    public void OptionPanelControll()
    {
        isOptionButton = !isOptionButton;

        if (isOptionButton)
        {
            //hoonSoundManager.PlaySound(0);
            Img_OptionButton.sprite = LobbySprites[0];//이미지변경
            StartCoroutine(OpenOptionPanel());
        }
        else
        {
            //hoonSoundManager.PlaySound(1);
            Img_OptionButton.sprite = LobbySprites[1];//이미지변경
            StartCoroutine(CloseOptionPanel());
        }
    }

    IEnumerator OpenOptionPanel()
    {
        //Vector3 targetPos = new Vector3(1824, -265, 0);
        Vector3 targetPos = imgOptionPanelObject.anchoredPosition - new Vector2(280, 0);
        float duration = 1f;
        float currentTime = 0f;
        JSW_SoundManager.Get().PlayEftSoundClick1();

        while (currentTime < duration)
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
        //        Vector3 targetPos = new Vector3(2105, -265, 0);
        Vector3 targetPos = imgOptionPanelObject.anchoredPosition + new Vector2(280, 0);
        float duration = 1f;
        float currentTime = 0f;
        JSW_SoundManager.Get().PlayEftSoundClick1();

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            //패널열기
            imgOptionPanelObject.anchoredPosition = Vector3.Lerp(imgOptionPanelObject.anchoredPosition, targetPos, currentTime / duration);
            yield return null;

        }
        imgOptionPanelObject.anchoredPosition = targetPos;

    }

    bool isSoundSwitch = true;
    bool isBGMSwitch = true;
    public AudioSource lobbyAudioSourceBGM;
    public AudioSource lobbyAudioSourceSoundEffect;
    public GameObject Img_OptionMenuObject_Background;
    public GameObject Img_OptionMenuObject;
    public GameObject img_PlayEnd;
    public Image btnSound_ImageComp;
    public Image btnBgm_ImageComp;
    public GameObject SoundManager;

    public void MoveSoundSwitch()
    {
        isSoundSwitch = !isSoundSwitch; //참거짓교환

        if (isSoundSwitch)
        {
            lobbyAudioSourceSoundEffect.enabled = true;
            //hoonSoundManager.PlaySound(0);
            Img_SoundSwitchObject.anchoredPosition = new Vector3(20, 0, 0); //스위치이동
            btnSound_ImageComp.sprite = LobbySprites[2]; //이미지변경
        }
        else
        {

            //hoonSoundManager.PlaySound(1);
            Img_SoundSwitchObject.anchoredPosition = new Vector3(-20, 0, 0); //스위치이동
            btnSound_ImageComp.sprite = LobbySprites[3]; //이미지변경
            lobbyAudioSourceSoundEffect.enabled = false;
        }

    }

    public void MoveBgmSwitch()
    {
        isBGMSwitch = !isBGMSwitch;

        if (isBGMSwitch)
        {
            //hoonSoundManager.PlaySound(0);
            lobbyAudioSourceBGM.enabled = true; //사운드켜기
            Img_BgmSwitchObejct.anchoredPosition = new Vector3(20, 0, 0); //스위치이동
            btnBgm_ImageComp.sprite = LobbySprites[2]; //이미지변경
        }
        else
        {
            //hoonSoundManager.PlaySound(1);
            lobbyAudioSourceBGM.enabled = false; //사운드끄기
            Img_BgmSwitchObejct.anchoredPosition = new Vector3(-20, 0, 0); //스위치이동
            btnBgm_ImageComp.sprite = LobbySprites[3]; //이미지변경

        }

    }

    public void CloseOptionSettings()
    {
        Img_OptionMenuObject_Background.SetActive(false);
        Img_OptionMenuObject.SetActive(false);
    }
    public void OpenOptionSettings()
    {
        Img_OptionMenuObject_Background.SetActive(true);
        Img_OptionMenuObject.SetActive(true);
    }

}

