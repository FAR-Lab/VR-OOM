using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ConditionManager))]
public class ConditionManagerEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		ConditionManager myScript = (ConditionManager)target;
		if(GUILayout.Button("SaveCarPosition"))
		{
			myScript.SaveCarPose();
		}
	}
}

