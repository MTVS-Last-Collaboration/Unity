using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//동기화 용도 클래스를 부모로 MonoBehaviourPun
public class LobbyGameManager : MonoBehaviourPun
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
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
        Vector3 initPosition = new Vector3(radomPos.x, 0.0f, radomPos.y);
        //플레이어 생성하자, 이름,위치.회전 , 프리팹 경로는 Resources 
        //player = PhotonNetwork.Instantiate("PlayerMale", initPosition, Quaternion.identity);
        player = PhotonNetwork.Instantiate("PlayerWoman", initPosition, Quaternion.identity);

        // player 오브젝트 캐싱 완료
        Debug.Log("Player instantiated and cached: " + player.gameObject);

        // 생성후 소유권을 Owner인 플레이어게만 권한을주자. Owner가 접속을 종료하면 같이 사라짐.
    }

}//클래스 끝
