using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FlowerData
{
    public string managerId;
    public string nickName;
    public byte[] voiceData;
    public int evolutionCount;
    public int harvestCoins;
    public string state;
}

public class Flower : MonoBehaviour
{
    public string managerId;
    public string nickName;
    public AudioClip voiceClip;
    //public bool isTouchAble = true;   // 꽃 터치시 조이스틱 등 비활성화 > ui끄면 다시 활성화
    public FlowerUIManager uiManager;
    public int evolutionCount = 0;
    public int harvestCoins = 300;
    public States curState;

    private void Awake()
    {
        uiManager = GetComponent<FlowerUIManager>();
    }

    public enum States
    {
        SPROUT,
        BUD,
        BLOSSOM
    }

    public void ResetFlower()
    {
        //managerId = string.Empty;
        //nickName = string.Empty;
        voiceClip = null;
        evolutionCount = 0;
        curState = States.SPROUT;
    }
}
