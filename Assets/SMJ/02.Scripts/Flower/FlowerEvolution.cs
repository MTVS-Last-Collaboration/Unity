using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class FlowerEvolution : MonoBehaviourPun
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
        SyncFlowerState(flower.curState);
    }

    [PunRPC]
    private void RPC_SyncFlowerState(Flower.States state)
    {
        flower.curState = state;
        SyncFlowerState(state);
    }

    private void SyncFlowerState(Flower.States state)
    {
        // 모든 꽃 오브젝트를 비활성화
        foreach (GameObject flowerObj in flowers)
        {
            flowerObj.SetActive(false);
        }

        // 현재 상태에 맞는 꽃 오브젝트만 활성화
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
    }

    public void CheckEvolutionCount()
    {
        if (!photonView.IsMine) return; // 소유자만 진화 체크 가능

        Flower.States newState = flower.curState;
        if (flower.evolutionCount >= blossomEvolCount)
        {
            newState = Flower.States.BLOSSOM;
            StartEvolution(newState);
        }
        else if (flower.evolutionCount >= budEvolCount)
        {
            newState = Flower.States.BUD;
            StartEvolution(newState);
        }
    }

    private void StartEvolution(Flower.States newState)
    {
        photonView.RPC("RPC_SyncFlowerState", RpcTarget.All, newState);
        StartCoroutine(EvolutionAnimation(newState));
    }

    public void NewFlower()
    {
        if (!photonView.IsMine) return;

        StartEvolution(Flower.States.SPROUT);
        goodsManager.IncreaseCoin(flower.harvestCoins);
    }

    IEnumerator EvolutionAnimation(Flower.States state)
    {
        flower.isTouchAble = false;
        yield return new WaitForSeconds(1f);

        // 애니메이션이 끝난 후 상태 동기화
        flower.isTouchAble = true;
    }
}