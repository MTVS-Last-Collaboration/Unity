using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JSW_DecoMineFloor : JSW_DecoMineObject
{
    public int ResourcesName;
    public DecoMineManager decoMineManager;
    //public bool isPurchased;
    //public TMP_Text isMineText;


    private void Awake()
    {
        decoMineManager = GameObject.Find("DecoMineManager").GetComponent<DecoMineManager>();
    }

    private void Start()
    {
        isMineText = transform.transform.GetChild(2).GetComponent<TMP_Text>();
    }
    public void OnClickMineDeco()
    {
        if (isPurchased)
        {
            decoMineManager.changeFloor(ResourcesName);
        }
    }
}
