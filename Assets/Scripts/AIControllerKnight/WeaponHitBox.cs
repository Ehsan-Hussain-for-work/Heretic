using System;
using UnityEngine;

public class WeaponHitBox : MonoBehaviour
{
    public float cooldown = 1.0f;
    public bool isActive = true;
    Collider collider;
    void Start()
    {
        collider =GetComponent<Collider>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit by weapon!");
            // Here you can add code to apply damage to the player
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
