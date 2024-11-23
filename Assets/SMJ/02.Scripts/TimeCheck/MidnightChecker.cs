using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidnightChecker : MonoBehaviour
{
    private DateTime nextAvailableTime;
    private bool canUseFeature = true;

    private const string NextAvailableTimeKey = "NextAvailableTime";
    private DateTime lastCheckedDate;
    public bool isNewDay { get; private set; }

    private void Start()
    {
        // 저장된 다음 사용 가능 시간 로드
        string savedTime = PlayerPrefs.GetString(NextAvailableTimeKey, "");
        if (!string.IsNullOrEmpty(savedTime))
        {
            nextAvailableTime = DateTime.Parse(savedTime);
            UpdateFeatureAvailability();
        }
        else
        {
            nextAvailableTime = DateTime.Now.Date;
        }

        lastCheckedDate = DateTime.Now.Date;
        isNewDay = false;

        // 코루틴 시작
        StartCoroutine(CheckTimeRoutine());
    }

    private IEnumerator CheckTimeRoutine()
    {
        while (true)
        {
            DateTime currentDate = DateTime.Now.Date;

            // 날짜가 변경되었는지 확인
            if (currentDate > lastCheckedDate)
            {
                isNewDay = true;
                lastCheckedDate = currentDate;
                Debug.Log("자정이 지나 새로운 날이 되었습니다!");
            }
            else
            {
                isNewDay = false;
            }

            UpdateFeatureAvailability();
            yield return new WaitForSeconds(10); // 10초마다 체크 (필요에 따라 조정 가능)
        }
    }

    private void UpdateFeatureAvailability()
    {
        DateTime now = DateTime.Now;

        if (now >= nextAvailableTime)
        {
            canUseFeature = true;
            Debug.Log("기능 사용 가능!");
        }
    }
    public TimeSpan timeUntilAvailable;
    public bool UseFeature()
    {
        if (canUseFeature)
        {
            canUseFeature = false;
            // 다음 자정으로 다음 사용 가능 시간 설정
            nextAvailableTime = DateTime.Now.Date.AddDays(1);
            SaveNextAvailableTime();
            return true;
        }
        else
        {
            timeUntilAvailable = nextAvailableTime - DateTime.Now;
            return false;
        }
    }

    private void SaveNextAvailableTime()
    {
        PlayerPrefs.SetString(NextAvailableTimeKey, nextAvailableTime.ToString());
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveNextAvailableTime();
    }
}
