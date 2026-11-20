using Unity.VisualScripting;
using UnityEngine;

public class PlayerSwordHitbox : MonoBehaviour
{
    public float cooldown = 1.0f;
    public bool isActive = true;
    public int damage = 20;
    Collider collider;
    void Start()
    {
        collider = GetComponent<Collider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            return;
        }
        if (other.CompareTag("Enemy"))
        {
            AIController enemy = GetComponentInParent<AIController>();
            AIControllerBomber bomber = GetComponentInParent<AIControllerBomber>();
            AIControllerKnight knight = GetComponentInParent<AIControllerKnight>();

            Debug.Log("Hitbox triggered by: " + other.name);

            if (enemy != null)
            {
                enemy.TakeDamage(damage + Random.Range(2, 10));
                Debug.Log("Enemy hit! Remaining health: " + enemy.health);
            }
            else if (bomber != null)
            {
                enemy.TakeDamage(damage + Random.Range(2, 10));
                Debug.Log("Bomber hit! Remaining health: " + bomber.health);
            }
            else if (knight != null)
            {
                enemy.TakeDamage(damage + Random.Range(2, 10));
                Debug.Log("Knight hit! Remaining health: " + knight.health);
            }

            isActive = false;
            collider.enabled = (false);
            Invoke(nameof(ResetHitbox), cooldown);
        }
    }

    void ResetHitbox()
    {
        isActive = true;
        collider.enabled = true;

    }
}
