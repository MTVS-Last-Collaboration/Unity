using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BoardController : MonoBehaviour
{
    [Header("Board References")]
    [SerializeField] private Board board;          // 게시판
    [SerializeField] private WritePanel writePanel;// 글쓰기 패널
    [SerializeField] private ClickBoard clickBoard;// 게시판 클릭 핸들러

    [Header("UI Buttons")]
    [SerializeField] private Button writeButton;   // 글쓰기 버튼
    [SerializeField] private Button sortDateButton;// 최신순 정렬 버튼
    [SerializeField] private Button sortLikeButton;// 인기순 정렬 버튼
    [SerializeField] private Button exitBoardButton;// 게시판 종료 버튼

    private HoonSoundManagerLogin sound;
    private void Start()
    {
        sound = GameObject.Find("SMJ").GetComponent<HoonSoundManagerLogin>();
        InitializeButtons();
    }

    private void InitializeButtons()
    {
        // 버튼 이벤트 연결
        writeButton.onClick.AddListener(OnWriteButtonClick);
        exitBoardButton.onClick.AddListener(OnClickExitBoard);
    }

    public void OnClickExitBoard()
    {
        sound.PlaySound("smjAudioClopAttay", 1);
        clickBoard.particle.EnableChecking();
        clickBoard.ExitBoard();
    }

    // 글쓰기 버튼 클릭
    public void OnWriteButtonClick()
    {
        sound.PlaySound("smjAudioClopAttay", 0);
        writePanel.Show();
    }

    // 최신순 정렬 버튼 클릭
    public void OnSortByDateClick()
    {
        board.SortByDate();
    }

    // 인기순 정렬 버튼 클릭
    public void OnSortByPopularClick()
    {
        board.SortByPopular();
    }

    private void OnDestroy()
    {
        writeButton.onClick.RemoveListener(OnWriteButtonClick);
        exitBoardButton.onClick.RemoveListener(OnClickExitBoard);
    }
}