using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingComputer : MonoBehaviour
{
    [SerializeField] float heightDetectorError;
    [SerializeField] float engineEnergyLossPercent = 0.05f; 

    float[] rcsThrustersThrottle;

    public void ExecuteLandingUpdate(Vector3 velocity, Vector3 angularVelocity, float heightFromGround, Thruster mainEngine, Thruster[] rcsThrusters)
    {
        rcsThrustersThrottle = new float[rcsThrusters.Length];

        UpdateCancelYawRotation(angularVelocity);
        UpdateCancelSideVelocity(velocity, rcsThrusters);
        NaiveUpdateCancelVerticalVelocity(velocity, heightFromGround, mainEngine);
        UpdateThrusterThrottle(rcsThrusters);
    }

    private void UpdateCancelSideVelocity(Vector3 velocity, Thruster[] rcsThrusters)
    {
        var flatVelocity = velocity;
        flatVelocity.y = 0;

        for (int i = 0; i < rcsThrusters.Length; i++)
        {
            float dot = Vector3.Dot(rcsThrusters[i].GetDirection(), flatVelocity);

            if (dot < -0.5f)
            {
                rcsThrustersThrottle[i] = 1;
            }
        }

    }

    private void UpdateThrusterThrottle(Thruster[] rcsThrusters)
    {
        for (int i = 0; i < rcsThrusters.Length; i++)
        {
            rcsThrusters[i].SetThrottle(rcsThrustersThrottle[i]);
        }
    }

    private void UpdateCancelYawRotation(Vector3 angularVelocity)
    {
        float yRot = angularVelocity.y;
        float strength = Mathf.Clamp01(Mathf.Abs(yRot * 0.1f));

        if (yRot < 0)
        {
            for (int i = 0; i < 4; i++)
            {
                rcsThrustersThrottle[i] = strength;
            }
        }
        else
        {
            for (int i = 4; i < 8; i++)
            {
                rcsThrustersThrottle[i] = strength;
            }
        }
    }

    private void NaiveUpdateCancelVerticalVelocity(Vector3 velocity, float heightFromGround, Thruster mainEngine)
    {
        if (heightFromGround < 0)
            return;

        heightFromGround += heightDetectorError;

        float yVelocity = -velocity.y;

        float burnDuration = yVelocity / (mainEngine.GetMaxImpulseStrength() + Physics.gravity.y);
        float timeToGroundHit = heightFromGround / yVelocity;

        Debug.Log($"Time to ground: {timeToGroundHit} BurnDuration: {burnDuration}");

        if (timeToGroundHit < burnDuration)
        {
            mainEngine.SetThrottle(1);
        }
        else
        {
            mainEngine.SetThrottle(0);
        }

        if (heightFromGround < 1 && yVelocity < 0)
        {
            mainEngine.Shutdown();
        }
    }

    private void CompleteUpdateCancelVerticalVelocity(Vector3 velocity, float heightFromGround, Thruster mainEngine)
    {
        if (heightFromGround < 0)
            return;

        // Acceleration is caused by the sum of gravity and the engine strength, so:
        // h''(t) = g + e; 
        // h'(t) = v0 + gt + et;                        (Integral from h'')
        // h(t) = h0 + v0*t + 0.5*g*t*t + 0.5*e*t*t     (Integral from h')
        // where:
        // h0 = start height
        // v0 = start vertical velocity
        // g = gravity
        // e = main engine impule strength

        //To solve we need:
        // Solve for h0 => At what height do we need to start burning to land perfectly?
        // h(t) = 0 (We have landed)
        // h'(t) = 0 (We have 0 velocity)

        //Calculation:
        // 0 = v0 + gt + et  /-v0
        // -v0 = gt + et     / format
        // -v0 = (g+e)*t     / /(g+e)
        // -v0/(g+e) = t
        // so the burn duration (b) is equal to: b == t == -v0/(g+e)

        //insert into h(t) = 0
        // 0 = h0 + v0*b + 0.5*g*b*b + 0.5*e*t*t    /-h0 /*-1
        // h0 = -v0b -0.5*gbb -0.5*ebb;

        float correctedHeight = heightFromGround + heightDetectorError;

        float v0 = velocity.y;
        float e = mainEngine.GetMaxImpulseStrength() * (1- engineEnergyLossPercent); //engine seems to not be applying the force 100% so we use a slightly reduced engine impulse
        float g = Physics.gravity.y;

        float b = -v0 / (g + e);
        float h0 = -v0 * b - 0.5f * g * b * b - 0.5f * e * b * b;
        Debug.Log($"{Time.time} : Height: {heightFromGround}  CorrectedHeight: {correctedHeight}, Height to start burn is: {h0}");

        //Turn on engine if we should burn, otherwise turn off
        if (correctedHeight <= h0)
        {
            mainEngine.SetThrottle(1);
        }
        else
        {
            mainEngine.SetThrottle(0);
        }

        //if we have landed, shutdown the engine (This is helpfull in case we reach v=0 slighly above the ground
        if (correctedHeight < 1 && v0 > 0)
        {
            mainEngine.Shutdown();
        }
    }


}