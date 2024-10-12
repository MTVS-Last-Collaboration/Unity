using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // 캐싱 값 가지고 있는 객체 (저장 매체)
    // 코드 몰아 넣는 객체 인스턴스 하나 (코드 실행 매체)

    public GameObject album_UI;
    public GameObject Album2;
    public GameObject Album_Loading;
    public GameObject Calender1;
    public GameObject Calender2;
    public GameObject Mong_1;
    public GameObject Mong_Chat_2;


    // Start is called before the first frame update
    void Start()
    {
        album_UI = GameObject.Find("UI_Album");
        Album2 = GameObject.Find("Album2");
        Album_Loading = GameObject.Find("Album_Loading");
        Calender1 = GameObject.Find("Calender1");
        Calender2 = GameObject.Find("Calender2");
        Mong_1 = GameObject.Find("Mong_1");
        Mong_Chat_2 = GameObject.Find("Mong_Chat_2");
        AllActiveFasle();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AllActiveFasle()
    {
        album_UI.SetActive(false);
        Album2.SetActive(false);
        Album_Loading.SetActive(false);
        Calender1.SetActive(false);
        Calender2.SetActive(false);
        Mong_1.SetActive(false);
        Mong_Chat_2.SetActive(false);
    }
}
