using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEvolution : MonoBehaviour
{
    private Flower flower;

    public GameObject[] flowers = new GameObject[3];
    public const int budEvolCount = 1; //10
    public const int blossomEvolCount = 3; //30

    private void Start()
    {
        flower = GetComponent<Flower>();
    }

    public void CheckEvolutionCount()
    {
        if (flower.evolutionCount >= budEvolCount)
        {
            flower.curState = Flower.States.BUD;
            StartCoroutine(Evolution(flower.curState));
        }
        else if (flower.evolutionCount >= blossomEvolCount)
        {
            flower.curState = Flower.States.BLOSSOM;
            StartCoroutine(Evolution(flower.curState));
        }
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
        yield return new WaitForSeconds(1f);
        //추후 연출 추가
        //꽃 모델 교체
        switch (state)
        {
            case Flower.States.SPROUT:
                break;
            case Flower.States.BUD:
                //여기서 바꾸기
                break;
            case Flower.States.BLOSSOM:
                break;
        }
    }
}
