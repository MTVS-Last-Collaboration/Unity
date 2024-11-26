using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSW_ClickCalender : MonoBehaviour
{
    public GameObject uiManager;
    public JSW_CameraControllTest cameraControllTest;
    public JSW_CalenderManager calenderManger;
    private bool isPlayerInRange = false;
    public Vector3 RawScale;
    public GameObject Calender;

    private void Start()
    {
        uiManager = GameObject.Find("UIManager");
        RawScale = Calender.transform.localScale;
    }

    private void OnMouseDown()
    {
        //추후 클릭 성공 시 플레이어 움직임 막기
        if (isPlayerInRange && cameraControllTest.cameraPos != "Calender")
        {
            cameraControllTest.CameraToCalender();
            StartCoroutine(OpenCalenderUI());
        }
    }

    IEnumerator OpenCalenderUI()
    {
        yield return new WaitForSeconds(0.8f);
        uiManager.GetComponent<JSW_UIManager>().OnClickCalender();
        calenderManger.InitCalender();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isPlayerInRange = true;
            //checkID = other.GetComponent<CheckID>();
            iTween.ScaleTo(Calender, iTween.Hash(
                "scale", RawScale * 1.3f,        // 목표 스케일 (1, 1, 1)
                "time", 0.3f,                // 애니메이션 시간 (조정 가능)
                "easeType", "easeInCirc", // 통통 튀는 느낌의 easeType
                "oncomplete", "OnCompleteOpening", // 애니메이션 완료 시 호출할 함수
                "oncompletetarget", gameObject
            ));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isPlayerInRange = false;
            iTween.ScaleTo(Calender, iTween.Hash(
                "scale", RawScale * 1f,        // 목표 스케일 (1, 1, 1)
                "time", 0.5f,                // 애니메이션 시간 (조정 가능)
                "easeType", "easeInCirc", // 통통 튀는 느낌의 easeType
                "oncomplete", "OnCompleteOpening", // 애니메이션 완료 시 호출할 함수
                "oncompletetarget", gameObject
            ));
        }
    }
}
