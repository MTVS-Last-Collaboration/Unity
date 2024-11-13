using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//이 클래스는 UI를 켜고끄는걸 담당합니다.
public class LoginUI : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject startImg;
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
    public Image imgStartButton;

    void Start()
    {
        imgMoodChoiceBlackBg = GameObject.Find("Img_MoodChoiceBlackBG");
        // 버튼에 리스너 추가
        //btnStartButton.onClick.AddListener(OnButtonPress);
    }
    /*void Update()
    {
        
    }*/
    /*
     * public void OnPointerEnter(PointerEventData eventData)
    {
        //throw new System.NotImplementedException()
        imgStartButton.sprite = loginImageArray[1];
        print(11111);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
        imgStartButton.sprite = loginImageArray[0];
        print(00000);

    }

    */




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


    public void OffStartImage()
    {
        hoonSoundManagerLogin.PlaySound(0); //buttonSound
        startImg.SetActive(false);
       
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
    
    }

    public void CloseUI(GameObject objecName)
    {
        hoonSoundManagerLogin.PlaySound(0);
        objecName.SetActive(false);
        print("ObjectName" + objecName.name);

    }
        
}

   

