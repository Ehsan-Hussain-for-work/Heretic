using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class AIControllerBomber : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    public StateMachine StateMachine { get; private set; }
    public AIAnimationControllerBomber aiAnimationController { get; private set; }
    // public Animator Animator { get; private set; } // Not needed since we're not using animations
    public Transform[] Waypoints;
    public Transform Player;

    public float AttackRange = 2f; // New attack range variable
    public LayerMask PlayerLayer;
    public StateType currentState;

    [Header("Attack Settings")]
    public Collider bombAreaCollider;
    public Collider explosionField;
    public ParticleSystem explosionEffect;
    public float bombTimer = 3f; 
    public Boolean insideRange = false;
    public int hitCount = 0;


    [Header("Vision Settings")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float eyeHeight = 1.6f; // where the AI "looks" from
    public LayerMask obstacleMask;
    public LayerMask playerMask;

    [Header("Vision Stability")]
    public float visionPersistence = 0.5f; // seconds to keep seeing after losing sight
    private float lastSeenTime = -999f;

    [Header("Enemy Stats")]
    public int health = 100;
    public bool isDead = false;
    public int damage = 50;

    // Add State Machine code Here
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            Player = playerObject.transform;
        }

        Agent = GetComponent<NavMeshAgent>();
        aiAnimationController = GetComponent<AIAnimationControllerBomber>();
        // Animator = GetComponent<Animator>(); // Commented out since we're not using animations

                 StateMachine = new StateMachine();
        StateMachine.AddState(new IdleStateBomber(this));
        StateMachine.AddState(new RoamingStateBomber(this));
        StateMachine.AddState(new ChaseStateBomber(this));
        StateMachine.AddState(new AttackStateBomber(this)); 

        StateMachine.TransitionToState(StateType.Idle);
        
        explosionEffect.Pause();
    }

    void Update()
    {
        if (!isDead)
        {
            StateMachine.Update();
            currentState = StateMachine.GetCurrentStateType();
        }

        if (aiAnimationController.IsPlayerInsideRange() && !isDead)
        {
            insideRange = true;
        } 
        else
        {
            insideRange = false;
        }
    }

    private void FixedUpdate()
    {
        if (insideRange && !isDead)
        {
            bombTimer -= Time.fixedDeltaTime;
            if (bombTimer <= 0f)
            {
                aiAnimationController.animator.SetTrigger("doExplosion");
                Agent.isStopped = true;

                explosionEffect.Play();
                Destroy(gameObject, 1.5f); // Destroy the bomber after 1.5 second
            }
        }
        else
        {
            bombTimer = 3f;
        }
    }


    // 
    public bool CanSeePlayer()
    {
        if (Player == null)
        {
            return false;
        }

        Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPosition = Player.position + Vector3.up * 0.5f;
        Vector3 directionToPlayer = (targetPosition - eyePosition).normalized;
        float distanceToPlayer = Vector3.Distance(eyePosition, targetPosition);
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        // Check field of view
        if (angleToPlayer > viewAngle / 2f)
        {
            return Time.time - lastSeenTime < visionPersistence;
        }

        // Check distance
        if (distanceToPlayer > viewDistance)
        {
            return Time.time - lastSeenTime < visionPersistence;
        }

        // Perform raycast
        if (Physics.Raycast(eyePosition, directionToPlayer, out RaycastHit hit, viewDistance))
        {
            // If hit the player
            if (hit.transform == Player)
            {
                lastSeenTime = Time.time;
                return true;
            }
        }
    

        // If recently seen, still count as visible
        bool recentlySeen = Time.time - lastSeenTime < visionPersistence;

        return recentlySeen;
    }


    // New method to check if the AI is within attack range
    public bool IsPlayerInAttackRange()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        return distanceToPlayer <= AttackRange;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2f, 0) * transform.forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewDistance);
    }

    public bool CheckBombExplosionAreaCollision(out Collider collidedObject, string Tag)
    {
        SphereCollider sphere = this.explosionField as SphereCollider;
        Vector3 center = sphere.transform.TransformPoint(sphere.center);
        float radiusBomb = sphere.radius;

        Collider[] hits = Physics.OverlapSphere(center, radiusBomb, PlayerLayer);

        foreach (var hit in hits)
        {

            if (hit.CompareTag(Tag))
            {
                collidedObject = hit;
                return true;
            }
        }

        collidedObject = null;
        return false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        health -= damage;

        Debug.Log("Enemy Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public bool CheckPlayerBomberCollision(out Collider collidedObject, string Tag)
    {

        Collider bombAreaCollider = this.bombAreaCollider;

        // Overlap check
        Collider[] hits = Physics.OverlapSphere(bombAreaCollider.transform.position, 2, PlayerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(Tag))
            {
                collidedObject = hit;
                return true;
            }
        }

        collidedObject = null;
        return false;
    }


    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Agent.isStopped = true;
        Agent.ResetPath();
        Agent.enabled = false;

        aiAnimationController.animator.SetFloat("CharacterSpeed", 0f);
        aiAnimationController.animator.SetTrigger("doDeath");

        Destroy(gameObject, 5f);
    }

}
