using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineUtil
{
    static public Vector3 InterpCurvePositioin(SplineNode n0, SplineNode n1, float t)
    {
        float omt = 1.0f - t;

        float omt2 = omt * omt;
        float omt3 = omt2 * omt;

        float t2 = t * t;
        float t3 = t2 * t;

        omt = 3.0f * omt * t2;
        omt2 = 3.0f * omt2 * t;

        return (omt3 * n0.Position) + (omt2 * n0.OutPoint) + (omt * n1.InPoint) + (t3 * n1.Position);
    }

    static public Vector3 InterpCurveTangent(SplineNode n0, SplineNode n1, float t)
    {
        float omt = 1.0f - t;

        float omt2 = omt * omt;

        float t2 = t * t;

        return (3.0f * omt2 * (n0.OutPoint - n0.Position) +
               6.0f * omt * t * (n1.InPoint - n0.OutPoint) +
               3.0f * t2 * (n1.OutPoint - n1.Position)).normalized;
    }

    static public SplineSample Interp(Spline spline, float percent)
    {
        var distance = spline.Distance * percent;

        var curveCount = spline.Curves.Count;
        var targetIndex = 0;

        for (int i = 0; i < curveCount; i++)
        {
            targetIndex = i;
            if (distance <= spline.Curves[i].DistanceGlobal)
            {
                break;
            }
        }

        var targetCurve = spline.Curves[targetIndex];

        var t = 1 - (targetCurve.DistanceGlobal - distance) / targetCurve.DistanceLocal;

        var pos = InterpCurvePositioin(targetCurve.node0, targetCurve.node1, t);
        var forward = InterpCurveTangent(targetCurve.node0, targetCurve.node1, t);

        var scale = Vector3.Lerp(targetCurve.node0.Scale, targetCurve.node1.Scale, t);
        var roll = Mathf.Lerp(targetCurve.node0.Roll, targetCurve.node1.Roll, t);
        return new SplineSample() { Position = pos, Forward = forward, Scale = scale, Roll = roll };
    }

    static public Mesh DuplicateMesh(Mesh mesh)
    {
        var newMesh = new Mesh
        {
            vertices = mesh.vertices,
            uv = mesh.uv,
            uv2 = mesh.uv2,
            uv3 = mesh.uv3,
            uv4 = mesh.uv4,
            normals = mesh.normals,
            tangents = mesh.tangents,
            colors = mesh.colors,
            subMeshCount = mesh.subMeshCount,
            boneWeights = mesh.boneWeights,
            bindposes = mesh.bindposes,
            name = mesh.name + "Duplicated"
        };

        for (int s = 0; s < mesh.subMeshCount; s++)
            newMesh.SetTriangles(mesh.GetTriangles(s), s);

        newMesh.RecalculateBounds();

        return newMesh;
    }
}

public enum AxisType
{
    X = 0,
    Y = 1,
    Z = 2,
};