using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyConstantForce : MonoBehaviour
{
    [SerializeField] private Vector3 m_impulse;

    [SerializeField] private Vector3 m_force;
    [SerializeField] private Vector3 m_torque;

    [SerializeField]
    private MyRigidbody m_myRigidbody;
    private void Awake()
    {
        m_myRigidbody = GetComponent<MyRigidbody>();
    }

    private void Start()
    {
        if(m_myRigidbody != null)
        {
            m_myRigidbody.AddForce(m_impulse, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if(m_myRigidbody != null)
        {
            m_myRigidbody.AddForce(m_force);
            m_myRigidbody.AddTorque(m_torque);
        }
    }
}
