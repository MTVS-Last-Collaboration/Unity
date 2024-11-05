using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Rendering.DebugUI;

public class ChangeInputDate : MonoBehaviour
{
    //public InputField dateInputField;
    public TMP_InputField dateInputField;
    public TMP_InputField coupleDateInputField;
    

    void Start()
    {
        //dateInputField.onEndEdit.AddListener(FormatDate);
        // InputField의 값이 변경될 때마다 OnInputValueChanged 메서드를 호출하도록 설정
        coupleDateInputField.onValueChanged.AddListener(OnInputValueChanged);

    }

    void FormatDate(string input)
    {
        print("입력을 받는중");
        // 입력이 8자리 숫자이고, 정수로 변환 가능한지 확인
        if (input.Length == 8 && long.TryParse(input, out _))
        {
            // "yyyyMMdd" 형식으로 입력된 문자열을 DateTime으로 변환
            if (DateTime.TryParseExact(input, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                // 변환된 DateTime을 "yyyy-MM-dd" 형식으로 다시 표시
                dateInputField.text = date.ToString("yyyy-MM-dd");
            }
            else
            {
                Debug.LogError("잘못된 날짜 형식입니다.");
            }
        }
        else
        {
            Debug.LogError("8자리 날짜 형식이 아닙니다. 예: 20240101");
        }
    }

    // InputField의 값이 변경될 때 호출되는 메서드
    void OnInputValueChanged(string input)
    {
        // 입력이 8자리 숫자인지 확인
        if (input.Length == 8 && int.TryParse(input, out _))
        {
            // 연도, 월, 일로 분리
            string year = input.Substring(0, 4);
            string month = input.Substring(4, 2);
            string day = input.Substring(6, 2);

            // 날짜 형식으로 변환
            string formattedDate = $"{year}-{month}-{day}";

            // InputField의 텍스트를 날짜 형식으로 변경
            coupleDateInputField.text = formattedDate;

            // 커서를 텍스트 끝으로 이동 (자동 변경 후 다시 편집 가능하게 함)
            coupleDateInputField.MoveTextEnd(false);
        }
    }

}//클래스 끝
