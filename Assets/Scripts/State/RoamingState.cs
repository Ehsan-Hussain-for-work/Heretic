using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Roamingstate : IState
{
    private AIController aiController;
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    public StateType Type => StateType.Patrol;

    public Roamingstate(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {

        aiController.Agent.isStopped = false;
        MoveToNextWaypoint();
    }

    public void Execute()
    {
        if (aiController.CanSeePlayer())
        {
            aiController.StateMachine.TransitionToState(StateType.Chase);
            return;
        }

        if (!isWaiting && !aiController.Agent.pathPending &&
aiController.Agent.remainingDistance <= aiController.Agent.stoppingDistance)
        {
            aiController.StartCoroutine(WaitAndAnimate());
        }

    }

    public void Exit()
    {
        aiController.Agent.isStopped = false;
    }

    private IEnumerator WaitAndAnimate()
    {
        isWaiting = true;
        aiController.Agent.isStopped = true;

        // Play patrol point animation 
        aiController.aiAnimationController.animator.SetTrigger("doScream");
        // Wait for animation duration (1.5 sec here, adjust to your animation length) 
        yield return new WaitForSeconds(5);

        aiController.Agent.isStopped = false;
        MoveToNextWaypoint();
        isWaiting = false;
    }

    private void MoveToNextWaypoint()
    {
            aiController.Agent.destination = aiController.Agent.transform.position +  new Vector3(Random.Range(-5,5),0,Random.Range(-5,5));
    }
}