using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static string SavePath => $"{Application.persistentDataPath}/save.txt";

    public static SaveLoadSystem Instance;

    private void Start()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Load();
    }

    public void Save()
    {
        GameManager.Save();
        var state = LoadFile();
        CaptureState(state);
        SaveFile(state);
    }

    public void Load()
    {
        GameManager.Load();
        var state = LoadFile();
        RestoreState(state);
    }

    private Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(SavePath))
            return new Dictionary<string, object>();

        using (FileStream stream = File.Open(SavePath, FileMode.Open))
        {
            var formatter = new BinaryFormatter();
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }

    private void SaveFile(object state)
    {
        using (FileStream stream = File.Open(SavePath, FileMode.Create))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, state);
        }
    }

    private void CaptureState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsOfType<UniqueObjectId>())
        {
            state[saveable.GetUID()] = saveable.CaptureState();
        }
    }

    private void RestoreState(Dictionary<string, object> state)
    {
        foreach(var saveable in FindObjectsOfType<UniqueObjectId>())
        {
            if(state.TryGetValue(saveable.GetUID(), out object value))
            {
                saveable.RestoreState(value);
            }
        }
    }

    [MenuItem("SaveLoadSystem/Generate missing UIDs")]
    private static void GenerateMissingUIDs()
    {
        foreach (var saveable in FindObjectsOfType<UniqueObjectId>())
        {
            saveable.GenerateUID();
            EditorUtility.SetDirty(saveable);
            print("Generated missing UIDs");
        }

    }

    [MenuItem("SaveLoadSystem/Clear Non-Static UIDs")]
    private static void ClearNonStaticUIDs()
    {
        foreach (var saveable in FindObjectsOfType<UniqueObjectId>())
        {
            saveable.ClearUID();
            EditorUtility.SetDirty(saveable);
            print("Cleared Non-Static UIDs");
        }
    }

    [MenuItem("SaveLoadSystem/Delete SaveFile")]
    public static void DeleteSaveFile()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            print("SaveFile deleted.");
        }
    }
}
