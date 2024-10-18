using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;


public class JSW_DecoObject : MonoBehaviour
{

    public int decoObjectPositionX;
    public int decoObjectPositionZ;

    public int decoObjectLengthX;
    public int decoObjectLengthZ;

    public int decoObjectRotation;

    public int funitureNum;

    public bool isMovingFuniture;


    public void SetpositionInfo(int posX, int posZ, int lenX, int lenZ, int rot)
    {
        decoObjectPositionX = posX;
        decoObjectPositionZ = posZ;

        decoObjectLengthX = lenX;
        decoObjectLengthZ = lenZ;

        decoObjectRotation = rot;
    }

    public int[] GetPositionInfo()
    {
        int[] posInfo = new int[] { decoObjectPositionX, decoObjectPositionZ, decoObjectLengthX, decoObjectLengthZ, decoObjectRotation };
        return posInfo;
    }

    public Vector3 PlayerPushPosition(int playerX, int playerZ)
    {
        int posX = decoObjectPositionX;
        int posZ = decoObjectPositionZ;
        int lenX = decoObjectLengthX;
        int lenZ = decoObjectLengthZ;
        Vector3 playerVector3 = new Vector3(playerX, 1, playerZ);
        Vector3 minVector3 = new Vector3(playerX, 1, playerZ);
        
        float minDistance = 100000;

        // 12시방향
        if (decoObjectRotation == 0)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j < posZ + lenZ; j++)
                {
                    if (Vector3.Distance(new Vector3(i, 1, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 1, j), playerVector3);
                        minVector3 = new Vector3(i, 1, j);
                    }
                }
            }
        }

        // 3시방향
        if (decoObjectRotation == 1)
        {
            for (int i = posX; i < posX + lenZ; i++)
            {
                for (int j = posZ; j > posZ - lenX; j--)
                {
                    if (Vector3.Distance(new Vector3(i, 1, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 1, j), playerVector3);
                        minVector3 = new Vector3(i, 1, j);
                    }
                }
            }

        }
        // 6시방향
        if (decoObjectRotation == 2)
        {
            for (int i = posX; i < posX + lenX; i++)
            {
                for (int j = posZ; j > posZ - lenZ; j--)
                {
                    if (Vector3.Distance(new Vector3(i, 1, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 1, j), playerVector3);
                        minVector3 = new Vector3(i, 1, j);
                    }
                }
            }
            
        }
        // 9시방향
        if (decoObjectRotation == 3)
        {
            for (int i = posX; i > posX - lenZ; i--)
            {
                for (int j = posZ; j < posZ + lenX; j++)
                {
                    if (Vector3.Distance(new Vector3(i, 1, j), playerVector3) < minDistance)
                    {
                        minDistance = Vector3.Distance(new Vector3(i, 1, j), playerVector3);
                        minVector3 = new Vector3(i, 1, j);
                    }
                }
            }

        }
        return minVector3;
    }  
}
