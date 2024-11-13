using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MailHistoryManager : MonoBehaviour
{
    public int buttonNumber = 0;

    GameObject mailBox;
    MailManager mailManager;

    // Start is called before the first frame update
    void Start()
    {
        mailBox = GameObject.Find("MailBoxHoon");
        if (mailBox != null )
        {
            //print("find MailBoxObjectManager");
            mailManager = mailBox.GetComponent<MailManager>();
            if( mailManager != null )
            {
                //Debug.LogError("find MailManager");
            }
   
        }
       
    }

    // Update is called once per frame
   /* void Update()
    {
        
    }*/
}
