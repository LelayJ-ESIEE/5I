using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Commentaire_Mono))]
public class Commentaire_Inspector : Editor {

	bool mDeployed = true;
	public override void OnInspectorGUI()
	{
		var lTarget = target as Commentaire_Mono;
		if( lTarget == null )
		{
			return;
		}
		mDeployed = GUIHlp.drawDeployArrow( mDeployed );
		/*
		if( GUILayout.Button( mDeployed ? "\u25BC" : "\u25B6", GUILayout.ExpandWidth(false)))
		{
			mDeployed = !mDeployed;
		}*/
		//mDeployed = GUILayout.Toggle(mDeployed, "Commentaire");
		if( mDeployed )
		{
			lTarget.mComment = GUILayout.TextArea( lTarget.mComment );
		}
		//EditorGUILayout.EndToggleGroup();
	}
}
