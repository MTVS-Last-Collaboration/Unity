using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoonCheckShareRoom : MonoBehaviour
{
    public int shareIndex;
    public GameObject roomMarker;
    public Button addShareRoom;
    public HoonCreateRoom hoonCreateRoom; //방만들 정보를 선택합니다.
    
    bool isRoomMaker = false;

    //
    void Start()
    {
        GameObject hoonLoobyCanvas = GameObject.Find("HoonLoobyCanvas");
        hoonCreateRoom = hoonLoobyCanvas.GetComponent<HoonCreateRoom>();
        addShareRoom.onClick.AddListener(hoonCreateRoom.AddSharedRoom);
    }

    /*void Update()
    {
        
    }*/
    public void CheckShareNumber()
    {
        Debug.Log("이방의 컬렉션 번호" + shareIndex);
        hoonCreateRoom.shareIndex = shareIndex;

    }

    public void CheckRoomMarker(GameObject obj)
    {
        isRoomMaker = !isRoomMaker;

        if (isRoomMaker)
        {
            obj.SetActive(true);
            //print("onMark");
            hoonCreateRoom.checkShareMarkCount++;
            //print("마크가 되었는지 확인" + hoonCreateRoom.checkCollectionMarkCount);
        }
        else
        {
            obj.SetActive(false);
            //print("offMark");
            hoonCreateRoom.checkShareMarkCount = hoonCreateRoom.checkShareMarkCount - 1;
            //print("마크가 되었는지 확인" + hoonCreateRoom.checkCollectionMarkCount);
        }

    }


}//클래스
