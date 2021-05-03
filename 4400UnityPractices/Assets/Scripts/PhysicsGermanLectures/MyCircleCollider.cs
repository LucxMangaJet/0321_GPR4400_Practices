using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCircleCollider : MyCollider
{
    public float ActualRadius => transform.lossyScale.x * m_radius;
    public float Radius => m_radius;
    [SerializeField]
    protected float m_radius;

    public override bool isColliding(MyCollider _other)
    {
        if (_other == null)
            return false;
        if(_other is MyCircleCollider)
        {
            MyCircleCollider other = (MyCircleCollider)_other;
            float distance = (other.Position - this.Position).sqrMagnitude;
            float radii = other.m_radius * other.transform.lossyScale.x + this.m_radius * other.transform.lossyScale.x;

            return distance < (radii * radii);
        }
        else if (_other is MyBox2DCollider)
        {
            MyBox2DCollider other = (MyBox2DCollider)_other;
            Vector3 closestPoint = other.GetClosestPoint(Position);
            float distance = Vector3.SqrMagnitude(closestPoint - Position);

            return distance < (Radius * Radius);
        }
        return base.isColliding(_other);
    }

    public override MyContactPoint GenerateContactPoint(MyCollider _other)
    {
        if (_other == null)
            return default;
        if(_other is MyCircleCollider)
        {
            MyCircleCollider other = (MyCircleCollider)_other;
            Vector3 direction = other.Position - this.Position;
            float distance = direction.magnitude;
            direction = direction.normalized;
            return new MyContactPoint(other.Position + -direction * other.ActualRadius,-direction,distance - other.ActualRadius - this.ActualRadius, this.m_rigidbody, other.m_rigidbody);
        }
        return base.GenerateContactPoint(_other);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        float realRadius = transform.lossyScale.x * m_radius;

        Gizmos.DrawWireSphere(transform.position + m_offset, ActualRadius);
    }
}
