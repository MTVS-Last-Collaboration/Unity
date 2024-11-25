using Newtonsoft.Json;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq; // for LINQ operations
using System.Net.NetworkInformation;
using static HoonCreateRoom;
using static JSW_InitRoom;
using static LoginTest;
using static System.Net.WebRequestMethods;

public class HoonCreateRoom : MonoBehaviour
{
    //캐싱데이터
    public GameObject imgMyStorageMenuObject;
    public GameObject imgGetRoomObject;
    public GameObject imgShowRoomListObject;
    public HoonUIController hoonUIController;
    public GameObject choiceRoomErr;
    public GameObject choiceRoomOk;
    public OnMoveTrigger onMoveTrigger;
    public Transform colloectionContent;
    public GameObject btn_MyCollection; //생성할 버튼 프리팹
    public GameObject[] presetRoomArray;

    List<RoomData> collectionRoomList;
    bool isCreateCollectionStart = false;
    bool isApplyRoom = false;


    //변화되는정보
    public string myToken;
    public int checkCollectionMarkCount = 0;
    public int checkPresetMarkCount = 0;
   
    public int collectionIndex = 0;
    public int presetIndex = 0;
    void Start()
    {
        ViewCollectionRoom();
       
    }

    // Update is called once per frame
    /* void Update()
    {       
    
    }*/
    
    //보관함을 보여주게 하는 버튼기능
    public void VeiwRoomStorage(GameObject obj)
    {
        print(obj.name);
        if(obj.name == "Btn_MyStorageMenu")
        {
            imgMyStorageMenuObject.SetActive(true);
            imgGetRoomObject.SetActive(false);
            imgShowRoomListObject.SetActive(false);

        }
        else if(obj.name == "Btn_GetRoom")
        {
            imgMyStorageMenuObject.SetActive(false);
            imgGetRoomObject.SetActive(true);
            imgShowRoomListObject.SetActive(false);
        }
        else if (obj.name == "Btn_ShowRoomList")
        {
            imgMyStorageMenuObject.SetActive(false);
            imgGetRoomObject.SetActive(false);
            imgShowRoomListObject.SetActive(true);
        }

    }
    //선택했는지 유무를 시각화하고 판단하는 버튼기능
    public void ViewRoomMark(GameObject obj)
    {
        obj.SetActive(false);
    }
    //마크가 되고 방을 만들면 실제 방을 생성하는 기능
    public void MoveRoom()
    {
        //hoonUIController.isMyMarkObject , hoonUIController.isGetMarkObject, hoonUIController.isShareMarkObejct

        if(checkCollectionMarkCount != 1)
        {
            print("방을선택해라");
            choiceRoomErr.SetActive(true);
            return;
        }

        //UI컨트롤 스크립트에서 마크가 되었는지 판단하고 판단한 값에 따라 방을 생성하기
        if (checkCollectionMarkCount == 1)
        {
            print("내방마크됨" + hoonUIController.isMyMarkObject);
            choiceRoomOk.SetActive(true);
            //방정보를 갱신하자.
            ApplySavaeRoom();

            

        }
        else
        {
            print("내방마크되지않음" + hoonUIController.isMyMarkObject);
            return;
        }

    }



    //Http 통신 코드 테스트 -------------------------
    //Http Post
    //프리셋룸에 정보를 가져옵니다.
    public void FindRoomPreset()
    {    
        //CloseLoginUI();
        StartCoroutine(GetRoomPreset());
        print("로그인UI닫기");

        
    }

    [Serializable]
    public class PresetRoomInfo
    {
        public string id; // 서버에서 받은 값에 따라 자료형 수정
        public string name;
        public string preview; // `PreviewInfo` 클래스와 매핑
        public string createdAt;
        public override string ToString()
        {
            return $"Room ID: {id}, Name: {name}, Created At: {createdAt}, Preview: {preview}";
        }

    }

    [Serializable]
    public class PreviewInfo
    {
        public string wallpaperName;
        public string floorName;
        public List<string> furnitureNames;
        public int totalFurniture;
        public override string ToString()
        {
            return $"Wallpaper: {wallpaperName}, Floor: {floorName}, " +
                   $"Furniture: [{string.Join(", ", furnitureNames)}], Total: {totalFurniture}";
        }
    }
    //서버에 프리셋룸 정보 요청
    IEnumerator GetRoomPreset()
    {
        string url = "http://125.132.216.190:12223/api/rooms/presets";

        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(url); //Get url
        //UnityWebRequest request = UnityWebRequest.Get(urlTodayMission); //Get url 미션가져오기테스트용
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            // JSON 배열을 List<RoomInfo>로 변환
            List<PresetRoomInfo> roomInfoList = JsonConvert.DeserializeObject<List<PresetRoomInfo>>(responseText);
            // roomInfoList를 문자열로 출력
            foreach (var room in roomInfoList)
            {
                Debug.Log(room.ToString()); // ToString 메서드 호출
            }
            

        }
    }

    //API 연결용 코드-------------------------------------------
    //프리셋 파싱하기-------------------------------------------
    [System.Serializable]
    public class Preset
    {
        public int presetId;
        public string name;
        public Wallpaper wallpaper;
        public Floor floor;
        public List<FurnitureLayout> furnitureLayouts;
    }
    [System.Serializable]
    public class Wallpaper
    {
        public int id;
        public string name;
        public int wallpaperNumber;
    }
    [System.Serializable]
    public class Floor
    {
        public int id;
        public string name;
        public int floorNumber;
    }
    [System.Serializable]
    public class FurnitureLayout
    {
        public int furnitureId;
        public string name;
        public int positionX;
        public int positionY;
        public int rotation;
    }
    //프리셋파싱 끝내기------------------
    //내보관함 파싱하기---------------
    public class RoomPreview
    {
        public string wallpaperName;
        public string floorName;
        public List<string> furnitureNames;
        public int totalFurniture;
    }
    public class RoomData
    {
        public int id;
        public string source;
        public List<int> savedAt;
        public RoomPreview roomPreview;
        
        // 배열을 DateTime으로 변환하는 헬퍼 메서드
        public DateTime GetSavedAtAsDateTime()
        {
            if (savedAt != null && savedAt.Count >= 7)
            {
                return new DateTime(savedAt[0], savedAt[1], savedAt[2], savedAt[3], savedAt[4], savedAt[5], DateTimeKind.Utc)
                    .AddTicks(savedAt[6]); // 마지막 값을 Ticks로 처리
            }
            throw new InvalidOperationException("savedAt 배열의 형식이 올바르지 않습니다.");
        }

    }
    //내보관함파싱끝내기


    //Http Post---------------------------
    //방공유설정 시작하기 1-1
    public void OnListMyRoom()
    {
        //string jsonData = "";
       //StartCoroutine(PostShareRoomStart(jsonData));
        StartCoroutine(PostShareRoomStart());
    }
    //
    IEnumerator PostShareRoomStart()
    {
        myToken = LoginInfoManager.instance.myToken;
        string urlTrue = "http://125.132.216.190:12223/api/rooms/sharing?isShared=true";
       
        UnityWebRequest request = new UnityWebRequest(urlTrue, "POST");  // HTTP POST 요청 준비
                                                                    
        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData); // JSON 데이터를 담아 요청 생성
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발사
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

        }

    }
    //방공유설정 그만하기 1-2
    public void OffListMyRoom()
    {
        string jsonData = "";
        StartCoroutine(PostShareRoomEnd(jsonData));
    }

    IEnumerator PostShareRoomEnd(string jsonData)
    {
        myToken = LoginInfoManager.instance.myToken;

        string urlFalse = "http://125.132.216.190:12223/api/rooms/sharing?isShared=false";

        UnityWebRequest request = new UnityWebRequest(urlFalse, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

        }

    }

    //공유방 저장하기, 공유된 방 리스트 골랐을때 내보관함으로 가져오는것
    public void SvaeSharedRoom()
    {
        //string jsonData = "";
        StartCoroutine(PostSaveShareRoomNumber(presetIndex));
    }
    //Post 공유보관함->내보관함
    IEnumerator PostSaveShareRoomNumber(int presetID)
    {
        string urlRoomNum = "http://125.132.216.190:12223/api/rooms/collection/shared/" + presetID; //공유방 방번호가 들어갑니다.

        UnityWebRequest request = new UnityWebRequest(urlRoomNum, "POST");

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        //request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

        }
    
    }
    //프리셋방 저장하기, 프리셋 리스트에 있는 방을 내보관함으로 가져올때
    public void AddPresetRoom()
    {
        //string jsonData = "";
        //int presetIndex = 10;
        StartCoroutine(PostAddPresetRoomNumber(presetIndex));
    }
    //Post 기본보관함 -> 내보관함
    IEnumerator PostAddPresetRoomNumber(int presetID)
    {
        string urlPresetRoomNum = "http://125.132.216.190:12223/api/rooms/collection/preset/" + presetID; //프리셋 방번호가 들어갑니다.
        print("프리셋 주소" + urlPresetRoomNum);
        UnityWebRequest request = new UnityWebRequest(urlPresetRoomNum, "POST");

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes();
        // 빈 바디 전송
        request.uploadHandler = new UploadHandlerRaw(new byte[0]);
        request.downloadHandler = new DownloadHandlerBuffer();
        //해더설명
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            //응답받은 데이터를 json으로 파싱하고 리스트로 저장한다음 필요한 정보를 저장해주고 리스트의 
           
        }
    }
    //현재방 상태 저장
    public void SaveCurrentRoom()
    {
        string jsonData = "";
        StartCoroutine(PostSaveCurrentRoomNumber(jsonData));
    }    
    //Post 현재방
    IEnumerator PostSaveCurrentRoomNumber(string jsonData)
    {
        string urlCurrentRoomNum = "http://125.132.216.190:12223/api/rooms/collection/current"; //현재상태저장 .

        UnityWebRequest request = new UnityWebRequest(urlCurrentRoomNum, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

        }
    }    
    //저장된 방 상태 적용
    public void ApplySavaeRoom()
    {
        //need collectionRoomId 
        //string jsonData = "";
        //int collectionIndex = 2;
        StartCoroutine(PostApplySavaeRoom(collectionIndex));
    }
    //
    IEnumerator PostApplySavaeRoom(int collectionNum)
    {
        string urlApplyRoomNum = "http://125.132.216.190:12223/api/rooms/collection/apply/" + collectionNum; //collectionRoomId 필요.

        UnityWebRequest request = new UnityWebRequest(urlApplyRoomNum, "POST");

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(new byte[0]);
        request.uploadHandler = new UploadHandlerRaw(new byte[0]); //빈바디를 보냅니다.
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            isApplyRoom = true;
        }

        if (isApplyRoom)
        {
            // 내방으로 가는 코드를 넣자.
            onMoveTrigger.GoOtherRoom();
            print("드가자~");

        }
        else
        {
            print("ㅎㅎ 못가");
        }


    }
    //Http Get
    //공유된 방 목록 조회 
    public void ViewSharedRoom()
    {
        StartCoroutine(GetSharedRoom());
    }
    //Get 공유방
    IEnumerator GetSharedRoom()
    {
        string url = "http://125.132.216.190:12223/api/rooms/shared";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(url); //Get url
        //UnityWebRequest request = UnityWebRequest.Get(urlTodayMission); //Get url 미션가져오기테스트용
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            // JSON 배열을 List<RoomInfo>로 변환
            List<PresetRoomInfo> roomInfoList = JsonConvert.DeserializeObject<List<PresetRoomInfo>>(responseText);
            
        }

    }
    //프리셋 방 목록조회
    public void ViewPresetRoom()
    {
        print("방프리셋가져오기");
        StartCoroutine(GetPresetRoom());
    }
    //Get 프리셋
    IEnumerator GetPresetRoom()
    {
        print("토큰가져오기");
        string urlPreset = "http://125.132.216.190:12223/api/rooms/presets";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlPreset); //Get url
        //UnityWebRequest request = UnityWebRequest.Get(urlTodayMission); //Get url 미션가져오기테스트용
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        print("요청시작");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            // JsonConvert를 사용해 JSON 데이터를 리스트로 파싱
            List<Preset> presets = JsonConvert.DeserializeObject<List<Preset>>(responseText);

            print("프리셋의 크기" + presets.Count);

            // presetId 기준으로 정렬
            presets = presets.OrderBy(p => p.presetId).ToList();

            for (int i = 0; i < presets.Count; i++)
            {
                // 결과 출력: GetPresetList[0]
                Debug.Log(JsonConvert.SerializeObject(presets[i], Formatting.Indented));
                presetRoomArray[i].GetComponent<HoonCheckPresetRoom>().presetIndex = presets[i].presetId;
                print("저장된 presetIndex" + presets[i].presetId);

            }



        }

    }
    //저장된 방 목록조회
    public void ViewCollectionRoom()
    {
        StartCoroutine(GetCollectionRoom());
    }
    //Get 내목록
    IEnumerator GetCollectionRoom()
    {
        print("요청시작");
        string urlColletction = "http://125.132.216.190:12223/api/rooms/collection";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlColletction); //Get url
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);
        
        print("요청중");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
            if (request.responseCode == 403)
            {
                Debug.LogError("토큰없음.로그인을 해주세연");
            }

            if (request.responseCode == 500)
            {
                Debug.LogError("Internal Server Error: 서버에서 요청을 처리하지 못했습니다.");
            }

        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            // JSON 배열을 List<RoomInfo>로 변환
            List<RoomData> roomInfoList = JsonConvert.DeserializeObject<List<RoomData>>(responseText);

            // ID 기준으로 중복 제거를 위해 Dictionary 사용
            Dictionary<int, RoomData> roomDictionary = new Dictionary<int, RoomData>();
            foreach (RoomData room in roomInfoList)
            {
                // Dictionary에 이미 해당 id가 있으면 추가하지 않음
                if (!roomDictionary.ContainsKey(room.id))
                {
                    roomDictionary.Add(room.id, room);
                    
                }

            }
            // 중복 제거 후 다시 roomInfoList에 값 넣기(Dictionary의 값들로)
            collectionRoomList = new List<RoomData>(roomDictionary.Values);

            // 중복 제거된 roomInfoList 항목을 문자열로 출력
            foreach (RoomData room in collectionRoomList)
            {
                // JSON 문자열로 직렬화하여 출력
                string roomDataAsString = JsonConvert.SerializeObject(room, Formatting.Indented);
                Debug.Log("Room Info: " + roomDataAsString);
            }
           
            //이게 false일때
            if(!isCreateCollectionStart)
            {
                CreateCollectionRoomButton();
                isCreateCollectionStart = true;
            }
            else
            {
                print("이미 방을 만들었어용");
            }
            


        }
    }
    //내보관함 정보로 버튼을 생성합니다.
    public void CreateCollectionRoomButton()
    {
        //배열에 저장된 정보를 카운트 합니다.
        Debug.Log("roomInfoList.Count" + collectionRoomList.Count);

        //content의 위치를 캐싱해서 가져옵니다.
        for (int i = 0; i < collectionRoomList.Count; i++)
        {
            GameObject collectionRoom = Instantiate(btn_MyCollection, colloectionContent);
            //각방에 스크립트를 넣어주고 각 변수를 확인해주자.
            collectionRoom.GetComponent<HoonCheckCollectRoom>(). collectionIndex =collectionRoomList[i].id ;

        }
    }




    //방전체목록, 공유된 목록전체를 가져옵니다.
    public void ViewStatus()
    {
        StartCoroutine(GetCollectionRoom());
    }
    //Get 방전체목록
    IEnumerator GetStatus()
    {
        string url = "http://125.132.216.190:12223/api/rooms/status";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(url); //Get url
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            // JSON 배열을 List<RoomInfo>로 변환
            List<PresetRoomInfo> roomInfoList = JsonConvert.DeserializeObject<List<PresetRoomInfo>>(responseText);

        }
    }

    //------------------------
   



}//클래스끝
