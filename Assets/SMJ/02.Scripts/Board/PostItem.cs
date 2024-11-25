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
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    public CommentBoard commentBoard;
    [SerializeField] private Button likeButton;
    [SerializeField] private Button viewCommentsButton;
    [SerializeField] private float minHeight = 265f;
    [SerializeField] private float maxHeight = 1030f;
    [SerializeField] private int minVertical = 7;
    [SerializeField] private int maxVertical = 780;
    private bool isRequesting = false;
    private LayoutElement layout;
    [SerializeField] public PostData data;
    private bool isClickLike = false;
    private bool isInitialized = false;

    [SerializeField] private GameObject detailPostPanel;
    [SerializeField] private GameObject PostListPanel;
    public void Initialize(PostData postData)
    {
        data = postData;

        layout = GetComponent<LayoutElement>();
        InitializeButtons();
        UpdateUI();
    }

    private void OnEnable()
    {
        if (!isInitialized && data != null)
        {
            LoadLikeStatus();
        }
    }

    private void Start()
    {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(1.0f);
        detailPostPanel = GameObject.Find("SMJ/Board/Board_Canvas/BoardHandler/DetailPostPanel");
        PostListPanel = GameObject.Find("SMJ/Board/Board_Canvas/BoardHandler/PostListPanel");
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
        if (viewCommentsButton != null)
            viewCommentsButton.onClick.AddListener(OnToggleCommentPanel);

        if (likeButton != null)
            likeButton.onClick.AddListener(OnLikeButton);
    }

    public void OnToggleCommentPanel()
    {
        detailPostPanel = GameObject.Find("SMJ/Board/Board_Canvas/BoardHandler/DetailPostPanel");
        if (PostListPanel.activeSelf)
        {
            PostListPanel.SetActive(false);
        }
        else
        {
            PostListPanel.SetActive(true);
        }
        if (detailPostPanel != null)
        {
            // CommentBoard 가져오기 및 댓글 표시
            CommentBoard commentBoard = detailPostPanel.GetComponent<CommentBoard>();
            if (commentBoard != null)
            {
                commentBoard.item = gameObject.GetComponent<PostItem>();
                // DisplayCommentsForAnswer만 호출
                commentBoard.DisplayCommentsForAnswer(data.answerId);
                commentBoard.title.text = data.title;
                commentBoard.nickName.text = data.nickName;
                commentBoard.content.text = data.content;
                commentBoard.date.text = date;
                commentBoard.likeCountText.text = data.likeCount.ToString();
                detailPostPanel.SetActive(true);
            }
        }
        //bool isOpen = !commentPanel.activeSelf;
        //commentPanel.SetActive(isOpen);
        //ayout.preferredHeight = isOpen ? maxHeight : minHeight;
        //verticalLayoutGroup.padding.bottom = isOpen ? maxVertical : minVertical;
    }
    private IEnumerator WaitForNetworkAndShowPanel()
    {
        isRequesting = true;

        // 네트워크 매니저 초기화가 완료될 때까지 대기
        yield return new WaitForSeconds(0.1f);

        GameObject detailPostPanel = GameObject.Find("SMJ/Board/Board_Canvas/BoardHandler/DetailPostPanel");
        if (detailPostPanel != null)
        {
            CommentBoard commentBoard = detailPostPanel.GetComponent<CommentBoard>();
            if (commentBoard != null)
            {
                detailPostPanel.SetActive(true);
                commentBoard.DisplayCommentsForAnswer(data.answerId);
            }
        }

        isRequesting = false;
    }
    string date;
    private void UpdateUI()
    {
        if (data != null)
        {
            nickNameText.text = data.nickName;
            titleText.text = data.title;
            contentText.text = data.content;
            dateText.text = data.createdDate;
            date = data.createdDate;
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

    private void OnClickPost()
    {
        
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
                            commentBoard.likeCountText.text = data.likeCount.ToString();
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
            commentBoard = GameObject.Find("SMJ/Board/Board_Canvas/BoardHandler/DetailPostPanel").GetComponent<CommentBoard>();
            commentBoard.likeCountText.text = data.likeCount.ToString();
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
                            commentBoard.likeCountText.text = data.likeCount.ToString();
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
        if (viewCommentsButton != null)
            viewCommentsButton.onClick.AddListener(OnToggleCommentPanel);

        if (likeButton != null)
            likeButton.onClick.RemoveListener(OnLikeButton);
    }
}