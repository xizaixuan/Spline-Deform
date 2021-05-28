using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class Spline : MonoBehaviour
{
    [HideInInspector]
    public List<SplineNode> Nodes = new List<SplineNode>();
    [HideInInspector]
    public List<SplineCurve> Curves = new List<SplineCurve>();

    [HideInInspector]
    public float Distance;

    private bool m_Dirty = false;

    public void Reset()
    {
        Nodes.Clear();
        Curves.Clear();

        var defRadius = 1.0f;
        var vector = 0.5f;

        for (int i = 0; i < 4; i++)
        {
            float angle = Mathf.PI * 2.0f * i / 4;
            float sinfac = Mathf.Sin(angle);
            float cosfac = Mathf.Cos(angle);

            Vector3 pos = new Vector3(cosfac * defRadius, sinfac * defRadius, 0.0f);
            Vector3 rotVec = new Vector3(sinfac * vector, -cosfac * vector, 0.0f);

            AddNode(new SplineNode() { Position = pos });
        }
    }

    public void OnEnable()
    {
        RefreshCurves();
    }

    public void RefreshCurves()
    {
        Curves.Clear();
        var nodeCount = Nodes.Count;
        for (int i = 0; i < nodeCount - 1; i++)
        {
            var curNode = Nodes[i];
            var nextNode = Nodes[i + 1];
            
            Curves.Add(new SplineCurve(curNode, nextNode));
        }
        UpdateCurveDistance();
    }

    public void AddNode(SplineNode node)
    {
        Nodes.Add(node);

        SplineUtil.ComputeTangent(this);

        m_Dirty = true;
    }

    public void InsertNode(int insertIndex, SplineNode node)
    {
        Nodes.Insert(insertIndex, node);

        SplineUtil.ComputeTangent(this);

        m_Dirty = true;
    }

    public void RemoveNode(SplineNode node)
    {
        Nodes.Remove(node);

        SplineUtil.ComputeTangent(this);

        m_Dirty = true;
    }

    public void MoveNode(SplineNode node, Vector3 delta)
    {
        node.Position += delta;

        SplineUtil.ComputeTangent(this);

        m_Dirty = true;
    }

    private void UpdateCurveDistance()
    {
        Distance = 0;
        foreach (var curve in Curves)
        {
            Distance += curve.DistanceLocal;
            curve.DistanceGlobal = Distance;
        }
    }

    public void Update()
    {
        if (m_Dirty)
        {
            m_Dirty = false;
            RefreshCurves();
        }
    }
}