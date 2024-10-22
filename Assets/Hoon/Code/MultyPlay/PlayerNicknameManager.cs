using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNicknameManager : MonoBehaviourPun, IPunObservable
{
    public TextMeshProUGUI nickNameComp;
    PhotonView photonView;
    string nickName;
    string otherNickName;
   

    // Start is called before the first frame update
    void Start()
    {
        photonView = transform.GetComponent<PhotonView>();
        nickName = nickNameComp.text;

        if ( photonView.IsMine )
        {
            nickNameComp.text = LoginInfoManager.instance.nickName;
            print("아바타 닉네임" + nickNameComp.text);
          
        }
        else
        {
            nickNameComp.text = otherNickName;
            print("다른아바타 닉네임" + nickNameComp.text);

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