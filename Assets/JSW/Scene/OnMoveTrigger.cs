using ExitGames.Client.Photon.StructWrapping;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMoveTrigger : MonoBehaviourPunCallbacks
{
    public GameObject funiturePos;
    public JSW_LobbyGameManager lobbyGameManager;
    public GameObject byeUI;
    public JSW_SoundManager soundManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!other.transform.GetComponent<PhotonView>().IsMine) return;
            if (byeUI != null)
            {
                byeUI.SetActive(true);
            }
            else
            {
                GoOtherRoom();
            }
        }
    }

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
