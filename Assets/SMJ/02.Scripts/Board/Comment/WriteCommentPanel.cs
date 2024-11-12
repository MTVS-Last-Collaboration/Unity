using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WriteCommentPanel : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_InputField contentInput;  // 내용 입력 필드

    [SerializeField] private CommentBoard commentBoard;                  // 게시판 참조

    /*public void OnSubmit()
    {
        Debug.Log($"닉네임: {LoginInfoManager.instance.nickName}, 내용: {contentInput.text}");
        // 입력값 검증
        if (string.IsNullOrEmpty(contentInput.text))
        {
            Debug.Log("내용을 입력해주세요.");
            return;
        }
        
        string nickName = LoginInfoManager.instance.nickName;

        // 게시판에 글 추가
        commentBoard.CreateComment(nickName, contentInput.text, 0);
    }*/

    public void OnSubmitComment()
    {
        commentBoard.AddComment(contentInput.text);
        contentInput.text = string.Empty;
        // 입력 필드 초기화 등 필요한 처리
    }
}
