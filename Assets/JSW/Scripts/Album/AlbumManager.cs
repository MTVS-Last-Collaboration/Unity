using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumManager : MonoBehaviour
{
    public GameObject picUploadingUI;
    public GameObject PicFactory;
    public RectTransform trContent;

    // Start is called before the first frame update
    void Awake()
    {
        picUploadingUI = GameObject.Find("PicUploadingUI");
        trContent = GameObject.Find("AlbumContentBody").GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingPic()
    {
        GameObject newPic = Instantiate(PicFactory, trContent);
        string title = picUploadingUI.transform.GetChild(0).GetComponent<TMP_InputField>().text;
        string content = picUploadingUI.transform.GetChild(1).GetComponent<TMP_InputField>().text;
        Texture2D newImage = picUploadingUI.transform.GetChild(2).GetChild(0).GetComponent<GalleryAccess>().texture;
        string day = picUploadingUI.transform.GetChild(3).GetComponent<TMP_InputField>().text;
        newPic.GetComponent<AlbumItem>().SetContents(newImage, title, content, day);
        print("jj");
    }
}
