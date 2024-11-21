using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using UnityEngine;

[Serializable]
public class FlowerData
{
    public string managerId;
    public string nickName;
    public byte[] voiceData;
    public int evolutionCount;
    public string state;
}

public class Flower : MonoBehaviour
{
    public string managerId;
    public string nickName;
    public AudioClip voiceClip;
    public FlowerUIManager uiManager;
    public int evolutionCount = 0;
    public int harvestCoins = 10;
    public States curState;

    private const string ENDPOINT = "/scores";

    private void Awake()
    {
        uiManager = GetComponent<FlowerUIManager>();
    }

    public enum States
    {
        SEED = 0,
        SPROUT = 1,
        BUD = 2,
        BLOSSOM = 3
    }

    public static IEnumerator PostFlowerData(FlowerData flowerData, Action<bool, string> callback = null)
    {
        return NetworkManager.Instance.Post(ENDPOINT, flowerData, callback);
    }

    public static IEnumerator GetFlowerData(FlowerData flowerData, Action<bool, FlowerData> callback = null)
    {
        return NetworkManager.Instance.Get<FlowerData>(ENDPOINT, callback);
    }

    public void ResetFlower()
    {
        //managerId = string.Empty;
        //nickName = string.Empty;
        voiceClip = null;
        evolutionCount = 0;
        curState = States.SEED;
    }
}
