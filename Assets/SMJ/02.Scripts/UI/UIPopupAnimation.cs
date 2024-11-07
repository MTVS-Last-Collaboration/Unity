using UnityEngine;
using UnityEngine.UI;

public class UIPopupAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float startScale = 0.1f;
    [SerializeField] private float targetScale = 1.0f;
    [SerializeField] private float popupTime = 0.4f;
    [SerializeField] private float overshootScale = 1.2f;
    [SerializeField] private float settleTime = 0.5f;

    // 애니메이션을 적용할 UI RectTransform 저장
    private RectTransform targetUI;
    private Vector3 originalScale;

    // UI 요소를 받아서 애니메이션 설정 초기화
    public void SetTarget(RectTransform uiElement)
    {
        targetUI = uiElement;
        if (targetUI != null)
        {
            originalScale = targetUI.localScale;
        }
    }

    public void PlayPopupAnimation(RectTransform uiElement)
    {
        // 새로운 타겟 설정
        SetTarget(uiElement);

        if (targetUI == null) return;

        // 이전 애니메이션 중지
        iTween.Stop(targetUI.gameObject);

        // 시작 크기로 설정
        targetUI.localScale = Vector3.one * startScale;

        // 첫 번째 애니메이션: 빠르게 커지면서 약간 더 크게
        iTween.ScaleTo(targetUI.gameObject, iTween.Hash(
            "scale", Vector3.one * overshootScale,
            "time", popupTime,
            "easetype", iTween.EaseType.easeOutQuart,
            "oncomplete", "StartBounceAnimation",
            "oncompletetarget", gameObject
        ));
    }

    private void StartBounceAnimation()
    {
        if (targetUI == null) return;

        // 두 번째 애니메이션: 튀어오른 상태에서 정상 크기로 돌아오면서 출렁임
        iTween.ScaleTo(targetUI.gameObject, iTween.Hash(
            "scale", Vector3.one * targetScale,
            "time", settleTime,
            "easetype", iTween.EaseType.easeOutElastic,
            "oncomplete", "OnAnimationComplete",
            "oncompletetarget", gameObject
        ));
    }

    public bool OnAnimationComplete()
    {
        return true;
    }

    public void Hide(RectTransform uiElement)
    {
        if (uiElement == null) return;

        iTween.Stop(uiElement.gameObject);
        iTween.ScaleTo(uiElement.gameObject, iTween.Hash(
            "scale", Vector3.zero,
            "time", 0.2f,
            "easetype", iTween.EaseType.easeInBack
        ));
    }
}