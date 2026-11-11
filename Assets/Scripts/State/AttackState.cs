using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttackState : IState
{
    private AIController aiController;
    private float attackCooldown = 3f; // seconds between attacks 
    private float lastAttackTime = -999f;

    public StateType Type => StateType.Attack;

    public AttackState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        aiController.Agent.isStopped = true;
    }

    public void Execute()
    {
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
            
            int num = Random.Range(0, 2); // Randomly choose between kick and punch

            if (num == 0)
            {
                aiController.aiAnimationController.animator.SetTrigger("doKick");
            }
            else
            {
                aiController.aiAnimationController.animator.SetTrigger("doAttack");
            }
        }
        if (aiController.health <= 0)
        {
            aiController.Die();
        }
    }



    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }
}