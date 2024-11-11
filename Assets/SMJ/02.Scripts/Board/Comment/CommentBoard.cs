using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

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

    private int answerId;
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

        // NetworkManager 미리 초기화
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
    }

    public void LikeClick()
    {
        item.OnLikeButton();
    }

    public void Close()
    {
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
        yield return NetworkManager.Instance.GetArray<ServerCommentData>($"api/topic/{answerId}/comments",
            (success, commentList) =>
            {
                if (!gameObject.activeInHierarchy) return;

                if (success && commentList != null)
                {
                    try
                    {
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
                        Debug.LogError($"Error processing comments: {e.Message}");
                    }
                }
            });
    }

    public void AddComment(ServerCommentData serverComment)
    {
        var comment = new CommentData(
            serverComment.id,
            serverComment.authorNickname,
            serverComment.content,
            serverComment.likeCount
        );

        if (DateTime.TryParse(serverComment.createdDate, out DateTime date))
        {
            comment.createDate = date;
        }

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