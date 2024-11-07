using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject postPrefab;
    [SerializeField] private WritePanel writePanel;
    [SerializeField] private RectTransform postListContent;
    [SerializeField] private Canvas boardCanvas;
    [SerializeField] private TMP_Text dayTopicText;
    [SerializeField] private TMP_Text topicText;

    [Header("Sort Buttons")]
    [SerializeField] private Button sortByPopularButton;
    [SerializeField] private Button sortByDateButton;

    private List<PostData> posts = new List<PostData>();
    private bool isPopularSortActive = false;

    private TopicManager topicManager;

    //private DateTime date = DateTime.Now;

    private void Start()
    {
        DateTime time = new DateTime(2024, 11, 6);
        //InitTopic(DateTime.Today); //실전
        InitTopic(time);
        StartCoroutine(DailyWeeklyLikesCheck());
    }

    private void InitTopic(DateTime _date)
    {
        topicManager = GetComponent<TopicManager>();
        topicManager.GetDailyTopic(_date.ToString("yyyy-MM-dd"), (success) => {
            if (success)
            {
                topicText.text = topicManager.currentContent;  // 성공했을 때만 DelayTopic 실행
                InitPosts(topicManager.currentId);
            }
            else
            {
                Debug.Log("토픽 가져오기 실패");
                // 실패 시 처리할 코드
            }
        });
        TimeSpan difference = DateTime.Now - _date;
        if (difference.Days > 0)
        {
            dayTopicText.text = $"<{difference.Days}일전 주제>";
        }
        else if(difference.Days == 0)
        {
            dayTopicText.text = "<오늘의 주제>";
        }
    }

    private void InitPosts(int id)
    {
        topicManager.GetTopicAnswers((success) =>
        {
            if (success)
            {
                foreach (var answer in topicManager.CurrentAnswers)
                {
                    CreatePost(answer);

                    // 답변에 해당하는 댓글들 가져오기
                    if (topicManager.AnswerComments.TryGetValue(answer.id, out var comments))
                    {
                        foreach (var comment in comments)
                        {
                            CommentBoard commentBoard = GetLastPostCommentBoard();
                            if (commentBoard != null)
                            {
                                commentBoard.CreateComment(comment.authorNickname, comment.content, 0);
                            }
                        }
                    }
                }
            }
        });
    }
    private CommentBoard GetLastPostCommentBoard()
    {
        if (postListContent.childCount > 0)
        {
            var lastPost = postListContent.GetChild(postListContent.childCount - 1);
            return lastPost.GetComponentInChildren<CommentBoard>();
        }
        return null;
    }
    public void CreatePost(string nickName, string title, string content, int likeCount)
    {
        var post = new PostData(nickName, title, content, likeCount);
        posts.Add(post);
        RefreshPostList();
    }

    public void CreatePost(TopicAnswer answer)
    {
        CreatePost(
        answer.authorNickname,
        "",
        answer.content,
        0
    );

        var lastPost = postListContent.GetChild(postListContent.childCount - 1);
        var commentBoard = lastPost.GetComponentInChildren<CommentBoard>(true);
        if (commentBoard != null)
        {
            gameObject.SetActive(true);  // 댓글 로드를 위해 임시 활성화
            commentBoard.Initialize(answer, () => {
                commentBoard.gameObject.SetActive(false);  // 로드 완료 후 비활성화
            });
        }
    }

    private void RefreshPostList()
    {
        foreach (Transform child in postListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var post in posts)
        {
            GameObject postObj = Instantiate(postPrefab, postListContent);
            postObj.GetComponent<PostItem>().Initialize(post);
        }
    }

    private IEnumerator DailyWeeklyLikesCheck()
    {
        while (true)
        {
            CheckAndResetWeeklyLikes();

            DateTime now = DateTime.Now;
            DateTime tomorrow = now.Date.AddDays(1);
            float secondsUntilTomorrow = (float)(tomorrow - now).TotalSeconds;

            yield return new WaitForSeconds(secondsUntilTomorrow);
        }
    }

    private void CheckAndResetWeeklyLikes()
    {
        bool needsRefresh = false;
        foreach (var post in posts)
        {
            if (post.NeedsWeeklyReset())
            {
                post.ResetWeeklyLikes();
                needsRefresh = true;
            }
        }

        if (needsRefresh && isPopularSortActive)
        {
            SortByPopular();
        }
    }



    public void SortByPopular()
    {
        isPopularSortActive = true;
        posts.Sort((a, b) => b.GetWeeklyLikes().CompareTo(a.GetWeeklyLikes()));
        RefreshPostList();
    }

    public void SortByDate()
    {
        isPopularSortActive = false;
        posts.Sort((a, b) => b.createDate.CompareTo(a.createDate));
        RefreshPostList();
    }
}