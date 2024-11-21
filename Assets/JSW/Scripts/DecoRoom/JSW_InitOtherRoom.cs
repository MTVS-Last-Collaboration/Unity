using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class JSW_InitOtherRoom : MonoBehaviourPun
{
    JSW_DecorateRoomManager DRM;
    public int CoupleRoomCode;

    private string apiUrl = "http://125.132.216.190:12223/api/rooms/public/"; // Replace with the actual API endpoint

    private void Start()
    {
        DRM = GetComponent<JSW_DecorateRoomManager>();
        if (PhotonNetwork.CountOfPlayersInRooms == 1)
        {
            GetRoomStatus();
        }
    }

    // Call this function to start the GET request
    public void GetRoomStatus()
    {
        StartCoroutine(GetRoomStatusCoroutine());
        print("fdadsa");
    }

    private IEnumerator GetRoomStatusCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + CoupleRoomCode))
        {
            request.SetRequestHeader("Accept", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Process JSON response
                Debug.Log("Response: " + request.downloadHandler.text);

                // You can parse the JSON here
                RoomStatus roomStatus = JsonUtility.FromJson<RoomStatus>(request.downloadHandler.text);

                // Example: Access room data
                Debug.Log("Room ID: " + roomStatus.roomId);
                Debug.Log("Furniture Count: " + roomStatus.furnitureLayouts.Length);

                foreach (FurnitureLayout layout in roomStatus.furnitureLayouts)
                {
                    InitSetFuniture(layout.furnitureId, layout.furnitureName, layout.positionX, layout.positionY, layout.rotation, layout.width, layout.height);
                }
            }
            else
            {
                //InitSetFuniture(1, "(Prb)Plant2", 2, 4,4 , 1, 1);
                print("JSW_InitotherRoom인데 처음 가구들 설치할 때 호출하는 것임");
                Debug.LogError("Error: " + request.error);
            }
        }
    }

    // Define classes to match JSON structure
    [System.Serializable]
    public class RoomStatus
    {
        public int roomId;
        public int coupleId;
        public FurnitureLayout[] furnitureLayouts;
    }

    [System.Serializable]
    public class FurnitureLayout
    {
        // 여기에 아이디 받아 오면 될듯
        public int furnitureId;
        public string furnitureName;
        public int positionX;
        public int positionY;
        public int rotation;
        public int width;
        public int height;
    }

    public void InitSetFuniture(int id, string name, int posX, int posZ, int rot, int width, int height)
    {
        string finalFuni;
        if (name.Contains("(Prb)"))
        {
            finalFuni = name;
        }
        else
        {
            finalFuni = "(Prb)" + name;
        }
        photonView.RPC("SetFuniture1_CO", RpcTarget.AllBuffered, finalFuni, id, posX, posZ, rot, width, height);
    }

    [PunRPC]
    IEnumerator SetFuniture1_CO(string finalFuni, int id, int posX, int posZ, int dir, int width, int height)
    {
        GameObject funitureOb;
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

        funitureOb.transform.position = new Vector3(posX, 0.1f, posZ);

        //if (Mathf.Abs(playerDir.x) == Mathf.Abs(playerDir.z))
        //{
        //    if (Mathf.Abs(transform.forward.x) >= Mathf.Abs(transform.forward.z))
        //    {
        //        funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 0.1f, playerPos.z + playerDir.z * 1);
        //        playerDir.z = 0;
        //    }
        //    else
        //    {
        //        funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 0.1f, playerPos.z + playerDir.z * 1);
        //        playerDir.x = 0;
        //    }
        //}
        //else
        //{
        //    funitureOb.transform.position = new Vector3(playerPos.x + playerDir.x * 1, 0.1f, playerPos.z + playerDir.z * 1);
        //}

        //funitureOb.transform.forward = playerDir;

        //if (playerDir.z == 1)
        //{
        //    dir = 0;
        //}
        //else if (playerDir.x == 1)
        //{
        //    dir = 1;
        //}
        //else if (playerDir.z == -1)
        //{
        //    dir = 2;
        //}
        //else if (playerDir.x == -1)
        //{
        //    dir = 3;
        //}

        if (dir == 0)
        {
            funitureOb.transform.forward = new Vector3(0, 0, 1);
        }
        else if (dir == 1)
        {
            funitureOb.transform.forward = new Vector3(1, 0, 0);
        }
        else if (dir == 2)
        {
            funitureOb.transform.forward = new Vector3(0, 0, -1);
        }
        else if (dir == 3)
        {
            funitureOb.transform.forward = new Vector3(-1, 0, 0);
        }

        JSW_DecoObject jd = funitureOb.GetComponent<JSW_DecoObject>();

        if (DRM.IsCanAddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir))
        {
            DRM.AddNewFuniture((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir, finalFuni);
            funitureOb.GetComponent<JSW_DecoObject>().SetpositionInfo((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir, finalFuni);
            //JSW_InfoDecoObject infoDecoObejct = new JSW_InfoDecoObject((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir, finalFuni);
            //DRM.FunitureList.Add(infoDecoObejct);
            // 백엔드 연결되면 고치자
            if (!photonView.IsMine)
            {
                Destroy(funitureOb);
            }
        }
        else
        {
            PhotonNetwork.Destroy(funitureOb);
        }
        yield return null;
    }
}
