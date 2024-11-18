using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoonUIController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    /* void Update()
     {

     }*/

    public void OpenUI(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void CloseUI(GameObject obj)
    {
        obj.SetActive(false);
    }
}// 클래스끝 
