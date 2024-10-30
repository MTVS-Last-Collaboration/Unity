using ExitGames.Client.Photon;
// PhotonView를 사용하기 위해 추가
using Photon.Pun;
using Photon.Pun.Demo.SlotRacer;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems; //키보드, 

public class JSW_ButtonInter : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public float longPressDuration = 1.0f; // 길게 누른 시간을 구분하기 위한 기준 (예: 1초)
    public bool isPressing = false; // 버튼이 눌렸는지 여부
    public bool isSettingFuniture = false;
    private float pressTime = 0f; // 버튼이 눌린 시간


    [SerializeField]
    private JSW_PlayerDecorate playerDecorate;

    public PhotonView playerPhotonView;

    public JSW_VirtualJoyStick virtualJoyStick;

    public float doubleClickTime = 0.3f; // 더블 클릭으로 인식할 시간 간격

    private float lastClickTime = 0f; // 마지막 클릭 시간을 기록할 변수

    // 버튼이 눌렸을 때 호출되는 메서드
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true; // 버튼이 눌렸음을 표시
        pressTime = 0f; // 누른 시간 초기화
    }

    // 버튼이 떼어졌을 때 호출되는 메서드
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false; // 버튼을 떼었음을 표시

        // 버튼이 길게 눌렸는지 확인
        if (pressTime >= longPressDuration)
        {
            LongPress(); // 길게 눌렀을 때의 동작 실행
        }
        else
        {
            ShortPress(); // 짧게 눌렀을 때의 동작 실행
        }

        pressTime = 0f; // 누른 시간 초기화


        // 더블 클릭 관련 
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickTime && virtualJoyStick.locking == false)
        {
            virtualJoyStick.locking = true;
        }
        else if (timeSinceLastClick <= doubleClickTime && virtualJoyStick.locking == true)
        {
            virtualJoyStick.locking = false;
        }
        lastClickTime = Time.time;
    }

    // 매 프레임마다 호출되는 메서드
    void Update()
    {
        if (isPressing) // 버튼이 눌린 상태라면
        {
            if (pressTime >= 1f)
            {
                isSettingFuniture = true;
                if (!playerDecorate.IsCharacterMoving) playerDecorate.PushFunitureSetting();
            }
            pressTime += Time.deltaTime; // 누른 시간을 누적
        }
        else
        {
            isSettingFuniture = false;
            pressTime = 0f;
        }
    }

    private void Start()
    {
        if (playerDecorate == null)
        {
            print("플레이어 무브 없음");
            StartCoroutine(PlayerDeco());
        }

        if (playerPhotonView == null)
        {
            print("플레이어 포톤뷰 없음");
            StartCoroutine(PlayerPhotionView());
        }
    }

    // 버튼을 길게 눌렀을 때 실행되는 메서드
    private void LongPress()
    {
        Debug.Log("Button was pressed for a long time!");
        // 길게 눌렀을 때 실행할 동작 추가
    }

    // 버튼을 짧게 눌렀을 때 실행되는 메서드
    private void ShortPress()
    {
        Debug.Log("Button was pressed for a short time!");
        // 짧게 눌렀을 때 실행할 동작 추가
    }


    IEnumerator PlayerDeco()
    {
        yield return new WaitForSeconds(1.0f);

        // Hierarchy에 있는 모든 활성화된 오브젝트 탐색
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // PhotonView 컴포넌트가 있는지 확인
            PhotonView photonView = obj.GetComponent<PhotonView>();

            // PhotonView가 있고, isMine이 true인 경우
            if (photonView != null && photonView.IsMine)
            {
                // PlayerDecorate 컴포넌트를 가져옴
                playerDecorate = obj.GetComponent<JSW_PlayerDecorate>();
                //print("내 playerMoveControl 찾았다" + obj.name);
                break;
            }

        }

        //playerMoveControl = GameObject.Find("PlayerWoman(Clone)").GetComponent<PlayerMoveTest>();
        //playerMoveControl = LobbyGameManager.instance.player.gameObject.GetComponent<PlayerMoveTest>();
    }


    IEnumerator PlayerPhotionView()
    {
        yield return new WaitForSeconds(1.0f);

        // Hierarchy에 있는 모든 활성화된 오브젝트 탐색
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            // PhotonView 컴포넌트가 있는지 확인
            PhotonView photonView = obj.GetComponent<PhotonView>();

            // PhotonView가 있고, isMine이 true인 경우
            if (photonView != null && photonView.IsMine)
            {
                playerPhotonView = obj.GetComponent<PhotonView>();
                //print("내 포톤뷰 찾았다.");
                break;
            }
        }

        //playerPhotonView = GameObject.Find("PlayerWoman(Clone)").GetComponent<PhotonView>();
        //playerPhotonView = LobbyGameManager.instance.player.gameObject.GetComponent<PhotonView>();
    }
}
