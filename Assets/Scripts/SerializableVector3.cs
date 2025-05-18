using System;
using UnityEngine;

[Serializable] // JSON can’t handle Vector3 directly
public struct SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX; y = rY; z = rZ;
    }

    public Vector3 ToVector3() => new Vector3(x, y, z);

    public static SerializableVector3 FromVector3(Vector3 v)
        => new SerializableVector3(v.x, v.y, v.z);
}
