using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Globalization;

[System.Serializable]
public class ServerCommentPost
{
    public int answerId;
    public string content;
}

[System.Serializable]
public class ServerCommentData
{
    public int id;
    public int answerId;
    public string content;
    public string authorNickname;
    public int likeCount;
    public string createdDate;

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

    public string GetFormattedDate(string format = "MM/dd")
    {
        return CreatedDateTime.ToString(format);
    }
}
[System.Serializable]
public class ServerCommentResponse
{
    public List<ServerCommentData> items;
}

public class CommentBoard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject commentPanel;
    [SerializeField] private RectTransform commentListContent;
    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private WriteCommentPanel writePanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button recentButton;
    [SerializeField] public PostItem item;

    public int answerId;
    private Board parentBoard;
    private List<CommentData> comments = new List<CommentData>();
    private bool isInitialized = false;

    [SerializeField] public TMP_Text title;
    [SerializeField] public TMP_Text nickName;
    [SerializeField] public TMP_Text content;
    [SerializeField] public TMP_Text date;
    [SerializeField] public DateTime time;
    [SerializeField] public TMP_Text likeCountText;
    public int likeCount = 0;

    [SerializeField] public Button likeButton;
    private HoonSoundManagerLogin sound;
    public void Initialize(TopicAnswer answer, Board board)
    {
        Debug.Log($"CommentBoard Initialize - AnswerId: {answer.id}");
        answerId = answer.id;
        parentBoard = board;

        if (gameObject.activeInHierarchy)
        {
            LoadComments();
        }
        else
        {
            isInitialized = false;
        }
    }

    private void OnEnable()
    {
        Debug.Log($"[CommentBoard] OnEnable - Comments list content null?: {commentListContent == null}, Comments count: {comments.Count}");
        if (!isInitialized && answerId != 0)
        {
            LoadComments();
        }
        else if (comments.Count > 0)
        {
            RefreshCommentList();
        }
    }

    private void Start()
    {
        closeButton.onClick.AddListener(() => Close());
        likeButton.onClick.AddListener(() => LikeClick());
        commentPanel.SetActive(false);
        sound = GameObject.Find("SMJ").GetComponent<HoonSoundManagerLogin>();
        // NetworkManager 미리 초기화
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
    }

    public void LikeClick()
    {
        item.OnLikeButton();
    }

    public void Close()
    {
        sound.PlaySound("smjAudioClopAttay", 1);
        commentPanel.SetActive(false);
        closeButton.gameObject.SetActive(false);
        recentButton.gameObject.SetActive(true);
    }

    public void DisplayCommentsForAnswer(int id)
    {
        answerId = id;
        commentPanel.SetActive(true);
        closeButton.gameObject.SetActive(true);
        recentButton.gameObject.SetActive(false);
        ClearComments();
        LoadComments();
    }

    private void ClearComments()
    {
        foreach (Transform child in commentListContent)
        {
            Destroy(child.gameObject);
        }
        comments.Clear();
    }

    private void LoadComments()
    {
        if (!gameObject.activeInHierarchy) return;

        // NetworkManager가 이미 초기화되어 있으므로 바로 코루틴 시작
        StartCoroutine(LoadCommentsCoroutine());
        isInitialized = true;
    }

    private IEnumerator LoadCommentsCoroutine()
    {
        string url = $"api/topic/{answerId}/comments";
        Debug.Log($"Loading comments from URL: {url}");

        yield return NetworkManager.Instance.GetArray<ServerCommentData>(url,
            (success, commentList) =>
            {
                if (!gameObject.activeInHierarchy) return;

                if (success && commentList != null)
                {
                    try
                    {
                        // 서버 응답 전체를 JSON으로 출력
                        var rawResponse = JsonUtility.ToJson(commentList);
                        Debug.Log($"Raw server response: {rawResponse}");

                        if (commentList.Count > 0)
                        {
                            var firstComment = commentList[0];
                        }

                        comments.Clear();
                        foreach (var serverComment in commentList)
                        {
                            AddComment(serverComment);
                        }

                        if (comments.Count > 0)
                        {
                            RefreshCommentList();
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error processing comments: {e.Message}\nStackTrace: {e.StackTrace}");
                    }
                }
                else
                {
                    //Debug.LogError($"Failed to load comments. Success: {success}, CommentList null: {commentList == null}");
                }
            });
    }

    public void AddComment(ServerCommentData serverComment)
    {
        // 서버에서 받은 데이터 로깅
        Debug.Log($"서버 댓글 raw 데이터 - id: {serverComment.id}, nickname: {serverComment.authorNickname}, content: {serverComment.content}, date: {serverComment.createdDate}, likes: {serverComment.likeCount}");

        var comment = new CommentData(
            serverComment.id,
            serverComment.authorNickname,
            serverComment.content,
            serverComment.GetFormattedDate("MM/dd HH:mm"),
            serverComment.likeCount
        );

        // 생성된 CommentData 객체 로깅
        Debug.Log($"생성된 CommentData - id: {comment.id}, nickname: {comment.nickName}, content: {comment.content}, date: {comment.createdDate}, likes: {comment.likeCount}");

        comments.Add(comment);
    }

    public void AddComment(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning("Attempted to add empty comment");
            return;
        }

        CreateNewComment(answerId, text, () =>
        {
            LoadComments(); // 댓글 목록 새로고침
        });
    }

    public void CreateNewComment(int answerId, string content, Action onComplete = null)
    {
        var newComment = new ServerCommentPost
        {
            answerId = answerId,
            content = content
        };

        StartCoroutine(CreateCommentCoroutine(answerId, newComment, onComplete));
    }

    private IEnumerator CreateCommentCoroutine(int answerId, ServerCommentPost comment, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.Post($"/api/topic/comment/create", comment,
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
            });
    }

    private void RefreshCommentList()
    {
        if (commentListContent == null)
        {
            Debug.LogError("CommentListContent is null!");
            return;
        }

        foreach (Transform child in commentListContent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log($"RefreshCommentList - Comments count: {comments.Count}");
        foreach (var comment in comments)
        {
            var commentObject = Instantiate(commentPrefab, commentListContent);
            var commentItem = commentObject.GetComponent<CommentItem>();
            if (commentItem != null)
            {
                commentItem.Initialize(comment);
            }
        }
    }

    private void OnDestroy()
    {
        closeButton.onClick.RemoveAllListeners();
    }
}