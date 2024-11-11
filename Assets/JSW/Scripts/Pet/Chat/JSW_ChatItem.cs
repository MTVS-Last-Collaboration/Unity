using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_ChatItem : MonoBehaviour
{
    public TMP_Text chatText;
    public TMP_Text time;
    public TMP_Text mongName;


    private void Awake()
    {
        //chatText = GetComponent<TMP_Text>();

        RectTransform rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(1, 0.5f);
            rectTransform.anchorMax = new Vector2(1, 0.5f);
            rectTransform.pivot = new Vector2(1, 0.5f);
        

    }

   

    // Update is called once per frame
    void Update()
    {

    }

    public void SetText(string msg, string isMong, string times)
    {
        if(isMong == "Mong")
        {
            mongName.text = GameObject.Find("PetManager").GetComponent<JSW_PetManager>().MongName;
            chatText.text = msg;
            //time.text = times;
        }
        else
        {
            chatText.text = msg;
            time.text = times;
        }
    }
}
