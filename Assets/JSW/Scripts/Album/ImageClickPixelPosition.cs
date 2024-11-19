using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ImageClickPixelPosition : MonoBehaviour, IPointerClickHandler
{
    public RawImage rawImage; // 클릭할 Image (RawImage를 예로 사용)
    private Texture2D texture; // 이미지의 Texture2D
    public Making3DObject making3DObject;


    void Start()
    {
        if (rawImage != null)
        {
            texture = rawImage.texture as Texture2D;

            if (texture == null)
            {
                Debug.LogError("RawImage에 Texture2D가 할당되지 않았습니다.");
            }
        }
        else
        {
            Debug.LogError("RawImage가 할당되지 않았습니다.");
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        texture = rawImage.texture as Texture2D;

        if (rawImage == null || texture == null)
            return;

        RectTransform rectTransform = rawImage.rectTransform;

        // Step 1: 클릭 위치를 로컬 좌표로 변환
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            // Step 2: 로컬 좌표를 [0, 1]의 정규화 좌표로 변환
            Rect rect = rectTransform.rect;
            float normalizedX = (localPoint.x - rect.x) / rect.width;
            float normalizedY = (localPoint.y - rect.y) / rect.height;

            // Step 3: 텍스처의 픽셀 좌표로 변환
            int pixelX = Mathf.Clamp((int)(normalizedX * texture.width), 0, texture.width - 1);
            int pixelY = Mathf.Clamp((int)(normalizedY * texture.height), 0, texture.height - 1);

            making3DObject.SetTouchPos(pixelX, pixelY);

            Debug.Log($"클릭한 위치의 픽셀 좌표: ({pixelX}, {pixelY})");
        }
    }
}