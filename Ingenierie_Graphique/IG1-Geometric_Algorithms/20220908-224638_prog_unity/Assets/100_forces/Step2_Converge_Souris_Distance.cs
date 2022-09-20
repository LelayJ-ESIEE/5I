using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : deplacement de chaque point vers la souris, 
//            et la vitesse depend de la distance entre le point et la souris.
//			  Faire un deplacement quadratique (c'est-a-dire avec un carre) si possible.
//			  Plus on est loin, plus le deplacement est rapide.
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
// - "MouseHlp.getPositionOnZ()" vous donne la position 3D de la souris sur le plan Z == 0
//
public class Step2_Converge_Souris_Distance : MonoBehaviour {

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

			// je choisi une vitesse en "unites par secondes"

			// je veux exprimer la distance entre la souris et le point entre 0 et 1.
			float distance_classique = fromPointToMouse.length();
			float distance_max = 25.0f; // d'apres l'init, je connais la distance max.
			float distance_01 =  distance_classique / distance_max;

			// je veux que plus on est loin, plus la vitesse est grande, de facon quadratique
			// on peut faire distance_01*distance_01, mais la fonction pow permet de faire des choses amusantes plus tard
			float force_quadratique = Mathf.Pow(distance_01, 2.0f);

			float speed_us = force_quadratique * 8.0f;

			// la distance c'est :   vitesse (en unite par secondes) * temps (en secondes)
			float distance_u = speed_us * timeSinceLastFrame_s;

			Vector3 lMouvement = lDirection_PointVersSouris * distance_u;

			mGraph.mPoints[i] += lMouvement;
		}
		//TOREMOVEFOREXERCISES_END

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
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
