using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBoard : MonoBehaviour
{
    [SerializeField] private GameObject boardUIObject;
    [SerializeField] private GameObject playerUIObject;
    public void HandleInteraction()
    {
        playerUIObject.SetActive(false);
        boardUIObject.SetActive(true);
    }

    public void ExitBoard()
    {
        playerUIObject.SetActive(true);
        boardUIObject.SetActive(false);
    }
}
