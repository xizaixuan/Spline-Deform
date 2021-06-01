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

        AddNode(new SplineNode() { Position = new Vector3(-5, 0, 0), InPoint = new Vector3(-5, -5, 0), OutPoint = new Vector3(-5, 5, 0) });
        AddNode(new SplineNode() { Position = new Vector3( 5, 0, 0), InPoint = new Vector3( 5, 5, 0), OutPoint = new Vector3(5, -5, 0) });
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

        m_Dirty = true;
    }

    public void InsertNode(int insertIndex, SplineNode node)
    {
        Nodes.Insert(insertIndex, node);

        m_Dirty = true;
    }

    public void RemoveNode(SplineNode node)
    {
        Nodes.Remove(node);

        m_Dirty = true;
    }

    public void MoveNode(SplineNode node, Vector3 delta)
    {
        node.Position += delta;

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