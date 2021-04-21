using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySphereCollider : MyCollider
{
    [SerializeField]
    private float m_radius;

    public float ActualRadius => transform.lossyScale.x * m_radius;
    public override bool isColliding(MyCollider _other)
    {
        if (_other == null)
            return false;
        if (_other is MySphereCollider)
        {
            MySphereCollider other = (MySphereCollider)_other;
            float distance = (this.Position - other.Position).sqrMagnitude;
            float radii = this.m_radius * transform.lossyScale.x + other.m_radius * other.transform.lossyScale.x;

            return distance < (radii * radii);
        }
        else if (_other is MyBoxCollider)
        {
            MyBoxCollider other = (MyBoxCollider)_other;
            other.CalculateCubeCorners();
            Vector3 potentialAxis = this.Position - other.Position;
            potentialAxis = potentialAxis.normalized;

            float min1, max1, min2, max2;

            min1 = float.PositiveInfinity;
            max1 = float.NegativeInfinity;
            float tmp;
            for (int i = 0; i < other.CubeCorners.Length; i++)
            {
                tmp = Vector3.Dot(potentialAxis, other.cubeCorners[i]);
                if (tmp < min1)
                {
                    tmp = min1;
                }
                if (tmp > max1)
                {
                    max1 = tmp;
                }
            }

            min2 = float.PositiveInfinity;
            max2 = float.NegativeInfinity;

            tmp = Vector3.Dot(potentialAxis, other.Position);
            min2 = tmp - this.ActualRadius;
            max2 = tmp + this.ActualRadius;

            if (min1 > min2 && min1 < max2)
                return true;
            if (max1 > min2 && max1 < max2)
                return true;
            return false;
        }
        return base.isColliding(_other);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(Position, ActualRadius);
    }
}
