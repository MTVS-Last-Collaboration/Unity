using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentData
{
    public string nickName;
    public string content;
    public DateTime createDate;
    public int likeCount;        // UI에 표시될 전체 좋아요

    public CommentData(string nickName, string content)
    {
        this.nickName = nickName;
        this.content = content;
        this.createDate = DateTime.Now;
        this.likeCount = 0;
    }

    public void AddLike()
    {
        likeCount++;
    }
}
