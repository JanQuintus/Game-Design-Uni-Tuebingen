using UnityEngine;

public class PartCollectable : AInteractive, ISaveable
{
    [SerializeField] private AudioClipBundle innerDialogBundle;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private int globalProgress = 0;
    [SerializeField] private string pickUpText = "PartName";

    private bool _pickedUp = false;

    public override void Interact(bool isRelease)
    {
        _pickedUp = true;
        SoundController.Instance.PlaySoundAtLocation(pickUpClip, transform.position);
        if(innerDialogBundle != null && innerDialogBundle.GetRandomClip() != null)
            InnerDialog.Instance.PlayDialog(innerDialogBundle.GetRandomClip());
        if(globalProgress != -1) GameManager.GlobalProgress = globalProgress;
        Destroy(GetComponent<Collider>());
        for(int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    public override string GetText()
    {
        return "Take " + pickUpText;
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
