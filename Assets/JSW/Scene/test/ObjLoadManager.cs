using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ObjLoadManager : MonoBehaviour
{
    public string objFileUrl = "https://example.com/your-model.obj"; // OBJ 파일의 URL
    public string textureUrl = "https://example.com/your-texture.png"; // 텍스처 파일의 URL

    void Start()
    {
        StartCoroutine(DownloadAndLoadModelWithTexture());
    }

    IEnumerator DownloadAndLoadModelWithTexture()
    {
        // 1. OBJ 파일 다운로드
        UnityWebRequest objRequest = UnityWebRequest.Get(objFileUrl);
        yield return objRequest.SendWebRequest();

        if (objRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("OBJ 파일 다운로드 실패: " + objRequest.error);
            yield break;
        }

        string objData = objRequest.downloadHandler.text;
        Mesh mesh = LoadOBJFromString(objData);

        // 2. 텍스처 다운로드
        UnityWebRequest textureRequest = UnityWebRequestTexture.GetTexture(textureUrl);
        yield return textureRequest.SendWebRequest();

        if (textureRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("텍스처 다운로드 실패: " + textureRequest.error);
            yield break;
        }

        Texture2D texture = ((DownloadHandlerTexture)textureRequest.downloadHandler).texture;

        // 3. GameObject 생성 및 텍스처 적용
        GameObject obj = new GameObject("Loaded OBJ Model");
        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        Material material = new Material(Shader.Find("Standard")); // 표준 셰이더 사용
        material.mainTexture = texture; // 텍스처 설정
        renderer.material = material;

        Debug.Log("OBJ 모델 및 텍스처 로드 완료");
    }

    Mesh LoadOBJFromString(string objData)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        StringReader reader = new StringReader(objData);
        string line;

        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("v "))
            {
                string[] parts = line.Split(' ');
                vertices.Add(new Vector3(
                    float.Parse(parts[1]),
                    float.Parse(parts[2]),
                    float.Parse(parts[3])
                ));
            }
            else if (line.StartsWith("vt "))
            {
                string[] parts = line.Split(' ');
                uvs.Add(new Vector2(
                    float.Parse(parts[1]),
                    float.Parse(parts[2])
                ));
            }
            else if (line.StartsWith("vn "))
            {
                string[] parts = line.Split(' ');
                normals.Add(new Vector3(
                    float.Parse(parts[1]),
                    float.Parse(parts[2]),
                    float.Parse(parts[3])
                ));
            }
            else if (line.StartsWith("f "))
            {
                string[] parts = line.Substring(2).Split(' ');
                foreach (string part in parts)
                {
                    int vertexIndex = int.Parse(part.Split('/')[0]) - 1; // 인덱스는 1부터 시작하므로 1을 뺌
                    triangles.Add(vertexIndex);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        if (normals.Count > 0) mesh.normals = normals.ToArray();
        else mesh.RecalculateNormals();

        if (uvs.Count > 0) mesh.uv = uvs.ToArray();

        return mesh;
    }
}
