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

    public void SetText(string msg, Color chatColor)
    {
        chatText.text = msg;

        chatText.color = chatColor;
    }
}
