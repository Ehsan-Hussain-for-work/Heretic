public class ChaseStateKnight : IState
{
    private AIControllerKnight aiController;

    public StateType Type => StateType.Chase;

    public ChaseStateKnight(AIControllerKnight aiController)
    {
        this.aiController = aiController;
    }

    public void Enter()
    {
        // No animations, so no need to set any animator parameters 
    }

    public void Execute()
    {
        if (aiController.health <= 0)
        {
            aiController.Die();
        }

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
    }

    public void Exit()
    {
        // No cleanup necessary 
    }
}
