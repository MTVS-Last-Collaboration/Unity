using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class JSW_DecoObject : MonoBehaviourPun, IPunObservable
{

    public int decoObjectPositionX;
    public int decoObjectPositionZ;

    public int decoObjectLengthX;
    public int decoObjectLengthZ;

    public int decoObjectRotation;

    public int funitureNum;

    public bool isMovingFuniture;

    public string nameFuni;

    public int funitureLayoutId;
    public int otherFunitureLayoutId;

    public Vector3 myPos;
    public Quaternion myRot;
    public Vector3 myScale;
    PhotonView photonview;

    private void Start()
    {
        Vector3 size = transform.localScale;
        transform.localScale = Vector3.one * 0.2f;
        iTween.ScaleTo(gameObject, iTween.Hash(
        "scale", size, // 최종 크기
        "time", 1.0f,         // 애니메이션 시간
        "easetype", iTween.EaseType.easeOutElastic // 애니메이션 타입
    ));
    }
    void Update()
    {
        //PlayerMoveKey();
        //PlayerMoveJoyStick(joyStick.inputDirection);
        if (photonView != null && photonView.IsMine == false )
        {
            transform.position = myPos;
            transform.rotation = myRot;
            funitureLayoutId = otherFunitureLayoutId;
        }
        
        //if(Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    funitureLayoutId = 10;
        //}
    }

  

    public void SetpositionInfo(int posX, int posZ, int lenX, int lenZ, int rot, string funiName)
    {
        photonView.RPC("SetpositionInfo_RPC", RpcTarget.AllBuffered, posX, posZ, lenX, lenZ, rot, funiName);
    }

    [PunRPC]
    public void SetpositionInfo_RPC(int posX, int posZ, int lenX, int lenZ, int rot, string funiName)
    {
        decoObjectPositionX = posX;
        decoObjectPositionZ = posZ;

        decoObjectLengthX = lenX;
        decoObjectLengthZ = lenZ;

        decoObjectRotation = rot;
        nameFuni = funiName;
    }

    public int[] GetPositionInfo()
    {
        int[] posInfo = new int[] { decoObjectPositionX, decoObjectPositionZ, decoObjectLengthX, decoObjectLengthZ, decoObjectRotation };
        return posInfo;
    }

    public Vector3 PlayerPushPosition(int playerX, int playerZ)
    {
        int posX = decoObjectPositionX;
        int posZ = decoObjectPositionZ;
        int lenX = decoObjectLengthX;
        int lenZ = decoObjectLengthZ;
        Vector3 playerVector3 = new Vector3(playerX, 1, playerZ);
        Vector3 minVector3 = new Vector3(playerX, 1, playerZ);
        
        float minDistance = 100000;

        // 12시방향
        if (decoObjectRotation == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    if (Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3);
                        minVector3 = new Vector3(i, 0.1f, j);
                    }
                }
            }
        }

        // 3시방향
        if (decoObjectRotation == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    if (Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3);
                        minVector3 = new Vector3(i, 0.1f, j);
                    }
                }
            }

        }
        // 6시방향
        if (decoObjectRotation == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3);
                        minVector3 = new Vector3(i, 0.1f, j);
                    }
                }
            }
            
        }
        // 9시방향
        if (decoObjectRotation == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    if (Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 0.1f, j), playerVector3);
                        minVector3 = new Vector3(i, 0.1f, j);
                    }
                }
            }
        }
        return minVector3;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //print("내 위치를 보내자");
            stream.SendNext(transform.position);    //나의 위치를 하자.
            stream.SendNext(transform.rotation);    //나의 방향을 보내자
            stream.SendNext(funitureLayoutId);
        }
        else if (stream.IsReading)
        {
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
            otherFunitureLayoutId = (int)stream.ReceiveNext();
        }
    }
    //void OnDestroy()
    //{
    //    photonview = transform.GetComponent<PhotonView>();

    //    print("d2312");
    //    if (photonview != null && photonview.IsMine)
    //    {
    //        print("d");
    //        if (PhotonNetwork.CurrentRoom.Players[0] != null) photonview.TransferOwnership(PhotonNetwork.CurrentRoom.Players[0]);
    //        // 다른 플레이어 중 하나에게 소유권을 넘김 (예시로 마스터 클라이언트에게)
    //    }
    //}
}