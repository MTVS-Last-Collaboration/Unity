using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private RectTransform commentListContent;
    [SerializeField] private WriteCommentPanel writePanel;
    [SerializeField] private GameObject noCommentsText;
    [SerializeField] private GameObject loadingIndicator;
    [SerializeField] private GameObject errorMessage;

    private int answerId;
    private Board parentBoard;
    private List<CommentData> comments = new List<CommentData>();
    private bool isInitialized = false;

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

    private void LoadComments()
    {
        if (gameObject.activeInHierarchy)
        {
            ShowLoadingState(true);
            StartCoroutine(LoadCommentsCoroutine());
            isInitialized = true;
        }
    }

    private IEnumerator LoadCommentsCoroutine()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("CommentBoard is inactive, skipping comments load");
            yield break;
        }

        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.GetArray<ServerCommentData>($"api/topic/{answerId}/comments",
            (success, commentList) =>
            {
                if (!gameObject.activeInHierarchy) return;

                ShowLoadingState(false);

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
                            ShowNoComments(false);
                            ShowError(false);
                        }
                        else
                        {
                            ShowNoComments(true);
                            ShowError(false);
                            Debug.Log($"No comments found for answer {answerId}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error processing comments: {e.Message}");
                        ShowError(true);
                        ShowNoComments(false);
                    }
                }
                else
                {
                    if (success) // 성공했지만 댓글이 없는 경우
                    {
                        ShowNoComments(true);
                        ShowError(false);
                        Debug.Log($"No comments found for answer {answerId}");
                    }
                    else
                    {
                        ShowError(true);
                        ShowNoComments(false);
                        Debug.LogError($"Failed to load comments");
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

        var comment = new CommentData(
            answerId,
            LoginInfoManager.instance.nickName,
            text,
            0
        );

        ShowLoadingState(true);
        CreateNewComment(comment.answerId, comment.content, () =>
        {
            comments.Add(comment);
            var commentObject = Instantiate(commentPrefab, commentListContent);
            var commentItem = commentObject.GetComponent<CommentItem>();
            if (commentItem != null)
            {
                commentItem.Initialize(comment);
            }
            ShowLoadingState(false);
            ShowNoComments(false);
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
                    ShowError(true);
                }
                ShowLoadingState(false);
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

    private void ShowLoadingState(bool show)
    {
        if (loadingIndicator != null)
            loadingIndicator.SetActive(show);
    }

    private void ShowNoComments(bool show)
    {
        if (noCommentsText != null)
            noCommentsText.SetActive(show);
    }

    private void ShowError(bool show)
    {
        if (errorMessage != null)
            errorMessage.SetActive(show);
    }

    private void OnDisable()
    {
        Debug.Log($"CommentBoard OnDisable - Comments count: {comments.Count}");
        ShowLoadingState(false);
    }
}