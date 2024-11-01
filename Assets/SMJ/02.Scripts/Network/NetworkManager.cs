using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance;
    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("NetworkManager");
                instance = go.AddComponent<NetworkManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private string baseUrl;
    private string jwtToken;

    public void Initialize(string url, string token)
    {
        baseUrl = url;
        jwtToken = token;
    }

    // JWT 토큰 설정
    public void SetToken(string token)
    {
        jwtToken = token;
    }

    // 요청에 헤더 추가
    private void AddHeaders(UnityWebRequest request)
    {
        request.SetRequestHeader("Content-Type", "application/json");
        if (!string.IsNullOrEmpty(jwtToken))
        {
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
        }
    }

    public IEnumerator Post<T>(string endpoint, T data, Action<bool, string> callback = null)
    {
        string url = $"{baseUrl}{endpoint}";
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, jsonData))
        {
            AddHeaders(request);
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"POST 요청 실패: {request.error}");
                callback?.Invoke(false, request.error);
            }
        }
    }

    public IEnumerator Get<T>(string endpoint, Action<bool, T> callback = null)
    {
        string url = $"{baseUrl}{endpoint}";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            AddHeaders(request);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
                callback?.Invoke(true, result);
            }
            else
            {
                Debug.LogError($"GET 요청 실패: {request.error}");
                callback?.Invoke(false, default(T));
            }
        }
    }
}