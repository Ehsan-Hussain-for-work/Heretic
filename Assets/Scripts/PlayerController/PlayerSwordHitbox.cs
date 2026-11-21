using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerSwordHitbox : MonoBehaviour
{
    public float cooldown = 1.0f;
    public bool isActive;
    public int damage = 20;
    public PlayerController playerController;

    private bool onCooldown = false;
    Collider collider;

    void Start()
    {
        collider = GetComponent<Collider>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        if ((playerController.SwordActive || playerController.animator.GetBool("swordActive")) && !onCooldown)
        {
            isActive = true;
            collider.enabled = true;
        }
        else
        {
            isActive = false;
            collider.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive || (other is SphereCollider || other is BoxCollider))
        {
            return;
        }

        AIController enemy = other.GetComponentInParent<AIController>();
        AIControllerBomber bomber = other.GetComponentInParent<AIControllerBomber>();
        AIControllerKnight knight = other.GetComponentInParent<AIControllerKnight>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage + Random.Range(2, 10));
            Debug.Log("Enemy hit! Remaining health: " + enemy.health);
        }
        else if (bomber != null)
        {
            bomber.TakeDamage(damage + Random.Range(2, 10));
            Debug.Log("Bomber hit! Remaining health: " + bomber.health);
        }
        else if (knight != null)
        {
            knight.TakeDamage(damage + Random.Range(2, 10));
            Debug.Log("Knight hit! Remaining health: " + knight.health);
        }
        else
        {
            return;
        }

        onCooldown = true;
        isActive = false;
        collider.enabled = false;
        Invoke(nameof(ResetHitbox), cooldown);
    }

    void ResetHitbox()
    {
        onCooldown = false;
    }
}