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
    [SerializeField] private const int sproutEvolCount = 1;     // 10
    [SerializeField] private const int budEvolCount = 1;        // 20
    [SerializeField] private const int blossomEvolCount = 1;  // 30
    private ChatTestHttp points;

    [SerializeField] private Image flowerImage;
    [SerializeField] private Sprite[] flowerSprite;
    [SerializeField] private ParticleSystem evolutionEffect;
    
    // 애니메이션 관련 변수 추가
    [SerializeField] private float animationDuration = 1.0f;
    [SerializeField] private float startScale = 0.1f;
    private Vector3 originalScale;

    private HoonSoundManagerLogin sound;
    int count = 0;
    private void Start()
    {
        
        flower = GetComponent<Flower>();
        points = GameObject.Find("PointsManager").GetComponent<ChatTestHttp>();
        Flower.States newState = flower.curState;

        // 각 꽃의 원래 스케일 저장
        foreach (GameObject flowerObj in flowers)
        {
            if (flowerObj != null)
            {
                originalScale = flowerObj.transform.localScale;
            }
        }
        count++;
        
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
        print("꽃이름 : " + flower.gameObject.name + ", 카운트 : " + flower.evolutionCount + "몇번? : " + count);
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
        print("꽃이름 : " + flower.gameObject.name + ", 카운트 : " + flower.evolutionCount + "몇번? : " + count);
        GameObject targetFlower = null;

        // 현재 상태에 맞는 꽃 오브젝트만 활성화
        switch (state)
        {
            case Flower.States.SEED:
                flowerImage.sprite = flowerSprite[0];
                targetFlower = flowers[0];
                break;
            case Flower.States.SPROUT:
                flowerImage.sprite = flowerSprite[1];
                targetFlower = flowers[1];
                break;
            case Flower.States.BUD:
                flowerImage.sprite = flowerSprite[2];
                targetFlower = flowers[2];
                break;
            case Flower.States.BLOSSOM:
                flowerImage.sprite = flowerSprite[3];
                targetFlower = flowers[3];
                break;
        }

        if (targetFlower != null)
        {
            targetFlower.SetActive(true);
            // 초기 스케일을 작게 설정
            targetFlower.transform.localScale = originalScale * startScale;

            // iTween으로 스케일 애니메이션 실행
            iTween.ScaleTo(targetFlower, iTween.Hash(
                "scale", originalScale,
                "time", animationDuration,
                "easetype", iTween.EaseType.easeOutElastic
            ));
        }
    }

    public void CheckEvolutionCount(bool isFirst)
    {
        print("꽃이름 : " + flower.gameObject.name + ", 카운트 : " + flower.evolutionCount + "몇번? : " + count);
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
        print("꽃이름 : " + flower.gameObject.name + ", 카운트 : " + flower.evolutionCount + "몇번? : " + count);
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
        print("꽃이름 : " + flower.gameObject.name + ", 카운트 : " + flower.evolutionCount + "몇번? : " + count);
        flower.curState = newState; // 현재 상태 즉시 업데이트
        photonView.RPC("RPC_SyncFlowerState", RpcTarget.All, newState);
        StartCoroutine(EvolutionAnimation(newState, isFirst));
    }

    public void NewFlower()
    {
        StartCoroutine(PostNewSeed(() => {
            StartEvolution(Flower.States.SEED, false);
            points.AddPoints(0);
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
        print("꽃이름 : " + flower.gameObject.name + ", 카운트 : " + flower.evolutionCount + "몇번? : " + count);
        // 최초 씬이 시작될 때가 아닐 때만 사운드와 이펙트 재생
        if (!isFirst)
        {
            // 사운드 재생
            sound.PlaySound("smjAudioClopAttay", 3);

            // 진화 이펙트 재생
            if (evolutionEffect != null)
            {
                evolutionEffect.Stop();
                evolutionEffect.Clear();
                evolutionEffect.Play();
            }
            // 진화 완료 후 UI 업데이트
            FlowerUIManager uiManager = GetComponent<FlowerUIManager>();
            if (uiManager != null)
            {
                uiManager.UpdateUI(flower);
            }
            // 연출 대기 (애니메이션 시간만큼)
            yield return new WaitForSeconds(animationDuration);
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