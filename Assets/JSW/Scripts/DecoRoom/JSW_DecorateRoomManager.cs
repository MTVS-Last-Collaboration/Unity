using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class JSW_DecorateRoomManager : MonoBehaviour
{
    public bool [,] roomPosition = new bool [8, 8];
    public List<JSW_InfoDecoObject> FunitureList = new List<JSW_InfoDecoObject>();
    public JSW_PlayerDecorate playerDecorate;

    public TMP_Text warningAnimator_Text;
    public Animator warningAnimator;

    private void Start()
    {
        StartCoroutine(PlayerDecorate());
        //nowFuniture();
    }

    public void nowFuniture()
    {

        for(int i=0;i < FunitureList.Count; i++)
        {
            int x = FunitureList[i].decoPositionX;
            int z = FunitureList[i].decoPositionZ;
            int lenx = FunitureList[i].decoLengthX;
            int lenz = FunitureList[i].decoLengthZ;
            int rot = FunitureList[i].decoObjectRotation;
            string name = FunitureList[i].funitureName;
            AddNewFuniture(x, z, lenx, lenz, rot, name);
        }
    }
    // 새로운 배치
    // 이거 네모난 직사각형만 가능함
    public void AddNewFuniture(int posX, int posZ, int lenX,int lenZ, int rot, string name)
    {
        // 12시방향
        if (rot == 0) {
            for (int i = posX;i < posX + lenX;i++)
            {
                for (int j = posZ;j < posZ+lenZ;j++)
                {
                   
                    roomPosition[j,i] = true;
                }
            }
        }
        // 3시방향
        if (rot == 1) {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 6시방향
        if (rot == 2) {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 9시방향
        if (rot ==3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {

                    roomPosition[j, i] = true;
                }
            }
        }
        FunitureList.Add(new JSW_InfoDecoObject(posX, posZ, lenX, lenZ, rot, name));
        printRoomSpace();
    }


    // 이거 네모난 직사각형만 가능함
    public bool IsCanAddNewFuniture(int posX, int posZ, int lenX, int lenZ, int rot)
    {
        // 12시방향
        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        JSW_SoundManager.Get().PlayEftSoundClick2();
                        
                        if (warningAnimator != null)
                        {
                            warningAnimator_Text.text = "공간이 부족합니다";
                            warningAnimator.Play("New Animation", 0, 0f);
                        }

                        return false;
                    }
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        JSW_SoundManager.Get().PlayEftSoundClick2();
                        if (warningAnimator != null)
                        {
                            warningAnimator_Text.text = "공간이 부족합니다";
                            warningAnimator.Play("New Animation", 0, 0f);
                        }
                        return false;
                    }
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        JSW_SoundManager.Get().PlayEftSoundClick2();
                        if (warningAnimator != null)
                        {
                            warningAnimator_Text.text = "공간이 부족합니다";
                            warningAnimator.Play("New Animation", 0, 0f);
                        }
                        return false;
                    }
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {

                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        JSW_SoundManager.Get().PlayEftSoundClick2();
                        if (warningAnimator != null)
                        {
                            warningAnimator_Text.text = "공간이 부족합니다";
                            warningAnimator.Play("New Animation", 0, 0f);
                        }
                        return false;
                    }
                }
            }
        }
        return true;
    }


    // 밀기
    public bool isPushFuniture(int posX, int posZ, int lenX, int lenZ, int rot, int pushDir)
    {
        bool isResult = true;
        // 12시방향
        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {


                    roomPosition[j, i] = false;
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
           
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    roomPosition[j, i] = false;
                }
            }
        }

        if (pushDir == 0)
        {
            posZ += 1;
        }
        else if (pushDir == 1)
        {
            posX += 1;
        }
        else if (pushDir == 2)
        {
            posZ -= 1;
        }
        else if (pushDir == 3)
        {
            posX -= 1;
        }


        // 12시방향
        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult =  false;
                    }
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;

                    }
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }

        if (pushDir == 0)
        {
            posZ -= 1;
        }
        else if (pushDir == 1)
        {
            posX -= 1;
        }
        else if (pushDir == 2)
        {
            posZ += 1;
        }
        else if (pushDir == 3)
        {
            posX += 1;
        }

        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {


                    roomPosition[j, i] = true;
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    roomPosition[j, i] = true;
                }
            }
        }

        return isResult;
    }

    public bool isDrawFuniture(int posX, int posZ, int lenX, int lenZ, int rot, int pushDir)
    {
        bool isResult = true;


        if (pushDir == 0)
        {
            posZ += 1;
        }
        else if (pushDir == 1)
        {
            posX += 1;
        }
        else if (pushDir == 2)
        {
            posZ -= 1;
        }
        else if (pushDir == 3)
        {
            posX -= 1;
        }


        // 12시방향
        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;

                    }
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    if (j >= 8 || j < 0 || i >= 8 || i < 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }
        return isResult;
    }

    public void PushFuniture(int posX, int posZ, int lenX, int lenZ, int rot, int pushDir)
    {
        // 12시방향
        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {


                    roomPosition[j, i] = false;
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    roomPosition[j, i] = false;
                }
            }
        }

        if (pushDir == 0)
        {
            posZ += 1;
        }
        else if (pushDir == 1)
        {
            posX += 1;
        }
        else if (pushDir == 2)
        {
            posZ -= 1;
        }
        else if (pushDir == 3)
        {
            posX -= 1;
        }

        // 12시방향
        if (rot == 0)
        {
            
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    roomPosition[j, i] = true;
                }
            }
        }
        printRoomSpace();
    }

    void printRoomSpace()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (roomPosition[i, j]) print(i + " " + j);
            }
        }
    }

    public void DestroyFuniturePos(int posX, int posZ, int lenX, int lenZ, int rot)
    {
        if (rot == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
        // 3시방향
        if (rot == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {


                    roomPosition[j, i] = false;
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i > posX - lenX; i--)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
        // 9시방향
        if (rot == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    roomPosition[j, i] = false;
                }
            }
        }
    }

    public void PlayerSetFuniture1(string name)
    {
        playerDecorate.SetFuniture1(name);
    }

    public void PlayerDestroyFuni()
    {
        playerDecorate.DestroyFuniture();
    }


    IEnumerator PlayerDecorate()
    {
        yield return new WaitForSeconds(1.0f);

        while (true)
        {
            print("PlayerDecorate Finding");
            // Hierarchy에 있는 모든 활성화된 오브젝트 탐색
            GameObject[] allObjects = FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                // PhotonView 컴포넌트가 있는지 확인
                PhotonView photonView = obj.GetComponent<PhotonView>();

                // PhotonView가 있고, isMine이 true인 경우
                if (photonView != null && photonView.IsMine && obj.name.Contains("JSW_Player"))
                {
                    playerDecorate = obj.GetComponent<JSW_PlayerDecorate>();
                    //print("내 포톤뷰 찾았다.");
                    break;
                }
            }
            if (playerDecorate != null)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}

