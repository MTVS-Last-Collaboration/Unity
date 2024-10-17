using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoveTest : MonoBehaviour
{
    public CharacterController playerController;
    public float playerMoveSpeed = 3.0f;
    public Animator animator;
    public GameObject model;

    VirtualJoyStick joyStick;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //PlayerMoveKey();
        //PlayerMoveJoyStick(joyStick.inputDirection);



    }

    public void PlayerMoveKey()
    {
        //CC로 움직이게 하자

        float x = Input.GetAxisRaw("Horizontal");   //print("Horizontal=" + x);
        float y = Input.GetAxisRaw("Vertical");     //print("Vertical=" + y);

        Vector3 playerMoveDir = new Vector3(x, 0, y);
        playerMoveDir.Normalize();
        Vector3 playerMove = playerMoveDir * playerMoveSpeed * Time.deltaTime;

        //transform.position += playerMoveDir * playerMoveSpeed * Time.deltaTime;   //움직이기 시간값에 따라.
        playerController.Move(playerMove);  //플레이어 컨트롤러
        if (animator != null)    //animator null 아닐때
        {
            if (x != 0 || y != 0)    //값이 0이 아닐때
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
            else if (y == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else if (y == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            //대간선 방향으로 모델을 회전
            if (y == 1 && x == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 45, 0);//print("회전값y" + y + "회전값x" + x);
            }
            else if (y == 1 && x == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, -45, 0);//print("회전값y" + y + "회전값x" + x);
            }
            else if (y == -1 && x == -1)
            {
                model.transform.localEulerAngles = new Vector3(0, -135, 0);//print("회전값y" + y + "회전값x" + x);
            }
            else if (y == -1 && x == 1)
            {
                model.transform.localEulerAngles = new Vector3(0, 135, 0);//print("회전값y" + y + "회전값x" + x);
            }

        }

    }


    public void PlayerMoveJoyStick(Vector3 inputDirection)
    {
        float x = inputDirection.x;     //print("Horizontal=" + x);
        float z = inputDirection.y;     //print("Vertical=" + y);

        Vector3 playerMoveDir = new Vector3(x,0,z);
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
            else if (x > 0 && z > -0.5f &&  z < 0.5f) //오른쪽
            {
                model.transform.localEulerAngles = new Vector3(0, 90, 0); //print("회전값x" + x);
            }
            else if(x < 0 && z > -0.5f && z < 0.5f) //왼쪽
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
            else if(x > 0 && z < 0)// 오른쪽아래
            {
                model.transform.localEulerAngles = new Vector3(0, 135, 0);
            }
            else if ( x < 0 && z < 0)//왼쪽위
            {
                model.transform.localEulerAngles = new Vector3(0, -135, 0);
            }
            else if (x < 0 && z > 0)//왼쪽아래
            {
                model.transform.localEulerAngles = new Vector3(0, -45, 0);
            }
           
            


            

        }

    }


}//클래스 끝
