using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransformSaveable : MonoBehaviour, ISaveable
{
    public object CaptureState()
    {
        return new SaveState
        {
            PX = transform.position.x,
            PY = transform.position.y,
            PZ = transform.position.z,

            RX = transform.rotation.x,
            RY = transform.rotation.y,
            RZ = transform.rotation.z,
            RW = transform.rotation.w,

            SX = transform.localScale.x,
            SY = transform.localScale.y,
            SZ = transform.localScale.z,

            SceneName = SceneManager.GetActiveScene().name
        };
    }

    public void RestoreState(object state)
    {
        SaveState saveState = (SaveState)state;
        if (saveState.SceneName != SceneManager.GetActiveScene().name)
            return;

        Vector3 p = transform.position;
        p.x = saveState.PX;
        p.y = saveState.PY;
        p.z = saveState.PZ;

        Quaternion r = transform.rotation;
        r.x = saveState.RX;
        r.y = saveState.RY;
        r.z = saveState.RZ;
        r.w = saveState.RW;

        Vector3 s = transform.localScale;
        s.x = saveState.SX;
        s.y = saveState.SY;
        s.z = saveState.SZ;

        transform.position = p;
        transform.rotation = r;
        transform.localScale = s;
    }

    [System.Serializable]
    private struct SaveState
    {
        public float PX, PY, PZ;
        public float RX, RY, RZ, RW;
        public float SX, SY, SZ;
        public string SceneName;
    }
}
