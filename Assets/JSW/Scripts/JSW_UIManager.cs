using System.Collections;
using System.Collections.Generic;
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
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ImageSound);
        album_UI.SetActive(true);
        easingUI(album_UI, 1f);
        easingUIDark(PlayerInfoUI, 0f);
    }
    public void OnClickAlbum_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        album_UI.SetActive(false);
        easingUIDark(PlayerInfoUI, 1f);
    }



    public void OnClickAlbum_Making()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        album_UI.SetActive(false);
        Album2.SetActive(true);

        easingUI(Album2, 0.5f);
    }
    public void OnClickAlbum_Making_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        Album2.SetActive(false);
        album_UI.SetActive(true);

        easingUI(album_UI, 1f);
    }

    public void OnClickAlbum_Loading0()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //dasdsa
        Album2.SetActive(false);
        Album_Loading2.SetActive(true);
    }
    public void OnClickAlbum_Loading0_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //dasdsa
        Album2.SetActive(true);
        Album_Loading2.SetActive(false);
    }


    public void OnClickAlbum_Loading()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //dasdsa
        cameraControllTest.CameraTo3D();
        Album_Loading2.SetActive(false);
        Album_Loading.SetActive(true);
        StartCoroutine(OnClickAlbum_Loading_Back());
    }

    IEnumerator OnClickAlbum_Loading_Back()
    {
        yield return new WaitForSeconds(3f);
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_To3D);
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
    }


    public void OnClickCalender_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        easingUIDark(PlayerInfoUI, 1f);
        Calender1.SetActive(false);
        Calender2.SetActive(false);
    }

    // Mong과 관련된 코드
    public void OnClickMong()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_PetSound);
        easingUIDark(PlayerInfoUI, 0f);
        Mong_1.SetActive(true);
    }
    public void OnClickMong_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        easingUIDark(PlayerInfoUI, 1f);
        Mong_1.SetActive(false);
    }


    public void OnClickMongChat()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        Mong_1.SetActive(false);
        Mong_Chat_2.SetActive(true);
        easingUI(Mong_Chat_2, 2.0f);
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 1;
    }
    public void OnClickMongChat_Back()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound1);
        //Mong_Chat_2.GetComponent<CanvasGroup>().alpha = 0;
        Mong_Chat_2.SetActive(false);
        Mong_1.SetActive(true);
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
        Vector3 targetPos = new Vector3(1824, -265, 0);
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

    IEnumerator CloseOptionPanel()
    {
        Vector3 targetPos = new Vector3(2105, -265, 0);
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

    bool isSoundSwitch = true;
    bool isBGMSwitch = true;
    public AudioSource lobbyAudioSourceBGM;
    public AudioSource lobbyAudioSourceSoundEffect;
    public GameObject Img_OptionMenuObject;
    public GameObject img_PlayEnd;
    public Image btnSound_ImageComp;
    public Image btnBgm_ImageComp;

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


}

