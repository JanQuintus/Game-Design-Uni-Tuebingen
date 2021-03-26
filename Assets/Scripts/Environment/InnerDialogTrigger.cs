using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InnerDialogTrigger : MonoBehaviour, ISaveable
{
    private enum TriggerType
    {
        ENTER,
        STAY,
        EXIT
    }

    [SerializeField] private AudioClipBundle clipBundle;
    [SerializeField] private float canPlayAgainDelay;
    [SerializeField] private TriggerType triggerType = TriggerType.ENTER;

    private float delay = 0;

    private void Update()
    {
        if(delay >= 0)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
                delay = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.ENTER && delay == 0 && other.gameObject.tag.Equals("Player"))
        {
            InnerDialog.Instance.PlayDialog(clipBundle.GetRandomClip());
            delay = canPlayAgainDelay;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerType == TriggerType.STAY && delay == 0 && other.gameObject.tag.Equals("Player"))
        {
            InnerDialog.Instance.PlayDialog(clipBundle.GetRandomClip());
            delay = canPlayAgainDelay;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerType == TriggerType.EXIT && delay == 0 && other.gameObject.tag.Equals("Player"))
        {
            InnerDialog.Instance.PlayDialog(clipBundle.GetRandomClip());
            delay = canPlayAgainDelay;
        }
    }

    public object CaptureState()
    {
        return delay;
    }

    public void RestoreState(object state)
    {
        delay = (float)state;
    }
}
