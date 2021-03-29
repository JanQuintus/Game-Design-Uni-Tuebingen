using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, ISaveable
{
    [SerializeField] private float initialHealth = 100;

    [Header("Regenration")]
    [SerializeField] private bool canRegenerate = false;
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private float regenAmount = 0.25f;
    [SerializeField] private float regenDamageDelay = 2f;

    public UnityEvent<float> OnHealed;
    public UnityEvent<float> OnDamaged;
    public UnityEvent OnDeath;

    private float _regenTime = 0f;
    private float _lastDamage = 0f;
    private float _health = 100;

    private void Awake()
    {
        _health = initialHealth;
    }

    private void Update()
    {
        if(canRegenerate && _health > 0)
        {
            if (_lastDamage <= Time.time - regenDamageDelay)
                _regenTime += Time.deltaTime;
            if (_health < initialHealth && _regenTime >= regenInterval)
            {
                _regenTime = 0;
                Heal(Mathf.Min(1f, initialHealth - _health));
            }
        }
    }

    public void Damage(float amount)
    {
        if (amount < 0)
        {
            Heal(-amount);
            return;
        }
        if (_health <= 0)
            return;
        _health -= amount;
        _lastDamage = Time.time;
        _regenTime = 0;
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

    public void Revive()
    {
        if (_health > 0)
            return;
        _health = initialHealth;
        StopAllCoroutines();
        OnHealed.Invoke(initialHealth);
    }

    public float GetHealth() => _health;
    public float GetInitialHealth() => initialHealth;
    public float GetHealthPercentage() => _health / initialHealth;

    public object CaptureState()
    {
        return _health;
    }

    public void RestoreState(object state)
    {
        _health = (float)state;
        if(_health <= 0)
        {
            _health = 0;
            OnDeath.Invoke();
        }
    }
}
