using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoginInfoManager : MonoBehaviour
{
    public static LoginInfoManager instance;
    
    public GameObject myAvata;
    public string avataChoice;
    public TMP_InputField inputField_NickName;
    public string nickName;

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
            print("남자아바타 선택됨");
        }
        else
        {
            avataChoice = "PlayerWoman";
            print("여자아바타 선택됨");
        }
    }

    public void ChangeAvataNickName()
    {
        nickName = inputField_NickName.text; 
        //print("플레이어 넥네임" + nickName);
    }


}//클래스끝
