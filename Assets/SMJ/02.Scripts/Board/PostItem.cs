using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PostItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nickNameText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private TMP_Text dateText;
    [SerializeField] private TMP_Text likeCountText;
    [SerializeField] private GameObject commentPanel;
    [SerializeField] private LayoutElement layout;
    [SerializeField] private VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private CommentBoard comment;

    [SerializeField] private float minHeight = 265f;
    [SerializeField] private float maxHeight = 1030f;

    [SerializeField] private int minVertical = 7;
    [SerializeField] private int maxVertical = 780;

    [SerializeField] private Button openCommentButton;   // 댓글창 열기 버튼

    private PostData data;

    public void Initialize(PostData postData)
    {
        data = postData;
        UpdateUI();
    }

    private void Start()
    {
        // 추후 더미데이터는 백엔드에 저장 후, 여기에서 로드하자.
        comment.CreateComment("페이커", "쵸비화이팅!", 10);
        comment.CreateComment("페이커", "쵸비화이팅!", 2);
        comment.CreateComment("페이커", "쵸비화이팅!", 8);
        comment.CreateComment("페이커", "쵸비화이팅!", 11);

        layout = GetComponent<LayoutElement>();
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        InitializeButtons();
        if (commentPanel.activeSelf == true)
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

    private void InitializeButtons()
    {
        // 버튼 이벤트 연결
        openCommentButton.onClick.AddListener(OnToggleCommentPanel);
    }

    // 댓글창 열기 버튼 클릭
    public void OnToggleCommentPanel()
    {
        if (commentPanel.activeSelf == true)
        {
            commentPanel.SetActive(false);
            layout.preferredHeight = minHeight;
            verticalLayoutGroup.padding.bottom = minVertical;
        }
        else
        {
            commentPanel.SetActive(true);
            layout.preferredHeight = maxHeight;
            verticalLayoutGroup.padding.bottom = maxVertical;
        }
    }

    private void UpdateUI()
    {
        if (data != null)
        {
            nickNameText.text = data.nickName;
            titleText.text = data.title;
            contentText.text = data.content;
            dateText.text = data.createDate.ToString("yyyy-MM-dd HH:mm");
            likeCountText.text = $"♥ {data.likeCount}";
        }
    }

    public void OnLikeButton()
    {
        data.AddLike();
        UpdateUI();
    }

    private void OnDestroy()
    {
        openCommentButton.onClick.RemoveListener(OnToggleCommentPanel);
    }
}
