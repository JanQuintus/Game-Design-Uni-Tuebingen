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
        SetText("");
    }

    private void Update()
    {
        if (Physics.Raycast(source.position, source.forward, out RaycastHit hit, interactDistance, interactLayerMask))
        {
            AInteractive aInteractive = hit.transform.GetComponent<AInteractive>();
            if (aInteractive == null) aInteractive = hit.transform.GetComponentInChildren<AInteractive>();

            if(aInteractive != null)
            {
                SetText(aInteractive.GetText());
                if (aInteractive == _current)
                    return;
                _current = aInteractive;
                _current.Hover();
            }
            else
            {
                if (_current)
                {
                    SetText("");
                    _current.Unhover(); 
                    _current = null;
                }
            }
        }
        else
        {
            SetText("");

            if (_current)
            {
                _current.Unhover();
                _current = null;
            }
        }
    }

    private void SetText(string text)
    {
        interactText.gameObject.SetActive(text != "");
        interactText.SetText(text);
    }

    public void PerformInteract(bool isRelease)
    {
        if (_current != null)
        {
            _current.Interact(isRelease);
        }
    }
}
