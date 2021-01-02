using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float initialHealth = 100;

    public UnityEvent<float> OnHealed;
    public UnityEvent<float> OnDamaged;
    public UnityEvent OnDeath;


    private float _health = 100;

    private void Awake()
    {
        _health = initialHealth;
    }

    public void Damage(float amount)
    {
        if (amount < 0)
        {
            Heal(-amount);
            return;
        }
        _health -= amount;
        if(_health <= 0)
        {
            _health = 0;
            OnDeath.Invoke();
        }
        OnDamaged.Invoke(amount);
    }

    public void Heal(float amount)
    {
        if(amount < 0)
        {
            Damage(-amount);
            return;
        }
        _health += amount;
        _health = Mathf.Clamp(_health, 0, initialHealth);
        OnHealed.Invoke(amount);
    }
}
