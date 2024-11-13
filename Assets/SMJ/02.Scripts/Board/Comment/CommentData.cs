using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommentData
{
    public int id;
    public string nickName;
    public string content;
    public string createdDate;
    public int likeCount;
    public int answerId;

    public CommentData(int id, string nickName, string content, string createdDate, int likeCount)
    {
        this.id = id;
        this.nickName = nickName;
        this.content = content;
        this.createdDate = createdDate;
        this.likeCount = likeCount;
    }

    public void AddLike() => likeCount++;
    public void SubLike() => likeCount--;
}
