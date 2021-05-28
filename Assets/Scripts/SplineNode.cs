using System;
using UnityEngine;

[System.Serializable]
public class SplineNode
{
    public Vector3 Position;
    public Vector3 Scale;
    public float Roll;

    public Vector3 InTangent;
    public Vector3 OutTangent;
    
    public SplineNode()
    {
        Position = Vector3.zero;
        Scale = Vector3.one;
        Roll = 0;
        InTangent = Vector3.zero;
        OutTangent = Vector3.zero;
    }
}