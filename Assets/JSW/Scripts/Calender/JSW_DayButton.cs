using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class JSW_DayButton : MonoBehaviour
{
    public GameObject CalenderManager;
    // Start is called before the first frame update

    public void Start()
    {
        CalenderManager = GameObject.Find("CalenderManager");
        GetComponent<Button>().onClick.AddListener(OnClickMe);
    }
    public void OnClickMe()
    {
        JSW_SoundManager.Get().PlayEftSound(JSW_SoundManager.ESoundType.EFT_ButtonSound2);
        if (transform.GetChild(5).GetComponent<TMP_Text>().text != "") CalenderManager.GetComponent<JSW_CalenderManager>().OnClickResetNowDay(int.Parse(transform.GetChild(5).GetComponent<TMP_Text>().text));
    }
}
