using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyBox2DCollider : MyCollider
{
    private Vector3 halfExtent => Vector3.Scale(m_size / 2, transform.lossyScale);
    private Vector3 minPosition => Position + new Vector3(-halfExtent.x, -halfExtent.y, 0);
    private Vector3 maxPosition => Position + new Vector3(halfExtent.x, halfExtent.y, 0);

    [SerializeField]
    private Vector3 m_size;

    [SerializeField]
    private Transform m_testTransform;

    public override bool isColliding(MyCollider _other)
    {
        if (_other == null)
            return false;
        if(_other is MyBox2DCollider)
        {
            MyBox2DCollider other = (MyBox2DCollider)_other;
            if (this.minPosition.x > other.minPosition.x && this.minPosition.x < other.maxPosition.x && this.minPosition.y > other.minPosition.y && this.minPosition.y < other.maxPosition.y)
                return true;
            if (this.maxPosition.x > other.minPosition.x && this.maxPosition.x < other.maxPosition.x && this.maxPosition.y > other.minPosition.y && this.maxPosition.x < other.maxPosition.x)
                return true;
            return false;
        }
        else if(_other is MyCircleCollider)
        {
            MyCircleCollider other = (MyCircleCollider)_other;
            Vector3 closestPoint = GetClosestPoint(other.Position);
            float distance = Vector3.SqrMagnitude(closestPoint - other.Position);

            return distance < (other.Radius * other.Radius);
        }
        return base.isColliding(_other);
    }

    public Vector3 GetClosestPoint(Vector3 _pos)
    {
        Vector3 dir = _pos - Position;
        //Debug.DrawLine(Position, Position + dir, Color.magenta, 5.0f);
        return Position + new Vector3(Mathf.Clamp(dir.x, -halfExtent.x, halfExtent.x), Mathf.Clamp(dir.y, -halfExtent.y, halfExtent.y), Mathf.Clamp(dir.z, -halfExtent.z, halfExtent.z));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Vector3 upperLeft = Position + new Vector3(-halfExtent.x, halfExtent.y, 0);
        Vector3 upperRight = Position + new Vector3(halfExtent.x, halfExtent.y);
        Vector3 bottomLeft = Position + new Vector3(-halfExtent.x, -halfExtent.y); ;
        Vector3 bottomRight = Position + new Vector3(halfExtent.x, -halfExtent.y, 0);

        Gizmos.DrawLine(upperLeft, upperRight);
        Gizmos.DrawLine(upperLeft, bottomLeft);
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(upperRight, bottomRight);

        if (m_testTransform == null)
            return;
        Gizmos.color = Color.red;
        Vector3 pos = GetClosestPoint(m_testTransform.position);
        Gizmos.DrawSphere(pos, 0.1f);
    }
}
