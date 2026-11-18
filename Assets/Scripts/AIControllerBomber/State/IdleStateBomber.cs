using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleStateBomber : IState
{
    private AIControllerBomber aiController;
    private float idleDuration = 2f;
    private float idleTimer;

    public StateType Type => StateType.Idle;

    public IdleStateBomber(AIControllerBomber aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        idleTimer = 0f;
        //aiController.Animator.SetBool("isMoving", false); 
    }

    public void Execute()
    {
        if (aiController.health <= 0)
        {
            aiController.Die();
        }

        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            aiController.StateMachine.TransitionToState(StateType.Roaming);
        }
    }

    public void Exit()
    {
        // Cleanup if necessary 
    }
}
