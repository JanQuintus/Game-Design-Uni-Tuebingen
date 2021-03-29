using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTextTrigger : MonoBehaviour, ISaveable
{

    [SerializeField] private GameObject uiText;
    private bool _saw = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_saw && other.gameObject.tag.Equals("Player"))
        {
            uiText.SetActive(true);
            _saw = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_saw && other.gameObject.tag.Equals("Player"))
        {
            uiText.SetActive(false);
        }
    }

    public object CaptureState()
    {
        return _saw;
    }

    public void RestoreState(object state)
    {
        _saw = (bool)state;
    }
}
