using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PostData
{
    public string nickName;
    public string title;
    public string content;
    public DateTime createDate;
    public int likeCount;        // UI에 표시될 전체 좋아요
    private int weeklyLikeCount; // 내부 정렬용 주간 좋아요
    private DateTime weekStartDate;

    public PostData(string nickName, string title, string content)
    {
        this.nickName = nickName;
        this.title = title;
        this.content = content;
        this.createDate = DateTime.Now;
        this.weekStartDate = DateTime.Now.Date;
        this.likeCount = 0;
        this.weeklyLikeCount = 0;
    }

    public void AddLike()
    {
        likeCount++;
        weeklyLikeCount++;
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