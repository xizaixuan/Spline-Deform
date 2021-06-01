using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Modified : MonoBehaviour
{
    public MeshFilter MeshFilter = null;

    public Mesh OriginalMesh = null;

    public Mesh DeformMesh = null;

    public Spline spline = null;

    [Range(0, 1)]
    public float Percent = 0.5f;

    public void Start()
    {
        MeshFilter = GetComponent<MeshFilter>();

        OriginalMesh = MeshFilter?.sharedMesh;

        DeformMesh = SplineUtil.DuplicateMesh(OriginalMesh);

        MeshFilter.mesh = DeformMesh;
    }

    public void OnDestroy()
    {
        if (Application.isEditor)
            DestroyImmediate(DeformMesh);
        else
            Destroy(DeformMesh);

        DeformMesh = null;

        if (MeshFilter != null)
        {
            MeshFilter.mesh = OriginalMesh;
        }
    }

    public void Update()
    {
        if (spline != null)
        {
            var samplePoint = SplineUtil.Interp(spline, Percent);
            transform.localPosition = samplePoint.Position;

            var distance = spline.Distance * Percent;

            var vertices = OriginalMesh.vertices;
            var tempArray = new Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                var vertex = Quaternion.AngleAxis(90.0f, Vector3.forward) * vertices[i];
                
                var t = Percent + vertex.z * 1.0f / spline.Distance;
                var sample = SplineUtil.Interp(spline, t);

                var up = Vector3.Cross(sample.Forward, Vector3.Cross(Vector3.up, sample.Forward).normalized);
                var rotation = Quaternion.LookRotation(sample.Forward, up);

                var matrix = Matrix4x4.TRS(sample.Position, Quaternion.LookRotation(sample.Forward, up), Vector3.one);
                matrix = transform.worldToLocalMatrix * spline.transform.localToWorldMatrix * matrix;

                tempArray[i] = matrix * vertex;
            }

            DeformMesh.vertices = tempArray;
        }
    }
}
