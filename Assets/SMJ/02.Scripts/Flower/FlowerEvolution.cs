using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FlowerEvolution : MonoBehaviourPun
{
    private Flower flower;
    public GameObject[] flowers = new GameObject[3];
    [SerializeField] private const int sproutEvolCount = 1; //10
    [SerializeField] private const int budEvolCount = 1; //20
    [SerializeField] private const int blossomEvolCount = 1; //30
    private ChatTestHttp points;

    [SerializeField] private Image flowerImage;
    [SerializeField] private Sprite[] flowerSprite;

    private void Start()
    {
        flower = GetComponent<Flower>();
        points = GameObject.Find("PointsManager").GetComponent<ChatTestHttp>();
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
            case Flower.States.SEED:
                flowerImage.sprite = flowerSprite[0];
                flowers[0].SetActive(true);
                break;
            case Flower.States.SPROUT:
                flowerImage.sprite = flowerSprite[1];
                flowers[1].SetActive(true);
                break;
            case Flower.States.BUD:
                flowerImage.sprite = flowerSprite[2];
                flowers[2].SetActive(true);
                break;
            case Flower.States.BLOSSOM:
                flowerImage.sprite = flowerSprite[3];
                flowers[3].SetActive(true);
                break;
        }
    }

    public void CheckEvolutionCount()
    {
        // IsMine 체크 제거 - 모든 클라이언트에서 진화 체크 가능하도록
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
        else if (flower.evolutionCount >= sproutEvolCount)
        {
            newState = Flower.States.SPROUT;
            StartEvolution(newState);
        }
    }

    private void StartEvolution(Flower.States newState)
    {
        // 모든 클라이언트에서 진화 상태 동기화
        photonView.RPC("RPC_SyncFlowerState", RpcTarget.All, newState);
        StartCoroutine(EvolutionAnimation(newState));
    }

    public void NewFlower()
    {
        StartCoroutine(PostNewSeed(() => {
            StartEvolution(Flower.States.SEED);
            points.AddPoints(10);
            //추후 코인 연출
        }));
    }
    private IEnumerator PostNewSeed(Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.PostWithoutBody($"/api/flower/new-seed",
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("New seed successfully updated");
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to new seed: {response}");
                }
            });
    }

    IEnumerator EvolutionAnimation(Flower.States state)
    {
        //이펙트
        yield return new WaitForSeconds(1f);

        // 진화 완료 후 UI 업데이트
        FlowerUIManager uiManager = GetComponent<FlowerUIManager>();
        if (uiManager != null)
        {
            uiManager.UpdateUI(flower);
        }
    }

    [PunRPC]
    private void RPC_OnEvolutionComplete(Flower.States state)
    {
        // UI 매니저에 진화 완료 알림
        FlowerUIManager uiManager = GetComponent<FlowerUIManager>();
        if (uiManager != null)
        {
            uiManager.OnEvolutionComplete(state);
        }
    }
}