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
//using UnityEditor.PackageManager.Requests;
using UnityEngine.UI;
using Unity.VisualScripting;
//using UnityEditor.Presets;

public class HoonCreateRoom : MonoBehaviour
{
    //캐싱데이터
    public HoonCreateRoomInfo hoonCreateRoomInfo;
    public HoonUIController hoonUIController;
    public OnMoveTrigger onMoveTrigger;
    public GameObject imgMyStorageMenuObject;
    public GameObject imgGetRoomObject;
    public GameObject imgShowRoomListObject;
    public GameObject choiceRoomErr;
    public GameObject choiceRoomOk;
    public GameObject btn_MyCollection; //생성할 컬렉션버튼프리팹
    public GameObject img_ShareRoom; //생성할 공유룸이미지프리팹
    public Transform shareContent;
    public Transform colloectionContent;
    public Image[] img_Test;
    public GameObject[] presetRoomArray;
    public List<Sprite> roomImage = new List<Sprite>();
    public Dictionary<int, Sprite> collectionSpriteMap = new Dictionary<int, Sprite>();
    public Dictionary<int, Sprite> presetSpriteMap = new Dictionary<int, Sprite>(); // presetID와 Sprite 매칭
    public Dictionary<int, Sprite> shareSpriteMap = new Dictionary<int, Sprite>();

    List<CollectionRoomInfo> collectionRoomList;
    HashSet<int> collectionIDHash = new HashSet<int>(); // 처리된 room ID 추적
    List<ShareRoomInfo> shareRoomInfoList;
    HashSet<int> shareIDHash = new HashSet<int>(); // 처리된 room ID 추적
    bool isCreateCollectionStart = false;
    bool isApplyRoom = false;


    //변화되는정보
    public string myToken;
    public int checkCollectionMarkCount = 0;
    public int checkPresetMarkCount = 0;
    public int checkShareMarkCount = 0;

    public int collectionIndex = 0;
    public int presetIndex = 0;
    public int shareIndex = 0;
    void Start()
    {
        ViewCollectionRoom();
        ViewPresetRoom();
        ViewSharedRoom();
    }

    // Update is called once per frame
    /* void Update()
    {       
    
    }*/

    //보관함을 보여주게 하는 버튼기능
    public void VeiwRoomStorage(GameObject obj)
    {
        print(obj.name);
        if (obj.name == "Btn_MyStorageMenu")
        {
            imgMyStorageMenuObject.SetActive(true);
            imgGetRoomObject.SetActive(false);
            imgShowRoomListObject.SetActive(false);

        }
        else if (obj.name == "Btn_GetRoom")
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

        if (checkCollectionMarkCount != 1)
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
    public class PresetRoomAsset
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
        public string thumbnailUrl;

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
    public class CollectionRoomInfo
    {
        public int id;
        public string source;
        public List<int> savedAt;
        public RoomPreview roomPreview;
        public string thumbnailUrl;

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
    //공유방 파싱시작--------------------------
    [System.Serializable]
    public class ShareRoomPreview
    {
        public string wallpaperName;
        public string floorName;
        public List<string> furnitureNames;
        public int totalFurniture;
    }
    [System.Serializable]
    public class ShareRoomInfo
    {
        public int roomId;
        public string coupleName;
        public ShareRoomPreview roomPreview;
        public DateTime sharedAt; // 타입을 DateTime으로 변경
        public string thumbnailUrl;

    }
    //공유방 파싱끝내기--------------------------

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
    public void AddSharedRoom()
    {
        //string jsonData = "";
        StartCoroutine(PostAddShareRoomNumber(shareIndex));
    }
    //Post 공유보관함->내보관함
    IEnumerator PostAddShareRoomNumber(int shareID)
    {
        string urlRoomNum = "http://125.132.216.190:12223/api/rooms/collection/shared/" + shareID; //공유방 방번호가 들어갑니다.

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
            ViewCollectionRoom();
        }

    }
    //프리셋방 저장하기, 프리셋 리스트에 있는 방을 내보관함으로 가져올때
    public void AddPresetRoom()
    {
        //string jsonData = "";
        //int presetIndex = 10;

        if (checkPresetMarkCount == 1)
        {
            StartCoroutine(PostAddPresetRoomNumber(presetIndex));
        }
        else
        {
            print("방을선택해주세요");
        }

    }
    //Post 기본보관함 -> 내보관함
    IEnumerator PostAddPresetRoomNumber(int presetID)
    {
        string urlPresetRoomNum = "http://125.132.216.190:12223/api/rooms/collection/preset/" + presetID; //프리셋 방번호가 들어갑니다.
        print("프리셋 아이디" + presetIndex);
        UnityWebRequest request = new UnityWebRequest(urlPresetRoomNum, "POST");

        //byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes();
        // 빈 바디 전송
        request.uploadHandler = new UploadHandlerRaw(new byte[0]);
        request.downloadHandler = new DownloadHandlerBuffer();
        //해더설명
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + myToken); //Bearer에 공백 있어야함. 서버로 토큰 발
        yield return request.SendWebRequest();
        print("응답받았다");

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            Debug.LogError("downloadHandler: " + request.downloadHandler.text);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("서버응답" + responseText);

            //프리셋 -> 내보관함, 버튼생성하기
            Debug.LogError("버튼생성해보자.");
            //CreateCollectionRoomButton();
            ViewCollectionRoom(presetID);
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
        print("collectionIndex" + collectionIndex);
        StartCoroutine(PostApplySavaeRoom(collectionIndex));
    }
    //
    IEnumerator PostApplySavaeRoom(int collectionNum)
    {
        string urlApplyRoomNum = "http://125.132.216.190:12223/api/rooms/collection/apply/" + collectionNum; //collectionRoomId 필요.
        //Debug.LogError("collectionNum" + collectionNum);
        //Debug.LogError("urlApplyRoomNum" + urlApplyRoomNum);
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
            choiceRoomOk.SetActive(false);
            choiceRoomErr.SetActive(true);
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
        string urlShare = "http://125.132.216.190:12223/api/rooms/shared";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlShare); //Get url
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

            var settings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            // JSON 배열을 List<RoomInfo>로 변환
            //List<ShareRoomInfo> shareRoomInfoList = JsonConvert.DeserializeObject<List<ShareRoomInfo>>(responseText);
            shareRoomInfoList = JsonConvert.DeserializeObject<List<ShareRoomInfo>>(responseText);
            // roomId 기준으로 정렬
            shareRoomInfoList = shareRoomInfoList.OrderBy(share => share.roomId).ToList();

            for (int i = 0; i < shareRoomInfoList.Count; i++)
            {
                // 결과 출력: GetPresetList[0]
                Debug.Log(JsonConvert.SerializeObject(shareRoomInfoList[i], Formatting.Indented));
                
                
            }
            //CreateShareRoomButton(shareRoomInfoList[i].roomId); //크기만큼 방을 생성하기
            CreateShareRoomButton();
        }

    }
    public void DownloadShareImage(int roomID, string urlPresetImage, GameObject obj)
    {
        //print("ImageUrl" + urlPresetImage);
        StartCoroutine(WaitDownloadSharedImage(roomID, urlPresetImage, obj));
    }

    IEnumerator WaitDownloadSharedImage(int roomID, string urlPresetImage, GameObject obj )
    {

        // UnityWebRequest를 사용하여 이미지 다운로드
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlPresetImage);
        yield return request.SendWebRequest();
        //Debug.Log("Request completed.");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image download successful.");
            // 텍스처로 변환
            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = TextureToSprite(downloadedTexture);

            // presetID와 Sprite를 매칭하여 저장
            if (!shareSpriteMap.ContainsKey(roomID))
            {
                shareSpriteMap.Add(roomID, sprite);

            }
            else
            {
                shareSpriteMap[roomID] = sprite; // 이미 존재하면 덮어쓰기
                //img_Test[index].sprite = presetSpriteMap[roomID]; //이미지에 저장.
                //index++;
                //Debug.LogError("presetID" + roomID + "index" + index);
            }



        }
        else
        {
            Debug.LogError("Image download failed: " + request.error);
        }

    }


    public void CreateShareRoomButton()
    {
        //생성할때 hash에 중복된 아이디가 있다면 건너뜁니다.
        for (int i = 0; i < shareRoomInfoList.Count; i++)
        {
            int shareID = shareRoomInfoList[i].roomId;

            // shareIDHash 에 포함되어 이미 생성된 버튼이라면 스킵
            if (shareIDHash.Contains(shareID))
                continue;

            GameObject shareRoom = Instantiate(img_ShareRoom, shareContent); //shareContent 에생성
            shareRoom.GetComponent<HoonCheckShareRoom>().coupleName = shareRoomInfoList[i].coupleName; //이름값 넣어주기.
            shareRoom.GetComponent<HoonCheckShareRoom>().ChangeCoupleRoomName(); //이름바꾸어주기
            //각방에 스크립트를 넣어주고 각 변수를 확인해주자.
            shareRoom.GetComponent<HoonCheckShareRoom>().shareIndex = shareRoomInfoList[i].roomId;
            DownloadShareImage(shareRoomInfoList[i].roomId, shareRoomInfoList[i].thumbnailUrl, shareRoom);//0번을 가져옵니다.

            // 생성된 버튼 정보 기록
            shareIDHash.Add(shareID);

        }
        print("누적된 버튼생성량" + shareIDHash.Count);

    }

    public void CreateShareRoomButton(int id)
    {
        //생성할때 hash에 중복된 아이디가 있다면 건너뜁니다.
        for (int i = 0; i < shareRoomInfoList.Count; i++)
        {
            int shareID = shareRoomInfoList[i].roomId;

            // shareIDHash 에 포함되어 이미 생성된 버튼이라면 스킵
            if (shareIDHash.Contains(shareID))
                continue;

            GameObject shareRoom = Instantiate(img_ShareRoom, shareContent); //shareContent 에생성
            shareRoom.GetComponent<HoonCheckShareRoom>().coupleName = shareRoomInfoList[i].coupleName; //이름값 넣어주기.
            shareRoom.GetComponent<HoonCheckShareRoom>().ChangeCoupleRoomName(); //이름바꾸어주기
            //각방에 스크립트를 넣어주고 각 변수를 확인해주자.
            shareRoom.GetComponent<HoonCheckShareRoom>().shareIndex = shareRoomInfoList[i].roomId;
            //이미지를 변경해주자.
            shareRoom.GetComponent<Image>().sprite = shareSpriteMap[id];

            // 생성된 버튼 정보 기록
            shareIDHash.Add(shareID);

        }
        print("누적된 버튼생성량" + shareIDHash.Count);

    }
    //프리셋 방 목록조회
    public void ViewPresetRoom()
    {
        //print("방프리셋가져오기");
        StartCoroutine(GetPresetRoom());
    }
    //Get 프리셋
    IEnumerator GetPresetRoom()
    {
        //print("토큰가져오기");
        string urlPreset = "http://125.132.216.190:12223/api/rooms/presets";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlPreset); //Get url
        //UnityWebRequest request = UnityWebRequest.Get(urlTodayMission); //Get url 미션가져오기테스트용
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        //print("요청시작");
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
            //에러403 토큰보내기
        }
        else
        {
            string responseText = request.downloadHandler.text;
            //Debug.Log("서버응답" + responseText);

            // JsonConvert를 사용해 JSON 데이터를 리스트로 파싱
            List<Preset> presets = JsonConvert.DeserializeObject<List<Preset>>(responseText);

            print("프리셋의 크기" + presets.Count);

            // presetId 기준으로 정렬
            presets = presets.OrderBy(p => p.presetId).ToList();

            for (int i = 0; i < presets.Count; i++)
            {
                // 결과 출력: GetPresetList[0]
                //Debug.Log(JsonConvert.SerializeObject(presets[i], Formatting.Indented));
                //print("저장된 presetIndex" + presets[i].presetId);
                presetRoomArray[i].GetComponent<HoonCheckPresetRoom>().presetIndex = presets[i].presetId;
                DownloadPresetImage(presets[i].presetId, presets[i].thumbnailUrl);//0번을 가져옵니다.
            }




        }

    }
    public void DownloadPresetImage(int presetID, string urlPresetImage)
    {
        //print("ImageUrl" + urlPresetImage);
        StartCoroutine(WaitDownloadPresetImage(presetID, urlPresetImage));
    }


    // Texture2D를 Sprite로 변환하는 함수
    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f) // Pivot 설정 (중앙)
        );
    }

   public int imgIndex = 0;
    IEnumerator WaitDownloadPresetImage(int presetID, string urlPresetImage)
    {

        // UnityWebRequest를 사용하여 이미지 다운로드
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlPresetImage);
        yield return request.SendWebRequest();
        //Debug.Log("Request completed.");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image download successful.");
            // 텍스처로 변환
            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = TextureToSprite(downloadedTexture);


            // 중복 키 체크 및 추가
            if (!presetSpriteMap.ContainsKey(presetID))
            {
                presetSpriteMap.Add(presetID, sprite);//중복없으면 추가
            }
            else
            {
                presetSpriteMap[presetID] = sprite;//중복있으면 덮어쓰기
            }
            Debug.LogError("imgIndex" + imgIndex + "presetID" + presetID);
            img_Test[imgIndex].sprite = presetSpriteMap[presetID]; //이미지에 저장.
            imgIndex++;
            if (imgIndex == img_Test.Length)
            {
                imgIndex = 0;
                Debug.LogError("이미지인덱스초기화");
            }


        }
        else
        {
            Debug.LogError("Image download failed: " + request.error);
        }

    }

    //저장된 방 목록조회
    public void ViewCollectionRoom()
    {
        //Debug.LogError("왜갑자기 안돼는지 궁금합니다");
        StartCoroutine(GetCollectionRoom());
    }
    //Get 내목록
    IEnumerator GetCollectionRoom()
    {
        //print("요청시작");
        string urlColletction = "http://125.132.216.190:12223/api/rooms/collection";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlColletction); //Get url
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        //print("요청중");
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
            List<CollectionRoomInfo> roomInfoList = JsonConvert.DeserializeObject<List<CollectionRoomInfo>>(responseText);

            
            // ID 기준으로 중복 제거를 위해 Dictionary 사용
            Dictionary<int, CollectionRoomInfo> roomDictionary = new Dictionary<int, CollectionRoomInfo>();
            foreach (CollectionRoomInfo room in roomInfoList)
            {
                // Dictionary에 이미 해당 id가 있으면 추가하지 않음
                if (!roomDictionary.ContainsKey(room.id))
                {
                    roomDictionary.Add(room.id, room);

                }

            }
            // 중복 제거 후 다시 roomInfoList에 값 넣기(Dictionary의 값들로)
            collectionRoomList = new List<CollectionRoomInfo>(roomDictionary.Values);

            // 중복 제거된 roomInfoList 항목을 문자열로 출력
            foreach (CollectionRoomInfo room in collectionRoomList)
            {
                // JSON 문자열로 직렬화하여 출력
                string roomDataAsString = JsonConvert.SerializeObject(room, Formatting.Indented);
                Debug.Log("Room Info: " + roomDataAsString);
                
            }

            Debug.LogError("방버튼을 만듭시다.");
            CreateCollectionRoomButton();

            //이게 false일때
            if (!isCreateCollectionStart)
            {
                Debug.LogError("방버튼을 만듭시다.");
                CreateCollectionRoomButton();
                isCreateCollectionStart = true;
            }
            else
            {
                print("이미 방을 만들었어용");
            }

        }

    }

    public void ViewCollectionRoom(int id)
    {
        // Debug.LogError("왜갑자기 안돼는지 궁금합니다");
        StartCoroutine(GetCollectionRoom(id));
    }
    IEnumerator GetCollectionRoom(int id)
    {
        //print("요청시작");
        string urlColletction = "http://125.132.216.190:12223/api/rooms/collection";
        //Get 서버요청
        UnityWebRequest request = UnityWebRequest.Get(urlColletction); //Get url
        myToken = LoginInfoManager.instance.myToken;
        request.SetRequestHeader("Authorization", "Bearer " + myToken);

        //print("요청중");
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
            List<CollectionRoomInfo> roomInfoList = JsonConvert.DeserializeObject<List<CollectionRoomInfo>>(responseText);

            // ID 기준으로 중복 제거를 위해 Dictionary 사용
            Dictionary<int, CollectionRoomInfo> roomDictionary = new Dictionary<int, CollectionRoomInfo>();
            foreach (CollectionRoomInfo room in roomInfoList)
            {
                // Dictionary에 이미 해당 id가 있으면 추가하지 않음
                if (!roomDictionary.ContainsKey(room.id))
                {
                    roomDictionary.Add(room.id, room);

                }

            }
            // 중복 제거 후 다시 roomInfoList에 값 넣기(Dictionary의 값들로)
            collectionRoomList = new List<CollectionRoomInfo>(roomDictionary.Values);

            // 중복 제거된 roomInfoList 항목을 문자열로 출력
            foreach (CollectionRoomInfo room in collectionRoomList)
            {
                // JSON 문자열로 직렬화하여 출력
                string roomDataAsString = JsonConvert.SerializeObject(room, Formatting.Indented);
                Debug.Log("Room Info: " + roomDataAsString);
            }

            Debug.LogError("방버튼을 만듭시다.");
            CreateCollectionRoomButton(id);

            //이게 false일때
            if (!isCreateCollectionStart)
            {
                Debug.LogError("방버튼을 만듭시다.");
                CreateCollectionRoomButton(id);
                isCreateCollectionStart = true;
            }
            else
            {
                print("이미 방을 만들었어용");
            }



        }
    }
    public void CreateCollectionRoomButton()
    {
        //배열에 저장된 정보를 카운트 합니다.
        Debug.Log("roomInfoList.Count" + collectionRoomList.Count);

        //생성할때 hash에 중복된 아이디가 있다면 건너뜁니다.
        for (int i = 0; i < collectionRoomList.Count; i++)
        {
            int collectionID = collectionRoomList[i].id;

            // 이미 생성된 버튼이라면 스킵
            if (collectionIDHash.Contains(collectionID))
                continue;

            GameObject collectionRoom = Instantiate(btn_MyCollection, colloectionContent);
            //각방에 스크립트를 넣어주고 각 변수를 확인해주자.
            collectionRoom.GetComponent<HoonCheckCollectRoom>().collectionIndex = collectionRoomList[i].id;
            //썬네일을 받아서 이미지에 추가해줘야함.
            DownloadCollectionImage(collectionRoomList[i].id, collectionRoomList[i].thumbnailUrl, collectionRoom); 
            

            // 생성된 버튼 정보 기록
            collectionIDHash.Add(collectionID);

        }
        print("누적된 버튼생성량" + collectionIDHash.Count);

    }
    //내보관함 정보로 버튼을 생성합니다.
    public void CreateCollectionRoomButton(int id)
    {
        //배열에 저장된 정보를 카운트 합니다.
        Debug.Log("roomInfoList.Count" + collectionRoomList.Count);

        //생성할때 hash에 중복된 아이디가 있다면 건너뜁니다.
        for (int i = 0; i < collectionRoomList.Count; i++)
        {
            int collectionID = collectionRoomList[i].id;

            // 이미 생성된 버튼이라면 스킵
            if (collectionIDHash.Contains(collectionID))
                continue;

            GameObject collectionRoom = Instantiate(btn_MyCollection, colloectionContent);
            //각방에 스크립트를 넣어주고 각 변수를 확인해주자.
            collectionRoom.GetComponent<HoonCheckCollectRoom>().collectionIndex = collectionRoomList[i].id;
            collectionRoom.GetComponent<Image>().sprite = presetSpriteMap[id];
            // 생성된 버튼 정보 기록
            collectionIDHash.Add(collectionID);

        }
        print("누적된 버튼생성량" + collectionIDHash.Count);

    }

    public void DownloadCollectionImage(int roomID, string urlCollectionImage, GameObject obj)
    {
        //print("ImageUrl" + urlPresetImage);
        StartCoroutine(WaitDownloadCollectionImage(roomID, urlCollectionImage, obj));
    }

    IEnumerator WaitDownloadCollectionImage(int roomID, string urlCollectionImage, GameObject obj)
    {

        // UnityWebRequest를 사용하여 이미지 다운로드
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(urlCollectionImage);
        yield return request.SendWebRequest();
        //Debug.Log("Request completed.");
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Image download successful.");
            // 텍스처로 변환
            Texture2D downloadedTexture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = TextureToSprite(downloadedTexture);


            // 중복 키 체크 및 추가
            if (!collectionSpriteMap.ContainsKey(roomID))
            {
                collectionSpriteMap.Add(roomID, sprite);//중복없으면 추가
                obj.GetComponent<Image>().sprite = sprite;
            }
            else
            {
                collectionSpriteMap[roomID] = sprite;//중복있으면 덮어쓰기
                obj.GetComponent<Image>().sprite = sprite;
            }
            

        }
        else
        {
            Debug.LogError("Image download failed: " + request.error);
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
