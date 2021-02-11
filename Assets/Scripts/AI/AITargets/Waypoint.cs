using UnityEngine;

[System.Serializable]
public class Waypoint : MonoBehaviour {

    public virtual bool CanProceed(BaseAI ai) => true;

}
