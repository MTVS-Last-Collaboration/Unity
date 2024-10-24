using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [Header("Board References")]
    [SerializeField] private Board board;          // 게시판
    [SerializeField] private WritePanel writePanel;// 글쓰기 패널

    [Header("UI Buttons")]
    [SerializeField] private Button writeButton;   // 글쓰기 버튼
    [SerializeField] private Button sortDateButton;// 최신순 정렬 버튼
    [SerializeField] private Button sortLikeButton;// 인기순 정렬 버튼

    private void Start()
    {
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // 버튼 이벤트 연결
        writeButton.onClick.AddListener(OnWriteButtonClick);
        sortDateButton.onClick.AddListener(OnSortByDateClick);
        sortLikeButton.onClick.AddListener(OnSortByPopularClick);
    }

    // 글쓰기 버튼 클릭
    private void OnWriteButtonClick()
    {
        writePanel.Show();
    }

    // 최신순 정렬 버튼 클릭
    private void OnSortByDateClick()
    {
        board.SortByDate();
    }

    // 인기순 정렬 버튼 클릭
    private void OnSortByPopularClick()
    {
        board.SortByPopular();
    }
}
