using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JSW_IconSchedule : MonoBehaviour
{
    public JSW_ScheduleManager scheduleManager;
    public Image mainImage;
    public void OnClickIcon()
    {
        mainImage.sprite = GetComponent<Image>().sprite;
        scheduleManager.iconNumInput = transform.GetSiblingIndex();
    }
}
