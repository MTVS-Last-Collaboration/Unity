using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour
{
    public string managerId;
    public string nickName;
    public AudioClip voiceClip;
    public bool isFirst = false;
    public enum Evolution
    {
        SEED,
        GRASS,
        FLOWER
    }
}
