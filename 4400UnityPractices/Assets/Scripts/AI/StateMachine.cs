using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private List<State> states = new List<State>();
    private State current = null;

    public void Start()
    {
        if (states.Count == 0)
            Debug.LogError("Empty statemachine defined.");

        current = states[0];
    }

    public void Update()
    {
        if (current == null)
            return;

        current.OnUpdate?.Invoke();

        foreach (var transition in current.Transitions)
        {
            //Should we transition?
            if (transition.Condition != null && transition.Condition.Invoke())
            {
                ChangeStateTo(transition.Target);
                break;
            }
        }

    }

    private void ChangeStateTo(State target)
    {
        current.OnExit?.Invoke();

        current = target;

        target.OnEnter?.Invoke();
    }

    public State AddState(string stateName, System.Action enter, System.Action update = null, System.Action exit = null)
    {
        State newState = new State(stateName, enter, update, exit);
        states.Add(newState);
        return newState;
    }

    public string GetCurrentStateName()
    {
        return current != null ? current.Name : "Nothing";
    }
}

public class State
{
    public string Name;
    public System.Action OnEnter;
    public System.Action OnUpdate;
    public System.Action OnExit; 

    List<Transition> transitions = new List<Transition>();

    public List<Transition> Transitions { get => transitions; }

    public State(string name, System.Action enter, System.Action update = null, System.Action exit = null)
    {
        this.Name = name;
        OnEnter = enter;
        OnUpdate = update;
        OnExit = exit;
    }

    public void AddTransition(System.Func<bool> condition, State target)
    {
        transitions.Add(new Transition(condition, target));
    }

}

public class Transition
{
    public readonly State Target;
    public readonly System.Func<bool> Condition;
    public delegate bool FuncDelegate();


    public Transition(System.Func<bool> condition, State target)
    {
        this.Target = target;
        this.Condition = condition;
    }
}
