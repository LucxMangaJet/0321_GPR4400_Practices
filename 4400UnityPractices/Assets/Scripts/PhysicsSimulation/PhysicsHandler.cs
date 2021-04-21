using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHandler : MonoBehaviour
{
    List<Collider> allColliders;


    void CheckOverlap()
    {
        for (int a = 0; a < allColliders.Count; a++)
        {
            for (int b = a + 1; b < allColliders.Count; b++)
            {
                //Check(a,b);
            }
        }
    }

}
