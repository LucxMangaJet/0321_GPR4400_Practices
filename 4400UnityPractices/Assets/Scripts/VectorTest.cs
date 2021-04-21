using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;
public class VectorTest : MonoBehaviour
{
    public float xVal;
    public Vector position = new Vector(1,2,4);
    public float magnitude;
    public float magnitude1;
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        position = new Vector(xVal, 2, 4);
        magnitude = position.Magnitude;
        //magnitude1 = Vector.Length(position);
    }

    public void OnTriggerEnter(MyCollider other)
    {

    }
}
