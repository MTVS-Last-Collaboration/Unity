using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        Debug.Log($"닉네임: {LoginInfoManager.instance.nickName}, 제목: {titleInput.text}, 내용: {contentInput.text}");
        // 입력값 검증
        if (string.IsNullOrEmpty(titleInput.text) || string.IsNullOrEmpty(contentInput.text))
        {
            Debug.Log("제목과 내용을 입력해주세요.");
            return;
        }

        string nickName = LoginInfoManager.instance.nickName;

        // 게시판에 글 추가
        board.CreatePost(nickName, titleInput.text, contentInput.text, 0);
        Hide();
    }

    // 입력 필드 초기화
    private void ClearInputs()
    {
        titleInput.text = "";
        contentInput.text = "";
    }

    private void OnDestroy()
    {
        submitButton.onClick.RemoveListener(OnSubmit);
        exitButton.onClick.RemoveListener(Hide);
    }
}