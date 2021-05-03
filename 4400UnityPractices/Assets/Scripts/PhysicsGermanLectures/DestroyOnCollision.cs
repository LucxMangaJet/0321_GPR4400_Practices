using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCollision : MonoBehaviour
{
    private void MyOnCollisionEnter(MyCollider _other)
    {
        Debug.Log(_other);
        Destroy(this.gameObject);
    }
     
}
