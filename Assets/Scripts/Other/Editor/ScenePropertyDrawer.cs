using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

[CustomPropertyDrawer(typeof(SceneAttribute))]
public class ScenePropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var scenes = EditorBuildSettings.scenes.Select(scene => System.IO.Path.GetFileNameWithoutExtension(scene.path)).ToArray();

		EditorGUI.BeginChangeCheck();
		int index = EditorGUI.Popup(position, Array.IndexOf(scenes, property.stringValue), scenes);
		if (EditorGUI.EndChangeCheck())
		{
			property.stringValue = scenes[index];
		}
	}
}