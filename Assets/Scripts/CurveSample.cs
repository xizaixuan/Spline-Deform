using UnityEngine;

public struct CurveSample
{
    public readonly Vector3 position;
    public readonly float distance;
    public readonly float time;

    public CurveSample(Vector3 position, float distance, float time)
    {
        this.position = position;
        this.distance = distance;
        this.time = time;
    }
}
