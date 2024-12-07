using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class HoonPetInfo : MonoBehaviour
{
    public int mongLevel = 0; //펫레벨
    public TextMeshProUGUI textMongLevel;

    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
    public void MongLevelUpGet()
    {
        StartCoroutine(GetPetStatus());
    }


    private string url2 = "http://125.132.216.190:12223/api/pet";
    [System.Serializable]
    public class PetStatus
    {
        public string name;
        public int level;
        public int experience;
    }


    IEnumerator GetPetStatus()
    {
         
        // GET 요청을 생성합니다.
        UnityWebRequest request = UnityWebRequest.Get(url2);

        string jwtToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {jwtToken}");

        // 요청을 서버로 전송하고 응답을 기다립니다.
        yield return request.SendWebRequest();

        // 서버의 응답을 확인합니다.
        if (request.result == UnityWebRequest.Result.Success)
        {
            //// 성공 응답 처리
            //Debug.Log("펫 상태 조회 성공: " + request.downloadHandler.text);
            PetStatus petStatus = JsonUtility.FromJson<PetStatus>(request.downloadHandler.text);
            Debug.Log("펫 상태 조회 성공: 레벨 = " + petStatus.level + ", 경험치 = " + petStatus.experience);
            //MongName = petStatus.name;
            mongLevel = petStatus.level;
            textMongLevel.text = mongLevel.ToString();
            //Debug.LogError("펫 레벨: " + textMongLevel.text);
            //mongExpTarget = petStatus.experience;
            //SetNickNameMong();
        }
        else
        {
            // 실패 응답 처리
            Debug.LogError("펫 상태 조회 실패: " + request.error);
        }
    }
}//클래스 끝
