using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEvolution : MonoBehaviour
{
    private Flower flower;

    public GameObject[] flowers = new GameObject[3];
    [SerializeField] private const int budEvolCount = 1; //10
    [SerializeField] private const int blossomEvolCount = 1; //30
    private GoodsManager goodsManager;
    private void Start()
    {
        flower = GetComponent<Flower>();
        goodsManager = GoodsManager.Instance;
    }

    public void CheckEvolutionCount()
    {
        
        if (flower.evolutionCount >= blossomEvolCount)
        {
            flower.curState = Flower.States.BLOSSOM;
            StartCoroutine(Evolution(flower.curState));
        }
        else if (flower.evolutionCount >= budEvolCount)
        {
            flower.curState = Flower.States.BUD;
            StartCoroutine(Evolution(flower.curState));

        }
    }

    public void NewFlower()
    {
        flower.curState = Flower.States.SPROUT;
        StartCoroutine(Evolution(flower.curState));
        goodsManager.IncreaseCoin(flower.harvestCoins);
    }

    IEnumerator Evolution(Flower.States state)
    {
        for (int i = 0; i < flowers.Length; i++)
        {
            if (flowers[i].activeSelf)
            {
                flowers[i].SetActive(false);
                break;
            }
        }

        flower.isTouchAble = false;

        yield return new WaitForSeconds(1f);
        //추후 연출 추가
        //꽃 모델 교체
        switch (state)
        {
            case Flower.States.SPROUT:
                flowers[0].SetActive(true);
                break;
            case Flower.States.BUD:
                flowers[1].SetActive(true);
                break;
            case Flower.States.BLOSSOM:
                flowers[2].SetActive(true);
                break;
        }
        flower.isTouchAble = true;
    }
}
