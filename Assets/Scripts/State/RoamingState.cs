using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Roamingstate : IState
{
    private AIController aiController;
    private bool isWaiting = false;

    public StateType Type => StateType.Roaming;

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

        if (aiController.health <= 0)
        {
            aiController.Die();
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

        // Wait for animation duration (1.5 sec here, adjust to your animation length) 
        yield return new WaitForSeconds(3);
        

        aiController.Agent.isStopped = false;
        MoveToNextWaypoint();
        isWaiting = false;
    }

    private void MoveToNextWaypoint()
    {
        NavMeshAgent agent = aiController.Agent;

            Vector3 randomPosition = agent.transform.position +
                new Vector3(Random.Range(-10f, 10f), 0, Random.Range(-10f, 10f));

            NavMeshHit hit; // the position we will have to check if the enemy can reach it (navmesh related)

        if (NavMesh.SamplePosition(randomPosition, out hit, 2f, agent.areaMask)) //check whether the random position can be reached by the enemy (navmesh related)
        {
                NavMeshPath path = new NavMeshPath();

                // Check whether the enemy can reach that destination
                if (agent.CalculatePath(hit.position, path) && path.status == NavMeshPathStatus.PathComplete) // calculate path checks whether the enemy can even reach the destination, and makes sure the path is complete (not partial) 
            {
                    agent.SetDestination(hit.position);
                    return;
                }
            
        }
    }
}