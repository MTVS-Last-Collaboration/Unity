using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickFlower : MonoBehaviour
{
    private Flower targetFlower;
    private bool isPlayerInRange = false;
    private CheckID checkID;

    private void Start()
    {
        targetFlower = GetComponent<Flower>();
    }

    private void Update()
    {
        // 터치 입력 처리
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                CheckInteraction(Input.GetTouch(0).position);
            }
        }
        // 마우스 클릭 처리 (에디터 및 데스크톱용)
        else if (Input.GetMouseButtonDown(0))
        {
            CheckInteraction(Input.mousePosition);
        }
    }

    private void CheckInteraction(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            HandleInteraction();
        }
    }

    private void HandleInteraction()
    {
        //추후 클릭 성공 시 플레이어 움직임 막기
        if (isPlayerInRange && targetFlower != null && targetFlower.uiManager != null)
        {
            if (checkID != null)
            {
                if (checkID.IsMine(targetFlower) == true)
                {
                    //talkText
                    //연인에게 따뜻한 한마디 말하기
                    targetFlower.uiManager.ShowFlowerInfo(targetFlower, 0);
                    //녹음 완료시 + "(완료)" 추가 및 버튼액션 비활성화
                    //targetFlower.uiManager.UpdateButtonInteractable(true, 0);
                }
                else
                {
                    if (targetFlower.voiceClip == null) 
                    {
                        //voiceNullText
                        //아직 따뜻하지 않아요...
                        targetFlower.uiManager.ShowFlowerInfo(targetFlower, 1);
                    }
                    else
                    {
                        //resultText
                        //연인의 말한마디 듣기
                        targetFlower.uiManager.ShowFlowerInfo(targetFlower, 2);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            checkID = other.GetComponent<CheckID>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}