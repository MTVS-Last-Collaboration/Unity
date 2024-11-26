using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using static UnityEngine.ParticleSystem;

public class ClickFlower : MonoBehaviourPunCallbacks
{
    private Flower targetFlower;
    private bool isPlayerInRange = false;
    public CheckID checkID;

    public float detectionDistance = 1f;
    public float idHandlingRadius = 100f;
    public float checkInterval = 0.5f;

    private bool isClose = false;
    private WaitForSeconds delay;
    private bool isInitialized = false;

    [SerializeField] private Transform lerpTr;
    private Transform cameraTr;

    private Vector3 originPosition;
    private Quaternion originRotation;

    public CameraControllTest cameraControll;
    float curtime = 0;

    bool isFirst = false;
    public bool isFirstClick = false;

    private HoonSoundManagerLogin sound;

    public Camera mainCam;
    public ParticleTriggerController particle;

    private void Awake()
    {
        targetFlower = GetComponent<Flower>();
        delay = new WaitForSeconds(checkInterval);
        CheckForPlayer();
        StartCoroutine(WaitForInitialization());
    }

    private void Start()
    {
        // 이전에 있던 모든 초기화 코드 제거
        if (Camera.main != null)
        {
            cameraTr = mainCam.transform;
        }
        // 나머지 초기화는 코루틴으로 이동
        StartCoroutine(InitializeComponents());
    }
    private IEnumerator InitializeComponents()
    {
        // 씬이 완전히 로드될 때까지 대기
        yield return new WaitForSeconds(1f);

        // HoonLoobyCanvas 찾기 시도 (널체크 추가)
        GameObject lobbyCanvas = GameObject.Find("SMJ");
        if (lobbyCanvas != null)
        {
            sound = lobbyCanvas.GetComponent<HoonSoundManagerLogin>();
        }

        // 카메라가 아직 없다면 다시 시도
        if (cameraTr == null && Camera.main != null)
        {
            cameraTr = mainCam.transform;
        }

        // 카메라 컨트롤러 찾기
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView != null && playerPhotonView.IsMine)
            {
                cameraControll = player.GetComponent<CameraControllTest>();
                if (cameraControll != null)
                {
                    break;
                }
            }
        }
    }
    private IEnumerator WaitForInitialization()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        isInitialized = true;
        StartCoroutine(CheckForPlayerRoutine());
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        print("새 플레이어 진입!");
        if (isInitialized)
        {
            print("새 플레이어 진입 동기화!");
            CheckForPlayer();
            FlowerUIManager uiManager = GetComponent<FlowerUIManager>();
            StartCoroutine(uiManager.SyncStateForNewPlayer(newPlayer));
        }
    }

    private IEnumerator CheckForPlayerRoutine()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {
            CheckForPlayer();
            yield return delay;
        }
    }

    private void Update()
    {
        if (isClose == true)
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    CheckInteraction(Input.GetTouch(0).position);
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                CheckInteraction(Input.mousePosition);
            }
        }
    }



    [PunRPC]
    private void RPC_SyncFlowerClickId(int viewID) // CheckID 대신 ViewID 사용
    {
        PhotonView pv = PhotonView.Find(viewID);
        if (pv != null)
        {
            checkID = pv.GetComponent<CheckID>();
        }
    }

    public void CheckForPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, idHandlingRadius);
        bool foundPlayer = false;
        bool isNearby = false;

        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Player")) continue;
            CheckID playerCheckID = hitCollider.GetComponent<CheckID>();
            if (playerCheckID == null) continue;

            IDHandler playerIDHandler = hitCollider.GetComponent<IDHandler>();
            if (playerIDHandler == null) continue;

            // 로컬 플레이어의 컴포넌트만 가져오기
            if (playerCheckID.photonView.IsMine)
            {
                cameraControll = hitCollider.GetComponent<CameraControllTest>();
                if (cameraControll == null) continue;

                float distance = Vector3.Distance(gameObject.transform.position, hitCollider.GetComponent<PlayerInteraction>().playerModel.transform.position + new Vector3(0, 1, 0));
                if (distance < detectionDistance)
                {
                    isNearby = true;
                }

                if (targetFlower.managerId == playerIDHandler.ID)
                {
                    photonView.RPC("RPC_SyncFlowerClickId", RpcTarget.All, playerCheckID.photonView.ViewID);
                    //checkID = playerCheckID;
                    foundPlayer = true;
                }
                else if (string.IsNullOrEmpty(targetFlower.managerId))
                {
                    GameObject[] flowers = GameObject.FindGameObjectsWithTag("Flower");
                    bool isOtherFlowerOwner = false;

                    foreach (var flower in flowers)
                    {
                        Flower otherFlower = flower.GetComponent<Flower>();
                        if (otherFlower != targetFlower && playerCheckID.IsMine(otherFlower))
                        {
                            isOtherFlowerOwner = true;
                            break;
                        }
                    }

                    if (!isOtherFlowerOwner)
                    {
                        checkID = playerCheckID;
                        foundPlayer = true;
                    }
                }
                else if (!string.IsNullOrEmpty(targetFlower.managerId))
                {
                    foundPlayer = true;
                }
            }
        }

        isClose = isNearby;
        isPlayerInRange = isNearby;

        if (!foundPlayer)
        {
            if (!IsSomeoneOwner())
            {
                checkID = null;
            }
        }
    }

    public void HandleInteraction()
    {
        // 상호작용 기본 조건 체크
        if (!isPlayerInRange || targetFlower == null || targetFlower.uiManager == null || isFirstClick)
            return;

        particle.DisableChecking();
        isFirstClick = true;

        if (!isFirst)
        {
            originPosition = cameraTr.position;
            originRotation = cameraTr.rotation;
            isFirst = true;
        }

        StartCoroutine(LerpCamera());

        // UI 표시 로직
        if (checkID != null && checkID.IsMine(targetFlower))  // 자신의 꽃인 경우
        {
            if (targetFlower.curState == Flower.States.BLOSSOM)
            {
                // 만개한 꽃이고 녹음이 완료된 상태
                targetFlower.uiManager.ShowFlowerInfo(targetFlower, 5);
            }
            else if (targetFlower.uiManager.isRecordComplete)
            {
                // 녹음만 완료된 상태
                targetFlower.uiManager.ShowFlowerInfo(targetFlower, 3);
            }
            else
            {
                // 아무것도 완료되지 않은 상태
                targetFlower.uiManager.ShowFlowerInfo(targetFlower, 0);
            }
        }
        else  // 파트너의 꽃인 경우
        {
            if (!targetFlower.uiManager.isRecordComplete)
            {
                // 아직 녹음되지 않은 상태
                targetFlower.uiManager.ShowFlowerInfo(targetFlower, 1);
            }
            else if (!targetFlower.uiManager.isListenComplete)
            {
                // 녹음은 되었지만 아직 듣지 않은 상태
                targetFlower.uiManager.ShowFlowerInfo(targetFlower, 2);
            }
            else
            {
                // 이미 들은 상태
                targetFlower.uiManager.ShowFlowerInfo(targetFlower, 2);
            }
        }
    }

    private bool IsSomeoneOwner()
    {
        if (checkID != null && checkID.IsMine(targetFlower))
            return true;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, idHandlingRadius);
        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Player")) continue;

            CheckID playerCheckID = hitCollider.GetComponent<CheckID>();
            if (playerCheckID != null && playerCheckID.IsMine(targetFlower))
                return true;
        }
        return false;
    }

    private void CheckInteraction(Vector2 position)
    {
        if (mainCam == null)
        {
            Debug.LogError("Main Camera is null!");
            return;
        }

        Ray ray = mainCam.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            //sound.PlaySound("hoonAudioClipArray", 1);
            HandleInteraction();
        }
    }

    IEnumerator LerpCamera()
    {
        mainCam.targetDisplay = 0;
        mainCam.enabled = true;
        if (cameraControll != null)
        {
            cameraControll.isMoveAble = false;
            curtime = 0f;
            float lerpDuration = 0.5f;
            Vector3 startPosition = cameraTr.position;
            Quaternion startRotation = cameraTr.rotation;

            while (curtime < lerpDuration)
            {
                if (Vector3.Distance(cameraTr.position, lerpTr.position) < 0.1f)
                {
                    cameraTr.position = lerpTr.position;
                    cameraTr.rotation = lerpTr.rotation;
                    break;
                }

                curtime += Time.deltaTime;
                float t = curtime / lerpDuration;
                cameraTr.position = Vector3.Lerp(startPosition, lerpTr.position, t);
                cameraTr.rotation = Quaternion.Lerp(startRotation, lerpTr.rotation, t);
                yield return null;
            }

            cameraTr.position = lerpTr.position;
            cameraTr.rotation = lerpTr.rotation;
        }
    }

    public void ReturnCamera()
    {
        if (isFirst)
        {
            StartCoroutine(ReturnCameraTransform());
            isFirst = false;
            isFirstClick = false;
        }
    }

    IEnumerator ReturnCameraTransform()
    {
        if (cameraControll != null)
        {
            mainCam.enabled = false;
            curtime = 0f;
            float lerpDuration = 0.5f;
            Vector3 startPosition = cameraTr.position;
            Quaternion startRotation = cameraTr.rotation;

            while (curtime < lerpDuration)
            {
                if (Vector3.Distance(cameraTr.position, originPosition) < 0.1f)
                {
                    cameraTr.position = originPosition;
                    cameraTr.rotation = originRotation;
                    break;
                }

                curtime += Time.deltaTime;
                float t = curtime / lerpDuration;
                cameraTr.position = Vector3.Lerp(startPosition, originPosition, t);
                cameraTr.rotation = Quaternion.Lerp(startRotation, originRotation, t);
                yield return null;
            }

            cameraTr.position = originPosition;
            cameraTr.rotation = originRotation;
            cameraControll.isMoveAble = true;
        }
    }
}