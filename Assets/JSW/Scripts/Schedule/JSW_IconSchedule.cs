using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JSW_IconSchedule : MonoBehaviour
{
    public JSW_ScheduleItem scheduleItem;
    public Image mainImage; 
    public void OnClickIcon()
    {
        mainImage.sprite = GetComponent<Image>().sprite;
        scheduleItem.iconNum = transform.GetSiblingIndex();
    }

}
