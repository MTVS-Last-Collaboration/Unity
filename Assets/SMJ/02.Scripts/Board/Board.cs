using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class Board : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject postPrefab;
    [SerializeField] private WritePanel writePanel;
    [SerializeField] private RectTransform postListContent;
    [SerializeField] private Canvas boardCanvas;
    [SerializeField] private TMP_Text dayTopicText;
    [SerializeField] private TMP_Text topicText;

    [Header("Sort Buttons")]
    [SerializeField] private Button sortByPopularButton;
    [SerializeField] private Button sortByDateButton;

    private List<PostData> posts = new List<PostData>();
    private bool isPopularSortActive = false;

    private TopicManager topicManager;

    public int lastId = 0;

    public bool isFirstLoading = true;

    private void Start()
    {
        //DateTime time = new DateTime(2024, 11, 6);
        topicManager = GetComponent<TopicManager>();
        DateTime time = DateTime.Today;
        InitTopic(time);
        StartCoroutine(DailyWeeklyLikesCheck());

        // 정렬 버튼 이벤트 연결
        //sortByPopularButton.onClick.AddListener(SortByPopular);
        //sortByDateButton.onClick.AddListener(SortByDate);
    }

    private void OnDestroy()
    {
        //sortByPopularButton.onClick.RemoveListener(SortByPopular);
        //sortByDateButton.onClick.RemoveListener(SortByDate);
    }

    private bool isLoading = false;
    private TaskCompletionSource<bool> currentLoadingTask = null;

    public async Task<bool> InitTopic(DateTime _date)
    {
        // 이미 같은 날짜의 토픽을 로딩 중이면 현재 작업이 완료될 때까지 대기
        if (isLoading)
        {
            Debug.Log("[Board] Already loading, waiting for completion...");
            if (currentLoadingTask != null)
            {
                await currentLoadingTask.Task;
            }
            return true;
        }

        try
        {
            isLoading = true;
            currentLoadingTask = new TaskCompletionSource<bool>();
            Debug.Log($"크아악 Loading topic for date: {_date:yyyy-MM-dd}");

            // 보드 초기화는 한 번만 수행
            ClearBoard();
            SetPostsVisibility(false);

            bool result = await LoadTopicAndPosts(_date);
            currentLoadingTask.SetResult(result);
            return result;
        }
        finally
        {
            isLoading = false;
            currentLoadingTask = null;
        }
    }

    private async Task<bool> LoadTopicAndPosts(DateTime _date)
    {
        var topicLoadingTask = new TaskCompletionSource<bool>();
        topicManager.GetDailyTopic(_date.ToString("yyyy-MM-dd"), async (success) => {
            if (success)
            {
                topicText.text = topicManager.currentContent;
                Debug.Log($"[Board] Topic loaded: {topicManager.currentContent}");

                // 포스트 로딩
                bool postsLoaded = await LoadPosts(topicManager.currentId);
                if (postsLoaded)
                {
                    SetPostsVisibility(true);
                }

                // 날짜 텍스트 업데이트
                TimeSpan difference = DateTime.Now - _date;
                dayTopicText.text = difference.Days > 0
                    ? $"<{difference.Days}일전 주제>"
                    : "<오늘의 주제>";

                topicLoadingTask.SetResult(true);
            }
            else
            {
                Debug.LogError("[Board] Failed to load topic");
                topicLoadingTask.SetResult(false);
            }
        });

        return await topicLoadingTask.Task;
    }

    private async Task<bool> LoadPosts(int topicId)
    {
        var postsLoadingTask = new TaskCompletionSource<bool>();

        topicManager.GetTopicAnswers((success) => {
            if (success)
            {
                foreach (var answer in topicManager.CurrentAnswers)
                {
                    CreatePost(answer);
                }
                postsLoadingTask.SetResult(true);
            }
            else
            {
                Debug.LogError("[Board] Failed to load posts");
                postsLoadingTask.SetResult(false);
            }
        });

        return await postsLoadingTask.Task;
    }

    private void SetPostsVisibility(bool visible)
    {
        if (postListContent != null)
        {
            foreach (Transform child in postListContent)
            {
                if (child != null)
                {
                    child.gameObject.SetActive(visible);
                }
            }
        }
    }

    private void ClearBoard()
    {
        Debug.Log("[Board] Clearing board");
        posts?.Clear();

        if (postListContent != null)
        {
            foreach (Transform child in postListContent)
            {
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    private void InitPosts(int id)
    {
        Debug.Log($"InitPosts called with id: {id}");

        // posts 리스트와 UI 초기화
        ClearBoard();

        topicManager.GetTopicAnswers((success) =>
        {
            if (success)
            {
                Debug.Log($"Successfully got {topicManager.CurrentAnswers.Count} answers");
                foreach (var answer in topicManager.CurrentAnswers)
                {
                    CreatePost(answer);
                    Debug.Log($"Created post for answer: {answer.id}");
                }
            }
            else
            {
                Debug.Log("Failed to get topic answers");
            }
        });
    }

    public void CreatePost(int answerId, string nickName, string title, string content, string date, int likeCount)
    {
        var post = new PostData(answerId, nickName, title, content, date, likeCount);
        posts.Add(post);
        GameObject postObj = Instantiate(postPrefab, postListContent);
        postObj.GetComponent<PostItem>().Initialize(post);
    }

    public void CreatePost(TopicAnswer answer)
    {
        if (posts.Any(p => p.answerId == answer.id))
        {
            return;
        }
        var post = new PostData(
            answer.id,
            answer.authorNickname,
            answer.title,
            answer.content,
            answer.GetFormattedDate("MM/dd"),  // 변환된 DateTime 사용
            answer.likeCount
        );

        posts.Add(post);

        GameObject postObj = Instantiate(postPrefab, postListContent);
        postObj.SetActive(false);

        var postItem = postObj.GetComponent<PostItem>();
        postItem.Initialize(post);

        var commentBoard = postObj.GetComponentInChildren<CommentBoard>(true);
        postObj.GetComponent<PostItem>().commentBoard = commentBoard;
        if (commentBoard != null)
        {
            commentBoard.Initialize(answer, this);
            lastId = answer.id;

            if (topicManager.AnswerComments.TryGetValue(answer.id, out var comments))
            {
                foreach (var comment in comments)
                {
                    commentBoard.AddComment(comment);
                }
            }
        }
    }

    private void RefreshPostList(PostData post)
    {
        //foreach (Transform child in postListContent)
        //{
        //    Destroy(child.gameObject);
        //}
        //foreach (var post in posts)
        //{
        //    GameObject postObj = Instantiate(postPrefab, postListContent);
        //    postObj.GetComponent<PostItem>().Initialize(post);
        //}
        GameObject postObj = Instantiate(postPrefab, postListContent);
        postObj.GetComponent<PostItem>().Initialize(post);
        var commentBoard = gameObject.GetComponentInChildren<CommentBoard>(true);
        postObj.GetComponent<PostItem>().commentBoard = commentBoard;
    }

    private IEnumerator DailyWeeklyLikesCheck()
    {
        while (true)
        {
            CheckAndResetWeeklyLikes();

            DateTime now = DateTime.Now;
            DateTime tomorrow = now.Date.AddDays(1);
            float secondsUntilTomorrow = (float)(tomorrow - now).TotalSeconds;

            yield return new WaitForSeconds(secondsUntilTomorrow);
        }
    }

    private void CheckAndResetWeeklyLikes()
    {
        bool needsRefresh = false;
        foreach (var post in posts)
        {
            if (post.NeedsWeeklyReset())
            {
                post.ResetWeeklyLikes();
                needsRefresh = true;
            }
        }

        if (needsRefresh && isPopularSortActive)
        {
            SortByPopular();
        }
    }

    public void SortByPopular()
    {
        isPopularSortActive = true;
        posts.Sort((a, b) => b.GetWeeklyLikes().CompareTo(a.GetWeeklyLikes()));
        //RefreshPostList();
        //전부 삭제
        //inittopic부터 다시?
    }

    public void SortByDate()
    {
        isPopularSortActive = false;
        posts.Sort((a, b) => b.createdDate.CompareTo(a.createdDate));
        //RefreshPostList();
    }

    // 댓글 관련 메서드들
    public void CreateNewComment(int answerId, string content, Action onComplete = null)
    {
        var newComment = new ServerCommentData
        {
            content = content,
            authorNickname = PlayerPrefs.GetString("nickname", "Unknown"),
            createdDate = DateTime.Now.ToString("MM/dd HH:mm")
        };

        StartCoroutine(CreateCommentCoroutine(answerId, newComment, onComplete));
    }

    private IEnumerator CreateCommentCoroutine(int answerId, ServerCommentData comment, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
        bool isComplete = false;

        StartCoroutine(NetworkManager.Instance.Post($"api/topic/{answerId}/comments", comment,
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("Comment created successfully");
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to create comment: {response}");
                }
                isComplete = true;
            }));

        while (!isComplete) yield return null;
    }

    public void RefreshComments(int answerId, CommentBoard commentBoard)
    {
        StartCoroutine(RefreshCommentsCoroutine(answerId, commentBoard));
    }

    private IEnumerator RefreshCommentsCoroutine(int answerId, CommentBoard commentBoard)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
        bool isComplete = false;

        StartCoroutine(NetworkManager.Instance.GetArray<ServerCommentData>($"api/topic/{answerId}/comments",
            (success, result) =>
            {
                if (success && result != null)
                {
                    commentBoard.Initialize(new TopicAnswer { id = answerId }, this);
                    foreach (var comment in result)
                    {
                        commentBoard.AddComment(comment);
                    }
                }
                isComplete = true;
            }));

        while (!isComplete) yield return null;
    }
}