using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAIBehaviour : MonoBehaviour
{
    [SerializeField] float stunDuration, tauntDuration;
    [SerializeField] bool targetInSight;
    [SerializeField] Sprite rollSprite, idleSprite, tauntSprite;
    [SerializeField] SpriteRenderer renderer;


    private StateMachine stateMachine;
    private float hitTimestamp = -10000;
    private float stunTimestamp = -10000;
    private float tauntTimestamp = -10000;

    private void Start()
    {
        SetupStateMachine();
    }

    private void SetupStateMachine()
    {
        stateMachine = new StateMachine();
        State search = stateMachine.AddState("Search", SearchStart);
        State roll = stateMachine.AddState("Roll", RollStart, RollUpdate);
        State stunned = stateMachine.AddState("Stunned", StunnedStart, null, StunnedEnd);
        State taunted = stateMachine.AddState("Taunted", TauntedStart);

        search.AddTransition(TargetInSight, roll);
        roll.AddTransition(HasHitRecenty, stunned);
        stunned.AddTransition(StunFinished, taunted);
        taunted.AddTransition(TauntFinished, search);

        stateMachine.Start();
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.Label(transform.position + Vector3.up, stateMachine.GetCurrentStateName());
    }

    //States

    private void TauntedStart()
    {
        renderer.sprite = tauntSprite;
        tauntTimestamp = Time.time;
    }

    private void SearchStart()
    {
        renderer.sprite = idleSprite;
        //play animation
    }

    private void RollStart()
    {
        renderer.sprite = rollSprite;
    }

    private void RollUpdate()
    {
        transform.position += transform.right * transform.localScale.x * Time.deltaTime;
    }

    private void StunnedStart()
    {
        stunTimestamp = Time.time;
    }

    private void StunnedEnd()
    {
        //Flip
        Vector3 scale = transform.localScale;
        scale.x = -scale.x;
        transform.localScale = scale;
    }

    //Transitions

    private bool TargetInSight()
    {
        return targetInSight;
    }

    private bool StunFinished()
    {
        return Time.time - stunTimestamp > stunDuration;
    }

    private bool HasHitRecenty()
    {
        return Time.time - hitTimestamp < 0.1f;
    }

    private bool TauntFinished()
    {
        return Time.time - tauntTimestamp > tauntDuration;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hitTimestamp = Time.time;
    }
}
