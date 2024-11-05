using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickPositionHandler : MonoBehaviour, IPointerClickHandler
{
    public RectTransform imageRectTransform;


    //// 중심 기준
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    if (imageRectTransform == null)
    //    {
    //        Debug.LogError("imageRectTransform이 할당되지 않았습니다.");
    //        return;
    //    }

    //    // 클릭한 위치를 이미지의 로컬 좌표로 변환
    //    Vector2 localPoint;
    //    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        imageRectTransform,
    //        eventData.position,
    //        eventData.pressEventCamera,
    //        out localPoint))
    //    {
    //        // 로컬 좌표 출력 (이미지의 중심을 (0, 0)으로 기준)
    //        Debug.Log("이미지 기준 클릭한 위치: " + localPoint);
    //    }
    //    else
    //    {
    //        Debug.LogWarning("로컬 좌표로 변환에 실패했습니다.");
    //    }
    //}

    public void OnPointerClick(PointerEventData eventData)
    {
        if (imageRectTransform == null)
        {
            Debug.LogError("imageRectTransform이 할당되지 않았습니다.");
            return;
        }

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            imageRectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // 좌측 하단 기준으로 변환
            Vector2 bottomLeftPoint = new Vector2(
                localPoint.x + (imageRectTransform.rect.width / 2),
                localPoint.y + (imageRectTransform.rect.height / 2)
            );

            Debug.Log("좌측 하단 기준 클릭한 위치: " + bottomLeftPoint);
        }
        else
        {
            Debug.LogWarning("로컬 좌표로 변환에 실패했습니다.");
        }
    }
}
