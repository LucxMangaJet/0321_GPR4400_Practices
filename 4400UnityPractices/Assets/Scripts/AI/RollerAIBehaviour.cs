using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollerAIBehaviour : MonoBehaviour
{
    [SerializeField] float stunDuration, tauntDuration;
    [SerializeField] float rollSpeed;
    [SerializeField] string rollAnimationState, idleAnimationState, stunnedAnimationState, tauntedAnimationState;
    [SerializeField] Animator animator;

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        UnityEditor.Handles.color = Color.black;
        UnityEditor.Handles.Label(transform.position + Vector3.up, stateMachine.GetCurrentStateName());
        Gizmos.DrawRay(transform.position, GetMoveDirection() * 10);
    }
#endif

    private Vector3 GetMoveDirection()
    {
        return transform.right * transform.localScale.x;
    }

    //States

    private void TauntedStart()
    {
        tauntTimestamp = Time.time;
        animator.Play(tauntedAnimationState);
    }

    private void SearchStart()
    {
        animator.Play(idleAnimationState);
    }

    private void RollStart()
    {
        animator.Play(rollAnimationState);
    }

    private void RollUpdate()
    {
        transform.position += GetMoveDirection() * rollSpeed * Time.deltaTime;
    }

    private void StunnedStart()
    {
        stunTimestamp = Time.time;
        animator.Play(stunnedAnimationState);
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
        var hits = Physics2D.RaycastAll(transform.position, GetMoveDirection());
        foreach (var hit in hits)
        {
            if (hit.transform.TryGetComponent(out EnemyMarker marker))
            {
                return true;
            }
        }
        return false;
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
        //Deal damage?
    }
}
