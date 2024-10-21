using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public string managerId;
    public string nickName;
    public AudioClip voiceClip;
    public bool isTouchAble = true;
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
        managerId = string.Empty;
        nickName = string.Empty;
        voiceClip = null;
        evolutionCount = 0;
        curState = States.SPROUT;
    }
}
