using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class Spline : MonoBehaviour
{
    public List<SplineNode> Nodes = new List<SplineNode>();
    public List<SplineCurve> Curves = new List<SplineCurve>();

    public float Distance;

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

            AddNode(new SplineNode()
            {
                Position = pos,
                InVec = pos + rotVec,
                OutVec = pos - rotVec
            });
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

            var curve = new SplineCurve(curNode, nextNode);
            Curves.Add(curve);
            curve.Changed.AddListener(UpdateCurve);
        }

        UpdateCurve();
    }

    public void AddNode(SplineNode node)
    {
        Nodes.Add(node);

        if (Nodes.Count > 1)
        {
            var prevNode = Nodes[Nodes.IndexOf(node) - 1];
            var curve = new SplineCurve(prevNode, node);
            Curves.Add(curve);
            curve.Changed.AddListener(UpdateCurve);
        }

        UpdateCurve();
    }

    private void UpdateCurve()
    {
        Distance = 0;
        foreach (var curve in Curves)
        {
            Distance += curve.Distance;
        }
    }
}