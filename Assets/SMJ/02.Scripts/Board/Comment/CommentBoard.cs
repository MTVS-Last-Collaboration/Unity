using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class ServerCommentData
{
    public int id;
    public string content;
    public string authorNickname;
    public string createdDate;
}

public class CommentBoard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private WriteCommentPanel writeCommentPanel;
    [SerializeField] private RectTransform commentListContent;
    private List<CommentData> comments = new List<CommentData>();

    private int answerId;

    public void Initialize(TopicAnswer answer, Action onLoadComplete = null)
    {
        this.answerId = answer.id;
        // Board에서 코루틴 실행
        var board = GetComponentInParent<Board>();
        if (board != null)
        {
            //board.StartCoroutine(LoadCommentsCoroutine(onLoadComplete));
        }
    }
    public void CreateComment(string content)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        var newComment = new ServerCommentData
        {
            content = content
        };

        StartCoroutine(NetworkManager.Instance.Post($"api/topic/{answerId}/comments", newComment,
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("Comment created successfully");
                    LoadComments();
                    RefreshCommentList();
                }
                else
                {
                    Debug.LogError("Failed to create comment");
                }
            }));
    }

    public void CreateComment(string authorNickname, string content, int likeCount)
    {
        var comment = new CommentData(authorNickname, content, likeCount);
        comments.Add(comment);
        RefreshCommentList();
    }

    public void LoadComments()
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
        StartCoroutine(NetworkManager.Instance.GetArray<ServerCommentData>($"api/topic/{answerId}/comments",
            (success, result) =>
            {
                if (success && result != null)
                {
                    comments.Clear();
                    foreach (var serverComment in result)
                    {
                        var comment = new CommentData(
                            serverComment.authorNickname,
                            serverComment.content,
                            0  // 좋아요 수 초기화
                        );
                        if (DateTime.TryParse(serverComment.createdDate, out DateTime createDate))
                        {
                            comment.createDate = createDate;
                        }
                        comments.Add(comment);
                    }
                    RefreshCommentList();
                    Debug.Log($"Successfully loaded {comments.Count} comments for answer {answerId}");
                }
                else
                {
                    Debug.LogError($"Failed to load comments for answer {answerId}");
                }
            }));
    }
    private IEnumerator LoadCommentsCoroutine(Action onLoadComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
        bool isComplete = false;

        NetworkManager.Instance.GetArray<ServerCommentData>($"api/topic/{answerId}/comments",
            (success, result) =>
            {
                if (success && result != null)
                {
                    comments.Clear();
                    foreach (var comment in result)
                    {
                        CreateComment(comment.authorNickname, comment.content, 0);
                    }
                    Debug.Log($"Successfully loaded {comments.Count} comments for answer {answerId}");
                }
                else
                {
                    Debug.LogError($"Failed to load comments for answer {answerId}");
                }

                gameObject.SetActive(false);  // 댓글 로드 완료 후 비활성화
                isComplete = true;
                onLoadComplete?.Invoke();
            });

        while (!isComplete)
        {
            yield return null;
        }
    }

    private void RefreshCommentList()
    {
        foreach (Transform child in commentListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var comment in comments)
        {
            GameObject commentObj = Instantiate(commentPrefab, commentListContent);
            commentObj.GetComponent<CommentItem>().Initialize(comment);
        }
    }
}