using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; //키보드, 마우스 , 터치를 이벤트로 오브젝트에 보낼 수 있는 기능 지원

public class VirtualJoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] //에디터의 인스펙터 뷰에서 레버를 넣어줄수 있게 직열화
    private RectTransform lever;
    private RectTransform rectTransform;


    private void Awake()
    {
        //조이스틱 몸체를 캐싱
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var inputPos = eventData.position -rectTransform.anchoredPosition; //레버의 위치를 구하는 코드
        lever.anchoredPosition = inputPos;
        Debug.Log("Begin");
    }

    public void OnDrag(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition; //레버의 위치를 구하는 코드
        lever.anchoredPosition = inputPos;
        Debug.Log("Drag");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector2.zero; //원위치로 이동하게 변경
        Debug.Log("End");
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
