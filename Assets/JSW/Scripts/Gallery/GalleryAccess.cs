using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using WebSocketSharp;
using SFB;
public class GalleryAccess : MonoBehaviour
{
    public AspectRatioFitter aspectRatioFitter;
    public Texture2D texture;
    //public RawImage raw;

    // 갤러리에서 이미지를 선택하는 메서드
    public void PickImageFromGallery()
    {

#if UNITY_STANDALONE_WIN
        string[] ex = new string[]
        {
    "xbm", "tif", "jfif", "ico", "tiff", "gif", "svg", "jpeg", "svgz", "jpg", "webp", "png", "bmp", "pjp", "apng", "pjpeg", "avif"
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("열기", "", new ExtensionFilter[1] { new ExtensionFilter("이미지 파일", ex) }, false);
        if (paths.Length > 0)
        {
            byte[] fileData = System.IO.File.ReadAllBytes(paths[0]);
            texture = new Texture2D(2, 2);
            if (texture.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                GetComponent<Image>().sprite = sprite;
            }
        }
#else
            // 갤러리 접근 권한 확인 및 요청
            if (NativeGallery.CheckPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image) == NativeGallery.Permission.Granted ||
        NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image) == NativeGallery.Permission.Granted)
            {
                // 갤러리에서 이미지 선택
                NativeGallery.GetImageFromGallery((path) =>
                {
                    if (path != null)
                    {

                        Texture2D texture2 = new Texture2D(2, 2);
                        // 이미지 경로를 통해 Texture2D로 로드
                        texture = NativeGallery.LoadImageAtPath(path, -1, false);
                        Texture2D readableTexture = new Texture2D(texture.width, texture.height);
                        readableTexture.SetPixels(texture.GetPixels());
                        readableTexture.Apply();

                        byte[] pngData = readableTexture.EncodeToPNG();
                        texture2.LoadImage(pngData);
                        //raw.texture = texture2;
                        if (texture != null)
                        {
                            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

                            GetComponent<Image>().sprite = sprite;

                            aspectRatioFitter.aspectRatio = (float)texture.width / texture.height;

                            Debug.Log("이미지 로드 성공: " + path);
                        }
                        else
                        {
                            Debug.LogError("이미지 로드 실패");
                        }
                    }
                }, "이미지를 선택하세요");
            }
            else
            {
                Debug.LogError("갤러리 접근 권한이 없습니다.");
            }
#endif
    }

}
