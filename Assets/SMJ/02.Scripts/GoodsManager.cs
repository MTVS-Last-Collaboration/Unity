using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static FlowerUIManager;

public class GoodsManager : MonoBehaviour
{
    public int coin = 0;

    private static GoodsManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static GoodsManager Instance
    {
        get
        {
            if (instance == null)
            {
                return null;
            }
            return instance;
        }
    }

    public void IncreaseCoin(int amount)
    {
        coin += amount;
    }

    public void DecreaseCoin(int amount)
    {
        if (coin > 0)
        {
            coin -= amount;
            if (coin < 0)
            {
                coin = 0;
            }
        }
    }
}
