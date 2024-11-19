using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlbumItem : MonoBehaviour
{
    public Image image;
    public TMP_Text title;
    public TMP_Text content;
    public TMP_Text day;
    public AspectRatioFitter aspectRatioFitter;
    public AlbumManager albumManager;

    private void Start()
    {
       
    }

    public void SetContents(Texture2D image1, string title1, string content1, string day1)
    {
        albumManager = GameObject.Find("AlbumManager").GetComponent<AlbumManager>();
        Sprite sprite;
        if (image1 != null)
        {
            sprite = Sprite.Create(image1, new Rect(0, 0, image1.width, image1.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            sprite = null;
        }


        image.sprite = sprite;

        //aspectRatioFitter.aspectRatio = (float)image1.width / image1.height; <- 비율 조정

        title.text = title1;
        content.text = content1;
        day.text = day1;
    }

    public void OnClickDestroyButton()
    {
        albumManager = GameObject.Find("AlbumManager").GetComponent<AlbumManager>();
        albumManager.DestroyPic = gameObject;
        albumManager.OpenDeleteUI();
    }
}
