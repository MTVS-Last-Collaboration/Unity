using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_LobbyGameManager : MonoBehaviourPunCallbacks
{
    public static LobbyGameManager instance;

    string playerAvataType;
    public GameObject player;
    public Sprite man;
    public Sprite girl;
    public TMP_Text nickName;
    public Image profile;

    // Start is called before the first frame update
    void Start()
    {

        playerAvataType = LoginInfoManager.instance.avataChoice;
        nickName.text = LoginInfoManager.instance.nickName;
        if (playerAvataType == "MALE")
        {
            playerAvataType = "JSW_PlayerMale";
            profile.sprite = man;
        }
        else
        {
            playerAvataType = "JSW_PlayerWoman";
            profile.sprite = girl;

        }

        StartCoroutine(SpawnPlayer());
       

        // OnPhotonSerializeView 에서 데이터 전송 빈도수 설정하기 (perSeconds) 
        PhotonNetwork.SerializationRate = 30;
        // 대부분의 데이터 전송빈도 (perSeconds). 입장, Instantiate, Load, 나감
        PhotonNetwork.SendRate = 30;

        Player[] playerList = PhotonNetwork.PlayerList;

        foreach (Player player in playerList)
        {
            Debug.Log("Player Name: " + player.NickName + ", Player ID: " + player.UserId);
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    IEnumerator SpawnPlayer()
    {
        //룸에 입장이 될때까지 기다린다.
        yield return new WaitUntil(() => { return PhotonNetwork.InRoom; });

        Vector2 radomPos = Random.insideUnitCircle * 5.0f;
        Vector3 initPosition = new Vector3(1, 0.1f, 4);
        //플레이어 생성하자, 이름,위치.회전 , 프리팹 경로는 Resources 
        //player = PhotonNetwork.Instantiate("PlayerMale", initPosition, Quaternion.identity);
        player = PhotonNetwork.Instantiate(playerAvataType, initPosition, Quaternion.identity);

        // player 오브젝트 캐싱 완료
        Debug.Log("Player instantiated and cached: " + player.gameObject);

        // 생성후 소유권을 Owner인 플레이어게만 권한을주자. Owner가 접속을 종료하면 같이 사라짐.
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 떠난 플레이어의 네트워크 오브젝트를 가져오기
            PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();

            foreach (PhotonView photonView in allPhotonViews)
            {
                // 떠난 플레이어가 소유한 오브젝트인지 확인
                if (photonView.Owner == otherPlayer)
                {
                    // 소유권을 Master Client로 이전
                    photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
                }
            }
        }
    }

}
