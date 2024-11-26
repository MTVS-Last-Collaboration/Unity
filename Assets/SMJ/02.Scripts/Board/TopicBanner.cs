using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class TopicBanner : MonoBehaviour
{
    [SerializeField] private TMP_Text contentText;
    [SerializeField] private Button button;
    private int days;
    private Board board;
    private TopicManager topicManager;
    private HoonSoundManagerLogin sound;
    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        topicManager = GameObject.Find("Board").GetComponent<TopicManager>();
        sound = GameObject.Find("SMJ").GetComponent<HoonSoundManagerLogin>();
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
            sound.PlaySound("smjAudioClopAttay", 0);
            topicManager.CloseWeeklyTopics();
            button.interactable = false;
            DateTime now = new DateTime(2024, 11, 15).AddDays(-days);
            topicManager._date = now.ToString("yyyy-MM-dd");
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
        contentText.text = topic.content;
        days = day;
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(OnClickBanner);
    }
}