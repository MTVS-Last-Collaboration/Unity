using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;
using System.Globalization;

[System.Serializable]
public class TopicAnswer
{
    public int id;
    public string title;
    public string content;
    public string authorNickname;
    public string createdDate;
    public int likeCount;

    private DateTime? _parsedDate;
    public DateTime CreatedDateTime
    {
        get
        {
            if (!_parsedDate.HasValue && !string.IsNullOrEmpty(createdDate))
            {
                try
                {
                    // ISO 8601 형식의 날짜 문자열을 DateTime으로 파싱
                    _parsedDate = DateTime.Parse(createdDate, null, DateTimeStyles.RoundtripKind);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Date parsing failed for {createdDate}: {e.Message}");
                    _parsedDate = DateTime.MinValue;
                }
            }
            return _parsedDate ?? DateTime.MinValue;
        }
    }

    // 직렬화된 JSON에서 날짜를 올바르게 처리하기 위한 메서드
    public static TopicAnswer CreateFromJson(string json)
    {
        try
        {
            var answer = JsonUtility.FromJson<TopicAnswer>(json);
            // 직렬화 직후 날짜 파싱을 시도하여 유효성 검증
            var dateTime = answer.CreatedDateTime;
            return answer;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create TopicAnswer from JSON: {e.Message}");
            return null;
        }
    }

    public string GetFormattedDate(string format = "MM/dd")
    {
        return CreatedDateTime.ToString(format);
    }
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

    [SerializeField] private GameObject topicBannerPrefab;  // 토픽 배너 프리팹
    [SerializeField] private ScrollRect scrollView;         // 스크롤뷰 참조
    [SerializeField] private GameObject topicBannerObj;
    [SerializeField] private Button topicBannerButton;

    private List<Topic> weeklyTopics = new List<Topic>();  // 주간 토픽 저장 리스트

    private bool isClickTopicBanner = false;

    private void OnEnable()
    {
        if (topicBannerButton != null)
            topicBannerButton.onClick.AddListener(LoadWeeklyTopics);
    }

    private void Start()
    {
        StartCoroutine(GetWeeklyTopics());
    }

    [Serializable]
    public class ArrayWrapper<T>
    {
        public List<T> Items;
    }

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
                Topic topic = new Topic(currentId, currentContent, currentDate);
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
    [SerializeField] public List<TopicAnswer> CurrentAnswers => currentAnswers;

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
                        Debug.Log($"Raw createdDate string: {answer.createdDate}");
                        var dateTime = answer.createdDate;
                        Debug.Log($"Answer ID: {answer.id}, Created Date: {dateTime:yyyy-MM-dd HH:mm:ss}");
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

    public void LoadWeeklyTopics()
    {
        if (isClickTopicBanner == false)
        {
            isClickTopicBanner = true;
            topicBannerObj.SetActive(true);
        }
        else
        {
            isClickTopicBanner = false;
            topicBannerObj.SetActive(false);
        }
    }

    public void CloseWeeklyTopics()
    {
        isClickTopicBanner = false;
        topicBannerObj.SetActive(false);
    }

    private IEnumerator GetWeeklyTopics()
    {
        // 기존 배너들 제거
        foreach (Transform child in scrollView.content)
        {
            Destroy(child.gameObject);
        }

        weeklyTopics.Clear();

        // 오늘부터 7일 전까지의 데이터 가져오기
        for (int i = 0; i < 7; i++)
        {
            DateTime date = DateTime.Now.AddDays(-i);
            string formattedDate = date.ToString("yyyy-MM-dd");

            bool requestComplete = false;
            GetDailyTopic(formattedDate, (success) =>
            {
                if (success)
                {
                    // 현재 토픽의 정보로 새 Topic 객체 생성
                    Topic topic = new Topic(currentId, currentContent, currentDate);
                    weeklyTopics.Add(topic);

                    // 토픽 배너 생성
                    GameObject bannerObj = Instantiate(topicBannerPrefab, scrollView.content);
                    TopicBanner banner = bannerObj.GetComponent<TopicBanner>();
                    if (banner != null)
                    {
                        banner.Initialize(topic, i);
                    }
                }
                requestComplete = true;
            });

            // 각 요청이 완료될 때까지 대기
            yield return new WaitUntil(() => requestComplete);

            // 연속적인 서버 요청 사이에 짧은 딜레이 추가
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDestroy()
    {
        if (topicBannerButton != null)
            topicBannerButton.onClick.RemoveListener(LoadWeeklyTopics);
    }
}