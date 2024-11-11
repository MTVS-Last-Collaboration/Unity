using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_DecoShopItem : MonoBehaviour
{
    public int price;
    public int shopid;
    public TMP_Text mineShopObjectText;
    public JSW_DecoMineObject decoMineObject;
    public DecoShopManager decoShopManager;
    public bool isPurchase;

    public JSW_InitRoom initRoom;



    private void Awake()
    {
        decoShopManager = GameObject.Find("DecoShopManager").GetComponent<DecoShopManager>();
    }

    private void Start()
    {
        initRoom = GameObject.Find("DecorateRoomManager").GetComponent<JSW_InitRoom>();
        print(initRoom.initShopId[shopid]);
        if (initRoom.initShopId[shopid])
        {
            isPurchase = true;
            //transform.GetChild(1).GetComponent<TMP_Text>().text = "소유중";
            transform.GetChild(2).GetComponent<TMP_Text>().text = "소유중";
        }
        else
        {
            isPurchase = false;
            transform.GetChild(2).GetComponent<TMP_Text>().text = price.ToString()+"P";
        }
    }
    public void OnClickPurchase()
    {
        
        if (!isPurchase)
        {
            decoShopManager.DecoUIPurchase.SetActive(true);
            decoShopManager.nowPrice = price;
            decoShopManager.nowOwner = mineShopObjectText;
            decoShopManager.JDMO = decoMineObject;
            decoShopManager.DecoUIPurchase.transform.GetChild(0).GetComponent<Image>().sprite = transform.GetChild(0).GetComponent<Image>().sprite;
            decoShopManager.DecoUIPurchase.transform.GetChild(1).GetComponent<TMP_Text>().text = transform.GetChild(1).GetComponent<TMP_Text>().text;
            decoShopManager.JDSI = this;
            decoShopManager.shopId = shopid;
        }
    }
}
