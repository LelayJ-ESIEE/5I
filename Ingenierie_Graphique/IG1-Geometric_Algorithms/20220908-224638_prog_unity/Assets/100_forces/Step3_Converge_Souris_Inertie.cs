﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : deplacement regulier de chaque point vers la souris avec de l'inertie.
//			  pour cela a chaque instant :
//				- on ne modifie que l'acceleration vers la souris (mise a jour de mAccelerationsPoints).
//				- on en deduit une vitesse (mise a jour de mVitessesPoints).
//				- on effectue le mouvement lie a cette vitesse.
//			  il faut multiplier par le "temps ecoule depuis la derniere frame" pour passer 
//				- de l'acceleration a la vitesse
//				- de la vitesse a la position
//
//			bonus : ajouter du damping (ralentissement en fonction du carre de la Vitesse).
//				attention : il y a plein de formules pour le damping (hydrolique, spring, etc...)
//							et souvent elles ne sont pas exactes, donc faut pas avoir peur de faire des approximations ici.
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
// - "MouseHlp.getPositionOnZ()" vous donne la position 3D de la souris sur le plan Z == 0
//
public class Step3_Converge_Souris_Inertie : MonoBehaviour {

	/// notre graph
	public Graph mGraph = null;

	/// vitesse de chaque point
	public List<GVector3> mVitessesPoints = null;
	/// acceleration de chaque point
	public List<GVector3> mAccelerationsPoints = null;

	/// amortissement
	public float mDampingCoeff = 0.002f;

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


		/// preparer nos vitesses et nos accelerations, remplir les tableaux
		int nbPoints = mGraph.mPoints.Count;
		mVitessesPoints = new List<GVector3>();
		mAccelerationsPoints = new List<GVector3>();
		for(int i =0; i < nbPoints;++i)
		{
			mVitessesPoints.Add( new GVector3() );
			mAccelerationsPoints.Add( new GVector3() );
		}

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

		//TOREMOVEFOREXERCISES_BEGIN

		// timeSinceLastFrame_s contient le temps ecoule en secondes depuis la derniere frame
		float timeSinceLastFrame_s = Time.deltaTime;
		Vector3 lMousePosition3D = MouseHlp.getPositionOnZ();

		for(int i = 0; i < mGraph.mPoints.Count; ++i)
		{
			// remarque : ceci ne bouge pas la 3D. cela bouge juste le 'graph logique'

			// vecteur qui permet de "partir de notre sommet et arriver a la souris"
			Vector3 fromPointToMouse = (lMousePosition3D - mGraph.mPoints[i]);

			// on veut une direction : un vecteur de taille 1
			Vector3 lDirection_PointVersSouris = fromPointToMouse.normalized;

			/// maj acceleration, ce qui influence ensuite la vitesse, ce qui influence ensuite la position
			mAccelerationsPoints[i] = lDirection_PointVersSouris * 5.0f; 
			// definition vitesse : acceleration * temps
			mVitessesPoints[i]     += mAccelerationsPoints[i] * timeSinceLastFrame_s;


			// bonus : on enleve le damping (ralentissement)
			// je calcule "composante au carre * coeff_damping".
			// et je l'empeche avec un clamp de depasser la vitesse de base.
			// et j'utiliser Sign pour garder le meme signe qu'au depart (sinon le carre l'enleve)
			var V = mVitessesPoints[i];
			var dampingCarre  = new GVector3(
				Mathf.Sign(V.x)* Mathf.Clamp(V.x*V.x*mDampingCoeff, 0, Mathf.Abs(V.x)),
				Mathf.Sign(V.y)* Mathf.Clamp(V.y*V.y*mDampingCoeff, 0, Mathf.Abs(V.y)),
				Mathf.Sign(V.z)* Mathf.Clamp(V.z*V.z*mDampingCoeff, 0, Mathf.Abs(V.z))
			);

			/*
			// on le trouve souvent, mais je n'ai jamais ete tres convaincu par celui-ci
			var dampingSimple  = new GVector3(
				Mathf.Sign(V.x)* Mathf.Clamp(V.x*mDampingCoeff, 0, Mathf.Abs(V.x)),
				Mathf.Sign(V.y)* Mathf.Clamp(V.y*mDampingCoeff, 0, Mathf.Abs(V.y)),
				Mathf.Sign(V.z)* Mathf.Clamp(V.z*mDampingCoeff, 0, Mathf.Abs(V.z))
			);
			*/
			mVitessesPoints[i] -= dampingCarre;

			// definition position : vitesse * temps
			mGraph.mPoints[i]      += mVitessesPoints[i] * timeSinceLastFrame_s;
		}


		//TOREMOVEFOREXERCISES_END

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}


	/// afficher 2 boutons qui permettent de changer les vitesses notamment
	void OnGUI()
	{
		if( GUILayout.Button("Mettre vitesses aleatoires"))
		{
			for(int i = 0; i < mVitessesPoints.Count; ++i)
			{
				// on choisit une vitesse en 2D
				mVitessesPoints[i] = new GVector3( 
					UnityEngine.Random.value * 10.0f,
					UnityEngine.Random.value * 10.0f,
					0.0f);
			}
		}

		if( GUILayout.Button("Mettre vitesses \u00E0 0"))
		{
			for(int i = 0; i < mVitessesPoints.Count; ++i)
			{
				mVitessesPoints[i] = Vector3.zero;
			}
		}

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