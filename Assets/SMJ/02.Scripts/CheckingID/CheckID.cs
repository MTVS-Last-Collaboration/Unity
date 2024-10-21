using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckID : MonoBehaviour
{
    IDHandler idHandler;
    [SerializeField] private bool isFirst = false;
    private void Start()
    {
        idHandler = GetComponent<IDHandler>();
    }
    public bool IsMine(Flower flower)
    {
        if (flower.managerId == "" && isFirst == false)
        {
            //수확 완료 후 false
            isFirst = true;
            flower.managerId = idHandler.ID;
        }
        if (flower.managerId == idHandler.ID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ResetFirst()
    {
        isFirst = false;
    }
}