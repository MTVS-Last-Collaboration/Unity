using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System;

public class LoginInfoManager : MonoBehaviour
{
    public static LoginInfoManager instance;
    
    public GameObject myAvata;
    public string avataChoice;
    public TMP_InputField inputField_NickName;
    public string nickName;
    public TextMeshProUGUI dataPath;
    public string coupleCode;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
    }

    // Start is called before the first frame update
    void Start()
    {
        nickName = inputField_NickName.text;
        //print("플레이어 넥네임" + nickName);

        string jsonSyncPath = Application.persistentDataPath + "/DayComentTest.json";
        if (File.Exists(jsonSyncPath))
        {
            //print("json파일있음");
            string loadJsonText = File.ReadAllText(jsonSyncPath);
            //dataPath.text = "json파일있음" + loadJsonText;
            //dataPath.text = "json파일있음" + jsonSyncPath;
        }
        else
        {
            print("json파일없음");
            dataPath.text = "json파일없음"; 
            //string path = Application.dataPath + "/StreamingAssets/Hoon/DayComent.json";//로컬경로
            string path = Application.persistentDataPath + "/DayComentTest.json"; //동기화경로
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            // DayComentData 객체 생성
            DayComentData dayComentData = new DayComentData
            {
                date = currentDate, // 현재 날짜
                user1name = "null",
                user1mood = "null",
                user1coment = "null",
                user2name = "null",
                user2mood = "null",
                user2coment = "null"
            };

            // DayComentData를 JSON 문자열 배열로 변수에저장
            string jsonString = JsonConvert.SerializeObject(new[] { dayComentData }, Formatting.Indented);

            // JSON 파일로 저장
            File.WriteAllText(path, jsonString);
            Debug.Log("파일 생성 완료: " + path);
            Debug.Log("저장된 JSON 데이터: " + jsonString);

            if (File.Exists(jsonSyncPath))
            {
                print("json파일신규생성");
                dataPath.text = "json파일신규생성";
            }
            else
            {
                print("json파일생성실패");
                dataPath.text = "json파일생성실패";
            }

        }

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChoiceAvata(string avataName)
    {
        if(avataName == "male")
        {
            avataChoice = "PlayerMale";
            //print("남자아바타 선택됨");
            dataPath.text = "남자아바타 선택됨";
        }
        else
        {
            avataChoice = "PlayerWoman";
            //print("여자아바타 선택됨");
            dataPath.text = "여자아바타 선택됨";
        }
    }

    public void ChangeAvataNickName()
    {
        nickName = inputField_NickName.text;
        //print("플레이어 넥네임" + nickName);
        dataPath.text = "닉네임" + ":" + inputField_NickName.text;
    }


}//클래스끝
