using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommentItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text likeCountText;

    private CommentData data;

    public void Initialize(CommentData commentData)
    {
        data = commentData;
        print(commentData);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (data != null)
        {
            nickNameText.text = data.nickName;
            contentText.text = data.content;
            dateText.text = data.createDate.ToString("yyyy-MM-dd HH:mm");
            likeCountText.text = $"¢¾ {data.likeCount}";
        }
    }

    public void OnLikeButton()
    {
        data.AddLike();
        UpdateUI();
    }
}
