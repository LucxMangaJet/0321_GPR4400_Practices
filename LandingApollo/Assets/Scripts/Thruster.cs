using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float force;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] bool atPoint;

    bool on;

    private void Start()
    {
        particleSystem.Stop();
    }

    private void FixedUpdate()
    {
        if (on)
        {
            if (atPoint)
            {
                rigidbody.AddForceAtPosition(transform.forward * force * Time.fixedDeltaTime, transform.position, ForceMode.Impulse);
            }
            else
            {
                rigidbody.AddForce(transform.forward * force * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }
    }

    public void TurnOn()
    {
        on = true;
        particleSystem.Play();
    }

    public void TurnOff()
    {
        on = false;
        particleSystem.Stop();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = on ? Color.green : Color.gray;
        Vector3 target = transform.position - transform.forward;
        Gizmos.DrawLine(transform.position, target);
    }
}
