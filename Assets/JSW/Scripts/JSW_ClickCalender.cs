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

    private void Start()
    {
        uiManager = GameObject.Find("UIManager");
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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
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
}
