using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : deplacement chaque point vers les autres points
//			  mais ils ne doivent pas se toucher.
//
//            Cela signifie qu'on peut ajouter une force de repulsion, elle aussi basee sur la distance.
//			  Cette force doit etre tres forte quand on est proche, et de plus en plus faible quand on est loin.
//
//			  Pour cela, convertir vers des valeurs de l'ordre de [0-1] est souvent pratique (mais pas obligatoire).
//
//			  Note : a la place d'une force progressive, on peut aussi forcer les objets a s'eloigner violemment les uns des autres.
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
// - "MouseHlp.getPositionOnZ()" vous donne la position 3D de la souris sur le plan Z == 0
// - la dimension du terrain est de l'ordre de [-20,+20].
//
public class Step5_Converge_NoTouch : MonoBehaviour {

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

		// 1/ calculer les mouvements
		int nbPoints = mGraph.mPoints.Count;
		List<Vector3> lMouvementPoints = new List<Vector3>(new Vector3[nbPoints]);
		for(int i = 0; i < nbPoints; ++i)
		{
			Vector3 pointI = mGraph.mPoints[i];
			for(int j = i+1; j < nbPoints; ++j)
			{
				GVector3 pointJ = mGraph.mPoints[j];
				GVector3 IJ = pointJ - pointI;
				GVector3 IJ_dir = IJ.getNormalized();

				float distance = IJ.length();

				float distanceLimite = 3.0f;

				// cette distance01 vaut 1 sur la limite, et davantage au dela de la limite.
				float distance01 = Mathf.Abs( distance / distanceLimite);

				GVector3 mouvementIJ = IJ_dir * timeSinceLastFrame_s;

				float forceRapproche = distance01;
				lMouvementPoints[i] += mouvementIJ * forceRapproche;
				lMouvementPoints[j] -= mouvementIJ * forceRapproche;

				if( distance01 > 0.00001f)
				{
					// sera appliquee quasiment tout le temps

					// le probleme de la division est que c'est TRES fort des qu'on se rapproche trop.
					// il faut mettre un clamp dans ce cas.
					//float forceRepousse = -Mathf.Clamp(1.0f/distance01, 0.0f, 20.0f );

					// sinon on peut utiliser des exposants (racines carrees)
					float forceRepousse = -Mathf.Pow(distance01, 0.01f);

					lMouvementPoints[i] += mouvementIJ * forceRepousse;
					lMouvementPoints[j] -= mouvementIJ * forceRepousse;
				}else{
					lMouvementPoints[i] += new GVector3(0.01f,0,0);
					lMouvementPoints[j] -= new GVector3(0.01f,0,0);
				}

				// note: evidemment on peut tout faire en 1 ligne, mais je veux montrer
				// que ces forces sont INDEPENDANTES.
			}
		}

		// 2/ appliquer les mouvements
		for(int i = 0; i < nbPoints; ++i)
		{
			mGraph.mPoints[i] += lMouvementPoints[i];
		}

		//TOREMOVEFOREXERCISES_END

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
