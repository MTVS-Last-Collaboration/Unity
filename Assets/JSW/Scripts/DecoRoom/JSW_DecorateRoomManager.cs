using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class JSW_DecorateRoomManager : MonoBehaviour
{
    public bool [,] roomPosition = new bool [20, 20];
    public List<GameObject> FunitureList = new List<GameObject>();

    public enum funiture
    {
        TV,
        Table,
        Bed
    }


    // 새로운 배치
    // 이거 네모난 직사각형만 가능함
    public void AddNewFuniture(int posX, int posZ, int lenX,int lenZ, int rot)
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
            for (int i = posX; i< posX + lenX; i++)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
                    {
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
                    {
                        return false;
                    }
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
                    {
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

                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
                    {
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
            for (int i = posX; i < posX + lenX; i++)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
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
            for (int i = posX; i < posX + lenX; i++)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
                    {
                        isResult = false;
                    }
                }
            }
        }
        // 6시방향
        if (rot == 2)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
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
                    if (j >= 20 || j <= 0 || i >= 20 || i <= 0 || roomPosition[j, i] == true)
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
            for (int i = posX; i < posX + lenX; i++)
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
            for (int i = posX; i < posX + lenX; i++)
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
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 20; j++)
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
            for (int i = posX; i < posX + lenX; i++)
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


    // 여기서 필요한 것
    // 플레이어 위치 받기
    // 가구들 배치 받기

    // 플레이어가 필요한것
    // 물건 잡을 때 상호작용키 f
    // 물건 밀거나 당길 때 움직일 수 있는 장소인지를 매니저 배열에서 조사
    // 밀때 같이 이동하기
    // 잡을 때 1,1 정수 위치로 이동하기
    // 밀려고 할 때 다른 플레이어 위치 받아오기
    // 당길 때도 플레이어 위치 받아오기
    // 물건 꺼내기 꺼낼 때 주변 놓을 수 있을지 확인하기
}
