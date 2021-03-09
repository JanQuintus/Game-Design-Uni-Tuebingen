using UnityEngine;

public class EnterSpaceShip : AInteractive
{
    [SerializeField] private HangerDoorLever lever;
    [SerializeField] private AudioClip doorClosedDialog;

    public override void Interact(bool isRelease)
    {
        if (isRelease)
            return;

        if (!lever.HangerDoorOpen)
        {
            InnerDialog.Instance.PlayDialog(doorClosedDialog);
            return;
        }

        SceneLoader.Instance.LoadScene("Hub", true);
    }

    public override string GetText()
    {
        return "Enter Spaceship";
    }
}
