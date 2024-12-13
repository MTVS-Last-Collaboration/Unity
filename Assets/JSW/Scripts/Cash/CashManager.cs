using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class CashManager : MonoBehaviour, IOnEventCallback
{ 
    // Start is called before the first frame update
    public GameObject CashBackground1;
    public GameObject cashMenu;
    public GameObject CashBackground2;
    public GameObject isOkayCash;

    public GameObject moneyPanel1;
    public GameObject moneyPanel2;
    public GameObject moneyPanel3;
    public GameObject moneyPanel4;

    public DecoShopManager decoShopmanager;
    public int CashNum = 0;


    public void  OnClickCash()
    {
        CashBackground1.SetActive(true);
        cashMenu.SetActive(true);
    }
    public void OnClickCashBack()
    {
        moneyPanel1.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel2.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel3.transform.GetChild(1).gameObject.SetActive(false);
        CashBackground1.SetActive(false);
        cashMenu.SetActive(false);
    }

    public void OnClickIsCash()
    {
        CashBackground1.SetActive(false);
        CashBackground2.SetActive(true);
        isOkayCash.SetActive(true);
    }

    public void OnClickIsCashBack()
    {
        CashBackground1.SetActive(true);
        CashBackground2.SetActive(false);
        isOkayCash.SetActive(false);
    }

    public void OnClickCashPanel1(int num)
    {
        moneyPanel1.transform.GetChild(1).gameObject.SetActive(true);
        moneyPanel2.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel3.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel4.transform.GetChild(1).gameObject.SetActive(false);
        CashNum = num;
    }

    public void OnClickCashPanel2(int num)
    {
        moneyPanel1.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel2.transform.GetChild(1).gameObject.SetActive(true);
        moneyPanel3.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel4.transform.GetChild(1).gameObject.SetActive(false);
        CashNum = num;
    }
    public void OnClickCashPanel3(int num)
    {
        moneyPanel1.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel2.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel3.transform.GetChild(1).gameObject.SetActive(true);
        moneyPanel4.transform.GetChild(1).gameObject.SetActive(false);
        CashNum = num;
    }

    public void OnClickCashPanel4(int num)
    {
        moneyPanel1.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel2.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel3.transform.GetChild(1).gameObject.SetActive(false);
        moneyPanel4.transform.GetChild(1).gameObject.SetActive(true);
        CashNum = num;
    }


    public void AddPointsForCash()
    {
        print("포인트 : " + CashNum);
        StartCoroutine(PostAddPoints(CashNum));
        CashNum = 0;
    }

    private void OnEnable()
    {
        //PhotonNetwork.NetworkingClient.AddCallbackTarget(this);
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == 16) //포인트 증가
        {
            object[] receiveObjects = (object[])photonEvent.CustomData;
            int receiveString1 = (int)receiveObjects[0];
            decoShopmanager.point += receiveString1;
        }

    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }
    int temp = 0;
    // Coroutine을 통해 POST 요청을 수행
    private IEnumerator PostAddPoints(int points)
    {

        // 요청 URL 설정 (서버의 URL로 변경해야 합니다)
        string url = "http://125.132.216.190:12223/api/couple/add-points";

        string jwtToken = LoginInfoManager.instance.myToken;

        string jsonData = points.ToString();
        //string jsonData = JsonUtility.ToJson(points);

        // UnityWebRequest 생성 및 설정
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청 전송 및 응답 대기
        yield return request.SendWebRequest();

        // 응답 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            //yield return StartCoroutine(info.GetEvents());
            //decoShopmanager.point += points;

            object[] sendContent = new object[] { points };

            RaiseEventOptions eventOptions = new RaiseEventOptions();
            eventOptions.Receivers = ReceiverGroup.All;

            PhotonNetwork.RaiseEvent(16, sendContent, eventOptions, SendOptions.SendUnreliable);

            EventSystem.current.SetSelectedGameObject(null);


            isOkayCash.SetActive(false);
            CashBackground2.SetActive(false);
            Debug.Log("포인트가 성공적으로 추가되었습니다: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("포인트 추가 실패: " + request.error);
        }

    }
}
