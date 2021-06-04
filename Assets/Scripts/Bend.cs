using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class Bend : MonoBehaviour
{
    private Mesh m_OriginalMesh = null;

    private Mesh m_DeformMesh = null;

    public float IntervalLength = 1.0f;

    public float WalkDistance = 0;

    public Spline m_Spline;

    private Dictionary<float, SplineSample> m_SampleCached = new Dictionary<float, SplineSample>();


    public AxisType AxisType = AxisType.Z;
    
    private float m_XAxisMin, m_XAxisMax;
    private float m_YAxisMin, m_YAxisMax;
    private float m_ZAxisMin, m_ZAxisMax;

    public void OnEnable()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            if (m_OriginalMesh == null)
            {
                m_OriginalMesh = meshFilter.sharedMesh;
            }
            m_DeformMesh = SplineUtil.DuplicateMesh(m_OriginalMesh);
            meshFilter.sharedMesh = m_DeformMesh;
        }


        m_XAxisMin = m_YAxisMin = m_ZAxisMin = float.MaxValue;
        m_XAxisMax = m_YAxisMax = m_ZAxisMax = float.MinValue;

        foreach (var vertex in m_DeformMesh.vertices)
        {
            if (vertex.x < m_XAxisMin)
                m_XAxisMin = vertex.x;

            if (vertex.x > m_XAxisMax)
                m_XAxisMax = vertex.x;

            if (vertex.y < m_YAxisMin)
                m_YAxisMin = vertex.y;

            if (vertex.y > m_YAxisMax)
                m_YAxisMax = vertex.y;

            if (vertex.z < m_ZAxisMin)
                m_ZAxisMin = vertex.z;

            if (vertex.z > m_ZAxisMax)
                m_ZAxisMax = vertex.z;
        }
    }

    public void OnDisable()
    {
        if (Application.isEditor)
            DestroyImmediate(m_DeformMesh);
        else
            Destroy(m_DeformMesh);

        m_DeformMesh = null;

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.sharedMesh = m_OriginalMesh;
        }
    }

    public void Update()
    {
        Modify();
    }

    public void Modify()
    {
        m_SampleCached.Clear();

        var bendVertices = new List<Vector3>(m_DeformMesh.vertexCount);
        var axisQuaternion = Quaternion.identity;
        var axisUp = Quaternion.AngleAxis(90, Vector3.forward);

        var distanceRange = 0.0f;
        var axisMin = 0.0f;
        switch (AxisType)
        {
            case AxisType.X:
                {
                    axisQuaternion = Quaternion.AngleAxis(90, Vector3.up);
                    distanceRange = m_XAxisMax - m_XAxisMin;
                    axisMin = m_XAxisMin;
                }
                break;
            case AxisType.Y:
                {
                    axisQuaternion = Quaternion.AngleAxis(-90, Vector3.right);
                    distanceRange = m_YAxisMax - m_YAxisMin;
                    axisMin = m_YAxisMin;
                }
                break;
            case AxisType.Z:
                {
                    distanceRange = m_ZAxisMax - m_ZAxisMin;
                    axisMin = m_ZAxisMin;
                }
                break;
        }

        WalkDistance = Mathf.Clamp(WalkDistance, 0.0f, m_Spline.Distance);

        foreach (var vertex in m_OriginalMesh.vertices)
        {
            var point = axisQuaternion * vertex;
            var distance = point.z - axisMin;

            var distanceRate = distance / distanceRange;

            if (!m_SampleCached.TryGetValue(distanceRate, out SplineSample sample))
            {
                var distOnSpline = WalkDistance + distanceRate * distanceRange * IntervalLength;
                sample = SplineUtil.Interp(m_Spline, distOnSpline / m_Spline.Distance);
                m_SampleCached[distanceRate] = sample;
            }

            var up = Vector3.Cross(sample.Forward, Vector3.Cross(axisUp * sample.Forward, sample.Forward));
            var rotation = Quaternion.LookRotation(sample.Forward, up);

            var matrix = transform.worldToLocalMatrix * m_Spline.transform.localToWorldMatrix * Matrix4x4.TRS(sample.Position, rotation, Vector3.one);

            bendVertices.Add(matrix.MultiplyPoint3x4(new Vector3(point.x, point.y, 0.0f)));
        }

        m_DeformMesh.vertices = bendVertices.ToArray();
        m_DeformMesh.RecalculateBounds();
    }
}
