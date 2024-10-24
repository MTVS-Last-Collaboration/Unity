using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSW_ClickMong : MonoBehaviour
{
    public GameObject uiManager;
    public JSW_CameraControllTest cameraControll;
    private bool isPlayerInRange = false;

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
}
