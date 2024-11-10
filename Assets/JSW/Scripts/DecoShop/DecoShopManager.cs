using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DecoShopManager : MonoBehaviour
{
    public int point;
    public int nowPrice;
    public float targetPoint;
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

    private void Update()
    {
        targetPoint = Mathf.Lerp(targetPoint, point, Time.deltaTime * 10f);
        profilePrice.text = targetPoint.ToString("0");
    }

    public void PurchaseOkay()
    {

        if (point >= nowPrice)
        {
            DecoUIPurchase.SetActive(false);
            DecoUIOkay.SetActive(true);
            point = point - nowPrice;
            //StartCoroutine(PurchaseCo(point, targetPoint));
            nowOwner.text = "소유중";
            JDMO.isMineText.text = "소유중";
            JDMO.isPurchased = true;
            JDSI.isPurchase = true;
        }
    }
    //IEnumerator PurchaseCo(int point, int targetPoint)
    //{
    //    float changetPoint = point;
    //    while ((int)changetPoint != targetPoint)
    //    {
    //        changetPoint = Mathf.Lerp(changetPoint, targetPoint, Time.deltaTime * 10f);
    //        profilePrice.text = changetPoint.ToString("0");
    //        yield return null;
    //    }
    //    this.point = targetPoint;
    //    profilePrice.text = this.point.ToString("0");
    //}

    public void OnClickPurchaseNo()
    {
        DecoUIPurchase.SetActive(false);
    }

    public void OnClickPurchaseOkay()
    {
        DecoUIOkay.SetActive(false);
    }
}
