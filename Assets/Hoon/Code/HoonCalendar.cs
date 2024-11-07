using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
public class HoonCalendar : MonoBehaviour
{
    public TextMeshProUGUI textDate;
    public TextMeshProUGUI textYear;
    public TextMeshProUGUI textMonth;
    public TextMeshProUGUI textDay;
    int idxDayStart;

    // Start is called before the first frame update
    void Start()
    {
         
        /* print("지금" + DateTime.Now);
         print("오늘" + DateTime.Today);
         print("올해만" + DateTime.Today.Year);
         print("이달만" + DateTime.Today.Month);
         print("오늘만" + DateTime.Today.Day);*/

        // 오늘 날짜를 "MMMM d, yyyy" 형식으로 변환하여 출력
        //System.Globalization.CultureInfo.InvariantCulture 가 month 를 영문으로 변경
        /*string currentDate = DateTime.Now.ToString("yyyy, MMMM, d", System.Globalization.CultureInfo.InvariantCulture);
        Debug.Log("오늘 날짜 영문"  + currentDate);
        textDate.text = currentDate;*/
        int year = 2024;
        int month = 11;

        textYear.text = DateTime.Now.ToString("yyyy"); //연도를 표시
        textMonth.text = DateTime.Now.ToString("MMMM", new CultureInfo("en-US")); //월을 영문으로 표시


        // "Day"라는 이름을 가진 모든 오브젝트를 담을 리스트를 생성합니다.
        List<GameObject> dayObjects = new List<GameObject>();

        // 씬 내 모든 활성화된 GameObject를 검색합니다.
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // 모든 오브젝트를 순회하여 이름이 "Day"인 오브젝트만 리스트에 추가합니다.
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Day"))
            {
                dayObjects.Add(obj);
                //print(obj.name);
                //obj.GetComponentInChildren<TextMeshProUGUI>().text = "2";
            }
        }

        // 이름순으로 정렬합니다. 이정렬은 이름과 숫자가 있을때
        // name1, name11,name2 과 같이 숫자 크기와 관계없이 1을 우선으로 정렬 합니다. 
        //GameObject[] sortedObjects = dayObjects.OrderBy(obj => obj.name).ToArray();

        // 이름순으로 정렬합니다.
        // name1, name2 와 같이 숫자크기에 따라 정렬됩니다.
        GameObject[] sortedDayObjects = dayObjects
            .OrderBy(obj => ExtractNumberFromName(obj.name)) // 이름에서 숫자를 추출하여 정렬
            .ToArray();

        // 정렬된 결과를 출력합니다.
        foreach (GameObject obj in sortedDayObjects)
        {
            Debug.Log(obj.name);
        }


        //textDate.text = DateTime.DaysInMonth(year, month);

        // 현재 날짜를 가져옵니다.
        DateTime now = DateTime.Now;

        // 이번 달 1일의 날짜를 설정합니다.
        DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);

        // 이번 달 1일의 요일을 구합니다.
        DayOfWeek firstDayOfWeek = firstDayOfMonth.DayOfWeek;
        // 결과를 출력합니다.
        Debug.Log($"이번 달 1일의 요일: {firstDayOfWeek}");

        string firstDayinMonth = firstDayOfWeek.ToString();
        // 이번 달의 일수를 구합니다.
        int daysInMonth = DateTime.DaysInMonth(now.Year, now.Month);

        Debug.Log($"이번 달 일수: {daysInMonth}");
        print("이번달 1일 요일" + firstDayinMonth);

        // 1일의 요일을 정수로 변환하여 idxDayStart에 저장합니다.
        idxDayStart = (int)firstDayOfMonth.DayOfWeek;
        print("이달의 1일이 해당되는 날짜" + idxDayStart);


        //1일이 무슨요일인지 확인하기 //시작하는 배열의 숫자를 정해주자. int 
        /*if (firstDayinMonth == "Sunday")
        {
            print("1일을 일요일");
            idxDayStart = 0;
        }
        else if (firstDayinMonth == "Monday")
        {
            print("1일을 월요일");
            idxDayStart = 1;
        }
        else if(firstDayinMonth == "Tuesday")
        {
            print("1일을 화요일");
            idxDayStart = 2;

        }
        else if (firstDayinMonth == "Wednsday")
        {
            print("1일을 수요일");
            idxDayStart = 3;
        }
        else if (firstDayinMonth == "Thursday")
        {
            print("1일을 목요일");
            idxDayStart = 4;
        }
        else if (firstDayinMonth == "Friday")
        {
            print("1일을 금요일");
            idxDayStart = 5;
        }
        else if (firstDayinMonth == "Saterday")
        {
            print("1일을 토요일");
            idxDayStart = 6;
        }*/

        Debug.Log("day오브젝트개수" + sortedDayObjects.Length);
       

        //정렬된 시작일 2를 출력하는 코드
        //sortedDayObjects[idxDayStart].GetComponentInChildren<TextMeshProUGUI>().text = "2";

        int idx = 1;
        for (int i = 0; i < sortedDayObjects.Length; i++ )
        {
            if (i >= idxDayStart && i < daysInMonth + idxDayStart)
            {
                sortedDayObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = idx.ToString();
                idx++;
            }
            else //그외에는0
            {
                sortedDayObjects[i].GetComponentInChildren<TextMeshProUGUI>().text = ""; 
            }
            
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 이름에서 숫자를 추출하는 함수
    int ExtractNumberFromName(string name)
    {
        // "day" 이후의 숫자를 정규식으로 추출
        var match = Regex.Match(name, @"\d+"); // 숫자를 찾는 정규식
        return match.Success ? int.Parse(match.Value) : 0; // 숫자가 있으면 그 값을 반환, 없으면 0 반환
    }
    
    public void GetDateText()
    {
        //누르버튼의 텍스트를 숫자로 변환하자.
        textDay.text = transform.GetComponentInChildren<TextMeshProUGUI>().text;

    }


}//클래스끝
