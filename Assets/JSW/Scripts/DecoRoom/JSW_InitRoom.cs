using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Networking;


public class JSW_InitRoom : MonoBehaviourPun
{
    public GameObject funiturePos;
    JSW_DecorateRoomManager DRM;
    public DecoMineManager DMM;
    public bool[] initShopId = new bool[45];

    private string apiUrl = "http://125.132.216.190:12223/api/rooms/status"; // Replace with the actual API endpoint

    private void Start()
    {
        DRM = GetComponent<JSW_DecorateRoomManager>();
        GetRoomStatus();
        GetShopStatus();
    }

    // Call this function to start the GET request
    public void GetRoomStatus()
    {
        StartCoroutine(GetRoomStatusCoroutine());
        print("dsadsa");
    }

    private IEnumerator GetRoomStatusCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
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
                RoomStatus0 roomStatus = JsonUtility.FromJson<RoomStatus0>(request.downloadHandler.text);

                // Example: Access room data
                Debug.Log("Room ID: " + roomStatus.data.roomId);
                Debug.Log("Furniture Count: " + roomStatus.data.furnitureLayouts.Length);


                if (PhotonNetwork.IsMasterClient)
                {
                    foreach (FurnitureLayout layout in roomStatus.data.furnitureLayouts)
                    {
                        InitSetFuniture(layout.furnitureLayoutId, layout.furnitureId, layout.furnitureName, layout.positionX, layout.positionY, layout.rotation, layout.width, layout.height);
                    }
                }

                DMM.floorNum = roomStatus.data.floor.floorNumber-1;
                DMM.wallNum = roomStatus.data.wallpaper.wallpaperNumber-1;
                print("처음인데 잘 나왔어요!!!!!!!");

            }
            else
            {
                InitSetFuniture(0, 1, "(Prb)Plant2", 4, 3, 3, 1, 1);
                print("JSW_InitRoom인데 처음 가구들 설치할 때 호출하는 것임");
                Debug.LogError("Error: " + request.error);

                print("안나왓어요!!!!!!!!!!");
            }
        }
    }

    [System.Serializable]
    public class RoomStatus0
    {
        public string message;
        public RoomStatus data;
    }

    // Define classes to match JSON
    // structure
    [System.Serializable]
    public class RoomStatus
    {
        public int roomId;
        public int coupleId;
        public FurnitureLayout[] furnitureLayouts;
        public Floor floor;
        public Wallpaper wallpaper;
    }

    [System.Serializable]
    public class Floor
    {
        public int id;
        public string name;
        public int floorNumber;
    }
    [System.Serializable]
    public class Wallpaper
    {
        public int id;
        public string name;
        public int wallpaperNumber;
    }

    [System.Serializable]
    public class ShoplistItemWrapper
    {
        public ShoplistItem[] items;
    }

    [System.Serializable]
    public class ShoplistItem
    {
        public int itemId;
        public string itemType;
        public string itemName;
        public string purchasedAt;
        public int pricePaid;
        public int width;
        public int height;
        public int number;
    }

    private string shopUrl = "http://125.132.216.190:12223/api/shop/purchased-items";

    public void GetShopStatus()
    {
        StartCoroutine(GetShopStatusCoroutine());
    }

    private IEnumerator GetShopStatusCoroutine()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(shopUrl))
        {

            request.SetRequestHeader("Accept", "application/json");
            string jwtToken = LoginInfoManager.instance.myToken;
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Process JSON response
                Debug.Log("Responsesssss: " + request.downloadHandler.text);

                string jsonResponse = "{\"items\":" + request.downloadHandler.text + "}";
                Debug.Log("Response: " + jsonResponse);

                // JSON 데이터를 ShoplistItemWrapper로 파싱
                ShoplistItemWrapper wrapper = JsonUtility.FromJson<ShoplistItemWrapper>(jsonResponse);


                foreach (ShoplistItem layout in wrapper.items)
                {
                    initShopId[layout.itemId] = true;
                    //layout.id, layout.itemType, layout.name, layout.price, layout.imageUrl, layout.xSize, layout.zSize);
                }
            }
            else
            {
                print("JSW_InitRoom인데 처음 가구들 설치할 때 호출하는 것임");
                Debug.LogError("Error: " + request.error);
            }
            //for (int i=0; i < 45;i++)
            //{
            //    print(i+ " = " + initShopId[i]);
            //}
        }
    }


    [System.Serializable]
    public class FurnitureLayout
    {
        // 여기에 아이디 받아 오면 될듯
        public int furnitureLayoutId;
        public int furnitureId;
        public string furnitureName;
        public int positionX;
        public int positionY;
        public int rotation;
        public int width;
        public int height;
    }

    public void InitSetFuniture(int layoutId,int id, string name, int posX, int posZ, int rot, int width, int height)
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
        photonView.RPC("SetFuniture1_CO", RpcTarget.AllBuffered, layoutId, finalFuni, id, posX,posZ, rot, width, height);
    }

    [PunRPC]
    IEnumerator SetFuniture1_CO(int layoutId,string finalFuni, int id, int posX, int posZ, int dir, int width, int height)
    {
        GameObject funitureOb;

        //GameObject funitureOb = Instantiate(funitureObject1);
        if (photonView.IsMine)
        {
            funitureOb = PhotonNetwork.Instantiate(finalFuni, transform.position + transform.forward, transform.rotation);
            funitureOb.transform.SetParent(funiturePos.transform);

        }
        else
        {
            GameObject prefab = Resources.Load<GameObject>(finalFuni);
            funitureOb = Instantiate(prefab, transform.position + transform.forward, transform.rotation);

        }

        funitureOb.transform.position = new Vector3(posX, 0.1f, posZ );



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
            funitureOb.GetComponent<JSW_DecoObject>().funitureLayoutId = layoutId;
            //JSW_InfoDecoObject infoDecoObejct = new JSW_InfoDecoObject((int)funitureOb.transform.position.x, (int)funitureOb.transform.position.z, jd.decoObjectLengthX, jd.decoObjectLengthZ, dir, finalFuni);
            //DRM.FunitureList.Add(infoDecoObejct);
            // 백엔드 연결되면 고
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
