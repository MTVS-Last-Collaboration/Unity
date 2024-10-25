using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PostItem : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text likeCountText;
    [SerializeField] private GameObject commentPanel;

    private PostData data;

    public void Initialize(PostData postData)
    {
        data = postData;
        UpdateUI();
    }

    private void UpdateUI()
    {
        titleText.text = data.title;
        contentText.text = data.content;
        dateText.text = data.createDate.ToString("yyyy-MM-dd HH:mm");
        likeCountText.text = $"¢¾ {data.likeCount}";
    }

    public void OnLikeButton()
    {
        data.AddLike();
        UpdateUI();
    }
}
