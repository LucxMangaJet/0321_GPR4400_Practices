using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem;
    [SerializeField] float force;
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] bool atPoint;

    float throttle;
    float particleMaxRate;
    bool shutdown;
    private void Start()
    {
        var emission = particleSystem.emission;
        particleMaxRate = emission.rateOverTimeMultiplier;
        emission.rateOverTimeMultiplier = 0;
    }

    private void FixedUpdate()
    {
        if (throttle > 0 && !shutdown)
        {
            if (atPoint)
            {
                rigidbody.AddForceAtPosition(transform.forward * throttle * force * Time.fixedDeltaTime, transform.position, ForceMode.Impulse);
            }
            else
            {
                rigidbody.AddForce(transform.forward * throttle * force * Time.fixedDeltaTime, ForceMode.Impulse);
            }
        }
    }

    public void Shutdown()
    {
        SetThrottle(0);
        shutdown = true;
    }

    public void SetThrottle(float percentThrottle)
    {
        throttle = Mathf.Clamp01(percentThrottle);
        var emission = particleSystem.emission;
        emission.rateOverTimeMultiplier = throttle * particleMaxRate;
    }

    public Vector3 GetDirection()
    {
        return transform.forward;
    }

    public float GetMaxImpulseStrength()
    {
        return force;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = throttle > 0 ? Color.green : Color.gray;
        Vector3 target = transform.position - transform.forward * throttle;
        Gizmos.DrawLine(transform.position, target);
    }
}
