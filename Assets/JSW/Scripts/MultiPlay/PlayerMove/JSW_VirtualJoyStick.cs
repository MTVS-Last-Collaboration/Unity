using ExitGames.Client.Photon;
// PhotonView를 사용하기 위해 추가
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems; //키보드, 마우스 , 터치를 이벤트로 오브젝트에 보낼 수 있는 기능 지원


public class JSW_VirtualJoyStick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static JSW_VirtualJoyStick instance;
    public JSW_ButtonInter buttonInter;
    public JSW_DecorateRoomManager DecorateRoomManager;

    [SerializeField] //private 라도 에디터의 인스펙터 뷰에서 레버를 넣어줄수 있게 직열화
    private RectTransform lever;
    private RectTransform rectTransform;

    [SerializeField, Range(10, 150)] //레버가 움직일수 있는 범위를 지정, 인스펙터에 공개
    private float leverRange = 0f;

    //인풋 디렉션 변수
    public Vector3 inputDirection;
    private bool isInput;

    private float pressTime = 0f; // 버튼이 눌린 시간
    private float pressX = 0f;
    private float pressY = 0f;

    [SerializeField]
    private JSW_PlayerMove playerMoveControl;
    public PhotonView playerPhotonView;
    public JSW_PlayerDecorate playerDecorate;


    private void Awake()
    {
        instance = this;
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
        if (playerMoveControl != null)
        {
            playerMoveControl.PlayerMoveJoyStick(Vector3.zero);
        }

    }
    //중복되는 인풋 코드를 함수화하기.
    Vector3 worldInputDirection;
    private void ControllJoyStickLever(PointerEventData eventData)
    {
        var inputPos = eventData.position - rectTransform.anchoredPosition; //레버의 위치를 구하는 코드
        //인풋포스의 길이와 레퍼레인지를 비교하고 레버레인지가 짧으면 바로적용, 길면 인풋포스를 정규화 하고 인풋 포스를 곱하자.
        var inputVector = inputPos.magnitude < leverRange ? inputPos : inputPos.normalized * leverRange;
        lever.anchoredPosition = inputVector; //조건에 따라 레버를 움직이게하자.
        //inputVector는 해상도로 만들어진값으로 캐릭터 이동속도에 적합하지않음. leverRange로 나누어 0~1 값으로 정규화 하여 이용하자.
        //캐릭터 정규화된 이동 벡터에 이동속도, 시간을 곱해서 이동하게 하자.
        inputDirection = inputVector / leverRange;
        // 카메라의 방향 벡터 계산
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0;
        camRight.y = 0;
        // 카메라의 forward와 right 벡터의 y축 값은 그대로 사용 (수직 방향 포함)
        camForward.Normalize();
        camRight.Normalize();

        
        // 정규화된 입력값을 카메라의 방향으로 변환
        worldInputDirection = (camForward * inputDirection.y + camRight * inputDirection.x).normalized;
        inputDirection.y = worldInputDirection.z;
        inputDirection.x = worldInputDirection.x;
    }

    public bool locking;
    public void lockingTest()
    {
        locking = true;
    }
    public void lockinnTest2()
    {
        locking = false;
    }
    private void IntputControllVector()
    {
        //캐릭터에게 입력 백터를 전달
        //Debug.Log(inputDirection.x + "/" + inputDirection.y);
        //무브쪽으로 보내자.

        // 테스트 용


        if (playerMoveControl != null)
        {
            if (buttonInter.isPressing || locking || playerDecorate.IsCharacterMoving)
            {
                if (buttonInter.isSettingFuniture || locking || playerDecorate.IsCharacterMoving)
                {
                    float x = inputDirection.x;
                    float y = inputDirection.y;

                    print("Inpux " + x + " INputy" + y + "x" + pressX + " y " + pressY);
                    if (x > 0.1f && y > -0.5f && y < 0.5f) //오른쪽
                    {
                        if (pressX < 0) pressX = 0;
                        pressX += Time.deltaTime;
                    }
                    else if (x < -0.1f && y > -0.5f && y < 0.5f) //왼쪽
                    {
                        if (pressX > 0) pressX = 0;
                        pressX -= Time.deltaTime;
                    }
                    else if (y > 0.1f && x > -0.5f && x < 0.5f) //위
                    {
                        if (pressY < 0) pressY = 0;
                        pressY += Time.deltaTime;
                    }
                    else if (y < -0.1f && x > -0.5f && x < 0.5f) //아래
                    {
                        if (pressY > 0) pressY = 0;
                        pressY -= Time.deltaTime;
                    }
                    if (pressX >= 1.0f)
                    {
                        pressX = 0;
                        pressY = 0;
                        playerDecorate.isPushorPull(1);
                        // 오른쪽 옮기기

                    }
                    else if (pressX <= -1.0f)
                    {
                        pressX = 0;
                        pressY = 0;
                        playerDecorate.isPushorPull(3);
                        // 왼쪽 옮기기

                    }
                    else if (pressY >= 1.0f)
                    {
                        pressX = 0;
                        pressY = 0;
                        playerDecorate.isPushorPull(0);
                        // 위쪽 옮기기

                    }
                    else if (pressY <= -1.0f)
                    {
                        pressX = 0;
                        pressY = 0;
                        playerDecorate.isPushorPull(2);
                        // 아래쪽 옮기기
                    }
                }
                else
                {
                    pressX = 0;
                    pressY = 0;
                }
            }
            else
            {
                if (!playerDecorate.IsCharacterMoving)
                {
                    playerMoveControl.PlayerMoveJoyStick(inputDirection);
                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        if (playerMoveControl == null)
        {
            print("플레이어 무브 없음");
            StartCoroutine(PlayerMoveControll());
        }

        if (playerPhotonView == null)
        {
            print("플레이어 포톤뷰 없음");
            StartCoroutine(PlayerPhotionView());
        }

        if (playerDecorate == null)
        {
            print("플레이어 꾸미기 없음");
            StartCoroutine(PlayerDecorate());
        }

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            locking = true;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            locking = false;
        }
        //print("플레이어 움직이게 하자.");
        if (playerPhotonView != null && playerPhotonView.IsMine && isInput)
        {
            IntputControllVector();
           
        }


    }

    IEnumerator PlayerMoveControll()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            // Hierarchy에 있는 모든 활성화된 오브젝트 탐색
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            print("PlayerMoveControll Finding");
            foreach (GameObject obj in allObjects)
            {
                // PhotonView 컴포넌트가 있는지 확인
                PhotonView photonView = obj.GetComponent<PhotonView>();

                // PhotonView가 있고, isMine이 true인 경우
                if (photonView != null && photonView.IsMine && obj.name.Contains("JSW_Player") )
                {
                    // PlayerMove 컴포넌트를 가져옴
                    playerMoveControl = obj.GetComponent<JSW_PlayerMove>();
                    //print("내 playerMoveControl 찾았다" + obj.name);
                    break;
                }

            }
            if (playerMoveControl != null)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        //playerMoveControl = GameObject.Find("PlayerWoman(Clone)").GetComponent<PlayerMoveTest>();
        //playerMoveControl = LobbyGameManager.instance.player.gameObject.GetComponent<PlayerMoveTest>();
    }

    IEnumerator PlayerDecorate()
    {
        yield return new WaitForSeconds(1f);

        while(true) {
            // Hierarchy에 있는 모든 활성화된 오브젝트 탐색
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            print("PlayerDecorate Finding");
            foreach (GameObject obj in allObjects)
            {
                // PhotonView 컴포넌트가 있는지 확인
                PhotonView photonView = obj.GetComponent<PhotonView>();

                // PhotonView가 있고, isMine이 true인 경우
                if (photonView != null && photonView.IsMine && obj.name.Contains("JSW_Player"))
                {
                    print("fadsfdae qrgfvcasv " + obj.name);
                    // PlayerMove 컴포넌트를 가져옴
                    playerDecorate = obj.GetComponent<JSW_PlayerDecorate>();
                    //print("내 playerMoveControl 찾았다" + obj.name);
                    break;
                }
            }

            if (playerDecorate != null)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
        

        //playerMoveControl = GameObject.Find("PlayerWoman(Clone)").GetComponent<PlayerMoveTest>();
        //playerMoveControl = LobbyGameManager.instance.player.gameObject.GetComponent<PlayerMoveTest>();
    }

    IEnumerator PlayerPhotionView()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            print("PlayerPhotionView Finding");
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // PhotonView 컴포넌트가 있는지 확인
                PhotonView photonView = obj.GetComponent<PhotonView>();

                // PhotonView가 있고, isMine이 true인 경우
                if (photonView != null && photonView.IsMine && obj.name.Contains("JSW_Player") )
                {
                    playerPhotonView = obj.GetComponent<PhotonView>();
                    //print("내 포톤뷰 찾았다.");
                    break;
                }
            }
            if (playerPhotonView != null)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }        // Hierarchy에 있는 모든 활성화된 오브젝트 탐색


        //playerPhotonView = GameObject.Find("PlayerWoman(Clone)").GetComponent<PhotonView>();
        //playerPhotonView = LobbyGameManager.instance.player.gameObject.GetComponent<PhotonView>();
    }


}
