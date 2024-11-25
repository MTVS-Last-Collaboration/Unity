using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomShareManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ceiling;
    public Camera RenderingCam;
    public GameObject cri;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickShareButton()
    {
        ceiling.SetActive(false);
        RenderingCam.cullingMask &= ~(1 << LayerMask.NameToLayer("Player"));

        RawImage ri = cri.GetComponent<RawImage>();

        // 각 모델을 위한 Render Texture 생성
        RenderTexture renderTextures = new RenderTexture(256, 256, 16);
        RenderingCam.targetTexture = renderTextures;

        // 모델 위치 조정 및 렌더링
        RenderingCam.Render();

        // UI에 해당 Render Texture 할당
        ri.texture = renderTextures;
        RenderingCam.targetTexture = renderTextures = new RenderTexture(256, 256, 16);
        //models[i].SetActive(false);
        //Destroy(models[i]);

        //if (i == models.Length - 1)
        //{
        //    RenderingCam.targetTexture = new RenderTexture(256, 256, 16);
        //}

        //if (i % 22 == 0 && i != 0)
        //{
        //    RenderingCam.targetTexture = new RenderTexture(256, 256, 16);

        //}
        ceiling.SetActive(true);
        
    }

    
}
