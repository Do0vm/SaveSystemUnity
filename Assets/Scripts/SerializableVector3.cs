// In a file like SerializableTypes.cs or with your other data classes

[System.Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(float rX, float rY, float rZ)
    {
        x = rX;
        y = rY;
        z = rZ;
    }

    
    public UnityEngine.Vector3 ToVector3()
    {
        return new UnityEngine.Vector3(x, y, z);
    }

    
    public static SerializableVector3 FromVector3(UnityEngine.Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator UnityEngine.Vector3(SerializableVector3 rValue)
    {
        return new UnityEngine.Vector3(rValue.x, rValue.y, rValue.z);
    }

    public static implicit operator SerializableVector3(UnityEngine.Vector3 rValue)
    {
        return new SerializableVector3(rValue.x, rValue.y, rValue.z);
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", x, y, z);
    }
}