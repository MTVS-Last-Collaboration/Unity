using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CoinCollectionEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private Canvas canvas;

    [Header("Effect Settings")]
    [SerializeField] private int coinCount = 10;
    [SerializeField] private float spreadRadius = 100f;
    [SerializeField] private float moveDelay = 0.1f;
    [SerializeField] private float scaleTime = 0.2f;
    [SerializeField] private float moveTime = 0.5f;
    [SerializeField] private float fadeTime = 0.3f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void PlayCoinEffect(Vector3 worldPosition)
    {
        // 시작 위치 (3D 월드 좌표 -> 스크린 좌표)
        Vector2 startScreenPos = mainCamera.WorldToScreenPoint(worldPosition);

        // 목표 UI의 스크린 좌표
        Vector2 targetScreenPos = RectTransformUtility.WorldToScreenPoint(null, targetUI.transform.position);

        // 스크린 좌표를 캔버스 내 로컬 좌표로 변환
        Vector2 startCanvasPos;
        Vector2 targetCanvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            startScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out startCanvasPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            targetScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out targetCanvasPos);

        StartCoroutine(SpawnCoinsCoroutine(startCanvasPos, targetCanvasPos));
    }

    private IEnumerator SpawnCoinsCoroutine(Vector2 startPos, Vector2 targetPos)
    {
        for (int i = 0; i < coinCount; i++)
        {
            GameObject coinObj = Instantiate(coinPrefab, canvas.transform);
            RectTransform coinRect = coinObj.GetComponent<RectTransform>();
            CanvasGroup canvasGroup = coinObj.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                canvasGroup = coinObj.AddComponent<CanvasGroup>();
            }

            coinRect.anchoredPosition = startPos;
            coinRect.localScale = Vector3.zero;

            Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
            Vector2 midPos = startPos + randomOffset;

            var coinMover = coinObj.AddComponent<CoinMover>();
            coinMover.Setup(coinRect, canvasGroup);

            StartCoroutine(AnimateCoin(coinObj, coinRect, canvasGroup, midPos, targetPos));

            yield return new WaitForSeconds(moveDelay);
        }
    }

    private IEnumerator AnimateCoin(GameObject coinObj, RectTransform coinRect, CanvasGroup canvasGroup, Vector2 midPos, Vector2 targetPos)
    {
        // 1. 스케일 업
        iTween.ScaleTo(coinObj, iTween.Hash(
            "scale", Vector3.one,
            "time", scaleTime,
            "easetype", iTween.EaseType.easeOutBack
        ));

        yield return new WaitForSeconds(scaleTime);

        // 2. 중간 위치로
        iTween.ValueTo(coinObj, iTween.Hash(
            "from", coinRect.anchoredPosition,
            "to", midPos,
            "time", moveTime / 2,
            "easetype", iTween.EaseType.easeOutQuad,
            "onupdate", "OnPositionUpdate"
        ));

        yield return new WaitForSeconds(moveTime / 2);

        // 3. 목표 위치로
        iTween.ValueTo(coinObj, iTween.Hash(
            "from", midPos,
            "to", targetPos,
            "time", moveTime / 2,
            "easetype", iTween.EaseType.easeInQuad,
            "onupdate", "OnPositionUpdate"
        ));

        yield return new WaitForSeconds(moveTime / 2);

        // 4. 스케일 다운 & 페이드 아웃
        iTween.ScaleTo(coinObj, iTween.Hash(
            "scale", Vector3.zero,
            "time", fadeTime,
            "easetype", iTween.EaseType.easeInBack
        ));

        iTween.ValueTo(coinObj, iTween.Hash(
            "from", 1f,
            "to", 0f,
            "time", fadeTime,
            "easetype", iTween.EaseType.easeInQuad,
            "onupdate", "OnAlphaUpdate"
        ));

        yield return new WaitForSeconds(fadeTime);

        Destroy(coinObj);
    }
}

// CoinMover 클래스는 이전과 동일

public class CoinMover : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public void Setup(RectTransform rect, CanvasGroup group)
    {
        rectTransform = rect;
        canvasGroup = group;
    }

    public void OnPositionUpdate(Vector2 pos)
    {
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = pos;
        }
    }

    public void OnAlphaUpdate(float alpha)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}