using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAIEnumBehaviour : MonoBehaviour
{
    public enum States { Search, Roll, Stunned, Taunt}

    private States currentState;

    private void Start()
    {
        ChangeStateTo(States.Search);   
    }

    private void Update()
    {
        UpdateState();
        CheckforTransition();
    }

    private void ChangeStateTo(States newState)
    {
        LeaveState(currentState);
        currentState = newState;
        EnterState(newState);
    }

    private void EnterState(States state)
    {
        switch (currentState)
        {
            case States.Stunned:
                //Play stunned animation
                break;

            case States.Roll:
                //Play roll animation
                break;
        }
    }

    private void LeaveState(States state)
    {
        switch (currentState)
        {
            case States.Stunned:
                //Turn around
                break;
        }
    }

    private void UpdateState()
    {

        switch (currentState)
        {
            case States.Search:
                // Raycasting, look for enemies
                break;
            case States.Roll:
                //Roll! Move forward
                break;

            case States.Stunned:
                break;

            case States.Taunt:
                break;
        }
    }

    private void CheckforTransition()
    {
        switch (currentState)
        {
            case States.Search:
                if (EnemyInSight())
                {
                    ChangeStateTo(States.Roll);
                }
                break;
            case States.Roll:
                if(DidHitSomething())
                {
                    ChangeStateTo(States.Stunned);
                }
                break;

            case States.Stunned:
                if (FinishedStun())
                {
                    ChangeStateTo(States.Taunt);
                }
                break;
            case States.Taunt:
                //
                break;
        }

    }

    private bool FinishedStun()
    {
        throw new NotImplementedException();
    }

    private bool DidHitSomething()
    {
        throw new NotImplementedException();
    }

    private bool EnemyInSight()
    {
        throw new NotImplementedException();
    }
}
