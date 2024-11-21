using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnMoveTrigger : MonoBehaviourPunCallbacks
{
    public GameObject funiturePos;
    public JSW_LobbyGameManager lobbyGameManager;
    public GameObject byeUI;
    public JSW_SoundManager soundManager;
    public GameObject imgCreateRoomBG;
    public HoonCreateRoom hoonCreateRoom;
    

    void Start()
    {
        //imgCreateRoomBG.SetActive(false);//시작할때 BG를 끈다.
        
              
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //print(1111);
            if (!other.transform.GetComponent<PhotonView>().IsMine) return;
            if (byeUI != null)
            {
                byeUI.SetActive(true);
                //print(2222);
            }
            else
            {
                imgCreateRoomBG.SetActive(true); //이타이밍에 BG를 켜준다. 나중에 UI컨트롤로 변경할수 있음.
                //GoOtherRoom();
                //print(3333);
            }
        }
    }

    //룸으로 입장하는 함수
    public void GoOtherRoom()
    {
        if (soundManager != null)
        {
            soundManager.PlayEftSound(JSW_SoundManager.ESoundType.EFT_DoorSound);
        }
        if (funiturePos != null)
        {
            for (int i = 0; i < funiturePos.transform.childCount; i++)
            {
                PhotonView pv = funiturePos.transform.GetChild(i).GetComponent<PhotonView>();
                if (pv.IsMine == true)
                {
                    // 남아 있는 플레이어 중 한 명에게 소유권을 넘깁니다.
                    if (PhotonNetwork.PlayerListOthers.Length != 0)
                    {
                        Player newOwner = PhotonNetwork.PlayerListOthers[0];
                        if (newOwner != null)
                        {
                            pv.TransferOwnership(newOwner);
                            //Debug.Log("Transferred ownership of " + photonView.name + " to " + newOwner.NickName);
                        }
                        else
                        {
                            //ebug.LogWarning("No available player to transfer ownership to.");
                        }
                    }
                }
            }
        }


        if (lobbyGameManager != null)
        {
            PhotonNetwork.Destroy(lobbyGameManager.player);
        }
        GetComponent<JSW_ConnectionManager>().enabled = true;
        GetComponent<JSW_ConnectionManager>().LeaveRoom();
    }

    public void CancelGoOtherRoom()
    {
        byeUI.SetActive(false);
    }
}
