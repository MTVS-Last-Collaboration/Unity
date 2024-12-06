using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialUIParents;
    public GameObject tutorialUI;
    public TMP_Text pageNumber;
    public int number = 1;

    public void ButtonClickTutorial()
    {
        number = 1;
        tutorialUI.transform.GetChild(number).gameObject.SetActive(true);
        tutorialUIParents.SetActive(true);
        pageNumber.text = number.ToString() + " / 6";
    }
    public void ButtonClickTutorial_Back()
    {
        tutorialUI.transform.GetChild(number).gameObject.SetActive(false);
        tutorialUIParents.SetActive(false);
    }

    public void OnRightClick()
    {
        tutorialUI.transform.GetChild(number).gameObject.SetActive(false);
        number += 1;
        if (number >= 7)
        {
            number = 1;
        }
        tutorialUI.transform.GetChild(number).gameObject.SetActive(true);
        pageNumber.text = number.ToString() + " / 6";
    }

    public void OnLeftClick()
    {
        tutorialUI.transform.GetChild(number).gameObject.SetActive(false);
        number -= 1;
        if (number <= 0)
        {
            number = 6;
        }
        tutorialUI.transform.GetChild(number).gameObject.SetActive(true);
        pageNumber.text = number.ToString() + " / 6";
    }
}
