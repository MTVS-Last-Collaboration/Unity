using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class ClickFlower : MonoBehaviourPunCallbacks  // MonoBehaviour에서 변경
{
    private Flower targetFlower;
    private bool isPlayerInRange = false;
    public CheckID checkID;

    public float detectionDistance = 1f;
    public float idHandlingRadius = 100f;
    public float checkInterval = 0.5f;

    private bool isClose = false;
    private WaitForSeconds delay;
    private bool isInitialized = false;  // 초기화 여부 체크를 위한 변수 추가

    private void Awake()
    {
        targetFlower = GetComponent<Flower>();
        delay = new WaitForSeconds(checkInterval);
        CheckForPlayer();
        StartCoroutine(WaitForInitialization());
    }

    private IEnumerator WaitForInitialization()
    {
        // 포톤 네트워크 연결 및 플레이어 초기화 대기
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
        yield return new WaitForSeconds(1f); // 초기화를 위한 딜레이

        while (true)
        {
            print($"현재 체크중인 플레이어: {gameObject.name}, managerId: {targetFlower.managerId}");
            CheckForPlayer();
            yield return delay;
        }
    }
    private void Update()
    {
        print(gameObject.name + " : " + isClose);
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
        bool isNearby = false;  // 거리 체크를 위한 변수 추가

        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Player")) continue;

            CheckID playerCheckID = hitCollider.GetComponent<CheckID>();
            if (playerCheckID == null) continue;

            IDHandler playerIDHandler = hitCollider.GetComponent<IDHandler>();
            if (playerIDHandler == null) continue;

            print($"검사중인 플레이어: {hitCollider.gameObject.name}, ID: {playerIDHandler.ID}, 꽃의 managerId: {targetFlower.managerId}");

            float distance = Vector3.Distance(gameObject.transform.position, hitCollider.transform.position);

            // 어떤 플레이어라도 가까이 있으면 true
            if (distance < detectionDistance)
            {
                isNearby = true;
            }

            // 이미 이 꽃의 소유자인 경우
            if (targetFlower.managerId == playerIDHandler.ID)
            {
                checkID = playerCheckID;
                foundPlayer = true;
            }
            // 아직 아무도 할당되지 않은 꽃이고, 다른 꽃의 소유자가 아닌 플레이어를 발견한 경우
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
            // 상대방의 꽃인 경우
            else if (!string.IsNullOrEmpty(targetFlower.managerId))
            {
                foundPlayer = true;
            }
        }

        // 모든 플레이어 체크가 끝난 후에 거리에 따른 상태 설정
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
        print(isPlayerInRange);
        if (isPlayerInRange && targetFlower != null && targetFlower.uiManager != null)
        {
            if (checkID != null)
            {
                //print(checkID.IsMine(targetFlower));
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
            HandleInteraction();
        }
    }
}