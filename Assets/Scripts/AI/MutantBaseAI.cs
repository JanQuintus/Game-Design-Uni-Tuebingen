using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class MutantBaseAI : BaseAI
{
    [SerializeField] private float findPlayerInterval = 0.5f;
    [SerializeField] private Transform headPosition;
    [SerializeField] private float maxFindPlayerDistance = 30f;
    [SerializeField] private LayerMask obstacleLayerMask;

    private Health _health;

    /*protected override void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDeath.AddListener(() =>
        {
            isAlive = false;
        });
    }*/

    void Start()
    {
        StartCoroutine(TryAttackPlayerCor());
    }

    private IEnumerator TryAttackPlayerCor()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(findPlayerInterval);
            float distance = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distance <= maxFindPlayerDistance)
            {
                if (!Physics.Raycast(headPosition.position, PlayerController.Instance.GetHeadPosition(), distance, obstacleLayerMask))
                    MoveTo(PlayerController.Instance.transform.position);
            }
        }
    }

    public Health GetHealth => _health;
}
