using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class JSW_InitRoom : MonoBehaviour
{


    private string apiUrl = "http://125.132.216.190:12223/api/rooms/my"; // Replace with the actual API endpoint

    private void Start()
    {
        GetRoomStatus();
    }

    // Call this function to start the GET request
    public void GetRoomStatus()
    {
        StartCoroutine(GetRoomStatusCoroutine());
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
                RoomStatus roomStatus = JsonUtility.FromJson<RoomStatus>(request.downloadHandler.text);

                // Example: Access room data
                Debug.Log("Room ID: " + roomStatus.roomId);
                Debug.Log("Furniture Count: " + roomStatus.furnitureLayouts.Length);
            }
            else
            {
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
        public int furnitureId;
        public string furnitureName;
        public int positionX;
        public int positionY;
        public int rotation;
        public int width;
        public int height;
    }
}
