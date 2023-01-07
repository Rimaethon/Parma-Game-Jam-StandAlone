using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectHealthController : MonoBehaviour
{
    [Header("General")]
    [SerializeField,ReadOnlyField(true)]private float health = 100;
    [SerializeField,ReadOnlyField(true)]private float maxHealth = 100;
    public float Health { get => health;
        set
        {
            var newValue = Mathf.Clamp(value, 0, MaxHealth);
            if (newValue == health) return;
            OnHealthChanged.Invoke(health, newValue);
            if (newValue>health)
            {
                if (health == 0) OnResurrect.Invoke();
                OnHealthIncreased.Invoke();
            } else
            {
                if (newValue == 0) OnDeath.Invoke();
                OnHealthDecreased.Invoke();
            }
            health = newValue;
        }
    }
    public float MaxHealth { get => maxHealth; set
        {
            var newValue = Mathf.Clamp(value, 0, Mathf.Infinity);
            if (newValue == maxHealth) return;
            OnMaxHealthChanged.Invoke(maxHealth, newValue);
            maxHealth = newValue;
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }
    } 
    public float NormalizedHealth { get => health / maxHealth; }
    public void SetFullHealth() { Health = MaxHealth; }
    [Header("Events")]
    public HealthEvent OnHealthChanged;
    public UnityEvent OnHealthIncreased;
    public UnityEvent OnHealthDecreased;
    public UnityEvent OnDeath;
    public UnityEvent OnResurrect;
    public HealthEvent OnMaxHealthChanged;


    [System.Serializable]
    public class HealthEvent:UnityEvent<float, float>{}
}
