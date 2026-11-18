using System;
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

    public Boolean HitPlayerBomb() // Attack Animation Event to check if the player his hit 
    {
        Collider objectHit;
        if (aiController.CheckBombExplosionAreaCollision(out objectHit, "Player"))
        {
            Debug.Log("Player hit by the explosion!");
            return true;
        }
        return false;
    }

    public Boolean IsPlayerInsideRange()
    {
        Collider objectHit;
        if (aiController.CheckPlayerBomberCollision(out objectHit, "Player"))
        {
            return true;
        }
        return false;
    }

}
