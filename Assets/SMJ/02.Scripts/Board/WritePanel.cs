using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Net;

[Serializable]
public class WritePost
{
    public int dailyTopicId;
    public string content;
}
public class WritePanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField titleInput;    // 제목 입력 필드
    [SerializeField] private TMP_InputField contentInput;  // 내용 입력 필드
    [SerializeField] private Button submitButton;          // 글쓰기 완료 버튼
    [SerializeField] private Button exitButton;            // 나가기 버튼

    [SerializeField] private Board board;                  // 게시판 참조

    private void Start()
    {
        // 버튼 이벤트 등록
        submitButton.onClick.AddListener(OnSubmit);
        exitButton.onClick.AddListener(Hide);
    }

    // 패널 표시
    public void Show()
    {
        gameObject.SetActive(true);
        ClearInputs();
    }

    // 패널 숨김
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // 글쓰기 완료
    private void OnSubmit()
    {
        
        // 입력값 검증
        if (string.IsNullOrEmpty(contentInput.text))
        {
            Debug.Log("내용을 입력해주세요.");
            return;
        }
        
        Debug.Log($"닉네임: {LoginInfoManager.instance.nickName}, 제목: {titleInput.text}, 내용: {contentInput.text}");
        
        string nickName = LoginInfoManager.instance.nickName;

        // 게시판에 글 추가
        board.CreatePost(nickName, titleInput.text, contentInput.text, 0);

        CreatePostAnswer(PlayerPrefs.GetInt("dailyTopicId"), contentInput.text);
        Hide();
    }

    // 입력 필드 초기화
    private void ClearInputs()
    {
        titleInput.text = "";
        contentInput.text = "";
    }
    public void CreatePostAnswer(int dailyTopicId, string content, Action onComplete = null)
    {
        var newPost = new WritePost
        {
            dailyTopicId = dailyTopicId,
            content = content,
        };
        Debug.Log($"topicId: {dailyTopicId}, 내용: {content}");
        StartCoroutine(PostAnswer(dailyTopicId, newPost, onComplete));
    }
    private IEnumerator PostAnswer(int dailyTopicId, WritePost post, Action onComplete)
    {
        NetworkManager.Instance.Initialize("http://125.132.216.190:12223", PlayerPrefs.GetString("token"));
        Debug.Log($"topicId: {dailyTopicId}, 내용: {post.content}");

        yield return NetworkManager.Instance.Post($"/api/topic/answer/create", post,
            (success, response) =>
            {
                Debug.Log($"topicId: {dailyTopicId}, 내용: {post.content}");
                if (success)
                {
                    Debug.Log("Post created successfully");
                    onComplete?.Invoke();
                }
                else
                {
                    Debug.LogError($"Failed to create Post: {response}");
                }
            });

        Debug.Log($"topicId: {dailyTopicId}, 내용: {post.content}");
    }

    private void OnDestroy()
    {
        submitButton.onClick.RemoveListener(OnSubmit);
        exitButton.onClick.RemoveListener(Hide);
    }
}