public class ChaseState : IState
{
    private AIController aiController;

    public StateType Type => StateType.Chase;

    public ChaseState(AIController aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        // No animations, so no need to set any animator parameters 
    }

    public void Execute()
    {
        if (!aiController.CanSeePlayer())
        {
            aiController.StateMachine.TransitionToState(StateType.Roaming);
            return;
        }

        if (aiController.IsPlayerInAttackRange())
        {
            aiController.StateMachine.TransitionToState(StateType.Attack);
            return;
        }

        aiController.Agent.destination = aiController.Player.position;

        if (aiController.health <= 0)
        {
            aiController.Die();
        }
    }

    public void Exit()
    {
        // No cleanup necessary 
    }
}
