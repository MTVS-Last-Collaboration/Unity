using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoonCheckPresetRoom : MonoBehaviour
{
    public int presetIndex;
    public GameObject roomMarker;
    public HoonCreateRoom hoonCreateRoom; //방만들 정보를 선택합니다.
    bool isRoomMaker = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
    public void CheckPresetNumber()
    {
        Debug.Log("이방의 컬렉션 번호" + presetIndex);
        hoonCreateRoom.presetIndex = presetIndex;

    }

    public void CheckRoomMarker(GameObject obj)
    {
        isRoomMaker = !isRoomMaker;

        if (isRoomMaker)
        {
            obj.SetActive(true);
            //print("onMark");
            hoonCreateRoom.checkPresetMarkCount++;
            //print("마크가 되었는지 확인" + hoonCreateRoom.checkCollectionMarkCount);
        }
        else
        {
            obj.SetActive(false);
            //print("offMark");
            hoonCreateRoom.checkPresetMarkCount = hoonCreateRoom.checkPresetMarkCount - 1;
            //print("마크가 되었는지 확인" + hoonCreateRoom.checkCollectionMarkCount);
        }

    }

}//클래스끝

