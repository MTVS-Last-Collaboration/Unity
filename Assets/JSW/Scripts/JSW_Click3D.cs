using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JSW_Click3D : MonoBehaviour
{
    public JSW_UIManager uiManager;
    public JSW_CameraControllTest cameraControllTest;
    public bool ObjectRotate;
    GameObject Object1_3D;
    public bool isPlayerInRange = false;
    public GameObject Delete_3D_UI;
    public Vector3 RawScale;
    public GameObject sunBan;


    // Start is called before the first frame update
    // Trigger 넣자
    void Awake()
    {
        cameraControllTest = GameObject.Find("PlayerPos").GetComponent<JSW_CameraControllTest>();
    }
    private void Start()
    {
        RawScale = sunBan.transform.localScale;
    }

    private void OnMouseDown()
    {
        if (!isPlayerInRange) return;
        //추후 클릭 성공 시 플레이어 움직임 막기
        if (cameraControllTest.cameraPos != "3D")
        {
            if (transform.GetChild(2).transform.gameObject!=null) {
                Object1_3D = transform.GetChild(2).transform.gameObject;
            }
            ObjectRotate = true;
            cameraControllTest.CameraTo3D();
            uiManager.time = 0;
            Delete_3D_UI.SetActive(true);
        }
        else
        {
            if (transform.GetChild(2).transform.gameObject != null)
            {
                Object1_3D = transform.GetChild(2).transform.gameObject;
            }
            Object1_3D.transform.forward = Camera.main.transform.forward;
            ObjectRotate = false;
            cameraControllTest.ResetCamera();
            uiManager.time = 1;
            Delete_3D_UI.SetActive(false);
        }
    }
    private void Update()
    {
        if (ObjectRotate && Object1_3D != null)
        {
            Object1_3D.transform.Rotate(0, Time.deltaTime * 30f, 0);
        }  
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            isPlayerInRange = true;
            //checkID = other.GetComponent<CheckID>();
            iTween.ScaleTo(sunBan, iTween.Hash(
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
            //if (targetFlower != null && targetFlower.uiManager != null)
            //{
            //    targetFlower.uiManager.HideFlowerInfo();
            //    checkID = null;
            //}
            //추후 끄는 버튼 생성
            iTween.ScaleTo(sunBan, iTween.Hash(
            "scale", RawScale,        // 목표 스케일 (1, 1, 1)
            "time", 0.3f,                // 애니메이션 시간 (조정 가능)
            "easeType", "easeInCirc", // 통통 튀는 느낌의 easeType
            "oncomplete", "OnCompleteOpening", // 애니메이션 완료 시 호출할 함수
            "oncompletetarget", gameObject
             ));
            cameraControllTest.ResetCamera();
            Delete_3D_UI.SetActive(false);
        }
    }
}
