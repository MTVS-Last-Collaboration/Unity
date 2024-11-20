using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;


public class ChatTest : MonoBehaviour
{
    public string objUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/14c26e19-8d91-4e7b-a3a3-10078d668cd6.obj";
    public string mtlUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/609e4fbe-2d08-4575-ac01-4b576b3dfbd8.mtl";
    public string textureBaseUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/44f8ff9f-189f-4893-bbf9-a9795f7d3b10.png"; // 텍스처 파일의 경로

    void Start()
    {
        StartCoroutine(DownloadAndLoadOBJWithMTL());
    }

    IEnumerator DownloadAndLoadOBJWithMTL()
    {
        // Step 1: Download OBJ file
        UnityWebRequest objRequest = UnityWebRequest.Get(objUrl);
        yield return objRequest.SendWebRequest();

        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download OBJ: " + objRequest.error);
            yield break;
        }

        string objData = objRequest.downloadHandler.text;

        // Step 2: Download MTL file
        UnityWebRequest mtlRequest = UnityWebRequest.Get(mtlUrl);
        yield return mtlRequest.SendWebRequest();

        if (mtlRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download MTL: " + mtlRequest.error);
            yield break;
        }

        string mtlData = mtlRequest.downloadHandler.text;

        // Step 3: Parse OBJ and MTL

        var mesh = OBJLoaderHelper.FastFloatParse(objData); // OBJ 파싱 라이브러리 필요
        Dictionary<string, Material> materials = ParseMTL(mtlData);

        // Step 4: Create GameObject and apply materials
        GameObject objObject = new GameObject("Loaded OBJ");
        MeshRenderer renderer = objObject.AddComponent<MeshRenderer>();
        MeshFilter filter = objObject.AddComponent<MeshFilter>();

       // filter.mesh = mesh;

        // Set materials (assuming 1 material for simplicity)
        if (materials.Count > 0)
        {
            renderer.material = materials["default"]; // MTL에서 기본 머티리얼 이름을 지정하세요
        }

        objObject.transform.position = Vector3.zero;

        Debug.Log("OBJ with MTL loaded successfully!");
    }

    Dictionary<string, Material> ParseMTL(string mtlData)
    {
        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        string[] lines = mtlData.Split('\n');
        Material currentMaterial = null;
        string materialName = "";

        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("newmtl"))
            {
                if (currentMaterial != null && !string.IsNullOrEmpty(materialName))
                {
                    materials.Add(materialName, currentMaterial);
                }

                materialName = trimmedLine.Split(' ')[1];
                currentMaterial = new Material(Shader.Find("Standard"));
            }
            else if (trimmedLine.StartsWith("map_Kd"))
            {
                string textureFileName = trimmedLine.Split(' ')[1];
                string textureUrl = textureBaseUrl + textureFileName;
                StartCoroutine(DownloadTexture(textureUrl, currentMaterial));
            }
        }

        if (currentMaterial != null && !string.IsNullOrEmpty(materialName))
        {
            materials.Add(materialName, currentMaterial);
        }

        return materials;
    }

    IEnumerator DownloadTexture(string url, Material material)
    {
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(url);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download texture: " + textureRequest.error);
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(textureRequest);
        material.mainTexture = texture;
    }
}
