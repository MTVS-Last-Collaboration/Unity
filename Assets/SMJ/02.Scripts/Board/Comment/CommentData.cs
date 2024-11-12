using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentData
{
    public int answerId;
    public string nickName;
    public string content;
    public DateTime createDate;
    public int likeCount;        // UI에 표시될 전체 좋아요

    public CommentData(int answerId, string nickName, string content, int likeCount)
    {
        this.answerId = answerId;
        this.nickName = nickName;
        this.content = content;
        this.createDate = DateTime.Now;
        this.likeCount = likeCount;
    }

    public void AddLike()
    {
        likeCount++;
    }

    public void SubLike()
    {
        likeCount--;
    }
}
