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
        if (isInitialized)
        {
            CheckForPlayer();
        }
    }

    private IEnumerator CheckForPlayerRoutine()
    {
        while (true)
        {
            if (isInitialized)
            {
                CheckForPlayer();
            }
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

        foreach (var hitCollider in hitColliders)
        {
            if (!hitCollider.CompareTag("Player")) continue;

            CheckID playerCheckID = hitCollider.GetComponent<CheckID>();
            if (playerCheckID == null) continue;

            float distance = Vector3.Distance(gameObject.transform.position, hitCollider.transform.position);

            // 이미 이 꽃의 소유자인 경우
            if (targetFlower.managerId == playerCheckID.GetComponent<IDHandler>().ID)
            {
                checkID = playerCheckID;
                foundPlayer = true;

                if (distance < detectionDistance)
                {
                    isClose = true;
                    isPlayerInRange = true;
                }
                else
                {
                    isClose = false;
                    isPlayerInRange = false;
                }
                break;
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

                    if (distance < detectionDistance)
                    {
                        isClose = true;
                        isPlayerInRange = true;
                    }
                    else
                    {
                        isClose = false;
                        isPlayerInRange = false;
                    }
                    break;
                }
            }
            // 상대방의 꽃인 경우
            else if (!string.IsNullOrEmpty(targetFlower.managerId))
            {
                // checkID는 할당하지 않음
                if (distance < detectionDistance)
                {
                    isClose = true;
                    isPlayerInRange = true;
                }
                else
                {
                    isClose = false;
                    isPlayerInRange = false;
                }
                foundPlayer = true;
                break;
            }
        }

        if (!foundPlayer)
        {
            isClose = false;
            isPlayerInRange = false;
            if (!IsSomeoneOwner())
            {
                checkID = null;
            }
        }
    }

    private void HandleInteraction()
    {
        print(isPlayerInRange);
        if (isPlayerInRange && targetFlower != null && targetFlower.uiManager != null)
        {
            if (checkID != null)
            {
                print(checkID.IsMine(targetFlower));
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