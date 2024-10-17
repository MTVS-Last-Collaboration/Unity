using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JSW_ScheduleItem : MonoBehaviour
{
    public TMP_Text scheduleText;

    void Start()
    {
        scheduleText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetText(string text)
    {
        print(text);
        scheduleText = transform.GetChild(0).GetComponent<TMP_Text>();
        scheduleText.text = text;
    }
}
