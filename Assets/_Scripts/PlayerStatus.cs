using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    int health, stamina, maxHealth = 100, maxStamina;
    [SerializeField] GameObject deathScreen;

    // Start is called before the first frame update
    void Start()
    {
        deathScreen.SetActive(false);
        health = maxHealth;
        stamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        health = Mathf.Clamp(health, 0, maxHealth);
        if (health <= 0)
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyWeapon"))
        {
            TakeDamage(10);
        }
    }

    void Die()
    {
        Time.timeScale = 0;
        deathScreen.SetActive(true);
    }
}
