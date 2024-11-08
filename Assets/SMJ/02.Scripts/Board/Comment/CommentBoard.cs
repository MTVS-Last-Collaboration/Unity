using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[System.Serializable]
public class ServerCommentData
{
    public int id;
    public string content;
    public string authorNickname;
    public string createdDate;
    public int likeCount;
}
public class CommentBoard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private RectTransform commentListContent;
    [SerializeField] private WriteCommentPanel writePanel;

    private int answerId;
    private Board parentBoard;
    private List<CommentData> comments = new List<CommentData>();

    public void Initialize(TopicAnswer answer, Board board)
    {
        Debug.Log($"CommentBoard Initialize - AnswerId: {answer.id}");
        answerId = answer.id;
        parentBoard = board;
    }

    public void AddComment(ServerCommentData serverComment)
    {
        //Debug.Log($"AddComment called - Author: {serverComment.authorNickname}, Content: {serverComment.content}");

        var comment = new CommentData(
            serverComment.id,
            serverComment.authorNickname,
            serverComment.content,
            serverComment.likeCount
        );

        //if (DateTime.TryParse(serverComment.createdDate, out DateTime date))
        //{
        //    comment.createDate = date;
        //}

        comments.Add(comment);
        //Debug.Log($"Comment added - Total comments: {comments.Count}");

        //if (gameObject.activeInHierarchy)
        //{
        //    Debug.Log("Refreshing comment list");
        //    RefreshCommentList();
        //}
        //else
        //{
        //    Debug.Log("GameObject is inactive, skipping refresh");
        //}
    }

    public void AddComment(string text)
    {
        var comment = new CommentData(
            answerId,
            LoginInfoManager.instance.nickName,
            text,
            0
        );
        //추후에 Post
        comments.Add(comment);
        var commentObject = Instantiate(commentPrefab, commentListContent);
        var commentItem = commentObject.GetComponent<CommentItem>();
        if (commentItem != null)
        {
            commentItem.Initialize(comment);
        }
        //RefreshCommentList(comment);
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
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Debug.Log($"[CommentBoard] OnEnable - Comments list content null?: {commentListContent == null}, Comments count: {comments.Count}");
        if (comments.Count > 0)  // 기존 댓글이 있을 때만 UI 갱신
        {
            RefreshCommentList();
        }
    }

    private void OnDisable()
    {
        Debug.Log($"CommentBoard OnDisable - Comments count: {comments.Count}");
    }
}