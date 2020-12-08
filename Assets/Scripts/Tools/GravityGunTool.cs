using UnityEngine;

public class GravityGunTool : ATool
{
    [SerializeField]
    private float distance;

    public override void Shoot(bool isRightClick = false)
    {
        if (isRightClick)
            return;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
                rb.useGravity = !rb.useGravity;
        }
    }
}
