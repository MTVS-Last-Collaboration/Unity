using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class JSW_CalenderManager : MonoBehaviour
{
    public GameObject year;
    public GameObject month;
    public GameObject CalenderBody;

    public enum Week
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }

    // Start is called before the first frame update
    void Start()
    {
        year = GameObject.Find("C_Year");
        month = GameObject.Find("C_Month");
        CalenderBody = GameObject.Find("CalenderBody");
        print(System.DateTime.Today.Year.ToString());
        year.GetComponent<TMP_Text>().text = System.DateTime.Now.ToString("yyyy");
        month.GetComponent<TMP_Text>().text = System.DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
        print(GetDayFirstWeek(2024, 10));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public DayOfWeek GetDayFirstWeek(int year, int month)
    {
        DateTime firstDate = new DateTime(year, month, 1);
        return (firstDate.DayOfWeek);
    }
}
