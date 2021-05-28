using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SplineCurve
{
    private const int SampleCount = 60;
    public readonly List<SplineSample> Samples = new List<SplineSample>(SampleCount);

    public SplineNode node0;
    public SplineNode node1;

    public float DistanceLocal;
    public float DistanceGlobal;

    public SplineCurve(SplineNode n0, SplineNode n1)
    {
        node0 = n0;
        node1 = n1;

        Sampling();
    }

    private void Sampling()
    {
        DistanceLocal = 0;

        var stepValue = 1.0f / SampleCount;
        for (int i = 0; i <= SampleCount; i++)
        {
            var prePos = SplineUtil.InterpCurve(node0, node1, Mathf.Clamp01((i - 1) * stepValue));
            var curPos = SplineUtil.InterpCurve(node0, node1, Mathf.Clamp01(i * stepValue));

            DistanceLocal += Vector3.Distance(prePos, curPos);

            Samples.Add(new SplineSample() { Position = curPos, Forward = Vector3.zero });
        }
    }
}
