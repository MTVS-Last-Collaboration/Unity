using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class JSW_CalenderManager : MonoBehaviourPun
{
    public GameObject year;
    public GameObject month;
    public GameObject calenderBody;
    public GameObject[] days = new GameObject[42];
    

    public int nowYear;
    public int nowMonth;
    public int nowDay;


    public GameObject rightCalenderBody;
    public GameObject[] days2 = new GameObject[7];
    public TMP_Text rightCalenderTitle;

    public JSW_ScheduleManager scheduleManager;

    // Start is called before the first frame update
    void Start()
    {
        year = GameObject.Find("C_Year");
        month = GameObject.Find("C_Month");
        calenderBody = GameObject.Find("CalenderBody");
        rightCalenderBody = GameObject.Find("RightCalenderBody");
        rightCalenderTitle = GameObject.Find("RightCalenderTitle").GetComponent<TMP_Text>();
        scheduleManager = GameObject.Find("ScheduleManager").GetComponent<JSW_ScheduleManager>();

        print(System.DateTime.Today.Year.ToString());
        for (int i =0; i < 42;i++)
        {
            days[i] = calenderBody.transform.GetChild(0).GetChild(0).GetChild(i).gameObject;
        }
        for (int i = 0; i < 7; i++)
        {
            days2[i] = rightCalenderBody.transform.GetChild(i).gameObject;
        }

        nowYear = System.DateTime.Today.Year;
        nowMonth = System.DateTime.Today.Month;
        nowDay = System.DateTime.Today.Day;
        year.GetComponent<TMP_Text>().text = System.DateTime.Now.ToString("yyyy");
        month.GetComponent<TMP_Text>().text = System.DateTime.Now.ToString("MMMM", CultureInfo.InvariantCulture);
        InitCalender();

    }


    // 해당 년 월의 첫번재 날 반환
    public DayOfWeek GetDayFirstWeek(int year, int month)               
    {
        DateTime firstDate = new DateTime(year, month, 1);
        return (firstDate.DayOfWeek);
    }

    // 캘린더 날짜 초기화
    public void InitCalender()
    {
        int dayNum = 1;
        // 캘린더 마지막 줄 켜줄껀지 끌껀지
        if ((int)GetDayFirstWeek(nowYear, nowMonth) + EndDay(nowYear, nowMonth) > 35)
        {
            OnLastTailCalender();
        }
        else
        {
            offLastTailCalender();
        }

        for (int i = 0; i < 42; i++)
        {
            if ((int)GetDayFirstWeek(nowYear, nowMonth) <= i && i < (int)GetDayFirstWeek(nowYear, nowMonth) + EndDay(nowYear, nowMonth)) 
            {
                days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text = (dayNum).ToString();
                string datee = "" + nowYear + nowMonth.ToString("00") + dayNum.ToString("00");
                if (scheduleManager.scheduleDictionary.ContainsKey(datee))
                {
                    days[i].transform.GetChild(scheduleManager.scheduleDictionary[datee][0].iconCode + 1).transform.gameObject.SetActive(true);
                }
                else
                {
                    days[i].transform.GetChild(1).gameObject.SetActive(false);
                    days[i].transform.GetChild(2).gameObject.SetActive(false);
                    days[i].transform.GetChild(3).gameObject.SetActive(false);
                    days[i].transform.GetChild(4).gameObject.SetActive(false);
                }
                dayNum++;
            }
            else
            {
                days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text = "";
                days[i].transform.GetChild(1).gameObject.SetActive(false);
                days[i].transform.GetChild(2).gameObject.SetActive(false);
                days[i].transform.GetChild(3).gameObject.SetActive(false);
                days[i].transform.GetChild(4).gameObject.SetActive(false);
            }
        }


        TMP_Text tmp_nowToday = days[nowDay + (int)GetDayFirstWeek(nowYear,nowMonth) - 1].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();
        TMP_Text tmp_elseToday = days[(int)GetDayFirstWeek(nowYear, nowMonth)].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();

        if (System.DateTime.Today.Year == nowYear && System.DateTime.Today.Month == nowMonth && System.DateTime.Today.Day == nowDay)
        {
            days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            days[(int)GetDayFirstWeek(nowYear, nowMonth)].transform.GetChild(0).gameObject.SetActive(true);
        }

        string dayString = "" + nowYear.ToString() + nowMonth.ToString("D2") + nowDay.ToString("D2");
        scheduleManager.ResetSchedule(dayString);

        DateTime ScheduleDate = new DateTime(nowYear, nowMonth, nowDay);
        scheduleManager.scheduleNowDay.text = ScheduleDate.ToString("MM 월 dd 일 dddd");

        changeNowRightCalender();
    }
    public void InitCalender2()
    {
        int dayNum = 1;
        // 캘린더 마지막 줄 켜줄껀지 끌껀지
        if ((int)GetDayFirstWeek(nowYear, nowMonth) + EndDay(nowYear, nowMonth) > 35)
        {
            OnLastTailCalender();
        }
        else
        {
            offLastTailCalender();
        }

        for (int i = 0; i < 42; i++)
        {
            if ((int)GetDayFirstWeek(nowYear, nowMonth) <= i && i < (int)GetDayFirstWeek(nowYear, nowMonth) + EndDay(nowYear, nowMonth))
            {
                days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text = (dayNum).ToString();
                string datee = "" + nowYear + nowMonth.ToString("00") + dayNum.ToString("00");
                if (scheduleManager.scheduleDictionary.ContainsKey(datee))
                {
                    days[i].transform.GetChild(scheduleManager.scheduleDictionary[datee][0].iconCode + 1).transform.gameObject.SetActive(true);
                }
                else
                {
                    days[i].transform.GetChild(1).gameObject.SetActive(false);
                    days[i].transform.GetChild(2).gameObject.SetActive(false);
                    days[i].transform.GetChild(3).gameObject.SetActive(false);
                    days[i].transform.GetChild(4).gameObject.SetActive(false);
                }
                dayNum++;
            }
            else
            {
                days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text = "";
                days[i].transform.GetChild(1).gameObject.SetActive(false);
                days[i].transform.GetChild(2).gameObject.SetActive(false);
                days[i].transform.GetChild(3).gameObject.SetActive(false);
                days[i].transform.GetChild(4).gameObject.SetActive(false);
            }
        }


        TMP_Text tmp_nowToday = days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();
        TMP_Text tmp_elseToday = days[(int)GetDayFirstWeek(nowYear, nowMonth)].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();

        days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(0).gameObject.SetActive(true);

        string dayString = "" + nowYear.ToString() + nowMonth.ToString("D2") + nowDay.ToString("D2");
        scheduleManager.ResetSchedule(dayString);

        DateTime ScheduleDate = new DateTime(nowYear, nowMonth, nowDay);
        scheduleManager.scheduleNowDay.text = ScheduleDate.ToString("MM 월 dd 일 dddd");

        changeNowRightCalender();
    }

    public void UpdateDaySchedule()
    {
        int i = (int)GetDayFirstWeek(nowYear, nowMonth) + nowDay - 1;
        if ((int)GetDayFirstWeek(nowYear, nowMonth) <= i && i < (int)GetDayFirstWeek(nowYear, nowMonth) + EndDay(nowYear, nowMonth))
        {
            string datee = "" + nowYear + nowMonth.ToString("00") + nowDay.ToString("00");
            if (scheduleManager.scheduleDictionary.ContainsKey(datee))
            {
                if (scheduleManager.scheduleDictionary[datee].Count == 0)
                {
                    days[i].transform.GetChild(1).gameObject.SetActive(false);
                    days[i].transform.GetChild(2).gameObject.SetActive(false);
                    days[i].transform.GetChild(3).gameObject.SetActive(false);
                    days[i].transform.GetChild(4).gameObject.SetActive(false);
                }
                else
                {
                    days[i].transform.GetChild(1).gameObject.SetActive(false);
                    days[i].transform.GetChild(2).gameObject.SetActive(false);
                    days[i].transform.GetChild(3).gameObject.SetActive(false);
                    days[i].transform.GetChild(4).gameObject.SetActive(false);
                    days[i].transform.GetChild(scheduleManager.scheduleDictionary[datee][0].iconCode + 1).transform.gameObject.SetActive(true);
                }
            }
            else
            {
                days[i].transform.GetChild(1).gameObject.SetActive(false);
                days[i].transform.GetChild(2).gameObject.SetActive(false);
                days[i].transform.GetChild(3).gameObject.SetActive(false);
                days[i].transform.GetChild(4).gameObject.SetActive(false);
            }
        }
        else
        {
            days[i].transform.GetChild(2).gameObject.GetComponent<TMP_Text>().text = "";
        }
        changeNowRightCalender();
    }

    public int EndDay(int year, int month)
    {
        if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
        {
            return 31;
        }
        else if (month == 2)
        {
            if (isYoonYear(year))
            {
                return 29;
            }
            else
            {
                return 28;
            }
        }
        else
        {
                return 30;
        }

    }

    public bool isYoonYear(int year)
    {
        if (year % 400 == 0)
        {
            return true;
        }
        else if (year % 100 == 0)
        {
            return false;
        }
        else if (year % 4 == 0)
        {
            return true;
        }
        return false;
    }


    // 달력 왼쪽 버튼 누르면 1달 내려감
    public void OnClickDownMonth()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound2);
        TMP_Text tmp_nowToday = days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();
        days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(0).gameObject.SetActive(false);

        if (nowMonth == 1)
        {
            nowYear -= 1;
            nowMonth = 12;
        }
        else
        {
            nowMonth -= 1;
        }

        DateTime firstDate = new DateTime(nowYear, nowMonth, 1);
        year.GetComponent<TMP_Text>().text = firstDate.ToString("yyyy");
        month.GetComponent<TMP_Text>().text = firstDate.ToString("MMMM", CultureInfo.InvariantCulture);

        nowDay = 1;

        InitCalender();
    }

    // 달력 오른쪽 버튼 누르면 1달 올라감
    public void OnClickUpMonth()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound2);
        TMP_Text tmp_nowToday = days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();
        days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(0).gameObject.SetActive(false);

        if (nowMonth == 12)
        {
            nowYear += 1;
            nowMonth = 1;
        }
        else
        {
            nowMonth += 1;
        }

        DateTime firstDate = new DateTime(nowYear, nowMonth, 1);
        year.GetComponent<TMP_Text>().text = firstDate.ToString("yyyy");
        month.GetComponent<TMP_Text>().text = firstDate.ToString("MMMM", CultureInfo.InvariantCulture);

        nowDay = 1;

        InitCalender();
    }

    public void OnClickResetNowDay(int day)
    {
        TMP_Text tmp_nowToday = days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();
        days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(0).gameObject.SetActive(false);
        nowDay = day;
        TMP_Text newToday = days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(5).gameObject.GetComponent<TMP_Text>();
        days[nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1].transform.GetChild(0).gameObject.SetActive(true);

        string dayString = "" + nowYear.ToString() + nowMonth.ToString("D2") + nowDay.ToString("D2");
        scheduleManager.ResetSchedule(dayString);
        DateTime ScheduleDate = new DateTime(nowYear, nowMonth, nowDay);
        scheduleManager.scheduleNowDay.text = ScheduleDate.ToString("MM 월 dd 일 dddd");
        changeNowRightCalender();
    }

    public void OnLastTailCalender()
    {
        for (int i = 35; i < 42; i++)
        {
            days[i].SetActive(true);
        }
    }

    public void offLastTailCalender()
    {
        for (int i = 35; i < 42; i++)
        {
            days[i].SetActive(false);
        }
    }

    public void changeNowRightCalender()
    {
        int k = 0;
        int nowDayEnd = nowDay + (int)GetDayFirstWeek(nowYear, nowMonth) - 1;
        rightCalenderTitle.text = (((nowDayEnd) /7 + 1).ToString() + " Weeks");
        
        for (int i = nowDayEnd - nowDayEnd % 7;i < nowDayEnd - nowDayEnd % 7 + 7;i++)
        {
            if(days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text != "")
            {
                string datee = "" + nowYear + nowMonth.ToString("00") + (int.Parse(days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text)).ToString("00");
                if (scheduleManager.scheduleDictionary.ContainsKey(datee))
                {
                    if (scheduleManager.scheduleDictionary[datee].Count == 0)
                    {
                        days[i].transform.GetChild(1).gameObject.SetActive(false);
                        days[i].transform.GetChild(2).gameObject.SetActive(false);
                        days[i].transform.GetChild(3).gameObject.SetActive(false);
                        days[i].transform.GetChild(4).gameObject.SetActive(false);
                    }
                    else
                    {
                        days[i].transform.GetChild(scheduleManager.scheduleDictionary[datee][0].iconCode + 1).transform.gameObject.SetActive(true);
                    }
                }
                else
                {
                    days[i].transform.GetChild(1).gameObject.SetActive(false);
                    days[i].transform.GetChild(2).gameObject.SetActive(false);
                    days[i].transform.GetChild(3).gameObject.SetActive(false);
                    days[i].transform.GetChild(4).gameObject.SetActive(false);
                }
            }
            else
            {
                days[i].transform.GetChild(1).gameObject.SetActive(false);
                days[i].transform.GetChild(2).gameObject.SetActive(false);
                days[i].transform.GetChild(3).gameObject.SetActive(false);
                days[i].transform.GetChild(4).gameObject.SetActive(false);
            }
            days2[k++].transform.GetChild(5).GetComponent<TMP_Text>().text = days[i].transform.GetChild(5).gameObject.GetComponent<TMP_Text>().text;
        }
    }
}
