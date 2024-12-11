using Newtonsoft.Json.Linq;
using Photon.Pun;
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
    private bool oneClick;

    public int MongLevel = 0;

    public float MongUITime = 0;

    public GameObject AlbumUI;

    private void Start()
    {
        uiManager = GameObject.Find("UIManager");
    }

    private void Update()
    {
        if (oneClick == true)
        {
            MongUITime += Time.deltaTime;
            if (MongUITime >= 4.0f)
            {
                MongUITime = 0;
                oneClick = false;
            }
        }
    }

    private void OnMouseDown()
    {
        //추후 클릭 성공 시 플레이어 움직임 막기
        if (isPlayerInRange && !oneClick)
        {
            if (AlbumUI.activeSelf == true) return;
            uiManager.GetComponent<JSW_UIManager>().OnClickMong();
            cameraControll.CameraToMong();
            oneClick = true;
        }
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
            oneClick = false;
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


}
