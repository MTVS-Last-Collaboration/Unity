using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public class Topic
{
    public int id;
    public string content;
    public string date;

    private const string TOPIC_ID_KEY = "dailyTopicId";
    private const string TOPIC_CONTENT_KEY = "content";

    public Topic(int id, string content, string date)
    {
        this.id = id;
        this.content = content;
        this.date = date;
        SaveTopicData();
    }

    private void SaveTopicData()
    {
        PlayerPrefs.SetInt(TOPIC_ID_KEY, id);
        PlayerPrefs.SetString(TOPIC_CONTENT_KEY, content);
        PlayerPrefs.Save();
    }

    public static int GetSavedTopicId()
    {
        return PlayerPrefs.GetInt(TOPIC_ID_KEY);
    }

    public static string GetSavedContent()
    {
        return PlayerPrefs.GetString(TOPIC_CONTENT_KEY);
    }

    public static Topic FromJson(string json)
    {
        return JsonUtility.FromJson<Topic>(json);
    }
}

public class TopicManager : MonoBehaviour
{
    private Topic currentTopic;

    public string currentContent => currentTopic?.content ?? Topic.GetSavedContent();
    public string currentDate => currentTopic?.date ?? "";

    public void GetDailyTopic(string date, Action<bool> onComplete = null)
    {
        NetworkManager.Instance.Initialize("/api/board/date/", PlayerPrefs.GetString("token"));
        StartCoroutine(NetworkManager.Instance.Get<Topic>($"/api/board/date/{date}", (success, result) =>
        {
            if (success && result != null)
            {
                currentTopic = result;
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError("일일 토픽 조회에 실패했습니다.");
                onComplete?.Invoke(false);
            }
        }));
    }
}