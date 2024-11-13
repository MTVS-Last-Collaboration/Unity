using Photon.Pun;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;

public class PlayerNicknameManager : MonoBehaviourPun, IPunObservable
{
    public TextMeshProUGUI nickNameComp;
    PhotonView photonView;
    string nickName;
    string otherNickName;
    public string userNumber;
   
    // Start is called before the first frame update
    void Start()
    {
        photonView = transform.GetComponent<PhotonView>();
        nickName = nickNameComp.text;

        //유저번호 할당하기
        //print("내오브젝트 이름" + gameObject.name);
        if (gameObject.name.Contains("PlayerMale"))
        {
            userNumber = "user1";
            //print("내유저 번호" + userNumber);
        }
        else
        {
            userNumber = "user2";
            //print("내유저 번호" + userNumber);
        }

        if ( photonView.IsMine )
        {
            nickNameComp.text = LoginInfoManager.instance.nickName;
            //print("PlayerNicknameManager" + nickNameComp.text);
          
        }
        else
        {
            nickNameComp.text = otherNickName;
            //print("다른아바타 닉네임" + nickNameComp.text);

        }
    
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(LoginInfoManager.instance.nickName); //print("닉네임을 보내자");
           
        }
        else if (stream.IsReading)
        {
            otherNickName = (string)stream.ReceiveNext(); // print("닉네임을 받자");

        }
    }

    // Update is called once per frame
    /*void Update()
      {

      }*/

}//클래스끝