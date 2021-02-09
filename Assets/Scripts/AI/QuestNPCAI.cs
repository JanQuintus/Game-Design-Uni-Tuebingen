using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPCAI : BaseAI
{
    [SerializeField] private AITarget aITarget;
    [SerializeField] private AInteractive interactive;

    protected override void Awake()
    {
        base.Awake();
        SetTarget(aITarget);

        interactive.OnInteractionStart.AddListener(() => {
            _canMove = false;
            _lookPosition = PlayerController.Instance.transform.position;
        });
        interactive.OnInteractionEnd.AddListener(() => _canMove = true);
    }
}
