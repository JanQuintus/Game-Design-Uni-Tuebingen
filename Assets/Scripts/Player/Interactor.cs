using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text interactText;
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private Transform source;
    [SerializeField] private LayerMask interactLayerMask;

    private AInteractive _current = null;

    private void Awake()
    {
        interactText.SetText("");
        interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Physics.Raycast(source.position, source.forward, out RaycastHit hit, interactDistance, interactLayerMask))
        {
            AInteractive aInteractive = hit.transform.GetComponent<AInteractive>();
            if (aInteractive == null) aInteractive = hit.transform.GetComponentInChildren<AInteractive>();

            if(aInteractive != null)
            {
                interactText.SetText(aInteractive.GetText());
                if (aInteractive == _current)
                    return;
                _current = aInteractive;
                interactText.gameObject.SetActive(true);
                _current.Hover();
            }
            else
            {
                if (_current)
                {
                    interactText.SetText("");
                    interactText.gameObject.SetActive(false);
                    _current.Unhover(); 
                    _current = null;
                }
            }
        }
        else
        {
            if (_current)
            {
                interactText.SetText("");
                interactText.gameObject.SetActive(false);
                _current.Unhover();
                _current = null;
            }
        }
    }

    public void PerformInteract(bool isRelease)
    {
        if (_current != null)
        {
            _current.Interact(isRelease);
        }
    }
}
