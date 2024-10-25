using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class JSW_DecoMineObject : MonoBehaviour
{
    public string ResoucesName;
    public JSW_DecorateRoomManager decorateRoomManager;
    public bool isPurchased;
    public TMP_Text isMineText;


    private void Awake()
    {
        decorateRoomManager = GameObject.Find("DecorateRoomManager").GetComponent<JSW_DecorateRoomManager>();
    }

    private void Start()
    {
        isMineText = transform.transform.GetChild(2).GetComponent<TMP_Text>();
    }
    public void OnClickMineDeco()
    {
        if (isPurchased)
        {
            decorateRoomManager.PlayerSetFuniture1(ResoucesName);
        }
    }
}
