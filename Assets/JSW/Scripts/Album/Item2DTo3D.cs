using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item2DTo3D : MonoBehaviour
{
    public Making3DObject making3dObject;

    // Start is called before the first frame update
    void Start()
    {
        making3dObject = GameObject.Find("AlbumManager").GetComponent<Making3DObject>();
    }

    public void OnClickButtonMe()
    {
        making3dObject.ClickPic((GetComponent<RawImage>().texture));
    }
}
