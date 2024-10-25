using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSW_DecoShopItem : MonoBehaviour
{
    public int price;
    public TMP_Text mineShopObjectText;
    public JSW_DecoMineObject decoMineObject;
    public DecoShopManager decoShopManager;
    public bool isPurchase;

    private void Awake()
    {
        decoShopManager = GameObject.Find("DecoShopManager").GetComponent<DecoShopManager>();
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
        }
    }
}
