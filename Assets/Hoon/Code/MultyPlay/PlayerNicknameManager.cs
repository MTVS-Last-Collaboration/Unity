using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNicknameManager : MonoBehaviour
{
    public TextMeshProUGUI nickNameComp;

    // Start is called before the first frame update
    void Start()
    {
        nickNameComp.text = LoginInfoManager.instance.nickName;
        print("아바타 닉네임" + nickNameComp.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
