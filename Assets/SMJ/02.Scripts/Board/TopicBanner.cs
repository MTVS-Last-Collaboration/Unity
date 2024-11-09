using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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

    public async void OnClickBanner()
    {
        try
        {
            topicManager.CloseWeeklyTopics();
            button.interactable = false;
            DateTime now = DateTime.Now.AddDays(-days);
            await InitTopic(now);
        }
        finally
        {
            button.interactable = true;
        }
    }

    public async Task InitTopic(DateTime day)
    {
        await board.InitTopic(day);
    }

    public void Initialize(Topic topic, int day)
    {
        days = day;
        titleText.text = day == 0 ? "<坷疵狼 林力>" : $"<{day}老傈 林力>";
        contentText.text = topic.content;
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClickBanner);
    }
}