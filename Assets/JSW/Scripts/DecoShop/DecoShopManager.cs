using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DecoShopManager : MonoBehaviour
{
    public int point;
    public int nowPrice;
    public TMP_Text nowOwner;
    public TMP_Text profilePrice;
    
    public JSW_DecoShopItem JDSI;
    public JSW_DecoMineObject JDMO;

    public GameObject DecoUIPurchase;
    public GameObject DecoUIOkay;


    void Start()
    {
        profilePrice.text = point.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PurchaseOkay()
    {

        if (point >= nowPrice)
        {
            DecoUIPurchase.SetActive(false);
            DecoUIOkay.SetActive(true);
            point -= nowPrice;
            profilePrice.text = point.ToString();
            nowOwner.text = "소유중";
            JDMO.isMineText.text = "소유중";
            JDMO.isPurchased = true;
            JDSI.isPurchase = true;
        }
    }

    public void OnClickPurchaseNo()
    {
        DecoUIPurchase.SetActive(false);
    }

    public void OnClickPurchaseOkay()
    {
        DecoUIOkay.SetActive(false);
    }
}
