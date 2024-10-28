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

    [Header("Sort Buttons")]
    [SerializeField] private Button sortByPopularButton;
    [SerializeField] private Button sortByDateButton;

    private List<PostData> posts = new List<PostData>();
    private bool isPopularSortActive = false;

    private void Start()
    {
        // 추후 더미데이터는 백엔드에 저장 후, 여기에서 로드하자.
        CreatePost("", "<이번주의 썸톡 이슈>", "TOPIC : 식사 자리에서 연인의 친구가 깻잎을 못 떼고 있다면, 내 연인이 깻잎을 대신 떼어주는 게 괜찮을까?", 10);
        StartCoroutine(DailyWeeklyLikesCheck());
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

    public void CreatePost(string nickName, string title, string content, int likeCount)
    {
        var post = new PostData(nickName, title, content, likeCount);
        posts.Add(post);
        RefreshPostList();
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
}