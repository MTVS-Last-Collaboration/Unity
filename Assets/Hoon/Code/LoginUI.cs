using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    public GameObject startImg;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OffStartImage()
    {
        startImg.SetActive(false);
    }

    public void CloseUI(GameObject objecName)
    {
        objecName.SetActive(false);
    }

  
        
}
