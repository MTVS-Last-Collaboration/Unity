using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjFileLoad : MonoBehaviour
{
    public string objUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/3dmodel.obj"; // .obj 파일 URL
    public string textureUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/texture.png"; // 텍스처 PNG URL

    void Start()
    {
        // 코루틴으로 모델 로드 시작
        StartCoroutine(LoadOBJFromURL(objUrl, textureUrl));
    }

    IEnumerator LoadOBJFromURL(string objPath, string texturePath)
    {
        // Step 1: .obj 파일 다운로드
        UnityWebRequest objRequest = UnityWebRequest.Get(objPath);
        yield return objRequest.SendWebRequest();

        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("OBJ 파일 다운로드 실패: " + objRequest.error);
            yield break;
        }

        string objData = objRequest.downloadHandler.text;

        // Step 2: 텍스처 다운로드
        Texture2D texture = null;
        if (!string.IsNullOrEmpty(texturePath))
        {
            UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(texturePath);
            yield return textureRequest.SendWebRequest();

            if (textureRequest.result == UnityWebRequest.Result.Success)
            {
                texture = DownloadHandlerTexture.GetContent(textureRequest);
            }
            else
            {
                Debug.LogError("텍스처 다운로드 실패: " + textureRequest.error);
            }
        }

        // Step 3: .obj 모델 로드
        OBJLoader loader = new OBJLoader();

        GameObject loadedModel = loader.Load(objData);

        if (loadedModel != null)
        {
            // 씬에 모델 배치
            loadedModel.transform.position = Vector3.zero;
            loadedModel.transform.localScale = Vector3.one;

            // Step 4: 텍스처 적용
            if (texture != null)
            {
                ApplyTextureToModel(loadedModel, texture);
            }

            Debug.Log("OBJ 모델 로드 완료!");
        }
        else
        {
            Debug.LogError(".obj 파일 로드 실패");
        }
    }

    void ApplyTextureToModel(GameObject model, Texture2D texture)
    {
        Renderer renderer = model.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(Shader.Find("Standard"));
            material.mainTexture = texture;
            renderer.material = material;
        }
        else
        {
            Debug.LogError("모델에 Renderer가 없습니다!");
        }
    }
}
