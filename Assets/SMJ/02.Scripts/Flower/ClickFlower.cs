using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFlower : MonoBehaviour
{
    private Flower targetFlower;
    private bool isPlayerInRange = false;

    private void Start()
    {
        targetFlower = GetComponent<Flower>();
    }

    private void OnMouseDown()
    {
        if (isPlayerInRange && targetFlower != null && targetFlower.uiManager != null)
        {
            targetFlower.uiManager.ShowFlowerInfo(targetFlower);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (targetFlower != null && targetFlower.uiManager != null)
            {
                targetFlower.uiManager.HideFlowerInfo();
            }
        }
    }
}