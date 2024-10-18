using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class VoiceRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private AudioSource audioSource;
    private bool isRecording = false;
    private int recordingFrequency = 44100;
    private string microphoneName;

    private Flower flower;

    void Start()
    {
        flower = GetComponent<Flower>();
        audioSource = GetComponent<AudioSource>();
        microphoneName = Microphone.devices[0]; // 첫 번째 사용 가능한 마이크 사용
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No microphone detected!");
                return;
            }

            // 이전 녹음 클립이 있다면 제거
            if (recordedClip != null)
            {
                Destroy(recordedClip);
            }

            recordedClip = Microphone.Start(microphoneName, false, 10, recordingFrequency);
            if (recordedClip == null)
            {
                Debug.LogError("Failed to start recording!");
                return;
            }

            isRecording = true;
            Debug.Log("Recording started.");
            // UI 업데이트 로직 (예: 녹음 중 표시)

            if (recordingCoroutine != null)
            {
                StopCoroutine(recordingCoroutine);
            }
            recordingCoroutine = StartCoroutine(StopRecordingAfterMaxDuration());
        }
        else
        {
            Debug.Log("Already recording.");
        }
    }
    private IEnumerator StopRecordingAfterMaxDuration()
    {
        yield return new WaitForSeconds(10);
        if (isRecording)
        {
            StopRecording();
        }
    }
    private Coroutine recordingCoroutine;

    public void StopRecording()
    {
        if (recordingCoroutine != null)
        {
            StopCoroutine(recordingCoroutine);
            recordingCoroutine = null;
        }
        if (isRecording)
        {
            // 실제 녹음된 길이 계산
            int lastSample = Microphone.GetPosition(microphoneName);
            print(lastSample);
            Microphone.End(microphoneName);

            if (lastSample <= 0)
            {
                // Microphone.GetPosition()이 0을 반환한 경우, 전체 클립 길이를 사용
                lastSample = recordedClip.samples;
            }

            if (lastSample > 0)
            {
                // 실제 녹음된 길이만큼의 새 AudioClip 생성
                AudioClip newClip = AudioClip.Create("Recorded", lastSample, recordedClip.channels, recordingFrequency, false);
                float[] samples = new float[lastSample];
                recordedClip.GetData(samples, 0);
                newClip.SetData(samples, 0);

                // 새로 만든 클립으로 교체
                Destroy(recordedClip);
                recordedClip = newClip;
                audioSource.clip = recordedClip;
                flower.voiceClip = recordedClip;

                Debug.Log($"Recording stopped. Duration: {recordedClip.length} seconds");

                // 녹음된 오디오를 파일로 저장
                SaveRecording();
            }
            else
            {
                Debug.LogWarning("No audio was recorded or recording was too short.");
            }

            isRecording = false;
        }
        else
        {
            Debug.Log("No active recording to stop.");
        }
    }

    public void SaveRecording()
    {
        if (recordedClip != null)
        {
            string fileName = $"Recording_{System.DateTime.Now:yyyyMMdd_HHmmss}.wav";
            string directoryPath = Path.Combine(Application.streamingAssetsPath, "VoiceClips", gameObject.name);
            string filePath = Path.Combine(directoryPath, fileName);

            SavWav.Save(filePath, recordedClip);
            Debug.Log($"Recording saved to: {filePath}");
        }
    }

    public void PlayRecording()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.Log("No recorded sound to play. AudioSource or clip is null.");
            if (audioSource == null)
                Debug.Log("AudioSource is null");
            else if (audioSource.clip == null)
                Debug.Log("AudioSource.clip is null");
        }
    }
}

public static class SavWav
{
    const int HEADER_SIZE = 44;

    public static bool Save(string filename, AudioClip clip)
    {
        if (!filename.ToLower().EndsWith(".wav"))
        {
            filename += ".wav";
        }

        var filepath = filename;

        Debug.Log(filepath);

        // Make sure directory exists if user is saving to sub dir.
        Directory.CreateDirectory(Path.GetDirectoryName(filepath));

        using (var fileStream = CreateEmpty(filepath))
        {
            ConvertAndWrite(fileStream, clip);
            WriteHeader(fileStream, clip);
        }

        return true;
    }

    static FileStream CreateEmpty(string filepath)
    {
        var fileStream = new FileStream(filepath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i = 0; i < HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptyByte);
        }

        return fileStream;
    }

    static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        var samples = new float[clip.samples];

        clip.GetData(samples, 0);

        Int16[] intData = new Int16[samples.Length];

        Byte[] bytesData = new Byte[samples.Length * 2];

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * 32767);
            Byte[] byteArr = new Byte[2];
            byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        fileStream.Write(bytesData, 0, bytesData.Length);
    }

    static void WriteHeader(FileStream fileStream, AudioClip clip)
    {
        var hz = clip.frequency;
        var channels = clip.channels;
        var samples = clip.samples;

        fileStream.Seek(0, SeekOrigin.Begin);

        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);

        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);

        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);

        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);

        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);

        UInt16 two = 2;
        UInt16 one = 1;

        Byte[] audioFormat = BitConverter.GetBytes(one);
        fileStream.Write(audioFormat, 0, 2);

        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);

        Byte[] sampleRate = BitConverter.GetBytes(hz);
        fileStream.Write(sampleRate, 0, 4);

        Byte[] byteRate = BitConverter.GetBytes(hz * channels * 2); // sampleRate * numChannels * bytesPerSample
        fileStream.Write(byteRate, 0, 4);

        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);

        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);

        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
    }
}