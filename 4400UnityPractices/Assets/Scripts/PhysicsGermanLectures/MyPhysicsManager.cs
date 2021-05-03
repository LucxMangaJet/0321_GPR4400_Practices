using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MyPhysicsManager : MonoBehaviour
{
    public static MyPhysicsManager Instance { get; private set; }

    private List<MyRigidbody> m_activeRigidbodies = new List<MyRigidbody>();
    private List<MyRigidbody> m_toRemove = new List<MyRigidbody>();

    private bool m_isRunning = true;
    private void Awake()
    {

        if(Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_isRunning = !m_isRunning;
        }
    }
    private void FixedUpdate()
    {
        if (!m_isRunning)
            return;

        foreach (MyRigidbody rigidbody in m_activeRigidbodies)
        {
            rigidbody.Step(Time.fixedDeltaTime);
        }

        for (int i = 0; i < m_activeRigidbodies.Count; i++)
        {
            for(int j = i + 1; j < m_activeRigidbodies.Count; j++)
            {
                m_activeRigidbodies[i].CheckForCollision(m_activeRigidbodies[j]);
            }
        }

        foreach(MyRigidbody rigidbody in m_activeRigidbodies)
        {
            rigidbody.Draw();
        }

        foreach (MyRigidbody rigidbody in m_toRemove)
        {
            m_activeRigidbodies.Remove(rigidbody);
        }
        m_toRemove.Clear();
    }
    public void AddRigidbody(MyRigidbody _rigid)
    {
        if (_rigid == null)
            return;
        if (m_activeRigidbodies.Contains(_rigid))
            return;

        m_activeRigidbodies.Add(_rigid);
    }

    public void RemoveRigidbody(MyRigidbody _rigid)
    {
        if (_rigid == null)
            return;
        if (m_toRemove.Contains(_rigid))
            return;
        m_toRemove.Add(_rigid);
    }
}
