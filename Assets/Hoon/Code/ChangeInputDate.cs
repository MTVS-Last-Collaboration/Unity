using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeInputDate : MonoBehaviour
{
    //public InputField dateInputField;
    public TMP_InputField datteInputField;
    

    void Start()
    {
        datteInputField.onEndEdit.AddListener(FormatDate);
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
                datteInputField.text = date.ToString("yyyy-MM-dd");
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
}
