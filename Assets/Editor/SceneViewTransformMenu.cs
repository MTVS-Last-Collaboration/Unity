using UnityEngine;
using UnityEditor;

public class SceneViewTransformMenu : EditorWindow
{
    [MenuItem("Tools/Copy Scene View Transform to Selected #&q")] // Alt+Shift+Q
    private static void CopySceneViewTransform()
    {
        // 선택된 오브젝트가 없으면 리턴
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("Please select an object first!");
            return;
        }

        // 현재 활성화된 씬뷰를 가져옴
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView == null)
        {
            Debug.LogWarning("No Scene View is active!");
            return;
        }

        // Scene View 카메라의 Transform 정보를 가져옴
        Transform selectedTransform = Selection.activeGameObject.transform;
        Camera sceneCamera = sceneView.camera;

        // Undo 기능 등록
        Undo.RecordObject(selectedTransform, "Copy Scene View Transform");

        // 위치 복사
        selectedTransform.position = sceneCamera.transform.position;

        // 회전 설정 - 오브젝트의 z축이 카메라가 보는 방향과 같은 방향을 보도록 함
        Vector3 cameraDirection = sceneCamera.transform.forward; // 반전시키지 않음
        Vector3 up = sceneCamera.transform.up;

        // cameraDirection이 z축을 향하도록 회전 설정
        selectedTransform.rotation = Quaternion.LookRotation(cameraDirection, up);
    }

    // 메뉴 아이템 validation
    [MenuItem("Tools/Copy Scene View Transform to Selected #&v", true)]
    private static bool ValidateSelection()
    {
        return Selection.activeGameObject != null && SceneView.lastActiveSceneView != null;
    }
}