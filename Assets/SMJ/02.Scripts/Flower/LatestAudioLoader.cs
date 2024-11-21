using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.Networking;
using System.Collections;

public class LatestAudioLoader : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip latestAudioClip;
    private Flower flower;

    void Start()
    {
        flower = GetComponent<Flower>();
        audioSource = GetComponent<AudioSource>();
        //LoadLatestAudioClip();
    }

    //제출버튼 눌렀을때 Post방식으로 보내기
    //Get방식으로 가져오기 전에 쓰는 로컬 임시 방편
    void LoadLatestAudioClip()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "VoiceClips", gameObject.name);

        if (!Directory.Exists(directoryPath))
        {
            Debug.LogError($"Directory does not exist: {directoryPath}");
            return;
        }

        var directory = new DirectoryInfo(directoryPath);
        var latestFile = directory.GetFiles("*.wav")
                                  .OrderByDescending(f => f.LastWriteTime)
                                  .FirstOrDefault();

        if (latestFile == null)
        {
            Debug.LogWarning("No WAV files found in the directory.");
            return;
        }

        string relativePath = Path.Combine("VoiceClips", gameObject.name, latestFile.Name);
        StartCoroutine(LoadAudioClip(relativePath));
    }

    IEnumerator LoadAudioClip(string relativePath)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, relativePath);
        string url = $"file://{fullPath}";

        using var www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            latestAudioClip = DownloadHandlerAudioClip.GetContent(www);
            latestAudioClip.name = Path.GetFileNameWithoutExtension(relativePath);
            audioSource.clip = latestAudioClip;
            flower.voiceClip = latestAudioClip;
            Debug.Log($"Latest audio clip loaded: {relativePath}");
        }
        else
        {
            Debug.LogError($"Failed to load audio clip: {www.error}");
        }
    }
}