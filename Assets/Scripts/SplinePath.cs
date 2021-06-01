using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplinePath : MonoBehaviour
{
    [Range(0, 1)]
    public float Percent;

    public GameObject target;

    public Spline path;

    public void Update()
    {
        if (target != null && path != null)
        {
            var samplePoint = SplineUtil.Interp(path, Percent);
            target.transform.position = samplePoint.Position;

            var up = Vector3.Cross(samplePoint.Forward, Vector3.Cross(Quaternion.AngleAxis( samplePoint.Roll, Vector3.forward) * Vector3.up, samplePoint.Forward).normalized);
            target.transform.rotation = Quaternion.LookRotation(samplePoint.Forward, up);
            target.transform.localScale = samplePoint.Scale;
        }
    }
}