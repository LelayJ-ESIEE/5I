using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : 
//		Il y a une fonction tres compliquee qui s'appelle fonctionAMinimiser().
//		Elle renvoie un float. Ce float depend de la position des vertex du graph.
//		Bougez les sommets a chaque instant de maniere a minimiser PARFAITEMENT la valeur renvoyee par fonctionAMinimiser().
//	
//		Pour cela on fait de la descente de gradient sur les X, puis sur les Y: 
//		1-on note la valeur 1.
//		2-on fait un petit mouvement.
//		3-on note la valeur 2, a l'issue de ce mouvement.
//		4-on en deduit le mouvement qu'il faut faire pour que la valeur soit exactement 0.
//
//		je vous conseille d'utiliser pour l'etape 4/ une equation de droite de type "Y = A*X + B" pour resoudre cela.
//
//		Le code est a faire dans la fonction doOneStep().
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribu
// - la dimension du terrain est de l'ordre de [-20,+20]
//
// remarque : ici on est dans un cas parfait. l'algorithme va fonctionner parfaitement.
//			  cependant, dans le cas general, on ne peut pas prendre la valeur parfaite de convergence.
//			  il y a pleins d'algorithmes qui utilisent cette "DESCENTE DE GRADIENT" avec des valeurs intermediaires.
//
public class Step8C_BlackBox_Minimisation_incomplet : MonoBehaviour
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

		// supprimer les textes (numeros), et mettre des couleurs variees
		for(int i = 0; i < mGraph.mPoints.Count; ++i)
		{
			var lPoint = mGraph.getVisualPoint(i);
			var lTextMesh = lPoint.GetComponentInChildren<TextMesh>();
			Object.Destroy( lTextMesh );

			// mettre des couleurs variees
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

		// si on appuie sur la barre espace, on fait 1 step
		if (Input.GetKey(KeyCode.Space))
		{
			doOneStep();
		}

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}

	/// afficher un bouton notamment pour remettre les points au hasard
	void OnGUI()
	{
		if (GUILayout.Button("Repositionner les points au hasard"))
		{
			foreach (GVector3 lPoint in mGraph.mPoints)
			{
				lPoint.x = (2.0f * UnityEngine.Random.value - 1.0f) * 20.0f;
				lPoint.y = (2.0f * UnityEngine.Random.value - 1.0f) * 20.0f;
			}
			mGraph.updateScene();
		}

		if (GUILayout.Button("Executer une fois"))
		{
			doOneStep();
			mGraph.updateScene();
		}

		if (GUILayout.RepeatButton("Maintenir pour Executer"))
		{
			doOneStep();
			mGraph.updateScene();
		}
	}

	/// minimiser la fonction de cout pour chaque vertex du graph
	public void doOneStep()
	{
		// TODO COMPLETEZ ICI

	}

	/// bougers les sommets pour minimiser les valeurs de cette fonction.
	public float fonctionAMinimiser(GVector3 pVertex)
	{
		var lVertex = new GVector3(pVertex);

		// si on veut un mouvement de cercle en plus
		//
		//float timeSinceBeginning = Time.realtimeSinceStartup;
		//lVertex.x += (Mathf.Cos(timeSinceBeginning) * 3.0f); 
		//lVertex.y += (Mathf.Sin(timeSinceBeginning) * 3.0f);

		float part1 = Mathf.Abs(lVertex.magnitude - 15 );
		float part2 = Mathf.Abs( (lVertex - new GVector3(-6, 4, 0)).length() - 5 );
		float part3 = Mathf.Abs( (lVertex - new GVector3( 6, 4, 0)).length() - 5 );
		float part4 = Mathf.Abs( (lVertex - new GVector3( 0, 0, 0)).length() - 2 );
		float part5 = MathsHlp.distancePointToSegment2D(lVertex, new Vector3(-5,-8,0), new Vector3( 5,-8,0) );

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

