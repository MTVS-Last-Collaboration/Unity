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
    public RectTransform Img_SoundSwitchObject;
    public RectTransform Img_BgmSwitchObejct;
    bool isSoundSwitch = true;
    bool isBGMSwitch = true;
    public Image btnSound_ImageComp;
    public Image btnBgm_ImageComp;
    public AudioSource lobbyAudioSourceBGM;
    public AudioSource lobbyAudioSourceSoundEffect;
    public HoonSoundManagerLogin hoonSoundManager;
    public GameObject Img_OptionMenuObject;
    public GameObject img_PlayEnd;

    HoonChoiceRoom hoonChiceRoom;
    void Start()
    {
        Img_OptionMenuObject.SetActive(false);
        img_PlayEnd.SetActive(false);

        // 특정 컴포넌트를 찾기
        HoonChoiceRoom[] foundComponents = FindObjectsOfType<HoonChoiceRoom>();

        foreach (HoonChoiceRoom component in foundComponents)
        {
            // 컴포넌트를 가진 오브젝트
            GameObject currentObject = component.gameObject;

            // 부모 오브젝트 확인
            if (currentObject.transform.parent != null)
            {
                print($"오브젝트 '{currentObject.name}'의 부모는 '{currentObject.transform.parent.name}'입니다.");
            }
            else
            {
                print($"오브젝트 '{currentObject.name}'는 부모가 없습니다. (최상위 오브젝트)");
            }


        }
    }
    
    // Update is called once per frame
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
            hoonSoundManager.PlaySound(0);
            Img_OptionButton.sprite = LobbySprites[0];//이미지변경
            StartCoroutine(OpenOptionPanel());           
        }
        else
        {
            hoonSoundManager.PlaySound(1);
            Img_OptionButton.sprite = LobbySprites[1];//이미지변경
            StartCoroutine(CloseOptionPanel());
        }
    }

    IEnumerator OpenOptionPanel()
    {
        Vector3 startPos;
        Vector3 targetPos = new Vector3(1005, 165, 0);
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
        Vector3 targetPos = new Vector3(1350, 165, 0);
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

    public void MoveSoundSwitch()
    {
        isSoundSwitch = !isSoundSwitch; //참거짓교환

        if (isSoundSwitch)
        {
            lobbyAudioSourceSoundEffect.enabled = true;
            hoonSoundManager.PlaySound(0);
            Img_SoundSwitchObject.anchoredPosition = new Vector3(20,0,0); //스위치이동
            btnSound_ImageComp.sprite = LobbySprites[2]; //이미지변경
        }
        else
        {

            hoonSoundManager.PlaySound(1);
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
            hoonSoundManager.PlaySound(0);
            lobbyAudioSourceBGM.enabled = true; //사운드켜기
            Img_BgmSwitchObejct.anchoredPosition = new Vector3(20, 0, 0); //스위치이동
            btnBgm_ImageComp.sprite = LobbySprites[2]; //이미지변경
        }
        else
        {
            hoonSoundManager.PlaySound(1);
            lobbyAudioSourceBGM.enabled = false; //사운드끄기
            Img_BgmSwitchObejct.anchoredPosition = new Vector3(-20, 0, 0); //스위치이동
            btnBgm_ImageComp.sprite = LobbySprites[3]; //이미지변경

        }

    }
    
    bool isButtonSound = false;
    public void ButtonSoundTest()
    {
        hoonSoundManager.PlaySound("hoonAudioClipArray", 0);//버튼테스트

    }

}// 클래스끝 
