using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PostLikeData
{
    public int id;
    public int likeCount;
    public bool liked;
}

public class PostItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text likeCountText;
    [SerializeField] private GameObject commentPanel;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private CommentBoard comment;
    [SerializeField] private Button openCommentButton;
    [SerializeField] private Button likeButton;

    [SerializeField] private float minHeight = 265f;
    [SerializeField] private float maxHeight = 1030f;
    [SerializeField] private int minVertical = 7;
    [SerializeField] private int maxVertical = 780;

    private LayoutElement layout;
    private PostData data;
    private bool isClickLike = false;
    private bool isInitialized = false;

    public void Initialize(PostData postData)
    {
        data = postData;
        layout = GetComponent<LayoutElement>();
        InitializeButtons();
        UpdateUI();

        if (commentPanel.activeSelf)
        {
            layout.preferredHeight = maxHeight;
            verticalLayoutGroup.padding.bottom = maxVertical;
        }
        else
        {
            layout.preferredHeight = minHeight;
            verticalLayoutGroup.padding.bottom = minVertical;
        }
    }

    private void OnEnable()
    {
        if (!isInitialized && data != null)
        {
            LoadLikeStatus();
        }
    }

    private void LoadLikeStatus()
    {
        if (gameObject.activeInHierarchy)
        {
            isInitialized = true;
        }
    }

    private void InitializeButtons()
    {
        if (openCommentButton != null)
            openCommentButton.onClick.AddListener(OnToggleCommentPanel);

        if (likeButton != null)
            likeButton.onClick.AddListener(OnLikeButton);
    }

    public void OnToggleCommentPanel()
    {
        bool isOpen = !commentPanel.activeSelf;
        commentPanel.SetActive(isOpen);
        layout.preferredHeight = isOpen ? maxHeight : minHeight;
        verticalLayoutGroup.padding.bottom = isOpen ? maxVertical : minVertical;
    }

    private void UpdateUI()
    {
        if (data != null)
        {
            nickNameText.text = data.nickName;
            titleText.text = data.title;
            contentText.text = data.content;
            dateText.text = data.createDate.ToString("yyyy-MM-dd HH:mm");
            UpdateLikeUI();
        }
    }

    private void UpdateLikeUI()
    {
        if (likeCountText != null)
        {
            likeCountText.text = isClickLike ? $"¢¾ {data.likeCount}" : $"¢¾ {data.likeCount}";
        }
    }

    private IEnumerator CheckInitialLikeStatus()
    {
        if (!gameObject.activeInHierarchy)
        {
            Debug.Log("PostItem is inactive, skipping like status check");
            yield break;
        }

        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.GetWithoutBody($"api/topic/answer/{data.answerId}/like",
            (success, response) =>
            {
                if (!gameObject.activeInHierarchy) return;

                if (success)
                {
                    try
                    {
                        var likeData = JsonUtility.FromJson<PostLikeData>(response);
                        if (likeData != null)
                        {
                            isClickLike = likeData.liked;
                            data.likeCount = likeData.likeCount;
                            UpdateLikeUI();
                            Debug.Log($"Initial like status loaded: liked={isClickLike}, count={likeData.likeCount}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Error parsing like status: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogError($"Failed to get initial like status: {response}");
                }
            });
    }

    public void OnLikeButton()
    {
        if (likeButton != null)
            likeButton.interactable = false;

        StartCoroutine(PostLike(data.answerId, () => {
            data.AddLike();
            isClickLike = true;
            UpdateLikeUI();

            if (likeButton != null)
                likeButton.interactable = true;
        }));
    }

    private IEnumerator PostLike(int answerId, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.PostWithoutBody($"/api/topic/answer/{answerId}/like",
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("Like successfully updated");
                    onComplete?.Invoke();
                }
                else
                {
                    if (response.Contains("409"))
                    {
                        Debug.Log("Already liked post, trying to unlike");
                        StartCoroutine(PostLikeCancel(answerId, () => {
                            data.SubLike();
                            isClickLike = false;
                            UpdateLikeUI();
                            if (likeButton != null)
                                likeButton.interactable = true;
                        }));
                    }
                    else
                    {
                        Debug.LogError($"Failed to update like: {response}");
                        if (likeButton != null)
                            likeButton.interactable = true;
                    }
                }
            });
    }

    private IEnumerator PostLikeCancel(int answerId, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));

        yield return NetworkManager.Instance.PostWithoutBody($"/api/topic/answer/{answerId}/unlike",
            (success, response) =>
            {
                if (success)
                {
                    Debug.Log("LikeCancel successfully updated");
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to update likeCancel: {response}");
                    if (likeButton != null)
                        likeButton.interactable = true;
                }
            });
    }

    private void OnDestroy()
    {
        if (openCommentButton != null)
            openCommentButton.onClick.RemoveListener(OnToggleCommentPanel);

        if (likeButton != null)
            likeButton.onClick.RemoveListener(OnLikeButton);
    }
}