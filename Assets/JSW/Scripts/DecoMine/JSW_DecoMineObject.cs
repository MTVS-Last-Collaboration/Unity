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

    public int shopid;
    public JSW_InitRoom initRoom;

    private void Awake()
    {
        decorateRoomManager = GameObject.Find("DecorateRoomManager").GetComponent<JSW_DecorateRoomManager>();
    }

    private void Start()
    {
        isMineText = transform.transform.GetChild(2).GetComponent<TMP_Text>();

        initRoom = GameObject.Find("DecorateRoomManager").GetComponent<JSW_InitRoom>();
        if (initRoom.initShopId[shopid])
        {
            isPurchased = true;
            //transform.GetChild(1).GetComponent<TMP_Text>().text = "소유중";
            transform.GetChild(2).GetComponent<TMP_Text>().text = "소유중";
        }
        else
        {
            isPurchased = false;
            transform.GetChild(2).GetComponent<TMP_Text>().text = "";
            transform.gameObject.SetActive(false);
        }
        print("fdsafdasfdafdasfda");
    }
    public void OnClickMineDeco()
    {
        if (isPurchased)
        {
            decorateRoomManager.PlayerSetFuniture1(ResoucesName);
        }
    }
}
