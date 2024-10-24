using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_ChatItem : MonoBehaviour
{
    TMP_Text chatText;

    private void Awake()
    {
        chatText = GetComponent<TMP_Text>();
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string msg, string chatColor)
    {
        if(chatColor != "Black")
        {
            chatText.text = "µø±€¿Ã : " + msg;
            chatText.color = new Color(20, 20, 20);
        }
        else
        {
            chatText.text = msg;
            chatText.color = Color.black;
        }
       
    }
}
