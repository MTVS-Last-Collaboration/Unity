using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

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
        Debug.Log($"URL: {url}");
        Debug.Log($"Sending data: {jsonData}");

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            AddHeaders(request);

            yield return request.SendWebRequest();

            Debug.Log($"Response Code: {request.responseCode}");
            Debug.Log($"Response: {request.downloadHandler.text}");

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

    public IEnumerator PostWithoutBody(string endpoint, Action<bool, string> callback = null)
    {
        string url = $"{baseUrl}{endpoint}";
        Debug.Log($"URL: {url}");
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            AddHeaders(request);
            yield return request.SendWebRequest();
            Debug.Log($"Response Code: {request.responseCode}");
            Debug.Log($"Response: {request.downloadHandler.text}");
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

    [Serializable]
    private class ArrayWrapper<T>
    {
        public List<T> Items;
    }

    public IEnumerator GetArray<T>(string endpoint, Action<bool, List<T>> callback = null)
    {
        string url = $"{baseUrl}/{endpoint}";
        Debug.Log($"Making GET request to: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            AddHeaders(request);
            Debug.Log("Request headers added");

            yield return request.SendWebRequest();
            Debug.Log($"Request completed with result: {request.result}");
            Debug.Log($"Response code: {request.responseCode}");
            Debug.Log($"Response text: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonArray = request.downloadHandler.text;
                    string wrappedJson = $"{{\"Items\":{jsonArray}}}";

                    ArrayWrapper<T> wrapper = JsonUtility.FromJson<ArrayWrapper<T>>(wrappedJson);
                    Debug.Log($"JSON parsed successfully: {wrapper.Items.Count} items");
                    callback?.Invoke(true, wrapper.Items);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parsing failed: {e.Message}");
                    callback?.Invoke(false, null);
                }
            }
            else
            {
                Debug.LogError($"Request failed: {request.error}");
                callback?.Invoke(false, null);
            }
        }
    }
    public IEnumerator Get<T>(string endpoint, Action<bool, T> callback = null)
    {
        string url = $"{baseUrl}/{endpoint}";
        Debug.Log($"Making GET request to: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            AddHeaders(request);
            Debug.Log("Request headers added");

            yield return request.SendWebRequest();
            Debug.Log($"Request completed with result: {request.result}");
            Debug.Log($"Response code: {request.responseCode}");
            Debug.Log($"Response text: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    T result = JsonUtility.FromJson<T>(request.downloadHandler.text);
                    Debug.Log($"JSON parsed successfully: {JsonUtility.ToJson(result)}");
                    callback?.Invoke(true, result);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parsing failed: {e.Message}");
                    callback?.Invoke(false, default(T));
                }
            }
            else
            {
                Debug.LogError($"Request failed: {request.error}");
                callback?.Invoke(false, default(T));
            }
        }
    }

    public IEnumerator GetWithoutBody(string endpoint, Action<bool, string> callback = null)
    {
        string url = $"{baseUrl}/{endpoint}";
        Debug.Log($"Making GET request to: {url}");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            AddHeaders(request);
            Debug.Log("Request headers added");
            yield return request.SendWebRequest();
            Debug.Log($"Request completed with result: {request.result}");
            Debug.Log($"Response code: {request.responseCode}");
            Debug.Log($"Response text: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Request failed: {request.error}");
                callback?.Invoke(false, request.error);
            }
        }
    }
}