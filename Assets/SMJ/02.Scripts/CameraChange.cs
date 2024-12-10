using UnityEngine;
using Photon.Pun;
using System.Collections;

public class CameraSwitch : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera triggerCamera;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;
        if (triggerCamera == null)
            triggerCamera = GetComponentInChildren<Camera>();

        StartCoroutine(InitializeDisplays());
    }

    private IEnumerator InitializeDisplays()
    {
        Debug.Log("displays connected: " + Display.displays.Length);

        // 추가 디스플레이가 있는지 확인하고 활성화
        if (Display.displays.Length > 1)
        {
            //Display.displays[1].Activate();
            yield return new WaitForSeconds(0.5f); // 디스플레이 활성화 대기
        }

        // 초기 카메라 설정
        SwitchToMainCamera();
        Debug.Log("Displays initialized");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            StartCoroutine(SwitchToTriggerCamera());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine)
        {
            StartCoroutine(SwitchToMainCameraRoutine());
        }
    }

    private void SwitchToMainCamera()
    {
        mainCamera.enabled = true;
        mainCamera.targetDisplay = 0;
        triggerCamera.enabled = false;
        triggerCamera.targetDisplay = 5;
    }

    private IEnumerator SwitchToMainCameraRoutine()
    {
        triggerCamera.targetDisplay = 5;
        mainCamera.targetDisplay = 0;
        triggerCamera.enabled = false;
        mainCamera.enabled = true;
        yield return new WaitForSeconds(0.005f);
    }

    private IEnumerator SwitchToTriggerCamera()
    {
        mainCamera.targetDisplay = 5;
        triggerCamera.targetDisplay = 0; mainCamera.enabled = false;
        triggerCamera.enabled = true;
        yield return new WaitForSeconds(0.005f);
    }
}