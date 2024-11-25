using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JSW_ChatItem : MonoBehaviour
{
    public TMP_Text chatText;
    public TMP_Text time;
    public TMP_Text mongName;
    public GameObject UIManager;

    private void Awake()
    {
        //chatText = GetComponent<TMP_Text>();

        RectTransform rectTransform = GetComponent<RectTransform>();

            rectTransform.anchorMin = new Vector2(1, 0.5f);
            rectTransform.anchorMax = new Vector2(1, 0.5f);
            rectTransform.pivot = new Vector2(1, 0.5f);
        

    }

    public void SetText(string msg, string isMong, string times)
    {
        UIManager = GameObject.Find("UIManager");
        GameObject MongChat2 = UIManager.GetComponent<JSW_UIManager>().Mong_Chat_2;

        if (isMong == "Mong")
        {
            mongName.text = GameObject.Find("PetManager").GetComponent<JSW_PetManager>().MongName;
            //chatText.text = msg;
            chatText.text = msg;
            transform.parent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
            if (MongChat2.activeSelf == false) return;
            StartCoroutine(numberChat(msg, 0.03f));
        }
        else
        {
            chatText.text = msg;
            time.text = times;
            transform.parent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
            if (MongChat2.activeSelf == false) return;
            StartCoroutine(numberChat(msg, 0.003f));
        }
    }
    IEnumerator numberChat(string s, float time)
    {
        int num1 = 0;
        int num = s.Length;
        string ss = "";
        while(true)
        {
            if (num1 == num)
            {
                break;
            }
            ss += s[num1];
            chatText.text = ss;
            num1 += 1;
            yield return new WaitForSeconds(0.03f);
            transform.parent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        } 
    }
}
