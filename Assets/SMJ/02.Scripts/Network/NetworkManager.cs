using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

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
            print("내토큰" + jwtToken);
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
                if (request.responseCode == 409)
                {
                    // 409 에러는 정상적인 케이스이므로 로그 출력하지 않음
                    callback?.Invoke(false, request.error);
                }
                else
                {
                    Debug.LogError($"POST 요청 실패: {request.error}");
                    callback?.Invoke(false, request.error);
                }
            }
        }
    }

    [System.Serializable]
    private class ArrayWrapper<T>
    {
        public List<T> Items = new List<T>();
    }

    public IEnumerator GetArray<T>(string endpoint, Action<bool, List<T>> callback = null) where T : class
    {
        string url = $"{baseUrl}/{endpoint}";
        //Debug.Log($"Making GET request to: {url}");
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            AddHeaders(request);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string jsonArray = request.downloadHandler.text;
                    //Debug.Log($"Raw JSON response: {jsonArray}");

                    // 배열을 직접 파싱
                    List<T> items = JsonUtility.FromJson<ArrayWrapper<T>>($"{{\"Items\":{jsonArray}}}").Items;
                    callback?.Invoke(true, items);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON parsing failed: {e.Message}\nStack trace: {e.StackTrace}");
                    callback?.Invoke(false, null);
                }
            }
            else
            {
                if (request.error != "HTTP/1.1 404 Not Found")
                {
                    Debug.LogError($"Request failed: {request.error}");
                }
                callback?.Invoke(false, null);
            }
        }
    }
    public IEnumerator Get<T>(string endpoint, Action<bool, T> callback = null)
    {
        if (string.IsNullOrEmpty(baseUrl))
        {
            Debug.LogError("baseUrl이 초기화되지 않았습니다!");
            callback?.Invoke(false, default(T));
            yield break;
        }
        string url = $"{baseUrl}/{endpoint}";
        //Debug.Log($"Making GET request to: {url}");

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            AddHeaders(request);
            //Debug.Log("Request headers added");

            yield return request.SendWebRequest();
            //Debug.Log($"Request completed with result: {request.result}");
            //Debug.Log($"Response code: {request.responseCode}");
            //Debug.Log($"Response text: {request.downloadHandler.text}");

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;
                Debug.Log($"서버 응답 원본: {responseText}");  // 실제 응답 확인

                if (string.IsNullOrEmpty(responseText))
                {
                    Debug.LogError("Server response is empty");
                    callback?.Invoke(false, default(T));
                }
                else
                {
                    try
                    {
                        T result = JsonUtility.FromJson<T>(responseText);
                        callback?.Invoke(true, result);
                    }
                    catch (Exception e)
                    {
                        //Debug.LogError($"JSON parsing failed: {e.Message}");
                        //Debug.LogError($"Response that failed to parse: {responseText}");
                        callback?.Invoke(false, default(T));
                    }
                }
            }
            else
            {
                //Debug.LogError($"Request failed: {request.error}");
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

    public IEnumerator PostMultipartData(string endpoint, List<IMultipartFormSection> formData, Action<bool, string> callback)
    {
        string url = baseUrl + endpoint;
        jwtToken = PlayerPrefs.GetString("token");
        using (UnityWebRequest request = UnityWebRequest.Post(url, formData))
        {
            request.SetRequestHeader("Accept", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");
            print("내토큰 : " + jwtToken);
            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.Success)
            {
                callback?.Invoke(true, request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"[Response] Error: {request.error}");
                Debug.LogError($"[Response] ResponseCode: {request.responseCode}");
                Debug.LogError($"[Response] Body: {request.downloadHandler.text}");
                callback?.Invoke(false, request.error);
            }
        }
    }
}