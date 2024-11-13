using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

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

    private void Awake()
    {
        targetFlower = GetComponent<Flower>();
        delay = new WaitForSeconds(checkInterval);
        CheckForPlayer();
        StartCoroutine(WaitForInitialization());
    }

    private void Start()
    {
        sound = GameObject.Find("HoonLoobyCanvas").GetComponent<HoonSoundManagerLogin>();
        cameraTr = Camera.main.transform;

        // 로컬 플레이어의 카메라 컨트롤러 찾기
        GameObject localPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        if (localPlayer != null)
        {
            cameraControll = localPlayer.GetComponent<CameraControllTest>();
        }

        // 만약 카메라 컨트롤러를 찾지 못했다면, 씬에서 찾아보기
        if (cameraControll == null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach (var player in players)
            {
                var playerPhotonView = player.GetComponent<PhotonView>();
                if (playerPhotonView != null && playerPhotonView.IsMine)
                {
                    cameraControll = player.GetComponent<CameraControllTest>();
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

    private void CheckForPlayer()
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
                    checkID = playerCheckID;
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
        // 로컬 플레이어의 상호작용만 처리
        if (isPlayerInRange && targetFlower != null && targetFlower.uiManager != null && isFirstClick == false)
        {

            isFirstClick = true;

            if (isFirst == false)
            {
                originPosition = cameraTr.position;
                originRotation = cameraTr.rotation;
                isFirst = true;
            }

            StartCoroutine(LerpCamera());

            if (checkID != null)
            {
                if (checkID.IsMine(targetFlower) == true)
                {
                    targetFlower.uiManager.ShowFlowerInfo(targetFlower, 0);
                }
                else
                {
                    if (targetFlower.voiceClip == null)
                    {
                        targetFlower.uiManager.ShowFlowerInfo(targetFlower, 1);
                    }
                    else
                    {
                        targetFlower.uiManager.ShowFlowerInfo(targetFlower, 2);
                    }
                }
            }
            else
            {
                if (targetFlower.voiceClip == null)
                {
                    targetFlower.uiManager.ShowFlowerInfo(targetFlower, 1);
                }
                else
                {
                    targetFlower.uiManager.ShowFlowerInfo(targetFlower, 2);
                }
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
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
        {
            //sound.PlaySound("hoonAudioClipArray", 1);
            HandleInteraction();
        }
    }

    IEnumerator LerpCamera()
    {
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