using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MutantBaseAI : BaseAI
{
    private float maxPlayerDistance = 30f;
    private Health _health;

    private float _mpd2;

    protected override void Awake()
    {
        base.Awake();
        _mpd2 = maxPlayerDistance * maxPlayerDistance;
        _health = GetComponent<Health>();
        _health.OnDeath.AddListener(() =>
        {
            _isAlive = false;
            if (_target != null)
                _target.Untrack();
            _collider.material.staticFriction = 1f;
            _collider.material.dynamicFriction = 1f;
            _collider.material.frictionCombine = PhysicMaterialCombine.Average;
        });
    }

    protected void Update()
    {
        if (!_isAlive)
            return;

        if (_target == null && Vector3.SqrMagnitude(transform.position - PlayerController.Instance.transform.position) <= _mpd2)
            SetTarget(PlayerController.Instance.GetAITarget());
        else if (_target != null && Vector3.SqrMagnitude(transform.position - PlayerController.Instance.transform.position) > _mpd2)
            SetTarget(null);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!_isAlive)
            return;

        if (collision.collider.gameObject.layer == LayerMask.GetMask("AI"))
            return;

        if (collision.relativeVelocity.magnitude < 20f)
            return;
        float mass = collision.rigidbody ? collision.rigidbody.mass : 1f;
        _health.Damage((collision.relativeVelocity.magnitude - 20f) * mass);
    }

    public Health GetHealth => _health;
}
