using Dummiesman;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ObjFromStream : MonoBehaviour {
    public RawImage r;

    void Start()
    {
        StartCoroutine(LoadOBJWithTexture());
    }

    IEnumerator LoadOBJWithTexture()
    {
        // Step 1: Download OBJ file
        print("Ddfa");
        string objUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/180c33d4-725f-4d32-8a24-d2e4f317871d.obj";
        UnityWebRequest objRequest = UnityWebRequest.Get(objUrl);
        yield return objRequest.SendWebRequest();

        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download OBJ file: " + objRequest.error);
            yield break;
        }

        // Create a stream from OBJ file content
        var textStream = new MemoryStream(Encoding.UTF8.GetBytes(objRequest.downloadHandler.text));

        // Step 2: Load the OBJ file
        GameObject loadedObj = new OBJLoader().Load(textStream);

        // Step 3: Download Texture
        string textureUrl = "https://loveforest.s3.ap-northeast-2.amazonaws.com/c80e90f4-1698-46e8-b7d8-2f5033de823b.png";
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(textureUrl);
        print("ddd1");
        yield return textureRequest.SendWebRequest();
        print("ddd2");
        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            print("ddd3");
            Debug.LogError("Failed to download texture: " + textureRequest.error);
            yield break;
        }
        print("ddd4");

        Texture2D texture = DownloadHandlerTexture.GetContent(textureRequest);
        r.texture = texture;
        // Step 4: Create a Material and assign the texture
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.mainTexture = texture;

        // Step 5: Apply the Material to the loaded object
        var renderer = loadedObj.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = material;
        }

        // Optional: Adjust object's position or scale if needed
        loadedObj.transform.position = Vector3.zero;
    }
}
