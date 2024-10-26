using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class CommentBoard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject commentPrefab;
    [SerializeField] private WriteCommentPanel writeCommentPanel;
    [SerializeField] private RectTransform commentListContent;

    private List<CommentData> comments = new List<CommentData>();

    public void CreateComment(string nickName, string content)
    {
        var comment = new CommentData(nickName, content);
        comments.Add(comment);
        RefreshCommentList();
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
