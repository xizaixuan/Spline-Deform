using System;
using UnityEngine;

[System.Serializable]
public class SplineNode
{
    public Vector3 Position
    {
        get { return m_Position; }
        set
        {
            if (m_Position.Equals(value))
                return;

            m_Position = value;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
    public Vector3 InVec;
    public Vector3 OutVec;

    [SerializeField]
    private Vector3 m_Position;

    public event EventHandler Changed;

    public SplineNode()
    {
        Position = Vector3.zero;
        InVec = Vector3.zero;
        OutVec = Vector3.zero;
    }
}