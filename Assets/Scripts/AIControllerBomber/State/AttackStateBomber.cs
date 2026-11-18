using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttackStateBomber : IState
{
    private AIControllerBomber aiController;
    private float attackCooldown = 3f; // seconds between attacks 
    private float lastAttackTime = -999f;

    public StateType Type => StateType.Attack;

    public AttackStateBomber(AIControllerBomber aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        aiController.Agent.isStopped = true;
    }

    public void Execute()
    {
        if (aiController.health <= 0)
        {
            aiController.Die();
        }

        // If player moves out of attack range, go back to chase 
        if (Vector3.Distance(aiController.transform.position, aiController.Player.position) > aiController.AttackRange)
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }
    }



    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}