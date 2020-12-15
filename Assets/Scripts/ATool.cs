using UnityEngine;

public abstract class ATool : MonoBehaviour
{
    public abstract void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false);

    public abstract void Reset(bool isRelease);

    public abstract void Scroll(float delta);

    public abstract void Reload();
}