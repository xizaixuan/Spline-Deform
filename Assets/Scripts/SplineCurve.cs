using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SplineCurve
{
    private const int SampleCount = 60;
    public readonly List<CurveSample> Samples = new List<CurveSample>(SampleCount);

    public SplineNode node0;
    public SplineNode node1;

    public float Distance;

    public UnityEvent Changed = new UnityEvent();

    public SplineCurve(SplineNode n0, SplineNode n1)
    {
        node0 = n0;
        node1 = n1;

        node0.Changed += ComputeCurve;
        node1.Changed += ComputeCurve;

        ComputeCurve(null, null);
    }

    private void ComputeCurve(object sender, EventArgs e)
    {
        Samples.Clear();
        Distance = 0;

        var stepValue = 1.0f / SampleCount;
        for (int i = 0; i <= SampleCount; i++)
        {
            var prePos = GetLocation(Mathf.Clamp01((i - 1) * stepValue));
            var curPos = GetLocation(i * stepValue);

            Distance += Vector3.Distance(prePos, curPos);

            Samples.Add(new CurveSample(curPos, Distance, i * stepValue));
        }

        if (Changed != null) Changed.Invoke();
    }

    private Vector3 GetLocation(float t)
    {
        float omt = 1.0f - t;

        float omt2 = omt * omt;
        float omt3 = omt2 * omt;

        float t2 = t * t;
        float t3 = t2 * t;
        
        omt = 3.0f * omt * t2;
        omt2 = 3.0f * omt2 * t;

        return (omt3 * node0.Position) + (omt2 * node0.OutVec) + (omt * node1.InVec) + (t3 * node1.Position);
    }
}
