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

    [Header("General Settings")]
    [SerializeField] float unsafeLandingSpeed;
    [SerializeField] Vector3 maxRandomInitialVelocity;
    [SerializeField] Vector3 maxRandomInitialRotation;
    [SerializeField] float maxHeightDetection;

    [Header("Testing Settings")]
    [SerializeField] bool randomIntialSideVelocity;
    [SerializeField] bool randomInitialRotation;
    [SerializeField] bool manualControl;

    float currentHeight = -1;
    bool landed = false;

    private void Start()
    {
        rigidbody.centerOfMass = centerOfMass.position;
        float x = randomIntialSideVelocity ? Random.Range(-1, 1) * maxRandomInitialVelocity.x : 0;
        float z = randomIntialSideVelocity ? Random.Range(-1f, 1f) * maxRandomInitialVelocity.z : 0;
        rigidbody.velocity = new Vector3(x, Random.Range(-0.3f, -1f) * maxRandomInitialVelocity.y, z);

        if (randomInitialRotation)
        {
            float yRot = Random.value > 0.5f ? 1 : -1 * Random.Range(0.3f, 1f) * maxRandomInitialRotation.y;
            rigidbody.angularVelocity = new Vector3(0, yRot, 0);
        }
    }

    private void FixedUpdate()
    {
        UpdateHeight();

        if (manualControl || landed)
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
                sideThrusters[i].SetThrottle(1);
            }
            else
            {
                sideThrusters[i].SetThrottle(0);
            }
        }

        if (Input.GetKey(KeyCode.Space))
        {
            mainEngine.SetThrottle(1);
        }
        else
        {
            mainEngine.SetThrottle(0);
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
        else
        {
            Debug.Log("Huston we have landed!");

            mainEngine.Shutdown();
            foreach (var thruster in sideThrusters)
            {
                thruster.Shutdown();
            }
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
