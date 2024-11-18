using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Net;
using Photon.Pun;

public class VoiceRecorder : MonoBehaviourPun
{
    private string directoryPath;

    private AudioClip recordedClip;
    private AudioSource audioSource;
    private bool isRecording = false;
    private int recordingFrequency = 44100;
    private string microphoneName;
    private Flower flower;

    private void Awake()
    {
        InitializeDirectory();
    }

    void Start()
    {
        flower = GetComponent<Flower>();
        audioSource = GetComponent<AudioSource>();
        microphoneName = Microphone.devices[0]; // 첫 번째 사용 가능한 마이크 사용
    }

    public bool HasRecording()
    {
        return recordedClip != null;
    }

    public byte[] GetRecordedData()
    {
        if (recordedClip == null) return null;

        // AudioClip 데이터를 WAV 형식의 byte[]로 변환
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                var samples = new float[recordedClip.samples * recordedClip.channels];
                recordedClip.GetData(samples, 0);

                // WAV 헤더 작성
                writer.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"));
                writer.Write(36 + samples.Length * 2);
                writer.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"));
                writer.Write(System.Text.Encoding.UTF8.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)recordedClip.channels);
                writer.Write(recordedClip.frequency);
                writer.Write(recordedClip.frequency * 2);
                writer.Write((short)2);
                writer.Write((short)16);
                writer.Write(System.Text.Encoding.UTF8.GetBytes("data"));
                writer.Write(samples.Length * 2);

                // 샘플 데이터 작성
                foreach (float sample in samples)
                {
                    writer.Write((short)(sample * 32767));
                }
            }
            return stream.ToArray();
        }
    }

    public void SetRecordedData(byte[] data)
    {
        if (data == null || data.Length == 0) return;

        try
        {
            // WAV 헤더 건너뛰기 (44 bytes)
            int headerSize = 44;
            int samplesCount = (data.Length - headerSize) / 2; // 16-bit samples
            float[] samples = new float[samplesCount];

            // WAV 데이터를 float 배열로 변환
            for (int i = 0; i < samplesCount; i++)
            {
                short sample = BitConverter.ToInt16(data, headerSize + i * 2);
                samples[i] = sample / 32768f;
            }

            // 새로운 AudioClip 생성
            recordedClip = AudioClip.Create("ReceivedAudio", samplesCount, 1, recordingFrequency, false);
            recordedClip.SetData(samples, 0);

            // AudioSource와 Flower에 설정
            audioSource.clip = recordedClip;
            flower.voiceClip = recordedClip;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error setting recorded data: {e.Message}");
        }
    }

    public AudioClip GetAudioClip()
    {
        return recordedClip;
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

    private void InitializeDirectory()
    {
        // VoiceClips 디렉토리 경로 설정
        directoryPath = Path.Combine(Application.persistentDataPath, "VoiceClips", gameObject.name);

        try
        {
            // 디렉토리가 존재하지 않으면 생성
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                Debug.Log($"Created directory: {directoryPath}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error creating directory: {e.Message}");
        }
    }

    public void SaveRecording()
    {
        if (recordedClip != null)
        {
            try
            {
                // 파일 이름 생성
                string fileName = $"Recording_{System.DateTime.Now:yyyyMMdd_HHmmss}.wav";
                string filePath = Path.Combine(directoryPath, fileName);

                // 디렉토리 재확인 및 생성
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                SavWav.Save(filePath, recordedClip);
                Debug.Log($"Recording saved to: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving recording: {e.Message}");
            }
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

    public string GetBase64Recording()
    {
        if (recordedClip == null) return null;

        try
        {
            // AudioClip의 raw 데이터를 가져옵니다
            float[] samples = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData(samples, 0);

            // float 배열을 byte 배열로 변환
            byte[] bytesData = new byte[samples.Length * 4]; // float는 4바이트
            Buffer.BlockCopy(samples, 0, bytesData, 0, bytesData.Length);

            // Base64로 변환
            return Convert.ToBase64String(bytesData);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error converting to Base64: {e.Message}");
            return null;
        }
    }

    public bool SetBase64Recording(string base64String)
    {
        if (string.IsNullOrEmpty(base64String))
        {
            Debug.LogError("Base64 string is null or empty");
            return false;
        }

        try
        {
            // Base64를 byte 배열로 변환
            byte[] bytesData = Convert.FromBase64String(base64String);

            // byte 배열을 float 배열로 변환
            float[] samples = new float[bytesData.Length / 4];
            Buffer.BlockCopy(bytesData, 0, samples, 0, bytesData.Length);

            // 새로운 AudioClip 생성
            recordedClip = AudioClip.Create("ReceivedAudio", samples.Length / recordedClip.channels,
                recordedClip.channels, recordingFrequency, false);
            recordedClip.SetData(samples, 0);

            // AudioSource와 Flower에 설정
            audioSource.clip = recordedClip;
            flower.voiceClip = recordedClip;

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error converting from Base64: {e.Message}");
            return false;
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