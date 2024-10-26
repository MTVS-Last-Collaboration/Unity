using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentController : MonoBehaviour
{
    [Header("Comment References")]
    [SerializeField] private WriteCommentPanel writeCommentPanel;// 댓글쓰기 패널

    [Header("UI Buttons")]
    [SerializeField] private Button writeCommentButton;   // 댓글쓰기 버튼

    private void Start()
    {
        //writeCommentPanel = GameObject.Find("CommentWritePanel").GetComponent<WriteCommentPanel>();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // 버튼 이벤트 연결
        writeCommentButton.onClick.AddListener(OnWriteButtonClick);
    }

    // 댓글쓰기 버튼 클릭
    public void OnWriteButtonClick()
    {
        writeCommentPanel.OnSubmit();
    }

    private void OnDestroy()
    {
        writeCommentButton.onClick.RemoveListener(OnWriteButtonClick);
    }
}
