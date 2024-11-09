using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

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

    public void InitTopic(DateTime _date)
    {
        ClearBoard();

        topicManager.GetDailyTopic(_date.ToString("yyyy-MM-dd"), (success) => {
            if (success)
            {
                topicText.text = topicManager.currentContent;
                Debug.Log("토픽 가져오기 성공 : " + topicManager.currentContent);

                // 토픽 정보를 가져온 후 posts 초기화
                InitPosts(topicManager.currentId);

                // 날짜 차이에 따른 텍스트 설정
                TimeSpan difference = DateTime.Now - _date;
                if (difference.Days > 0)
                {
                    dayTopicText.text = $"<{difference.Days}일전 주제>";
                }
                else if (difference.Days == 0)
                {
                    dayTopicText.text = "<오늘의 주제>";
                }
            }
            else
            {
                Debug.Log("토픽 가져오기 실패");
            }
        });
    }
    public void ClearBoard()
    {
        // posts 리스트 클리어
        posts.Clear();

        // 모든 자식 오브젝트 제거
        foreach (Transform child in postListContent)
        {
            Destroy(child.gameObject);
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

    public void CreatePost(int answerId, string nickName, string title, string content, int likeCount)
    {
        var post = new PostData(answerId, nickName, title, content, likeCount);
        posts.Add(post);
        GameObject postObj = Instantiate(postPrefab, postListContent);
        postObj.GetComponent<PostItem>().Initialize(post);
    }

    public void CreatePost(TopicAnswer answer)
    {
        CreatePost(
            answer.id,
            answer.authorNickname,
            answer.title,
            answer.content,
            answer.likeCount
        );

        var lastPost = postListContent.GetChild(postListContent.childCount - 1);
        var commentBoard = lastPost.GetComponentInChildren<CommentBoard>(true);
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
        posts.Sort((a, b) => b.createDate.CompareTo(a.createDate));
        //RefreshPostList();
    }

    // 댓글 관련 메서드들
    public void CreateNewComment(int answerId, string content, Action onComplete = null)
    {
        var newComment = new ServerCommentData
        {
            content = content,
            authorNickname = PlayerPrefs.GetString("nickname", "Unknown"),
            createdDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
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