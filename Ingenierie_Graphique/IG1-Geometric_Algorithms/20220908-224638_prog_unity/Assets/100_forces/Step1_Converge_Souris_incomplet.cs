﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : deplacement regulier de chaque point vers la souris
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
// - "MouseHlp.getPositionOnZ()" vous donne la position 3D de la souris sur le plan Z == 0
//    (donc le X et le Y de ce vecteur 3D sont les memes que ceux de mGraph)
public class Step1_Converge_Souris_incomplet : MonoBehaviour {

	/// notre graph
	public Graph mGraph = null;

	// Use this for initialization
	void Start () 
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("Un Graph");
		mGraph.initAsRandomPoints(8, -25, 25, -18, 18);
		mGraph.link3ClosestPoints();

		// afficher le graph dans la 3D, en creant des objets si besoin : cote visuel
		mGraph.updateScene();
	}

	// Update is called once per frame
	void Update()
	{
		//----------------------------------------------------------------------
		//----------------- mise a jour du graph (cote logique) ----------------
		//----------------------------------------------------------------------

		// si on a bouge des objets de la scene (via l'inspector), on prend cela en compte pour mettre a jour le graph
		mGraph.updateFromScene();

		// si on clique, on vient mettre 'le sommet le plus proche du clic' sous le curseur de la souris
		if( Input.GetMouseButton(0) )
		{
			mettreSommetLePlusProcheSousLaSouris();
		}


		// TODO COMPLETEZ ICI


		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}

	/// afficher un bouton notamment pour remettre les points au hasard
	void OnGUI()
	{
		if( GUILayout.Button("Repositionner les points au hasard"))
		{
			foreach(GVector3 lPoint in mGraph.mPoints)
			{
				lPoint.x = (2.0f*UnityEngine.Random.value - 1.0f) * 20.0f;
				lPoint.y = (2.0f*UnityEngine.Random.value - 1.0f) * 20.0f;
			}
			mGraph.updateScene();
		}
	}

	/// prend le sommet du graph le plus proche de la souris,
	/// et le met exactement sous la souris.
	void mettreSommetLePlusProcheSousLaSouris()
	{
		// obtenir la position 3D de la souris (envoi un rayon sur le plan Z == 0)
		Vector3 lMousePosition3D = MouseHlp.getPositionOnZ();

		// trouver le numero du point le plus proche
		int indexPoint = mGraph.getClosestPointFrom( lMousePosition3D );

		// le mettre a la meme position que la souris 3D
		mGraph.mPoints[indexPoint] = lMousePosition3D;
	}
}

