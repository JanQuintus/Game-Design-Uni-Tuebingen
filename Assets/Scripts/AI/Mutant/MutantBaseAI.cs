using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MutantBaseAI : BaseAI
{
    [Header("Mutant AI")]
    [SerializeField] protected float attackTime = 1;
    [SerializeField] protected int attacks = 3;
    [SerializeField] protected RagdollController ragdollController;


    private float maxPlayerDistance = 30f;
    private Health _health;
    private float _attackTimeCD;

    private float _mpd2;
    private int _aiLayer;


    protected override void Awake()
    {
        base.Awake();
        _aiLayer = LayerMask.NameToLayer("AI");
        _mpd2 = maxPlayerDistance * maxPlayerDistance;
        _health = GetComponent<Health>();
        _health.OnDamaged.AddListener((dmg) =>
        {
            if (_isAlive)
            {
                animator.SetTrigger("Hit");
            }
        });
        _health.OnDeath.AddListener(() =>
        {
            _isAlive = false;
            if (_target != null)
                _target.Untrack();
            mainCollider.material.staticFriction = 1f;
            mainCollider.material.dynamicFriction = 1f;
            mainCollider.material.frictionCombine = PhysicMaterialCombine.Average;
            animator.enabled = false;
            _rb.isKinematic = true;
            ragdollController.EnableRagdoll(mainCollider.transform);
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

        if (_atTarget)
        {
            _attackTimeCD -= Time.deltaTime;
            if(_attackTimeCD <= 0)
            {
                _attackTimeCD = attackTime;
                _canMove = false;
                animator.SetInteger("AttackIndex", Random.Range(0, attacks));
                animator.SetTrigger("Attack");
            }
        }
        else
        {
            if(!_canMove && _attackTimeCD > 0)
            {
                _attackTimeCD -= Time.deltaTime;
                if (_attackTimeCD <= 0)
                    _canMove = true;
            }
            else
            {
                _attackTimeCD = .5f;
                animator.ResetTrigger("Attack");
            }
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!_isAlive)
            return;

        if (collision.collider.gameObject.layer == _aiLayer)
            return;

        if (collision.relativeVelocity.magnitude < 15f)
            return;
        float mass = collision.rigidbody ? collision.rigidbody.mass : 1f;
        if (collision.gameObject.isStatic)
            mass = 50;
        _health.Damage(collision.relativeVelocity.magnitude * mass);
    }

    public Health GetHealth => _health;
}
