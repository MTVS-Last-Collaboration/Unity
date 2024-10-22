using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class JSW_Schedule : MonoBehaviour
{
    public int iconCode { get; set; }
    public string Description { get; set; }

    public JSW_Schedule(int iconcode, string description)
    {
        iconCode = iconcode;
        Description = description;
    }
}
