using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

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

    public int shopId;

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
            JDMO.transform.gameObject.SetActive(true);
            DecoUIPurchase.SetActive(false);
            DecoUIOkay.SetActive(true);
            point = point - nowPrice;
            nowOwner.text = "소유중";
            JDMO.isMineText.text = "소유중";
            JDMO.isPurchased = true;
            JDSI.isPurchase = true;
            BuyId(shopId);
            JSW_SoundManager.Get().PlayEftSoundClick3();
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



    /// <summary>
    /// 
    /// </summary>
    /// <param name="points"></param>
    /// 


    public void BuyId(int id)
    {
        ItemIDs itemids = new ItemIDs
        {
            itemId = id
        };

        StartCoroutine(PostAddPoints(itemids));
    }

    public class ItemIDs
    {
        public int itemId;
    }

    // Coroutine을 통해 POST 요청을 수행
    private IEnumerator PostAddPoints(ItemIDs ids)
    {
        // 요청 URL 설정 (서버의 URL로 변경해야 합니다)
        //string url = "http://125.132.216.190:12223/api/couple/add-points";
        string url = "http://125.132.216.190:12223/api/shop/purchase";
        string jwtToken = LoginInfoManager.instance.myToken;

        //string jsonData = points.ToString();
        string jsonData = JsonUtility.ToJson(ids);

        // UnityWebRequest 생성 및 설정
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("구매가 성공적으로 추가되었습니다: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("구매 실패: " + request.error);
        }
    }
}
