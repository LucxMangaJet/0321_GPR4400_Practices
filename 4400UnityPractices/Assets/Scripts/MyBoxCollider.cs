using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomMath;

public class MyBoxCollider : MyCollider
{
    public Vector3[] cubeCorners => m_cubeCorners;

    private Vector3 halfExtent =>Vector3.Scale(m_size / 2, transform.lossyScale);
    private Vector3[] m_cubeCorners = new Vector3[8];
    [SerializeField]
    private Vector3 m_size;

    private static Vector3[] s_defaultCorners = new Vector3[]
    {
        new Vector3(-0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, 0.5f, -0.5f),
        new Vector3(0.5f, -0.5f, -0.5f),
        new Vector3(-0.5f, -0.5f, -0.5f),

        new Vector3(-0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, 0.5f, 0.5f),
        new Vector3(0.5f, -0.5f, 0.5f),
        new Vector3(-0.5f, -0.5f, 0.5f),
    };

    public Vector3[] CubeCorners => m_cubeCorners;
    public override bool isColliding(MyCollider _other)
    {
        if (_other == null)
            return false;
        if(_other is MyBoxCollider)
        {
            MyBoxCollider other = (MyBoxCollider)_other;

            Vector3[] mySegment = new Vector3[]
            {
                m_cubeCorners[4] - m_cubeCorners[0],
                m_cubeCorners[3] - m_cubeCorners[0],
                m_cubeCorners[1] - m_cubeCorners[0],
            };

            Vector3[] otherSegments = new Vector3[]
            {
                other.m_cubeCorners[4] - other.m_cubeCorners[0],
                other.m_cubeCorners[3] - other.m_cubeCorners[0],
                other.m_cubeCorners[1] - other.m_cubeCorners[0],
            };

            Vector3[] potentialSeperatingAxis = new Vector3[]
            {
                Vector3.Cross(mySegment[0], mySegment[1]),
                Vector3.Cross(mySegment[0], mySegment[2]),
                Vector3.Cross(mySegment[1], mySegment[2]),

                Vector3.Cross(mySegment[0], mySegment[1]),
                Vector3.Cross(mySegment[0], mySegment[2]),
                Vector3.Cross(mySegment[1], mySegment[2]),

                Vector3.Cross(mySegment[0], mySegment[0]),
                Vector3.Cross(mySegment[0], mySegment[1]),
                Vector3.Cross(mySegment[0], mySegment[2]),
                Vector3.Cross(mySegment[1], mySegment[0]),
                Vector3.Cross(mySegment[1], mySegment[1]),
                Vector3.Cross(mySegment[1], mySegment[2]),
                Vector3.Cross(mySegment[1], mySegment[0]),
                Vector3.Cross(mySegment[1], mySegment[1]),
                Vector3.Cross(mySegment[1], mySegment[2]),
            };

            Vector3 normalizedAxis;
            float min1, max1, min2, max2;
            float tmp;
            foreach(Vector3 axis in potentialSeperatingAxis)
            {
                if(axis.sqrMagnitude == 0)
                    continue;
                normalizedAxis = axis.normalized;

                min1 = float.PositiveInfinity;
                max1 = float.NegativeInfinity;

                for(int i = 0; i < this.m_cubeCorners.Length; i++)
                {
                    tmp = Vector3.Dot(normalizedAxis, this.m_cubeCorners[i]);
                    if(tmp < min1)
                    {
                        min1 = tmp;
                    }
                    if(tmp > max1)
                    {
                        max1 = tmp;
                    }
                }
                min2 = float.PositiveInfinity;
                max2 = float.NegativeInfinity;

                for(int i = 0; i < other.m_cubeCorners.Length; i++)
                {
                    tmp = Vector3.Dot(normalizedAxis, this.m_cubeCorners[i]);
                    if (tmp < min2)
                    {
                        min2 = tmp;
                    }
                    if (tmp > max2)
                    {
                        max2 = tmp;
                    }
                }

                if (min1 > min2 && min1 < max2)
                    continue;
                if (max1 > min1 && max1 < max2)
                    continue;

                return false;
            }

            return true;
        }
        else if(_other is MySphereCollider)
        {
            this.CalculateCubeCorners();
            MySphereCollider other = (MySphereCollider)_other;
            Vector3 potentialAxis = this.Position - other.Position;
            potentialAxis = potentialAxis.normalized;

            float min1, max1, min2, max2;

            min1 = float.PositiveInfinity;
            max1 = float.NegativeInfinity;
            float tmp;
            for(int i = 0; i < cubeCorners.Length; i++)
            {
                tmp = Vector3.Dot(potentialAxis, this.m_cubeCorners[i]);
                if(tmp < min1)
                {
                    tmp = min1;
                }
                if(tmp > max1)
                {
                    max1 = tmp;
                }
            }

            min2 = float.PositiveInfinity;
            max2 = float.NegativeInfinity;

            tmp = Vector3.Dot(potentialAxis, other.Position);
            min2 = tmp - other.ActualRadius;
            max2 = tmp + other.ActualRadius;
            if (min1 > min2 && min1 < max2)
                return true;
            if (max1 > min2 && max1 < max2)
                return true;
            return false;
        }
        return base.isColliding(_other);
    }
    public void CalculateCubeCorners()
    {
        Matrix trs = Matrix.TRS(transform.position, transform.localEulerAngles, transform.lossyScale);

        if(m_cubeCorners.Length != s_defaultCorners.Length)
        {
            m_cubeCorners = new Vector3[s_defaultCorners.Length];
        }

        for(int i = 0; i < s_defaultCorners.Length; i++)
        {
            m_cubeCorners[i] = trs * s_defaultCorners[i];
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        CalculateCubeCorners();

        Vector3 upperLeftFront = m_cubeCorners[0];
        Vector3 upperRightFront = m_cubeCorners[1];
        Vector3 bottomRightFront = m_cubeCorners[2];
        Vector3 bottomLeftFront = m_cubeCorners[3];

        Vector3 upperLeftBack = m_cubeCorners[4];
        Vector3 upperRightBack = m_cubeCorners[5];
        Vector3 bottomRightBack = m_cubeCorners[6];
        Vector3 bottomLeftBack = m_cubeCorners[7];

        Gizmos.DrawLine(upperLeftBack, upperRightBack);
        Gizmos.DrawLine(bottomLeftBack, bottomRightBack);
        Gizmos.DrawLine(upperLeftFront, upperRightFront);
        Gizmos.DrawLine(bottomLeftFront, bottomRightFront);

        Gizmos.DrawLine(upperLeftBack, upperLeftFront);
        Gizmos.DrawLine(upperRightBack, upperRightFront);
        Gizmos.DrawLine(bottomLeftBack, bottomLeftFront);
        Gizmos.DrawLine(bottomRightBack, bottomRightFront);

        Gizmos.DrawLine(upperLeftBack, bottomLeftBack);
        Gizmos.DrawLine(upperRightBack, bottomRightBack);
        Gizmos.DrawLine(upperRightFront, bottomRightFront);
        Gizmos.DrawLine(upperLeftFront, bottomLeftFront);
    }
}


        //Vector3 upperLeftFront = Position + new Vector3(-halfExtent.x, halfExtent.y, -halfExtent.z);
        //Vector3 upperRightFront = Position + new Vector3(halfExtent.x, halfExtent.y, -halfExtent.z);
        //Vector3 bottomRightFront = Position + new Vector3(halfExtent.x, -halfExtent.y, -halfExtent.z);
        //Vector3 bottomLeftFront = Position + new Vector3(-halfExtent.x, -halfExtent.y, -halfExtent.z); 
        //
        //Vector3 upperLeftBack = Position + new Vector3(-halfExtent.x, halfExtent.y, halfExtent.z);
        //Vector3 upperRightBack = Position + new Vector3(halfExtent.x, halfExtent.y, halfExtent.z);
        //Vector3 bottomLeftBack = Position + new Vector3(-halfExtent.x, -halfExtent.y, halfExtent.z); 
        //Vector3 bottomRightBack = Position + new Vector3(halfExtent.x, -halfExtent.y, halfExtent.z);