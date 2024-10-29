using Photon.Pun.Demo.PunBasics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SocialPlatforms;


public class JSW_PlayerDecorate : MonoBehaviourPun
{
    public Vector3 playerDir;
    public Vector3 playerPos;
    public GameObject funitureObject1;
    public GameObject funitureObject2;
    public JSW_DecorateRoomManager DRM;
    public bool IsCharacterMoving; 
    int dir = 0;

    // Start is called before the first frame update
    void Start()
    {
        DRM = GameObject.Find("DecorateRoomManager").GetComponent<JSW_DecorateRoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = new Vector3(Mathf.Round(transform.forward.x), Mathf.Round(transform.forward.y), Mathf.Round(transform.forward.z));
        playerPos = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
        
        //if(Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    print(1);
        //    //SetFuniture1();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    SetFuniture2();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    PushFunitureSetting();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    PushFuniture();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    DrawFuniture();
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    DestroyFuniture();
        //}
    }


    public void SetFuniture1(string name)
    {
        string finalFuni = "(Prb)" + name;
        photonView.RPC("SetFuniture1_RPC", RpcTarget.AllBuffered, finalFuni);
    }

    [PunRPC]
    public void SetFuniture1_RPC(string finalFuni)
    {
        GameObject funitureOb;
        int dir = 0;
        print(gameObject.name + " " + photonView);

        //GameObject funitureOb = Instantiate(funitureObject1);
        if (photonView.IsMine)
        {

            funitureOb = PhotonNetwork.Instantiate(finalFuni, transform.position + transform.forward, transform.rotation);
        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(finalFuni);
            funitureOb = Instantiate(prefab, transform.position + transform.forward, transform.rotation);
        }


        if (Mathf.Abs(playerDir.x) == Mathf.Abs(playerDir.z))
            {
                if (Mathf.Abs(transform.forward.x) >= Mathf.Abs(transform.forward.z))
                {
                    funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 0.1f, playerPos.z + playerDir.z * 1);
                    playerDir.z = 0;
                }
                else
                {
                    funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 0.1f, playerPos.z + playerDir.z * 2);
                    playerDir.x = 0;
                }
            }
            else
            {
                funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 0.1f, playerPos.z + playerDir.z * 2);
            }

            funitureOb.transform.forward = playerDir;

            if (playerDir.z == 1)
            {
                dir = 0;
            }
            else if (playerDir.x == 1)
            {
                dir = 1;
            }
            else if (playerDir.z == -1)
            {
                dir = 2;
            }
            else if (playerDir.x == -1)
            {
                dir = 3;
            }

            JSW_DecoObject jd = funitureOb.GetComponent<JSW_DecoObject>();


            if (DRM.IsCanAddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir))
            {
                print("helpme");
                DRM.AddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir);
                funitureOb.GetComponent<JSW_DecoObject>().SetpositionInfo((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir);
                DRM.FunitureList.Add(funitureOb);
            }
            else
            {
                PhotonNetwork.Destroy(funitureOb);
                print("no");
            }
            // 백엔드 연결되면 고치자
            if (!photonView.IsMine)
            {
                Destroy(funitureOb);
            }

    }

    public void SetFuniture2()
    {
        int dir = 0;

        GameObject funitureOb = Instantiate(funitureObject2);

        if (Mathf.Abs(playerDir.x) == Mathf.Abs(playerDir.z))
        {
            if (Mathf.Abs(transform.forward.x) >= Mathf.Abs(transform.forward.z))
            {
                funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 1);
                playerDir.z = 0;
            }
            else
            {
                funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 1, playerPos.z + playerDir.z * 2);
                playerDir.x = 0;
            }
        }
        else
        {
            funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 2);
        }

        funitureOb.transform.forward = playerDir;

        if (playerDir.z == 1)
        {
            dir = 0;
        }
        else if (playerDir.x == 1)
        {
            dir = 1;
        }
        else if (playerDir.z == -1)
        {
            dir = 2;
        }
        else if (playerDir.x == -1)
        {
            dir = 3;
        }

        JSW_DecoObject jd = funitureOb.GetComponent<JSW_DecoObject>();

        if (DRM.IsCanAddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir))
        {
            DRM.AddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir);
            funitureOb.GetComponent<JSW_DecoObject>().SetpositionInfo((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir);
            DRM.FunitureList.Add(funitureOb);
        }
        else
        {
            Destroy(funitureOb);
            print("no");
        }
    }


    public void isPushorPull(int num)
    {
        photonView.RPC("isPushorPull_RPC", RpcTarget.AllBuffered, num);
    }
    [PunRPC]
    public void isPushorPull_RPC(int num)
    {
        if (dir == 0)
        {
            if(num == 0)
            {
                PushFuniture();
            }
            else if (num == 2)
            {
                DrawFuniture();
            }
        }
        else if (dir == 1)
        {
            if (num == 1)
            {
                PushFuniture();
            }
            else if (num == 3)
            {
                DrawFuniture();
            }
        }
        else if (dir == 2)
        {
            if (num == 2)
            {
                PushFuniture();
            }
            else if (num == 0)
            {
                DrawFuniture();
            }
        }
        else if (dir == 3)
        {
            if (num == 3)
            {
                PushFuniture();
            }
            else if (num == 1)
            {
                DrawFuniture();
            }
        }
    }

    //public void PushFunitureSetting()
    //{
    //    photonView.RPC("PushFunitureSetting_RPC", RpcTarget.AllBuffered);
    //}
    //[PunRPC]
    public void PushFunitureSetting()
    {

        GameObject funitureOb = null;

        // 플레이어의 위치와 방향 설정 (정면 방향)
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // Ray 생성 (카메라 앞에서 앞으로 쏘는 Ray)
        Ray ray = new Ray(transform.position + Vector3.up * 0.75f, forward);

        // Ray를 시각적으로 확인하기 위해 그립니다
        //Debug.DrawRay(transform.position + Vector3.up * 1f , forward * 1.5f, Color.red);
        // Ray가 물체와 충돌하는지 검사
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            if (!(hit.transform.gameObject.tag == "Funiture")) return; 
            funitureOb = hit.collider.gameObject;

            if(funitureOb.GetComponent<PhotonView>() != null && !funitureOb.GetComponent<PhotonView>().IsMine)
            {
                funitureOb.GetComponent<PhotonView>().RequestOwnership();
            }

            Vector3 num = funitureOb.GetComponent<JSW_DecoObject>().PlayerPushPosition((int)playerPos.x, (int)playerPos.z);

            Vector3 numDir = (num - transform.position).normalized;
            Vector3 realnumDir = new Vector3(Mathf.Round(numDir.x), Mathf.Round(numDir.y), Mathf.Round(numDir.z));

            if (Mathf.Abs(realnumDir.x) == Mathf.Abs(realnumDir.z))
            {
                if (Mathf.Abs(numDir.x) >= Mathf.Abs(numDir.z))
                {
                    //funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 1);
                    realnumDir.z = 0;
                }
                else
                {
                    //funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 1, playerPos.z + playerDir.z * 2);
                    realnumDir.x = 0;
                }
            }
            else
            {
                //funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 2);
            }

            //funitureOb.transform.forward = playerDir;

            if (realnumDir.z == 1)
            {
                dir = 0;
                transform.position = new Vector3(num.x, transform.position.y, num.z - 1);
            }
            else if (realnumDir.x == 1)
            {
                dir = 1;
                transform.position = new Vector3(num.x - 1, transform.position.y, num.z);
            }
            else if (realnumDir.z == -1)
            {
                dir = 2;
                transform.position = new Vector3(num.x, transform.position.y, num.z + 1);
            }
            else if (realnumDir.x == -1)
            {
                dir = 3;
                transform.position = new Vector3(num.x + 1, transform.position.y, num.z);
            }
        }
    }

    public void PushFuniture()
    {
        photonView.RPC("PushFuniture_RPC", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void PushFuniture_RPC()
    {

        GameObject funitureOb = null;


        // 플레이어의 위치와 방향 설정 (정면 방향)
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // Ray 생성 (카메라 앞에서 앞으로 쏘는 Ray)
        Ray ray = new Ray(transform.position + Vector3.up * 0.75f, forward);

        // Ray를 시각적으로 확인하기 위해 그립니다
        Debug.DrawRay(transform.position, forward * 1.5f, Color.red);

        // Ray가 물체와 충돌하는지 검사
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            // 충돌한 물체가 있다면 물체의 이름 출력
            //Debug.Log("Ray hit: " + hit.collider.gameObject.name);
            funitureOb = hit.collider.gameObject;
            print(funitureOb.name);

            Vector3 num = funitureOb.GetComponent<JSW_DecoObject>().PlayerPushPosition((int)playerPos.x, (int)playerPos.z);

            Vector3 numDir = (num - transform.position).normalized;
            Vector3 realnumDir = new Vector3(Mathf.Round(numDir.x), Mathf.Round(numDir.y), Mathf.Round(numDir.z));

            if (Mathf.Abs(realnumDir.x) == Mathf.Abs(realnumDir.z))
            {
                if (Mathf.Abs(numDir.x) >= Mathf.Abs(numDir.z))
                {
                    //funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 1);
                    realnumDir.z = 0;
                }
                else
                {
                    //funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 1, playerPos.z + playerDir.z * 2);
                    realnumDir.x = 0;
                }
            }
            else
            {
                //funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 2, 1, playerPos.z + playerDir.z * 2);
            }


            int[] funitureInfo = funitureOb.GetComponent<JSW_DecoObject>().GetPositionInfo();

            if (realnumDir.z == 1)
            {

                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 0) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    print("NOAndReturn0");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 0);
                StartCoroutine(moveBlockTranslate(new Vector3(0, 0, 1), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionZ += 1;
            }
            else if (realnumDir.x == 1)
            {
                if ( !DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 1) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    print("NOAndReturn1");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 1);
                StartCoroutine(moveBlockTranslate(new Vector3(1, 0, 0), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionX += 1;
            }
            else if (realnumDir.z == -1)
            {
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 2) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    print("NOAndReturn2");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 2);
                StartCoroutine(moveBlockTranslate(new Vector3(0, 0, -1), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionZ -= 1;
            }
            else if (realnumDir.x == -1)
            {
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 3) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    print("NOAndReturn3");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 3);
                StartCoroutine(moveBlockTranslate(new Vector3(-1, 0, 0), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionX -= 1;
            }
        }
    }

    public void DrawFuniture()
    {
        photonView.RPC("DrawFuniture_RPC", RpcTarget.AllBuffered);
    }
    [PunRPC]
    public void DrawFuniture_RPC()
    {
        GameObject funitureOb = null;


        // 플레이어의 위치와 방향 설정 (정면 방향)
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // Ray 생성 (카메라 앞에서 앞으로 쏘는 Ray)
        Ray ray = new Ray(transform.position + Vector3.up * 0.75f, forward);

        // Ray를 시각적으로 확인하기 위해 그립니다
        Debug.DrawRay(transform.position, forward * 1.5f, Color.red);

        // Ray가 물체와 충돌하는지 검사
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            // 충돌한 물체가 있다면 물체의 이름 출력
            //Debug.Log("Ray hit: " + hit.collider.gameObject.name);
            funitureOb = hit.collider.gameObject;
            print(funitureOb.name);

            Vector3 num = funitureOb.GetComponent<JSW_DecoObject>().PlayerPushPosition((int)playerPos.x, (int)playerPos.z);

            Vector3 numDir = (num - transform.position).normalized;
            Vector3 realnumDir = new Vector3(Mathf.Round(numDir.x), Mathf.Round(numDir.y), Mathf.Round(numDir.z));

            if (Mathf.Abs(realnumDir.x) == Mathf.Abs(realnumDir.z))
            {
                if (Mathf.Abs(numDir.x) >= Mathf.Abs(numDir.z))
                {
                    realnumDir.z = 0;
                }
                else
                {
                    realnumDir.x = 0;
                }
            }

            int[] funitureInfo = funitureOb.GetComponent<JSW_DecoObject>().GetPositionInfo();

            if (realnumDir.z == 1)
            {

                if ( !DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 2) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    return;
                }
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isDrawFuniture((int)transform.position.x, (int)transform.position.z, 1, 1, funitureInfo[4], 2))
                {
                    print("NOAndReturn02222");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 2);
                StartCoroutine(moveBlockTranslate(new Vector3(0, 0, -1), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionZ -= 1;
            }
            else if (realnumDir.x == 1)
            {
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 3) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    return;
                }
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isDrawFuniture((int)transform.position.x, (int)transform.position.z, 1, 1, funitureInfo[4], 3))
                {
                    print("NOAndReturn12222");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 3);
                //transform.position = new Vector3(num.x - 1, 1.49f, num.z);
                StartCoroutine(moveBlockTranslate(new Vector3(-1, 0, 0), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionX -= 1;
            }
            else if (realnumDir.z == -1)
            {
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 0) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    return;
                }
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isDrawFuniture((int)transform.position.x, (int)transform.position.z, 1, 1, funitureInfo[4], 0))
                {
                    print("NOAndReturn22222");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 0);
                //transform.position = new Vector3(num.x, 1.49f, num.z + 1);
                StartCoroutine(moveBlockTranslate(new Vector3(0, 0, 1), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionZ += 1;
            }
            else if (realnumDir.x == -1)
            {
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isPushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 1) || funitureOb.GetComponent<JSW_DecoObject>().isMovingFuniture)
                {
                    return;
                }
                if (!DRM.GetComponent<JSW_DecorateRoomManager>().isDrawFuniture((int)transform.position.x, (int)transform.position.z, 1, 1, funitureInfo[4], 1))
                {
                    print("NOAndReturn32222");
                    return;
                }
                DRM.GetComponent<JSW_DecorateRoomManager>().PushFuniture(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4], 1);
                //transform.position = new Vector3(num.x + 1, 1.49f, num.z);
                StartCoroutine(moveBlockTranslate(new Vector3(1, 0, 0), funitureOb));
                funitureOb.GetComponent<JSW_DecoObject>().decoObjectPositionX += 1;
            }
        }
    }

    private IEnumerator moveBlockTranslate(Vector3 dir, GameObject funiture)
    {

        Vector3 targetPosition = new Vector3((float)Math.Round((funiture.transform.position + dir).x), (float)Math.Round((funiture.transform.position + dir).y), (float)Math.Round((funiture.transform.position + dir).z));
        IsCharacterMoving = true;
        funiture.GetComponent<JSW_DecoObject>().isMovingFuniture = true;
        while (Vector3.Magnitude(targetPosition - funiture.transform.position) >= 0.15f)
        {
            //funiture.transform.position = Vector3.Lerp(funiture.transform.position, targetPosition, speed * Time.deltaTime);
            funiture.transform.Translate(funiture.transform.InverseTransformDirection(dir) * Time.deltaTime * 3);
            transform.Translate(transform.InverseTransformDirection(dir) * Time.deltaTime * 3);
            if (Vector3.Magnitude(targetPosition - funiture.transform.position) >= 2f)
            {
                break;
            }
            yield return new WaitForFixedUpdate(); ;
        }
        funiture.transform.position = targetPosition;
        transform.position = new Vector3((float)Math.Round(transform.position.x),transform.position.y, (float)Math.Round(transform.position.z));
        funiture.GetComponent<JSW_DecoObject>().isMovingFuniture = false;
        IsCharacterMoving = false;
    }

    public void DestroyFuniture()
    {
        photonView.RPC("DestroyFuniture_RPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DestroyFuniture_RPC()
    {
        // 플레이어의 위치와 방향 설정 (정면 방향)
        Vector3 forward = transform.TransformDirection(Vector3.forward);

        // Ray 생성 (카메라 앞에서 앞으로 쏘는 Ray)
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, forward);

        // Ray를 시각적으로 확인하기 위해 그립니다
        Debug.DrawRay(transform.position, forward * 1.5f, Color.red);

        // Ray가 물체와 충돌하는지 검사
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            if (hit.collider.tag == "Funiture")
            {
                int[] funitureInfo = hit.collider.GetComponent<JSW_DecoObject>().GetPositionInfo();
                DRM.DestroyFuniturePos(funitureInfo[0], funitureInfo[1], funitureInfo[2], funitureInfo[3], funitureInfo[4]);
                Destroy(hit.collider.gameObject);
            }
        }
    }

}
