using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Making3DObject : MonoBehaviour
{
    public GameObject picPrefabItem;
    public Transform picTr;
    public AlbumManager albumManager;

    public Texture modelImage;
    public GameObject isMaking3DUI;
    public GameObject isMaking3DUIImage;


    public void OnClickButtonPicUITo3D()
    {
        int childCound = picTr.childCount;
        for (int i = 0; i < childCound; i++)
        {
            Destroy(picTr.GetChild(0).gameObject);
        }

        for (int i =0;i < albumManager.Albumlist.Count;i++)
        {
            GameObject item = Instantiate(picPrefabItem, picTr);
            item.GetComponent<RawImage>().texture = albumManager.Albumlist[i].sprite;
        }
    }

    public void ClickPic(Texture texture2d)
    {
        isMaking3DUI.SetActive(true);
        modelImage = texture2d;
        isMaking3DUIImage.GetComponent<RawImage>().texture = texture2d;
    }
}