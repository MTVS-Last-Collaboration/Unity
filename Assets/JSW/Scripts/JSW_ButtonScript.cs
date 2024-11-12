using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class JSW_ButtonScript : MonoBehaviour
{
    public Button myButton; // 버튼을 연결할 변수
    public TMP_Text buttonText; // 버튼 텍스트 컴포넌트를 연결할 변수
    public Color initColor;
    private bool isClicked = false; // 상태를 저장하는 변수

    void Start()
    {
        myButton = GetComponent<Button>();
        buttonText = transform.GetChild(0).transform.GetComponent<TMP_Text>();
        initColor = buttonText.color;
        // 버튼 클릭 시 ChangeTextColor 함수 호출 설정
        myButton.onClick.AddListener(ChangeTextColor);

    }

    void ChangeTextColor()
    {
        // 버튼 클릭 시 텍스트 색상 변경
        if (isClicked)
        {
            buttonText.color = Color.black; // 기본 색상
            isClicked = false;
        }
        else
        {
            buttonText.color = initColor; // 변경할 색상
            isClicked = true;
        }
    }
}
