using UnityEngine;

[System.Serializable]
public class HandData
{
    public Vector3 originPosition;
    public Quaternion originRotation;
    public Vector3[] fingerPositions;
    public Quaternion[] fingerRotations;
    public Vector3[] fingerScales;

}