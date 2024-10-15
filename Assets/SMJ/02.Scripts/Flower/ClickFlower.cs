using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFlower : MonoBehaviour
{
    private Flower targetFlower;
    private bool isPlayerInRange = false;
    private CheckID checkID;

    private void Start()
    {
        targetFlower = GetComponent<Flower>();
    }

    private void OnMouseDown()
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
            //if (targetFlower != null && targetFlower.uiManager != null)
            //{
            //    targetFlower.uiManager.HideFlowerInfo();
            //    checkID = null;
            //}
            //추후 끄는 버튼 생성
        }
    }
}