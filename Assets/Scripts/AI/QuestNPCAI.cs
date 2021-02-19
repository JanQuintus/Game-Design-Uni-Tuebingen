using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestNPCAI : BaseAI
{
    [SerializeField] private AITarget aITarget;

    protected override void Awake()
    {
        base.Awake();
        SetTarget(aITarget);
    }
}
