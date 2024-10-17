using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JSW_ScheduleManager : MonoBehaviour
{
    // 추후 아이콘도 추가할 예정


    public TMP_InputField input_Field;
    public RectTransform trcontent;
    public GameObject scheduleFactory;

    private void Awake()
    {
        input_Field = GameObject.Find("Schedule_Input").GetComponent<TMP_InputField>();
        trcontent = GameObject.Find("ScheduleContentBody").GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }



    public void ScheduleSubmit()
    {

        string chat = input_Field.text;

        CreateScheduleItem(chat, Color.black);
    }

    void CreateScheduleItem(string chat, Color chatColor)
    {
        // s의 내용으로 ChatItem을 만들자.
        GameObject go = Instantiate(scheduleFactory, trcontent);
        // 만들어진 go에서 ChatItem 컴포넌트 가져오자.
        JSW_ScheduleItem scheduleItem = go.GetComponent<JSW_ScheduleItem>();

        // 가져온 컴포넌트의 SetText 함수 실행
        scheduleItem.SetText(chat);
    }
}
