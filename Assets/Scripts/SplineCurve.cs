using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SplineCurve
{
    private const int SampleCount = 120;
    public readonly List<CurveSample> Samples = new List<CurveSample>(SampleCount);

    public SplineNode node0;
    public SplineNode node1;

    public float DistanceLocal;
    public float DistanceGlobal;

    public SplineCurve(SplineNode n0, SplineNode n1)
    {
        node0 = n0;
        node1 = n1;

        node0.Changed += Sampling;
        node1.Changed += Sampling;

        Sampling(null, null);
    }

    private void Sampling(object sender, EventArgs e)
    {
        Samples.Clear();
        DistanceLocal = 0;

        var stepValue = 1.0f / SampleCount;
        for (int i = 0; i <= SampleCount; i++)
        {
            var prePos = SplineUtil.InterpCurve(node0, node1, Mathf.Clamp01((i - 1) * stepValue));
            var curPos = SplineUtil.InterpCurve(node0, node1, Mathf.Clamp01(i * stepValue));

            DistanceLocal += Vector3.Distance(prePos, curPos);

            Samples.Add(new CurveSample(curPos, DistanceLocal, i * stepValue));
        }
    }
}
