using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] Transform elementsParent;
    [SerializeField] Transform raycastOrigin;
    [SerializeField] Transform centerOfMass;

    [Header("Modules")]
    [SerializeField] LandingComputer landingComputer;
    [SerializeField] Thruster mainEngine;
    [SerializeField] Thruster[] sideThrusters;

    [Header("Settings")]
    [SerializeField] float unsafeLandingSpeed;
    [SerializeField] bool randomIntialVelocity;
    [SerializeField] bool randomInitialRotation;

    [SerializeField] Vector3 maxRandomInitialVelocity;
    [SerializeField] Vector3 maxRandomInitialRotation;
    [SerializeField] float maxHeightDetection;
    [SerializeField] bool manualControl;

    float currentHeight = -1;

    private void Start()
    {
        rigidbody.centerOfMass = centerOfMass.position;
        if (randomIntialVelocity)
        {
            rigidbody.velocity = new Vector3(Random.Range(-1, 1) * maxRandomInitialVelocity.x, Random.Range(-0.3f, -1f) * maxRandomInitialVelocity.y, Random.Range(-1f, 1f) * maxRandomInitialVelocity.z);
        }

        if (randomInitialRotation)
        {
            rigidbody.angularVelocity = new Vector3(Random.Range(-1, 1) * maxRandomInitialRotation.x, Random.Range(-1, 1) * maxRandomInitialRotation.y, Random.Range(-1, 1) * maxRandomInitialRotation.z);
        }
    }

    private void FixedUpdate()
    {
        UpdateHeight();

        if (manualControl)
        {
            UpdateManualControl();
        }
        else
        {
            UpdateComputer();
        }
    }

    private void UpdateManualControl()
    {
        for (int i = 0; i < sideThrusters.Length; i++)
        {
            if (Input.GetKey((KeyCode)((int)KeyCode.Alpha0 + i)))
            {
                sideThrusters[i].TurnOn();
            }
            else
            {
                sideThrusters[i].TurnOff();
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            mainEngine.TurnOn();
        }
        else
        {
            mainEngine.TurnOff();
        }
    }

    private void UpdateHeight()
    {
        if (Physics.Raycast(raycastOrigin.position, Vector3.down, out RaycastHit hit, maxHeightDetection))
        {
            currentHeight = transform.position.y - hit.point.y;
        }
        else
        {
            currentHeight = -1;
        }
    }

    private void UpdateComputer()
    {
        landingComputer.ExecuteLandingUpdate(rigidbody.velocity, rigidbody.angularVelocity, currentHeight, mainEngine, sideThrusters);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > unsafeLandingSpeed)
        {
            Debug.Log("Explosion due to impact strength:" + collision.relativeVelocity.magnitude);
            Explode();
        }
    }

    private void Explode()
    {
        for (int i = 0; i < elementsParent.childCount; i++)
        {
            var c = elementsParent.GetChild(i);
            c.parent = null;
            var rb = c.gameObject.AddComponent<Rigidbody>();
            rb.velocity = rigidbody.velocity;
            rb.angularVelocity = rigidbody.angularVelocity;
        }
        Destroy(gameObject);
    }
}
