using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;


public class JSW_ClickMong : MonoBehaviour
{
    public GameObject uiManager;
    public JSW_CameraControllTest cameraControll;
    private bool isPlayerInRange = false;

    public int MongLevel = 0;

    private void Start()
    {
        uiManager = GameObject.Find("UIManager");
    }


    private void OnMouseDown()
    {
        //추후 클릭 성공 시 플레이어 움직임 막기
        if (isPlayerInRange)
        {
            uiManager.GetComponent<JSW_UIManager>().OnClickMong();
            cameraControll.CameraToMong();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            //checkID = other.GetComponent<CheckID>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            //if (targetFlower != null && targetFlower.uiManager != null)
            //{
            //    targetFlower.uiManager.HideFlowerInfo();
            //    checkID = null;
            //}
            //추후 끄는 버튼 생성
        }
    }

    public void MongLevelUp()
    {
        if (MongLevel < 3)
        {
            transform.GetChild(MongLevel++).gameObject.SetActive(false);
            transform.GetChild(MongLevel).gameObject.SetActive(true);
        }
    }

    //private string url = "http://125.132.216.190:12223/api/pet/add-exp";

    //IEnumerator AddPetExperience()
    //{
    //    // 요청을 생성합니다.
    //    UnityWebRequest request = new UnityWebRequest(url, "POST");

    //    // 인증 토큰이 필요한 경우 헤더에 추가합니다.
    //    // request.SetRequestHeader("Authorization", "Bearer YOUR_TOKEN");
    //    string jwtToken = LoginInfoManager.instance.myToken;

    //    request.SetRequestHeader("Accept", "application/json");
    //    //request.SetRequestHeader("accept", "application/json");
    //    request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

    //    // 요청을 서버로 전송하고 응답을 기다립니다.
    //    yield return request.SendWebRequest();

    //    // 서버의 응답을 확인합니다.
    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        // 성공 응답 처리
    //        Debug.Log("경험치 추가 성공: " + request.downloadHandler);
    //    }
    //    else
    //    {
    //        // 실패 응답 처리
    //        Debug.LogError("경험치 추가 실패: " + request.error);
    //    }
    //}

    //public void MongLevelUpGet()
    //{
    //    StartCoroutine(GetPetStatus());
    //}

    //private string url2 = "http://125.132.216.190:12223/api/pet";

    //IEnumerator GetPetStatus()
    //{
    //    // GET 요청을 생성합니다.
    //    UnityWebRequest request = UnityWebRequest.Get(url2);

    //    string jwtToken = LoginInfoManager.instance.myToken;
    //    request.SetRequestHeader("Accept", "application/json");
    //    request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

    //    // 요청을 서버로 전송하고 응답을 기다립니다.
    //    yield return request.SendWebRequest();

    //    // 서버의 응답을 확인합니다.
    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        // 성공 응답 처리
    //        Debug.Log("펫 상태 조회 성공: " + request.downloadHandler.text);
    //    }
    //    else
    //    {
    //        // 실패 응답 처리
    //        Debug.LogError("펫 상태 조회 실패: " + request.error);
    //    }
    //}
}
