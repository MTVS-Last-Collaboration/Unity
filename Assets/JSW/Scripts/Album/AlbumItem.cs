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

    public void SetContents(Texture2D image1, string title1, string content1, string day1)
    {
        Sprite sprite = Sprite.Create(image1, new Rect(0, 0, image1.width, image1.height), new Vector2(0.5f, 0.5f));

        image.sprite = sprite;

        //aspectRatioFitter.aspectRatio = (float)image1.width / image1.height; <- 비율 조정

        title.text = title1;
        content.text = content1;
        day.text = day1;
    }
}
