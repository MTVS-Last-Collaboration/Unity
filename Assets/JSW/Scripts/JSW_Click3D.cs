using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JSW_Click3D : MonoBehaviour
{

    public JSW_CameraControllTest cameraControllTest;
    public bool ObjectRotate;
    GameObject Object1_3D;

    // Start is called before the first frame update
    // Trigger 넣자
    void Awake()
    {
        cameraControllTest = GameObject.Find("PlayerPos").GetComponent<JSW_CameraControllTest>();
    }

    private void OnMouseDown()
    {
        //추후 클릭 성공 시 플레이어 움직임 막기
        if (cameraControllTest.cameraPos != "3D")
        {
            if (transform.GetChild(2).transform.gameObject!=null) {
                Object1_3D = transform.GetChild(2).transform.gameObject;
            }
            ObjectRotate = true;
            cameraControllTest.CameraTo3D();
        }
        else
        {
            Object1_3D.transform.forward = Camera.main.transform.forward;
            ObjectRotate = false;
            cameraControllTest.ResetCamera();
        }
    }
    private void Update()
    {
        if (ObjectRotate && Object1_3D != null)
        {
            Object1_3D.transform.Rotate(0, Time.deltaTime * 30f, 0);
        }  
    }
}
