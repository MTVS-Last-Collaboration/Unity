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

    public void CreatePost(string nickName, string title, string content)
    {
        var post = new PostData(nickName, title, content);
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