using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//이 클래스는 UI를 켜고끄는걸 담당합니다.
public class LoginUI : MonoBehaviour
{
    public GameObject startImg;
    public GameObject loginImg;
    public TextMeshProUGUI noticeConnect;

    void Start()
    {
        
    }

    /*void Update()
    {
        
    }*/

    public void OffStartImage()
    {
        startImg.SetActive(false);
    }

    public void OpenUI(GameObject obj)
    {
        obj.SetActive(true);
        
        if(obj.name == "Img_NewRegistMenu1")
        {

        }
    
    }

    public void CloseUI(GameObject objecName)
    {
        objecName.SetActive(false);

    }

    
  
        
}
