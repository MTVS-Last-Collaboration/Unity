using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CommentLikeData
{
    public int id;
    public int likeCount;
    public bool liked;
}

public class CommentItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text likeCountText;
    [SerializeField] private Button likeButton;

    private CommentData data;
    private bool isClickLike = false;

    public void Initialize(CommentData commentData)
    {
        data = commentData;
        InitializeButtons();
        UpdateUI();
    }

    private void InitializeButtons()
    {
        if (likeButton != null)
            likeButton.onClick.AddListener(OnLikeButton);
    }

    private void UpdateUI()
    {
        if (data != null)
        {
            nickNameText.text = data.nickName;
            contentText.text = data.content;
            dateText.text = data.createDate.ToString("yyyy-MM-dd HH:mm");
            UpdateLikeUI();
        }
    }

    private void UpdateLikeUI()
    {
        if (likeCountText != null)
        {
            likeCountText.text = isClickLike ? $"{data.likeCount}" : $"{data.likeCount}";
        }
    }

    public void OnLikeButton()
    {
        if (likeButton != null)
            likeButton.interactable = false;

        StartCoroutine(CommentLike(data.answerId, () => {
            data.AddLike();
            isClickLike = true;
            UpdateLikeUI();

            if (likeButton != null)
                likeButton.interactable = true;
        }));
    }

    private IEnumerator CommentLike(int commentId, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.PostWithoutBody($"/api/topic/comment/{commentId}/like",
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("Comment like successfully updated");
                    onComplete?.Invoke();
                }
                else
                {
                    if (response.Contains("409"))
                    {
                        Debug.Log("Already liked comment, trying to unlike");
                        StartCoroutine(CommentLikeCancel(commentId, () => {
                            data.SubLike();
                            isClickLike = false;
                            UpdateLikeUI();
                            if (likeButton != null)
                                likeButton.interactable = true;
                        }));
                    }
                    else
                    {
                        Debug.LogError($"Failed to update comment like: {response}");
                        if (likeButton != null)
                            likeButton.interactable = true;
                    }
                }
            });
    }

    private IEnumerator CommentLikeCancel(int commentId, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.PostWithoutBody($"/api/topic/comment/{commentId}/unlike",
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("Comment like cancel successfully updated");
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to update comment like cancel: {response}");
                    if (likeButton != null)
                        likeButton.interactable = true;
                }
            });
    }

    private void OnDestroy()
    {
        if (likeButton != null)
            likeButton.onClick.RemoveListener(OnLikeButton);
    }
}