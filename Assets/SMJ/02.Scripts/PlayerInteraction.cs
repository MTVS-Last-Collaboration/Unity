using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerInteraction : MonoBehaviourPun
{
    [SerializeField] private float detectionDistance = 2f; // 감지 거리
    [SerializeField] private LayerMask detectLayer; // 감지할 레이어
    [SerializeField] private Button detectionButton; // UI 버튼

    private void Awake()
    {
        detectionButton = GameObject.Find("Btn_Enter").GetComponent<Button>();
    }

    private void Start()
    {
        // 버튼에 클릭 이벤트 추가
        detectionButton.onClick.AddListener(DetectForwardObject);
    }

    private void DetectForwardObject()
    {
        if (photonView.IsMine)
        {
            // 플레이어의 정면 방향으로 레이캐스트 발사
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, detectionDistance, detectLayer))
            {
                hit.collider.gameObject.GetComponent<ClickFlower>()?.HandleInteraction();
                hit.collider.gameObject.GetComponent<ClickBoard>()?.HandleInteraction();
            }
            else
            {
                Debug.Log("감지된 오브젝트가 없습니다.");
            }
        }
    }
}
