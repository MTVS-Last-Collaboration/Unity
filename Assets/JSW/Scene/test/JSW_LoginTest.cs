using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class JSW_LoginManager : MonoBehaviour
{
    public string loginUrl = "https://example.com/api/login";
    public string email;  // 사용자로부터 입력받은 이메일
    public string password;  // 사용자로부터 입력받은 비밀번호

    public void OnLoginButtonClicked()
    {
        StartCoroutine(LoginCoroutine());
    }

    IEnumerator LoginCoroutine()
    {
        // 로그인 정보 JSON 포맷으로 변환
        string jsonData = "{\"email\": \"" + email + "\", \"password\": \"" + password + "\"}";

        // HTTP 요청 생성
        UnityWebRequest request = new UnityWebRequest(loginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("accept", "*/*");

        yield return request.SendWebRequest();

        // 요청 결과 처리
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Login successful: " + request.downloadHandler.text);
            // 받은 응답에서 JWT 토큰 등을 추출하고 저장 (예: PlayerPrefs 사용)
        }
        else
        {
            Debug.LogError("Login failed: " + request.error);
            // 실패 시 사용자에게 오류 메시지 표시
        }
    }
}