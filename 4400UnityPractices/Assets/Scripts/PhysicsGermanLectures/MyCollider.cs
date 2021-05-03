using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyCollider : MonoBehaviour
{
    public Vector3 Position
    {
        get
        {
            if(m_rigidbody != null)
            {
                return m_rigidbody.NextPosition + m_offset;
            }
            else
            {
                return transform.position + m_offset;
            }
        }
    }

    public bool isTrigger
    {
        get => m_isTrigger;
        set => m_isTrigger = value;
    }

    [SerializeField]
    protected bool m_isTrigger;
    [SerializeField]
    protected Vector3 m_offset;

    protected MyRigidbody m_rigidbody;

    private void Awake()
    {
        m_rigidbody = GetComponent<MyRigidbody>();
    }

    public virtual bool isColliding(MyCollider _other)
    {
        Debug.LogWarning($"The collider type {_other} is not implemented for {this}!", this);
        return false;
    }
    public virtual MyContactPoint GenerateContactPoint(MyCollider _other)
    {
        Debug.LogWarning($"The collider type {_other} is not implemented {this} !", this);
        return default;
    }
}
