using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyContactPoint 
{
    public Vector3 Point { get; }
    public Vector3 Normal { get; }
    public float Distance { get; }

    public MyRigidbody Other { get; }
    public MyRigidbody This { get; }

    public MyContactPoint(Vector3 point, Vector3 normal, float distance, MyRigidbody _this, MyRigidbody _other)
    {
        Point = point;
        Normal = normal;
        Distance = distance;
        This = _this;
        Other = _other;
    }
}
