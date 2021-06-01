using System;
using UnityEngine;

[System.Serializable]
public class SplineNode
{
    public Vector3 Position;
    public Vector3 Scale;
    public float Roll;

    public Vector3 InPoint;
    public Vector3 OutPoint;


    public SplineNode()
    {
        Position = Vector3.zero;
        Scale = Vector3.one;
        Roll = 0;
        InPoint = Vector3.zero;
        OutPoint = Vector3.zero;
    }
}