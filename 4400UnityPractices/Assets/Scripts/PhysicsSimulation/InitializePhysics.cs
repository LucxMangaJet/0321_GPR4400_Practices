using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializePhysics
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void OnInitialize()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private static void OnSceneChanged(Scene arg0, Scene arg1)
    {
        Debug.Log("Scene loaded: Setting up PhysicsHandler.");
        GameObject gameObject = new GameObject("__PhysicsHandler");
        gameObject.AddComponent<PhysicsHandler>();
        gameObject.hideFlags = HideFlags.NotEditable | HideFlags.DontSave;
    }
}
