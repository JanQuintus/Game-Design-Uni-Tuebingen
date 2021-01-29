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
            if(hit.transform.GetComponent<AInteractive>() != null)
            {
                _current = hit.transform.GetComponent<AInteractive>();
                interactText.gameObject.SetActive(true);
                interactText.SetText(_current.GetText());
                _current.ShowOutline();
            }
            else
            {
                if (_current)
                {
                    interactText.SetText("");
                    interactText.gameObject.SetActive(false);
                    _current.HideOutline(); 
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
                _current.HideOutline();
                _current = null;
            }
        }
    }

    public void PerformInteract(bool isRelease)
    {
        _current?.GetComponent<AInteractive>()?.Interact(isRelease);
    }
}
