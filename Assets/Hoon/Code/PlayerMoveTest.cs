using Photon.Pun;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using UnityEditor;
using UnityEngine;
//using UnityEngine.UIElements;

public class PlayerMoveTest : MonoBehaviourPun, IPunObservable
{
    public CharacterController playerController;
    public float playerMoveSpeed = 3.0f;
    public Animator animator;
    public GameObject model;

    public Vector3 myPos;
    Quaternion myRot;
    
    VirtualJoyStick joyStick;
    PhotonView photonview;

    float x;
    float z;
    float myDirectionX;
    float myDirectionZ;

    // Start is called before the first frame update
    void Start()
    {
        photonview = transform.GetComponent<PhotonView>();
        if (photonview != null) print("내 포톤뷰 있음" + photonView.ViewID);
        PhotonNetwork.SerializationRate = 30;
        photonView.ObservedComponents.Add(this);  // OnPhotonSerializeView 호출할 스크립트 추가
        photonView.Synchronization = ViewSynchronization.UnreliableOnChange;  // 데이터 동기화 설정
    }

    // Update is called once per frame
    void Update()
    {

        //PlayerMoveKey();
        //PlayerMoveJoyStick(joyStick.inputDirection);
        if(photonView.IsMine == false)  //print("내것이 아님 캐릭터 동기화");
        {
            transform.position = myPos; // 서버에서 받은 위치 및 회전을 부드럽게 동기화
            model.transform.rotation = myRot; //모델을 회전시키자

    
           if (animator != null)  //animator null 아닐때
            {
                if (x != 0 || z != 0)    //값이 0이 아닐때
                {
                    animator.SetBool("Walk", true); //걷기 켜기
                }
                else //0일때
                {
                    animator.SetBool("Walk", false); //걷기 끄기
                }
            }

        }


    }

    public void PlayerMoveKey()
    {
        //CC로 움직이게 하자
        float x = Input.GetAxisRaw("Horizontal");   //print("Horizontal=" + x);
        float z = Input.GetAxisRaw("Vertical");     //print("Vertical=" + y);

        Vector3 playerMoveDir = new Vector3(x, 0, z);
        playerMoveDir.Normalize();
        Vector3 playerMove = playerMoveDir * playerMoveSpeed * Time.deltaTime;

        //transform.position += playerMoveDir * playerMoveSpeed * Time.deltaTime;   //움직이기 시간값에 따라.
        playerController.Move(playerMove);  //플레이어 컨트롤러
        if (animator != null)    //animator null 아닐때
        {
            if (x != 0 || z != 0)    //값이 0이 아닐때
            {
                animator.SetBool("Walk", true); //걷기 켜기
            }
            else //0일때
            {
                animator.SetBool("Walk", false); //걷기 끄기
            }
        }

        //모델을 회전시키자.
        if (model != null)
        {
            //상하좌우 방향으로 모델을 회전
            if (x == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 90, 0); //print("회전값x" + x);
            }
            else if (x == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, -90, 0); //print("회전값x" + x);
            }
            else if (z == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (z == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            //대간선 방향으로 모델을 회전
            if (z == 1 && x == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 45, 0);//print("회전값y" + y + "회전값x" + x);
            }
            else if (z == 1 && x == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, -45, 0);//print("회전값y" + y + "회전값x" + x);
            }
            else if (z == -1 && x == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, -135, 0);//print("회전값y" + y + "회전값x" + x);
            }
            else if (z == -1 && x == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 135, 0);//print("회전값y" + y + "회전값x" + x);
            }

        }

    }


    public void PlayerMoveJoyStick(Vector3 inputDirection)
    {
       
        if (photonview.IsMine)
        {
            //print("내꺼 움직이자");
            x = inputDirection.x;     //print("Horizontal=" + x);
            z = inputDirection.y;     //print("Vertical=" + y);

            Vector3 playerMoveDir = new Vector3(x, 0, z);
            playerMoveDir.Normalize();
            Vector3 playerMove = playerMoveDir * playerMoveSpeed * Time.deltaTime;
            playerController.Move(playerMove);  //플레이어 컨트롤러


            if (animator != null)    //animator null 아닐때
            {
                if (x != 0 || z != 0)    //값이 0이 아닐때
                {
                    animator.SetBool("Walk", true); //걷기 켜기
                }
                else //0일때
                {
                    animator.SetBool("Walk", false); //걷기 끄기
                }
            }

            //모델을 회전시키자.
            if (model != null)
            {

                //print("회전값x" + x);
                //print("회전값z" + z);
                //상하좌우 방향으로 모델을 회전
                if (x == 0 && z == 0) //위
                {
                    model.transform.localEulerAngles = new Vector3(0, 0, 0); //print("회전값x" + x);
                }
                else if (x > 0 && z > -0.5f && z < 0.5f) //오른쪽
                {
                    model.transform.localEulerAngles = new Vector3(0, 90, 0); //print("회전값x" + x);
                }
                else if (x < 0 && z > -0.5f && z < 0.5f) //왼쪽
                {
                    model.transform.localEulerAngles = new Vector3(0, -90, 0); //print("회전값x" + x);
                }
                else if (z > 0 && x > -0.5f && x < 0.5f) //위
                {
                    model.transform.localEulerAngles = new Vector3(0, 0, 0); //print("회전값x" + x);
                }
                else if (z < 0 && x > -0.5f && x < 0.5f) //아래
                {
                    model.transform.localEulerAngles = new Vector3(0, 180, 0); //print("회전값x" + x);
                }
                else if (x > 0 && z > 0) // 오른쪽위
                {
                    model.transform.localEulerAngles = new Vector3(0, 45, 0);
                }
                else if (x > 0 && z < 0)// 오른쪽아래
                {
                    model.transform.localEulerAngles = new Vector3(0, 135, 0);
                }
                else if (x < 0 && z < 0)//왼쪽위
                {
                    model.transform.localEulerAngles = new Vector3(0, -135, 0);
                }
                else if (x < 0 && z > 0)//왼쪽아래
                {
                    model.transform.localEulerAngles = new Vector3(0, -45, 0);
                }

            }

        }
        /*if(photonView.IsMine == false)
        {
            print("내것이 아님");
            // 서버에서 받은 위치 및 회전을 부드럽게 동기화
            transform.position = myPos;
            
        }*/

    }
   
   
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
        if (stream.IsWriting)
        {
            //print("내 위치를 보내자");
            stream.SendNext(transform.position);    //나의 위치를 하자.
            stream.SendNext(model.transform.rotation);    //나의 모델의 방향을 보내자.
            stream.SendNext(x);
            stream.SendNext(z);
        }
        else if (stream.IsReading)
        {
            //print("내 위치를 받자");
            myPos = (Vector3)stream.ReceiveNext();
            myRot = (Quaternion)stream.ReceiveNext();
            x = (float)stream.ReceiveNext();
            z = (float)stream.ReceiveNext();


        }

    }


    public void OtherClientPlayerMove()
    {
        // 서버에서 받은 위치 및 회전을 부드럽게 동기화
        transform.position = myPos;
    }
    public void PlayerMove()
    {

    }
    public void RPCPlayerMoveJoyStick(Vector3 inputDirection)
    {

    }
}//클래스 끝
