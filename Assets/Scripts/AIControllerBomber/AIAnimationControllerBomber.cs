using UnityEngine; 
using UnityEngine.AI; 
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class AIAnimationControllerBomber : MonoBehaviour
{
    public Animator animator { get; private set; }
    private AIControllerBomber aiController;
    private NavMeshAgent agent;


    void Awake()
    {
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIControllerBomber>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // Update animation parameters 
        UpdateAnimations();
    }

    void UpdateAnimations()
    {
        float speed = agent != null ? agent.velocity.magnitude : 0f;
        animator.SetFloat("CharacterSpeed", speed);
    }

}
