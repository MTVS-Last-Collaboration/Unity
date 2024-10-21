using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ViewImageManager : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        image.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print("플레이어가 근처에 있음");
        if (other.gameObject.name.Contains("Player")) //게임오브젝트가 플레이어를 포함하고 있다면
        {
            print("이미지 보여주기");
            image.gameObject.SetActive(enabled);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        print("이미지 끄기");
        image.gameObject.SetActive(false);
    }


}//클래스 끝
