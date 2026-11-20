using UnityEngine; 
using UnityEngine.AI; 
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class AIAnimationControllerKnight : MonoBehaviour
{
    public Animator animator { get; private set; }
    private AIControllerKnight aiController;
    private NavMeshAgent agent;


    void Awake()
    {
        animator = GetComponent<Animator>();
        aiController = GetComponent<AIControllerKnight>();
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
