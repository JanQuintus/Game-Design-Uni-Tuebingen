using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MutantBaseAI : BaseAI
{
    [Header("Mutant AI")]
    [SerializeField] protected float attackTime = 1;
    [SerializeField] protected int attacks = 3;
    [SerializeField] protected RagdollController ragdollController;

    [Header("Mutant Audio")]
    [SerializeField] private AudioClipBundle damageClipBundle;
    [SerializeField] private AudioClipBundle deathClipBundle;
    [SerializeField] private AudioClipBundle growlClipBundle;

    private float maxPlayerDistance = 30f;
    private Health _health;
    private float _attackTimeCD;

    private float _mpd2;
    private int _aiLayer;
    private float _nextGrowl;


    protected override void Awake()
    {
        base.Awake();
        _aiLayer = LayerMask.NameToLayer("AI");
        _mpd2 = maxPlayerDistance * maxPlayerDistance;
        _nextGrowl = Random.Range(3f, 10f);
        _health = GetComponent<Health>();
        _health.OnDamaged.AddListener((damage) =>
        {
            animator.SetTrigger("Hit");
            if(_health.GetHealth() <= 0)
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(deathClipBundle.GetRandomClip());
            }
            else
            {
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(damageClipBundle.GetRandomClip(), Mathf.Clamp01(damage / 10f));
            }
        });
        _health.OnDeath.AddListener(() =>
        {
            if (_target != null)
                _target.Untrack(this);
            mainCollider.material.staticFriction = 1f;
            mainCollider.material.dynamicFriction = 1f;
            mainCollider.material.frictionCombine = PhysicMaterialCombine.Average;
            animator.enabled = false;
            _rb.isKinematic = true;
            _gravity.SetRB(ragdollController.GetHipsRB());
            ragdollController.EnableRagdoll(mainCollider.transform, _rb);
            enabled = false;
        });
    }

    protected void Update()
    {
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
                audioSource.PlayOneShot(growlClipBundle.GetRandomClip(), 2f);
            }
        }
        else
        {        
            _nextGrowl -= Time.deltaTime;
            if(_nextGrowl <= 0f)
            {
                audioSource.PlayOneShot(growlClipBundle.GetRandomClip());
                _nextGrowl = Random.Range(3f, 10f);
            }
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
        if (collision.collider.gameObject.layer == _aiLayer)
            return;

        if (collision.relativeVelocity.magnitude < 8f)
            return;
        float mass = collision.rigidbody ? collision.rigidbody.mass : 1f;
        if (collision.gameObject.isStatic)
            mass = 50;
        _health.Damage(collision.relativeVelocity.magnitude * mass);
    }

    public Health GetHealth => _health;
}
