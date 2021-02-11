using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AITarget : MonoBehaviour
{
    public abstract void Track(BaseAI ai);
    public abstract void Untrack(BaseAI ai);

    public abstract (Vector3, Vector3) GetNextWaypoint(BaseAI ai);
}
