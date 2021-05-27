using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineUtil
{
    static public void ComputeTangent(Spline spline)
    {
        if (spline.Nodes.Count >= 4)
        {
            int n = spline.Nodes.Count - 1;

            var p1 = new Vector3[n + 1];
            var p2 = new Vector3[n + 1];

            var a = new float[n + 1];
            var b = new float[n + 1];
            var c = new float[n + 1];
            var r = new Vector3[n + 1];

            // left most segment
            a[0] = 0.0f;
            b[0] = 2.0f;
            c[0] = 1.0f;
            r[0] = spline.Nodes[0].Position + 2.0f * spline.Nodes[1].Position;

            // internal segments
            for (int i = 1; i < n - 1; i++)
            {
                a[i] = 1.0f;
                b[i] = 4.0f;
                c[i] = 1.0f;
                r[i] = 4.0f * spline.Nodes[i].Position + 2.0f * spline.Nodes[i + 1].Position;
            }

            // right segment
            a[n - 1] = 2.0f;
            b[n - 1] = 7.0f;
            c[n - 1] = 0.0f;
            r[n - 1] = 8.0f * spline.Nodes[n - 1].Position + spline.Nodes[n].Position;

            for (int i = 1; i < n; i++)
            {
                float m = a[i] / b[i - 1];
                b[i] = b[i] - m * c[i - 1];
                r[i] = r[i] - m * r[i - 1];
            }

            p1[n - 1] = r[n - 1] / b[n - 1];

            for (int i = n - 2; i >= 0; --i)
            {
                p1[i] = (r[i] - c[i] * p1[i + 1]) / b[i];
            }

            for (int i = 0; i < n - 1; i++)
            {
                p2[i] = 2.0f * spline.Nodes[i + 1].Position - p1[i + 1];
            }

            p2[n - 1] = 0.5f * (spline.Nodes[n].Position + p1[n - 1]);

            for (int i = 0; i < n; i++)
            {
                spline.Nodes[i].OutVec = p1[i];
                spline.Nodes[i + 1].InVec = p2[i];
            }
        }
    }
}