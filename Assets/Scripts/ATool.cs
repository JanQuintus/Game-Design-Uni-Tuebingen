using UnityEngine;

public abstract class ATool : MonoBehaviour
{
    public System.Action OnFillChanged;

    public string ToolName = "ATool";
    public abstract void Shoot(Ray ray, bool isRelease = false, bool isRightClick = false);

    public abstract void Reset(bool isRelease);

    public abstract void Scroll(float delta);

    public abstract float GetFill();
    public abstract void SetFill(float fill);
    public abstract float GetFillPercentage();

    public abstract void OnEquip();
    public abstract void OnUnequip();

    public abstract void Reload();
}