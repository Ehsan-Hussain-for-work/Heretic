using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttackStateKnight : IState
{
    private AIControllerKnight aiController;
    private float attackCooldown = 3f; // seconds between attacks 
    private float lastAttackTime = -999f;

    public StateType Type => StateType.Attack;

    public AttackStateKnight(AIControllerKnight aiController)
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

        // Limit how often the AI attacks 
        if (Time.time - lastAttackTime > attackCooldown)
        {
            lastAttackTime = Time.time;
            
            aiController.aiAnimationController.animator.SetTrigger("doAttack");  
        }
    }



    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}