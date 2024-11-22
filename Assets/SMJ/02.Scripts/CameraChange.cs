using UnityEngine;

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

        // 초기 디스플레이 설정 (인덱스는 0부터 시작)
        mainCamera.targetDisplay = 0;    // Display 1
        triggerCamera.targetDisplay = 1;  // Display 2
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainCamera.targetDisplay = 1;     // Display 2
            triggerCamera.targetDisplay = 0;  // Display 1
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainCamera.targetDisplay = 0;     // Display 1
            triggerCamera.targetDisplay = 1;  // Display 2
        }
    }
}