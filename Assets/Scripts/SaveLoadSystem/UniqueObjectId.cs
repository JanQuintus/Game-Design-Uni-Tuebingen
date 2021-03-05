using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UniqueObjectId : MonoBehaviour
{
    [SerializeField] private string uid = null;
    [SerializeField] private bool staticUID = false;

    public string GetUID() => uid;


    private void Awake()
    {
        if (uid == null || uid.Length == 0) GenerateUID();
    }

    [ContextMenu("Generate UID")]
    public void GenerateUID() {
        if (uid == null || uid.Length == 0 || !staticUID)
            uid = Guid.NewGuid().ToString();
    }

    [ContextMenu("Clear UID")]
    public void ClearUID()
    {
        if(!staticUID) uid = null;
    }

    [ContextMenu("Clear UID Static")]
    public void ClearStatic()
    {
        uid = null;
    }

    public object CaptureState()
    {
        var state = new Dictionary<string, object>();

        foreach(ISaveable saveable in GetComponents<ISaveable>())
        {
            state[saveable.GetType().ToString()] = saveable.CaptureState();
        }

        return state;
    }

    public void RestoreState(object state)
    {
        var stateDictionary = (Dictionary<string, object>)state;

        foreach (ISaveable saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();

            if(stateDictionary.TryGetValue(typeName, out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

}
