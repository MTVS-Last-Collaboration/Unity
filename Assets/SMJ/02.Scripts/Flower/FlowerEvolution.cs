using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FlowerEvolution : MonoBehaviourPun
{
    [System.Serializable]
    private class ResponseData
    {
        public int moodCount;
    }
    private Flower flower;
    public GameObject[] flowers = new GameObject[3];
    [SerializeField] private const int sproutEvolCount = 1; //10
    [SerializeField] private const int budEvolCount = 1; //20
    [SerializeField] private const int blossomEvolCount = 1; //30
    private ChatTestHttp points;

    [SerializeField] private Image flowerImage;
    [SerializeField] private Sprite[] flowerSprite;

    [SerializeField] private ParticleSystem evolutionEffect;

    private HoonSoundManagerLogin sound;

    private void Start()
    {
        flower = GetComponent<Flower>();
        points = GameObject.Find("PointsManager").GetComponent<ChatTestHttp>();
        Flower.States newState = flower.curState;

        if (flower.evolutionCount >= blossomEvolCount)
        {
            newState = Flower.States.BLOSSOM;
            StartEvolution(newState, true);
        }
        else if (flower.evolutionCount >= budEvolCount)
        {
            newState = Flower.States.BUD;
            StartEvolution(newState, true);
        }
        else if (flower.evolutionCount >= sproutEvolCount)
        {
            newState = Flower.States.SPROUT;
            StartEvolution(newState, true);
        }
        sound = GameObject.Find("SMJ").GetComponent<HoonSoundManagerLogin>();
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

    public void CheckEvolutionCount(bool isFirst)
    {
        if (isFirst)
        {
            // 최초 로딩 시에는 즉시 해당 상태로 변경
            if (flower.evolutionCount >= blossomEvolCount)
            {
                StartEvolution(Flower.States.BLOSSOM, true);
            }
            else if (flower.evolutionCount >= budEvolCount)
            {
                StartEvolution(Flower.States.BUD, true);
            }
            else if (flower.evolutionCount >= sproutEvolCount)
            {
                StartEvolution(Flower.States.SPROUT, true);
            }
        }
        else
        {
            // 순차적 진화를 위한 코루틴 시작
            StartCoroutine(SequentialEvolution());
        }
    }

    private IEnumerator SequentialEvolution()
    {
        // SEED에서 시작
        if (flower.curState == Flower.States.SEED && flower.evolutionCount >= sproutEvolCount)
        {
            StartEvolution(Flower.States.SPROUT, false);
            yield return new WaitForSeconds(2f); // 진화 애니메이션 + 대기 시간
        }

        // SPROUT 진화
        if (flower.curState == Flower.States.SPROUT && flower.evolutionCount >= budEvolCount)
        {
            StartEvolution(Flower.States.BUD, false);
            yield return new WaitForSeconds(2f);
        }

        // BUD 진화
        if (flower.curState == Flower.States.BUD && flower.evolutionCount >= blossomEvolCount)
        {
            StartEvolution(Flower.States.BLOSSOM, false);
            yield return new WaitForSeconds(2f);
        }
    }

    private void StartEvolution(Flower.States newState, bool isFirst)
    {
        flower.curState = newState; // 현재 상태 즉시 업데이트
        photonView.RPC("RPC_SyncFlowerState", RpcTarget.All, newState);
        StartCoroutine(EvolutionAnimation(newState, isFirst));
    }

    public void NewFlower()
    {
        StartCoroutine(PostNewSeed(() => {
            StartEvolution(Flower.States.SEED, false);
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
                    try
                    {
                        // JSON 파싱
                        ResponseData data = JsonUtility.FromJson<ResponseData>(response);
                        GetComponent<FlowerUIManager>().recordCount = data.moodCount;

                        Debug.Log($"Mood count updated: {data.moodCount}");
                        onComplete?.Invoke();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to parse response: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to new seed: {response}");
                }
            });
    }

    IEnumerator EvolutionAnimation(Flower.States state, bool isFirst)
    {
        // 최초 씬이 시작될 때가 아닐 때만 사운드와 이펙트 재생
        if (!isFirst)
        {
            // 사운드 재생
            sound.PlaySound("smjAudioClopAttay", 3);

            // 진화 이펙트 재생
            if (evolutionEffect != null)
            {
                // 기존 이펙트 정지 및 초기화
                evolutionEffect.Stop();
                evolutionEffect.Clear();

                // 이펙트 재시작
                evolutionEffect.Play();
            }

            //연출 대기
            yield return new WaitForSeconds(1f);
        }

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