using UnityEngine;
using System.Collections;

public class ExplodeAndLoadEndScenePartCollectable : AInteractive, ISaveable
{
    [SerializeField] private AudioClipBundle innerDialogBundle;
    [SerializeField] private AudioClip pickUpClip;
    [SerializeField] private int globalProgress = 0;
    [SerializeField] private string pickUpText = "PartName";
    [SerializeField] private GameObject explosionParticles;
    [SerializeField] private AudioClip explosionClip;

    private bool _pickedUp = false;

    public override void Interact(bool isRelease)
    {
        _pickedUp = true;
        SoundController.Instance.PlaySoundAtLocation(pickUpClip, transform.position);
        InnerDialog.Instance.PlayDialog(innerDialogBundle.GetRandomClip());
        GameManager.GlobalProgress = globalProgress;
        explosionParticles.SetActive(true);
        SoundController.Instance.PlaySoundAtLocation(explosionClip, explosionParticles.transform.position, volume: 10, range: 10000);
        SoundController.Instance.PlaySoundAtLocation(explosionClip, explosionParticles.transform.position, volume: 10, range: 10000, pitch: 0.5f);
        SoundController.Instance.PlaySoundAtLocation(explosionClip, explosionParticles.transform.position, volume: 10, range: 10000, pitch: 1.5f);
        StartCoroutine(LoadScene());
        Destroy(GetComponent<Collider>());
        for(int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }

    private IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1.5f);
        SceneLoader.Instance.LoadScene("FinalScene", true);
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
