using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostData
{
    public int answerId;
    public string nickName;
    public string title;
    public string content;
    public string createdDate;
    public int likeCount;        // UI에 표시될 전체 좋아요
    private int weeklyLikeCount; // 내부 정렬용 주간 좋아요
    private DateTime weekStartDate;
    private CommentBoard comment;

    public PostData(int answerId, string nickName, string title, string content, string date, int likeCount)
    {
        this.answerId = answerId;
        this.nickName = nickName;
        this.title = title;
        this.content = content;
        this.createdDate = date;
        this.weekStartDate = DateTime.Now.Date;
        this.likeCount = likeCount;
        this.weeklyLikeCount = 0;
    }

    public void AddLike()
    {
        likeCount++;
        weeklyLikeCount++;
    }

    public void SubLike()
    {
        likeCount--;
        weeklyLikeCount--;
    }

    public int GetLike()
    {
        return likeCount;
    }

    public void ResetWeeklyLikes()
    {
        weeklyLikeCount = 0;
        weekStartDate = DateTime.Now.Date;
    }

    public bool NeedsWeeklyReset()
    {
        return (DateTime.Now.Date - weekStartDate).Days >= 7;
    }

    public int GetWeeklyLikes() => weeklyLikeCount;
}