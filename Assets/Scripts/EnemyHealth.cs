using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;

    public GameObject deathParticlesPrefab;

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log(gameObject.name + " took " + damage + " damage! Health is now " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        if (levelManager != null)
        {
            levelManager.OnEnemyKilled(transform.position);
        }

        if (deathParticlesPrefab != null)
        {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}