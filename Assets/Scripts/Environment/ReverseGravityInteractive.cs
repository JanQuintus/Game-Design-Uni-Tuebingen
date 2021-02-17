using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseGravityInteractive : AInteractive
{
    [SerializeField] Transform centerPos;
    [SerializeField] Vector3 extend = Vector3.one;
    [SerializeField] Vector3 gravity = new Vector3(0, 9.81f, 0);

    public override void Interact(bool isRelease)
    {
        base.Interact(isRelease);

        Collider[] colliders = Physics.OverlapBox(centerPos.position, extend/2f);
        foreach (Collider col in colliders) {
            GravityObject go = Utils.FindGravityObject(col);
            if (go == null)
                continue;

            if (!go.IsGravityChanged())
                go.SetLocalGravity(gravity);

            go.SetDefaultGravity(gravity);
        }

        OnInteractionEnd?.Invoke();

    }

    public override string GetText()
    {
        return "Reverse Gravity, Good Luck";
    }

    private void OnDrawGizmos()
    {
        if (centerPos == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centerPos.position, extend);



    }
}
