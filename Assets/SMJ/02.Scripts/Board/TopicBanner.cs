using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopicBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Button button;
    private int days;
    private Board board;
    private TopicManager topicManager;

    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        topicManager = GameObject.Find("Board").GetComponent<TopicManager>();

        if (button == null)
            button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(OnClickBanner);
    }

    public void OnClickBanner()
    {
        DateTime now = DateTime.Now.AddDays(-days);
        InitTopic(now);
    }

    public void InitTopic(DateTime day)
    {
        board.ClearBoard(); // 먼저 보드를 클리어
        board.InitTopic(day);
    }

    public void Initialize(Topic topic, int day)
    {
        days = day;
        titleText.text = day == 0 ? "<오늘의 주제>" : $"<{day}일전 주제>";
        contentText.text = topic.content;
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClickBanner);
    }
}