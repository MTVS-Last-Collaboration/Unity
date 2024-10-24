using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CheckID : MonoBehaviourPun
{
    IDHandler idHandler;
    [SerializeField] private bool isFirst = false;

    public bool IsMine(Flower flower)
    {
        if (idHandler == null)
        {
            idHandler = GetComponent<IDHandler>();
        }
        if (photonView.IsMine == true)
        {
            if (flower.managerId == "" && isFirst == false)
            {
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
        return false;
    }

    public void ResetFirst()
    {
        isFirst = false;
    }
}