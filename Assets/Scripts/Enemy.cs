using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float damageThreshold =0.2f;
    [SerializeField] private GameObject enemyDeadPartical;
    [SerializeField] private AudioClip deathClip;
    private float currentHealth;
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0 )
        {
            Die();
        }
    }
    private void Die()
    {
        GameManager.instance.RemoveEnemy( this );
        Instantiate(enemyDeadPartical,transform.position, Quaternion.identity );
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
        Destroy(gameObject);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactVelocity = collision.relativeVelocity.magnitude; //return int
        if(impactVelocity > damageThreshold)
        {
            TakeDamage(impactVelocity);
        }
    }
}
