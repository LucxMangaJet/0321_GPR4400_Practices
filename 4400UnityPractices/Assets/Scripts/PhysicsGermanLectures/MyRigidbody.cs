using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyRigidbody : MonoBehaviour
{
    public bool Colliding;
    public float Speed
    {
        get => m_linearVelocity.magnitude;
    }
    public float Mass
    {
        get => m_mass;
        set
        {
            m_mass = value;
            if (m_mass <= 0)
            {
                m_mass = 1e-7f;
            }
            m_inverseMass = 1.0f / m_mass;
        }
    }

    public Vector3 LinearVelocity
    {
        get
        {
            return m_linearVelocity;
        }
        set
        {
            if (isKinematic)
                return;
            m_linearVelocity = value;
        }
    }

    public bool isKinematic
    {
        get => m_isKinematic;
        set
        {
            m_isKinematic = value;
            m_linearVelocity = Vector3.zero;
            m_linearAcceleration = Vector3.zero;
        }
    }

    public Vector3 InertiaTensor
    {
        get => m_inertiaTensor;
        private set
        {
            m_inertiaTensor = value;
            m_inverseInertiaTensor = new Vector3(1.0f / m_inertiaTensor.x, 1.0f / m_inertiaTensor.y, 1.0f / m_inertiaTensor.z);
        }
    }

    public Vector3 NextPosition => m_position;

    [SerializeField]
    private float m_mass = 1;
    [SerializeField]
    private float m_drag = 1;
    [SerializeField]
    private float m_angluarDrag = 1;
    [SerializeField]
    private float m_gravityScale = 1;
    [SerializeField]
    private bool m_isKinematic = false;

    private float m_inverseMass;
    private Vector3 m_linearAcceleration = Vector3.zero;
    private Vector3 m_linearVelocity = Vector3.zero;
    private Vector3 m_angularAcceleration = Vector3.zero;
    private Vector3 m_angularVelocity = Vector3.zero;
    private Vector3 m_inertiaTensor = Vector3.one;
    private Vector3 m_inverseInertiaTensor = Vector3.one;

    private Vector3 m_position;
    private Vector3 m_rotation;

    private List<MyCollider> m_allColliders = new List<MyCollider>();
    private void OnValidate()
    {
        if (m_mass <= 0)
        {
            m_mass = 1e-7f;
        }
        m_inverseMass = 1.0f / m_mass;
        ResetInertiaTensor();
    }
    private void Awake()
    {
        m_inverseMass = 1.0f / m_mass;
        m_position = transform.position;
        m_rotation = transform.eulerAngles;

        m_allColliders = new List<MyCollider>(GetComponents<MyCollider>());

        ResetInertiaTensor();

        MyPhysicsManager.Instance.AddRigidbody(this);
    }
    private void OnDestroy()
    {
        MyPhysicsManager.Instance.RemoveRigidbody(this);
    }
    public void ResetInertiaTensor()
    {
        Collider col = GetComponent<Collider>();
        InertiaTensor = Vector3.one;
        if (col != null)
        {
            if (col is BoxCollider)
            {
                CalculateInertiaTensorCube();
            }
            if (col is SphereCollider)
            {
                CalculateInertiaTensorSphere();
            }
        }
    }
    public void AddForce(Vector3 _force)
    {
        AddForce(_force, ForceMode.Force);
    }
    public void AddForce(Vector3 _force, ForceMode _mode)
    {
        switch (_mode)
        {
            case ForceMode.Force:
                m_linearAcceleration += _force * m_inverseMass;
                break;
            case ForceMode.Acceleration:
                m_linearAcceleration += _force;
                break;
            case ForceMode.Impulse:
                m_linearVelocity += _force * m_inverseMass;
                break;
            case ForceMode.VelocityChange:
                m_linearVelocity += _force;
                break;
        }
    }

    public void AddTorque(Vector3 _torque)
    {
        AddTorque(_torque, ForceMode.Force);
    }
    public void AddTorque(Vector3 _torque, ForceMode _mode)
    {
        switch (_mode)
        {
            case ForceMode.Force:
                m_angularAcceleration += _torque * m_inverseMass;
                break;
            case ForceMode.Acceleration:
                m_angularAcceleration += _torque;
                break;
            case ForceMode.Impulse:
                m_angularVelocity += _torque * m_inverseMass;
                break;
            case ForceMode.VelocityChange:
                m_angularVelocity += _torque;
                break;
        }
    }
    public void AddForceAtPosition(Vector3 _force, Vector3 _position)
    {
        AddForceAtPosition(_force, _position, ForceMode.Force);
    }
    public void AddForceAtPosition(Vector3 _force, Vector3 _position, ForceMode _mode)
    {
        Vector3 distance = _position - transform.position;
        Vector3 torque = Vector3.Cross(distance, _force);

        AddTorque(torque);
        AddForce(_force, _mode);
    }
    public void Step(float _deltaTime)
    {
        if (m_isKinematic)
            return;

        m_linearAcceleration += MyPhysics.Gravity * m_gravityScale;

        m_linearVelocity += m_linearAcceleration * _deltaTime;
        m_linearVelocity += -m_linearVelocity.normalized * m_linearVelocity.magnitude * m_drag * _deltaTime;
        m_linearAcceleration = Vector3.zero;

        m_angularVelocity += Vector3.Scale(m_inverseInertiaTensor, m_angularAcceleration) * _deltaTime;
        m_angularVelocity += -m_angularVelocity.normalized * m_angularVelocity.magnitude * m_angluarDrag * _deltaTime;
        m_angularAcceleration = Vector3.zero;

        m_position += m_linearVelocity * _deltaTime;
        m_rotation += Mathf.Rad2Deg * m_angularVelocity * _deltaTime;
    }
    public void CheckForCollision(MyRigidbody _other)
    {
        foreach (MyCollider ownCollider in m_allColliders)
        {
            foreach (MyCollider otherCollider in _other.m_allColliders)
            {
                MyContactPoint point = ownCollider.GenerateContactPoint(otherCollider);
                if (point.Distance < 0)
                {
                    Colliding = true;
                    if (ownCollider.isTrigger || otherCollider.isTrigger)
                    {
                        this.SendMessage("MyOnTriggerEnter", otherCollider, SendMessageOptions.DontRequireReceiver);
                        _other.SendMessage("MyOnTriggerEnter", ownCollider, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        Solve(point);
                        Seperate(point);
                        this.SendMessage("MyOnCollisionEnter", point, SendMessageOptions.DontRequireReceiver);
                        _other.SendMessage("MyOnCollisionEnter", point, SendMessageOptions.DontRequireReceiver);
                    }
                }
                else
                {
                    Colliding = false;
                }
            }
        }
    }
    private void Solve(MyContactPoint _point)
    {
        float totalImpuls = _point.This.LinearVelocity.magnitude * _point.This.Mass + _point.Other.LinearVelocity.magnitude * _point.Other.Mass;

        _point.This.AddForceAtPosition(_point.Normal * totalImpuls, _point.Point, ForceMode.Impulse);
        _point.Other.AddForceAtPosition(-_point.Normal * totalImpuls, _point.Point, ForceMode.Impulse);
    }
    private void Seperate(MyContactPoint _point)
    {
        _point.This.m_position -= _point.Normal * ((_point.Distance * 0.5f)+ MyPhysics.SEPERATION_VALUE);
        _point.Other.m_position += _point.Normal * ((_point.Distance * 0.5f) + MyPhysics.SEPERATION_VALUE);
    }
    public void Draw()
    {
        transform.position = m_position;
        transform.eulerAngles = m_rotation;
    }
    private void CalculateInertiaTensorCube()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        Vector3 size = Vector3.Scale(boxCollider.size, transform.lossyScale);
        float x = (1 / 12.0f) * m_mass * (size.y * size.y + size.z * size.z);
        float y = (1 / 12.0f) * m_mass * (size.x * size.x + size.z * size.z);
        float z = (1 / 12.0f) * m_mass * (size.y * size.y + size.x * size.x);

        m_inertiaTensor = new Vector3(x, y, z);
    }
    private void CalculateInertiaTensorSphere()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        float radius = sphereCollider.radius * transform.lossyScale.x;

        float value = (2 / 5.0f) * m_mass * radius * radius;

        InertiaTensor = Vector3.one * value;
    }
}
