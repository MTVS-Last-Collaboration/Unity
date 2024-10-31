using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JSW_InfoDecoObject { 
    public int decoPositionX { get; set; }
    public int decoPositionZ { get; set; }

    public int decoLengthX { get; set; }
    public int decoLengthZ { get; set; }

    public int decoObjectRotation { get; set; }

    public string funitureName { get; set; }

    // 생성자: 초기 좌표값 설정
    public JSW_InfoDecoObject(int decoPosition, int decoPositionZ, int decoLengthX, int decoLengthZ, int decoObjectRotation, string funitureName)
    {
        this.decoPositionX = decoPosition;
        this.decoPositionZ = decoPositionZ;
        this.decoLengthX = decoLengthX;
        this.decoLengthZ = decoLengthZ;
        this.decoObjectRotation = decoObjectRotation;
        this.funitureName = funitureName;

    }
}