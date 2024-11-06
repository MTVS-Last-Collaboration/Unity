using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TopicAnswer
{
    public int id;
    public string content;
    public string authorNickname;
    public string createdDate;
}
[Serializable]
public class TopicAnswerResponse
{
    public List<TopicAnswer> answers;
}

[System.Serializable]
public class Topic
{
    public int id;
    public string content;
    public string date;

    private const string TOPIC_ID_KEY = "dailyTopicId";
    private const string TOPIC_CONTENT_KEY = "content";
    private const string TOPIC_ANSWERS_KEY = "topicAnswers";

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

    public static void SaveAnswers(string answersJson)
    {
        PlayerPrefs.SetString(TOPIC_ANSWERS_KEY, answersJson);
        PlayerPrefs.Save();
    }

    public static string GetSavedAnswers()
    {
        return PlayerPrefs.GetString(TOPIC_ANSWERS_KEY, "[]");
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

    public int currentId => currentTopic?.id ?? Topic.GetSavedTopicId();

    public void GetDailyTopic(string date, Action<bool> onComplete = null)
    {
        Debug.Log($"GetDailyTopic called with date: {date}");
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        StartCoroutine(NetworkManager.Instance.Get<Topic>($"api/topic/date/{date}", (success, result) =>
        {
            Debug.Log($"Get request callback - Success: {success}, Result: {result}");
            if (success && result != null)
            {
                currentTopic = result;
                Debug.Log($"Topic set: {JsonUtility.ToJson(currentTopic)}");
                onComplete?.Invoke(true);
            }
            else
            {
                Debug.LogError($"Daily topic request failed - Success: {success}, Result: {result}");
                onComplete?.Invoke(false);
            }
        }));
    }

    private List<TopicAnswer> currentAnswers = new List<TopicAnswer>();
    public List<TopicAnswer> CurrentAnswers => currentAnswers;

    private Dictionary<int, List<ServerCommentData>> answerComments = new Dictionary<int, List<ServerCommentData>>();
    public Dictionary<int, List<ServerCommentData>> AnswerComments => answerComments;

    public void GetTopicAnswers(Action<bool> onComplete = null)
    {
        Debug.Log($"Getting answers for topic ID: {currentId}");

        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
        StartCoroutine(NetworkManager.Instance.GetArray<TopicAnswer>($"api/topic/{currentId}/answers",
            async (success, result) =>
            {
                if (success && result != null)
                {
                    currentAnswers = result;
                    Debug.Log($"Successfully received {currentAnswers.Count} answers");

                    foreach (var answer in currentAnswers)
                    {
                        await LoadCommentsForAnswer(answer);
                    }

                    onComplete?.Invoke(true);
                }
                else
                {
                    Debug.LogError("Topic answers request failed");
                    onComplete?.Invoke(false);
                }
            }));
    }

    private async Task LoadCommentsForAnswer(TopicAnswer answer)
    {
        bool commentsLoaded = false;
        StartCoroutine(NetworkManager.Instance.GetArray<ServerCommentData>($"api/topic/{answer.id}/comments",
            (success, comments) =>
            {
                if (success && comments != null)
                {
                    answerComments[answer.id] = comments;
                    Debug.Log($"Loaded comments for answer {answer.id}");
                }
                else
                {
                    Debug.LogError($"Failed to load comments for answer {answer.id}");
                }
                commentsLoaded = true;
            }));

        while (!commentsLoaded)
        {
            await Task.Yield();
        }
    }
}