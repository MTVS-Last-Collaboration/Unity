using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildWindowManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //빌드에서 윈도우 크기를 제한하자.
        Screen.SetResolution(1600, 900, FullScreenMode.Windowed);
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
}
