using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class HoonCheckCollectRoom : MonoBehaviour
{
    public int collectionIndex;
    
    public GameObject roomMarker;
    public HoonCreateRoom hoonCreateRoom; //방만들 정보를 선택합니다.

    bool isRoomMaker = false;
    Transform conten; //부모오브젝트
    
    
    void Start()
    {
        GameObject hoonLobbyCanvas = GameObject.Find("HoonLoobyCanvas");
        if (hoonLobbyCanvas != null)
        {
            hoonCreateRoom = hoonLobbyCanvas.GetComponent<HoonCreateRoom>();
        }
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    public void CheckCollectionNumber()
    {
        Debug.Log("이방의 컬렉션 번호" + collectionIndex);
        hoonCreateRoom.collectionIndex = collectionIndex;

    }

    //마지막에 선택된 방의 컬렉션 값을 보냅니다.
    public void CheckRoomMarker(GameObject obj)
    {
        isRoomMaker = !isRoomMaker;

        if(isRoomMaker)
        {
            obj.SetActive(true);
            //print("onMark");
            hoonCreateRoom.checkCollectionMarkCount++;
            print("마크가 되었는지 확인" +hoonCreateRoom.checkCollectionMarkCount);
        }
        else
        {
            obj.SetActive(false);
            //print("offMark");
            hoonCreateRoom.checkCollectionMarkCount = hoonCreateRoom.checkCollectionMarkCount - 1;
            print("마크가 되었는지 확인" + hoonCreateRoom.checkCollectionMarkCount);
        }
       
    }
    
    

}//클래스 끝
