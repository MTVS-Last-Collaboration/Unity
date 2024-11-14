using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoonCoupleMenuManager : MonoBehaviour
{
    public GameObject CoupleMenu;
    HoonSoundManagerLogin soundMgr;

    // Start is called before the first frame update
    void Start()
    {
        soundMgr = transform.GetComponent<HoonSoundManagerLogin>();
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void ViewCoupleMenuControll()
    {
        soundMgr.PlaySound(0);
            
        CoupleMenu.SetActive(false);
    }

}
