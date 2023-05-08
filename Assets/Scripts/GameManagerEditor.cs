using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
[ExecuteInEditMode]
public class GameManagerEditor : Editor
{
	
	SerializedProperty DB_CompleteLvl2Miss1;
	SerializedProperty DB_CompleteLvl2Miss2;
	SerializedProperty DB_CompleteLvl3Miss1;
	SerializedProperty DB_CompleteLvl3Miss2;
	private bool debugButtonsShown = false;
	private GameObject thisGameObject;

	private void OnEnable()
	{
		DB_CompleteLvl2Miss1 = serializedObject.FindProperty("DB_ClickToCompleteLvl2Miss1");
		DB_CompleteLvl2Miss2 = serializedObject.FindProperty("DB_ClickToCompleteLvl2Miss2");
		DB_CompleteLvl3Miss1 = serializedObject.FindProperty("DB_ClickToCompleteLvl3Miss1");
		DB_CompleteLvl3Miss2 = serializedObject.FindProperty("DB_ClickToCompleteLvl3Miss2");
		thisGameObject = GameObject.Find("GameController");
	}
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		base.OnInspectorGUI();
		EditorGUILayout.Space(10);
		EditorGUILayout.PropertyField(DB_CompleteLvl2Miss1);
		EditorGUILayout.PropertyField(DB_CompleteLvl2Miss2);
		EditorGUILayout.PropertyField(DB_CompleteLvl3Miss1);
		EditorGUILayout.PropertyField(DB_CompleteLvl3Miss2);
		serializedObject.ApplyModifiedProperties();
	}

	private void MonitorDebugButtons()
	{
		bool missionsModified = false;
		if (DB_CompleteLvl2Miss1.boolValue == true)
		{
			DB_CompleteLvl2Miss1.boolValue = false;
			PlayerPrefs.SetInt("TotalGreenAliensCollected", 50);
			Debug.Log("Lvl 2 Mission 1 Completed (via Debugging)");
			missionsModified = true;
		}

		if (DB_CompleteLvl2Miss2.boolValue == true)
		{
			DB_CompleteLvl2Miss2.boolValue = false;
			PlayerPrefs.SetFloat("HighestL1Multiplier", 5f);
			Debug.Log("Lvl 2 Mission 2 Completed (via Debugging)");
			missionsModified = true;
		}

		if (DB_CompleteLvl3Miss1.boolValue == true)
		{
			DB_CompleteLvl3Miss1.boolValue = false;
			PlayerPrefs.SetInt("HighestL2BlueCollected", 10);
			Debug.Log("Lvl 3 Mission 1 Completed (via Debugging)");
			missionsModified = true;
		}

		if (DB_CompleteLvl3Miss2.boolValue == true)
		{
			DB_CompleteLvl3Miss2.boolValue = false;
			PlayerPrefs.SetFloat("HighestL2Score", 500);
			Debug.Log("Lvl 3 Mission 2 Completed (via Debugging)");
			missionsModified = true;
		}

		if (missionsModified == true)
		{
			//thisGameObject.GetComponent<GameManager>().LoadDataFromPlayerPrefs();
			thisGameObject.GetComponent<UIManager>().Start(); //This will make the UIManager redo the test to see if levels are unlocked and refresh the stats.
			missionsModified = false;
		}
	}
}
#endif