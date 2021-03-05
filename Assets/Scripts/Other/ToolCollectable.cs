using UnityEngine;

public class ToolCollectable : AInteractive, ISaveable
{
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private ATool tool;

    private bool _pickedUp = false;

    public override void Interact(bool isRelease)
    {
        _pickedUp = true;
        PlayerController.Instance.GetToolBelt().UnlockTool(tool);
        SoundController.Instance.PlaySoundAtLocation(pickUpClip, transform.position);
        Destroy(GetComponent<Collider>());
        for(int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    public override string GetText()
    {
        return "Take " + tool.ToolName;
    }

    public object CaptureState()
    {
        return _pickedUp;
    }

    public void RestoreState(object state)
    {
        _pickedUp = (bool)state;
        if (_pickedUp)
        {
            Destroy(GetComponent<Collider>());
            for (int i = 0; i < transform.childCount; i++)
                Destroy(transform.GetChild(i).gameObject);
        }
    }
}
