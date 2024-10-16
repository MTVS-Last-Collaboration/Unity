using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public string managerId;
    public string nickName;
    public AudioClip voiceClip;
    //public bool isFirst = false;
    public FlowerUIManager uiManager;

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
}
