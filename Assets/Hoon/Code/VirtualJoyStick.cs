using ExitGames.Client.Photon;
using Photon.Pun.Demo.SlotRacer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems; //키보드, 마우스 , 터치를 이벤트로 오브젝트에 보낼 수 있는 기능 지원

public class VirtualJoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] //private 라도 에디터의 인스펙터 뷰에서 레버를 넣어줄수 있게 직열화
    private RectTransform lever;
    private RectTransform rectTransform;

    [SerializeField, Range(10,150)] //레버가 움직일수 있는 범위를 지정, 인스펙터에 공개
    private float leverRange = 0f;

    //인풋 디렉션 변수
    public Vector3 inputDirection;
    private bool isInput;

    [SerializeField]
    private PlayerMoveTest playerMoveControl;


    private void Awake()
    {
        //조이스틱 몸체를 캐싱
        rectTransform = GetComponent<RectTransform>();

    }
    //마우스 클릭되면 호출되는 함수
    public void OnBeginDrag(PointerEventData eventData)
    {

        ControllJoyStickLever(eventData);
        //Debug.Log("Begin");
        isInput = true;
    }
    //마우스 드래그되면 호출되는 함수
    //오브젝트를 클릭해서 드래그 하는 도중에 들어오는 이벤트
    //하지만 클릭을 유지한 상태로 마우스를 멈추면 이벤트가 들어오지 않음.
    //따라서 인풋로직을 받아서 움직이는 코드는 update 에서 구현
    public void OnDrag(PointerEventData eventData)
    {
        ControllJoyStickLever(eventData);
        //Debug.Log("Drag");
    }
    //마우스 입력이 끝나면 호출되는 함수
    public void OnEndDrag(PointerEventData eventData)
    {
        lever.anchoredPosition = Vector3.zero; //lever 원위치로 이동하게 변경
        //Debug.Log("End");
        isInput = false;
        //컨트롤러 입력값 삭제하기
        playerMoveControl.PlayerMoveJoyStick(Vector3.zero);
    }

    //중복되는 인풋 코드를 함수화하기.
    private void ControllJoyStickLever(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition; //레버의 위치를 구하는 코드
        //인풋포스의 길이와 레퍼레인지를 비교하고 레버레인지가 짧으면 바로적용, 길면 인풋포스를 정규화 하고 인풋 포스를 곱하자.
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector; //조건에 따라 레버를 움직이게하자.
        //inputVector는 해상도로 만들어진값으로 캐릭터 이동속도에 적합하지않음. leverRange로 나누어 0~1 값으로 정규화 하여 이용하자.
        //캐릭터 정규화된 이동 벡터에 이동속도, 시간을 곱해서 이동하게 하자.
        inputDirection = inputVector / leverRange; 
    }

    private void IntputControllVector()
    {
        //캐릭터에게 입력 백터를 전달
        //Debug.Log(inputDirection.x + "/" + inputDirection.y);
        //무브쪽으로 보내자.
        playerMoveControl.PlayerMoveJoyStick(inputDirection);
    }


    // Start is called before the first frame update
    void Start()
    {
       if(playerMoveControl == null)
        {
            StartCoroutine("PlayerMoveControll");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInput)
        {
            IntputControllVector();
        }
    }

    IEnumerator PlayerMoveControll()
    {
        yield return new WaitForSeconds(0.5f);
        playerMoveControl = GameObject.Find("PlayerWoman(Clone)").GetComponent<PlayerMoveTest>();
    }
}
