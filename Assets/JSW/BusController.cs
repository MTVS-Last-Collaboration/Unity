using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class BusController : MonoBehaviour
{
    public GameObject Bus;
    public GameObject Pos1;
    public GameObject Pos2;
    public GameObject Pos3;

    public GameObject openingObject;
    public OnMoveTrigger onMoveTrigger;

    public Camera cameraBus;
    public GameObject byeUI;
    public AudioSource busAudio;

    public Scrollbar volumeScrollbar1;
    public Scrollbar volumeScrollbar2;

    public Text text1;
    public Text text2;

    public AudioSource audio1;
    public AudioSource audio2;

    private void Start()
    {
        StartCoroutine(Opening());
        volumeScrollbar1.onValueChanged.AddListener(OnVolumeChange1);
        volumeScrollbar2.onValueChanged.AddListener(OnVolumeChange2);
    }
    void OnVolumeChange1(float value)
    {
        // Scrollbar 값(0~1)을 dB 값으로 변환하여 AudioMixser에 전달
        float volume = value;
        audio1.volume = volume;
        text1.text = volume.ToString();

    }
    void OnVolumeChange2(float value)
    {
        // Scrollbar 값(0~1)을 dB 값으로 변환하여 AudioMixser에 전달
        float volume = value;
        audio2.volume = volume;
        text2.text = volume.ToString();
    }

    public void ClickByeBus()
    {
        StartCoroutine(GoBus());
    }

    public IEnumerator GoBus()
    {
        byeUI.SetActive(false);
        Bus.transform.position = Pos1.transform.position;
        Bus.transform.Rotate(0, 0, 180);
        cameraBus.targetDisplay = 0;
        busAudio.Play();

        while (true)
        {
            Bus.transform.position = Vector3.Lerp(Bus.transform.position, Pos2.transform.position, Time.deltaTime*3);
            if (Vector3.Distance(Bus.transform.position, Pos2.transform.position) <= 0.2f) break;
            yield return null;
        }
        cameraBus.cullingMask &= ~(1 << LayerMask.NameToLayer("Player_CheckFlower"));

        yield return new WaitForSeconds(0.2f);

        Vector3 size = Bus.transform.localScale;
        iTween.ScaleTo(Bus, iTween.Hash(
        "scale", size*1.1f, // 최종 크기
        "time", 0.5f,         // 애니메이션 시간
        "easetype", iTween.EaseType.easeOutElastic // 애니메이션 타입
        ));

        yield return new WaitForSeconds(1.5f);
        onMoveTrigger.GoOtherRoom();

        float elapsedTime = 0f;
        while (true)
        {
            elapsedTime += Time.deltaTime;

            // 0~1 사이의 비율 계산
            float t = elapsedTime / 2;

            // 비선형적으로 속도를 조정 (느리게 시작하여 점점 빨라짐)
            float easedT = Mathf.Pow(t, 2); // t^2으로 Ease-In 효과

            // Lerp로 위치 보간
            Bus.transform.position = Vector3.Lerp(Bus.transform.position, Pos3.transform.position, easedT);
            if (Vector3.Distance(Bus.transform.position, Pos3.transform.position) <= 0.1f) break;
            yield return null;
        }
    }

    public IEnumerator Opening()
    {
        openingObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        openingObject.transform.GetChild(4).gameObject.SetActive(false);

        iTween.ScaleTo(openingObject, iTween.Hash(
            "scale", Vector3.one * 100,        // 목표 스케일 (1, 1, 1)
            "time", 1f,                // 애니메이션 시간 (조정 가능)
            "easeType", "easeInCirc", // 통통 튀는 느낌의 easeType
            "oncomplete", "OnCompleteOpening", // 애니메이션 완료 시 호출할 함수
            "oncompletetarget", gameObject
        ));
    }

    public void OnCompleteOpening()
    {
        openingObject.SetActive(false);
    }

}
