using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.UIElements;

//이 클래스는 UI를 켜고끄는걸 담당합니다.
public class LoginUI : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    //스타트 이미지
    public GameObject startImg;
    public Image imgStartLogo;
    public Image imgStartBG;
    //스타트 버튼
    public Image imgStartButton;
    public Image imgStartButtonLogo;
    //로그인 이미지
    public GameObject loginImg;



    public TextMeshProUGUI noticeConnect;
    public GameObject registMenu;
    public GameObject registMenu1;
    public GameObject registMenu2;
    public GameObject registMenu3;
    public GameObject registMenuAll;
    GameObject imgMoodChoiceBlackBg;
    public HoonSoundManagerLogin hoonSoundManagerLogin;
    public Sprite[] loginImageArray;
    public Button btnStartButton;


    //pingPongStartLogo
    public float scaleSpeed = 0.5f; // 크기 변경 속도
    public float minScale = 1.0f; // 최소 크기
    public float maxScale = 1.2f; // 최대 크기

    void Start()
    {
        imgMoodChoiceBlackBg = GameObject.Find("Img_MoodChoiceBlackBG");
        #if UNITY_STANDALONE_WIN
            Screen.SetResolution(2340, 1080, true); // false는 창 모드
        #endif
        // 버튼에 리스너 추가
        //btnStartButton.onClick.AddListener(OnButtonPress);
    }
    void Update()
    {
        //로고가 null일때 미싱행, 로고가 있을때 계속 움직이게 하기
        if(imgStartLogo == null)
        {
            return;
            print("스케일 중단이야");
        }
        else
        {

            // Mathf.PingPong을 사용해 크기를 변화 (t * length), Mathf.PingPong(float t, float length)
            //t: 시간 또는 입력 값. 보통 Time.time이나 Time.timeSinceLevelLoad를 사용합니다.
            //length: 값이 반복되는 주기의 범위(0부터 이 값까지 반복)
            //Mathf.PingPong(시간,변화량)+최소값)

            //float scale = Mathf.PingPong(Time.time * scaleSpeed, maxScale - minScale) + minScale;
            //Vector3 scale =new Vector3( Mathf.PingPong(Time.time , minScale), Mathf.PingPong(Time.time, maxScale), 1f );// + minScale;
            Vector3 scale1 = new Vector3(Mathf.PingPong(Time.time * scaleSpeed, maxScale) + minScale, Mathf.PingPong(Time.time * scaleSpeed, maxScale) + minScale, 1.0f);
            imgStartLogo.rectTransform.localScale = scale1;

        }

    }
    
    public void OnButtonPress()
    {
        StartCoroutine(ChangeSpriteOnPress());
    }

    IEnumerator ChangeSpriteOnPress()
    {
        //버튼을 가져오고 가져온 버튼 오브젝트의 프레스 이미지를 바꿔주자.
        imgStartButton.sprite = loginImageArray[1];
        yield return new WaitForSeconds(0.1f);
        imgStartButton.sprite = loginImageArray[0];

    }

    public void OffStartImage() //스타트이미지를 끄자.
    {
        hoonSoundManagerLogin.PlaySound(0); //buttonSound

        //코루틴으로 알파값 낮추기(최종알파값, 지속시간)
        StartCoroutine(FadeToStartImage(0f, 2f));

        //스타트 로고에 알파값을 러프로 조정하자.
        //imgStartLogo.GetComponent<Image>().color = new Color(1, 1, 1, 1);

        //스타트 배경의 알파값을 러프로 조정
        



    }

    IEnumerator FadeToStartImage(float targetAlpha, float duration)
    {
        Color currentColor = imgStartLogo.color; // 현재 색상 가져오기
        float startAlpha = currentColor.a; // 시작 알파 값
        float currentTime = 0f;

        while (currentTime < duration) //지속시간보다 커질때까지.
        {
            currentTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
            imgStartLogo.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            imgStartBG.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            imgStartButton.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            imgStartButtonLogo.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null; // 다음 프레임까지 대기
        }

        // 최종 알파 값 설정
        imgStartLogo.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        imgStartButton.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        imgStartButtonLogo.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        startImg.SetActive(false); //이미지를 끄자.
    }

    public void OpenUI(GameObject obj)
    {
        hoonSoundManagerLogin.PlaySound(1);
        obj.SetActive(true);
        
        if(obj.name == "Img_NewRegistMenu1")
        {

        }
        if(obj.name == "LoginMenuBG")
        {
            registMenuAll.SetActive(true);

        }
        if(obj.name == "LoginBG")
        {

        }

    }

    public void CloseUI(GameObject objecName)
    {
        hoonSoundManagerLogin.PlaySound(0);
        objecName.SetActive(false);
        print("ObjectName" + objecName.name);

    }
        
}

   

