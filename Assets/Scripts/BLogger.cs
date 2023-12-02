using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MType { N, P, W, S}

public class BLogger : MonoBehaviour
{
    public static void Print(MType type, string text)
    {
        switch (type)
        {
            case MType.N:
                Debug.Log($"<color=red><b>{text}</b></color>");
                break;
            case MType.P:
                Debug.Log($"<color=green><b>{text}</b></color>");
                break;
            case MType.W:
                Debug.Log($"<color=orange><b>{text}</b></color>");
                break;
            case MType.S:
                Debug.Log($"<color=yellow><b>{text}</b></color>");
                break;
            default:
                break;
        }
    }
}
