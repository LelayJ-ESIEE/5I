using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : 
//		Il y a une fonction tres compliquee qui s'appelle fonctionAMinimiser(vertex).
//		Elle renvoie un float. Ce float depend de la position du vertex du graph.
//
//		Vous devez Bouger les sommets a chaque instant de maniere a minimiser la valeur renvoyee par fonctionAMinimiser().
//		Conseil : bougez dans n'importe quel sens, et ne gardez le mouvement que si la fonction evolue correctement.
//
//		On appelle "BlackBox" cette fonction, car on ne vous dit pas clairement comment elle fonctionne, comme une boite noire.
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
// - la dimension du terrain est de l'ordre de [-20,+20].
//
public class Step8A_BlackBox_DumbMinimisation : MonoBehaviour
{
	/// notre graph
	public Graph mGraph = null;


	// Use this for initialization
	void Start () 
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("Un Graph");
		/*
		for(int i = 0; i < 64; ++i)
		{
			mGraph.mPoints.Add( new Vector3( i/8 - 4, i%8 - 4, 0 )*3 );
		}*/

		// si on voulait plus de points
		for(int i = 0; i < 128; ++i)
		{
			mGraph.mPoints.Add( new Vector3( i/12 - 5.5f, i%12 - 5.5f, 0 )*2.5f );
		}

		// afficher le graph dans la 3D, en creant des objets si besoin : cote visuel
		mGraph.updateScene();
		for(int i = 0; i < mGraph.mPoints.Count; ++i)
		{
			var lPoint = mGraph.getVisualPoint(i);
			var lTextMesh = lPoint.GetComponentInChildren<TextMesh>();
			Object.Destroy( lTextMesh );
			var lRenderer = lPoint.GetComponentInChildren<Renderer>();
			lRenderer.sharedMaterial.color = Random.ColorHSV(); 
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
		int nbPoints = mGraph.mPoints.Count;

		// on bouge dans tous les sens, et on garde une variation qui nous rapproche si on en trouve une
		for(int p = 0; p < nbPoints; ++p)
		{
			GVector3 P = mGraph.mPoints[p];
			float currentValue = fonctionAMinimiser( P );

			for(int nbTry = 0; nbTry < 5; ++nbTry)
			{
				// tente plusieurs directions au hasard
				float angle_rad = Random.Range(0.0f, 2 * Mathf.PI);

				float vitesse = 3.0f;
				Vector3 lVariation = new Vector3( Mathf.Cos(angle_rad), Mathf.Sin(angle_rad), 0) * Time.deltaTime * vitesse;

				Vector3 lNewPointProposition = P + lVariation;

				float newValue = fonctionAMinimiser( lNewPointProposition );
				if( newValue < currentValue )
				{
					P.x = lNewPointProposition.x;
					P.y = lNewPointProposition.y;
					P.z = lNewPointProposition.z;
					currentValue = newValue;
					break;
				}
			}
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

	/// bougers les sommets pour minimiser les valeurs de cette fonction.
	public float fonctionAMinimiser(GVector3 pVertex)
	{
		float part1 = Mathf.Abs( pVertex.magnitude - 15 );
		float part2 = Mathf.Abs( (pVertex - new GVector3(-6, 4, 0)).length() - 5 );
		float part3 = Mathf.Abs( (pVertex - new GVector3( 6, 4, 0)).length() - 5 );
		float part4 = Mathf.Abs( (pVertex - new GVector3( 0, 0, 0)).length() - 2 );
		float part5 = MathsHlp.distancePointToSegment2D(pVertex, new Vector3(-5,-8,0), new Vector3( 5,-8,0) );
		float closest = part1;
		closest = Mathf.Min( part2, closest);
		closest = Mathf.Min( part3, closest);
		closest = Mathf.Min( part4, closest);
		closest = Mathf.Min( part5, closest);
		return closest;
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
