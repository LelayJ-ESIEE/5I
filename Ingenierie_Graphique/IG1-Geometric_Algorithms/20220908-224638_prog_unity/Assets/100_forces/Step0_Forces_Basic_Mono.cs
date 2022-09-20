using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : deplacement regulier de chaque point vers le bas
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
public class Step0_Forces_Basic_Mono : MonoBehaviour
{
	/// notre graph
	public Graph mGraph = null;

	// fonction appelee automatiquement lorsqu'on fait "Play" et que l'objet est actif (et son component)
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
	
	// cette fonction est appelee une fois par frame
	void Update()
	{
		//----------------------------------------------------------------------
		//----------------- mise a jour du graph (cote logique) ----------------
		//----------------------------------------------------------------------

		// si on a bouge des objets de la scene (via l'inspector), on prend cela en compte pour mettre a jour le graph
		mGraph.updateFromScene();

		// si on clique, on vient mettre 'le sommet le plus proche du clic' sous le curseur de la souris
		if( Input.GetMouseButton(0))
		{
			// obtenir la position 3D de la souris (envoi un rayon sur le plan Z = 0)
			var lMousePosition3D = MouseHlp.getPositionOnZ();

			// trouver le numero du point le plus proche
			int indexPoint = mGraph.getClosestPointFrom( lMousePosition3D );

			// le mettre a la meme position que la souris 3D
			mGraph.mPoints[indexPoint] = lMousePosition3D;
		}

		//TOREMOVEFOREXERCISES_BEGIN

		// timeSinceLastFrame_s contient le temps ecoule en secondes depuis la derniere frame
		float timeSinceLastFrame_s = Time.deltaTime;
		for(int i = 0; i < mGraph.mPoints.Count; ++i)
		{
			// remarque : ceci ne bouge pas la 3D. cela bouge juste le 'graph logique'

			// le calcul de la distance a parcourir en unites par secondes
			float speed_us = 6.0f;
			float distanceMouvement = timeSinceLastFrame_s * speed_us;

			// on affecte cette distance seulement sur l'axe y (verticalement)
			mGraph.mPoints[i].y -= distanceMouvement;
		}
		//TOREMOVEFOREXERCISES_END

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}
}
