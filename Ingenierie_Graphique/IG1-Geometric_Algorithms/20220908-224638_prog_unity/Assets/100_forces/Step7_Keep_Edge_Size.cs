using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Exercice : Maintenir la taille des liens par rapport a leur taille initiale.
//			  Il faut donc dans le Start() noter la taille des liens.
//			  puis la reaffecter dans le Update()
//
// utile : 
// - "mGraph.mPoints" est une liste de sommets, ils ont chacun x,y,z comme attribut
// - "Time.deltaTime" contient le temps ecoule en secondes depuis la derniere frame.
// - "MouseHlp.getPositionOnZ()" vous donne la position 3D de la souris sur le plan Z == 0
// - la dimension du terrain est de l'ordre de [-20,+20].
// - mGraph.mEdges contient les liens entre les sommets du graph
// - chaque edge dispose des attributs 'a' et 'b' qui sont les numeros des sommets
//
public class Step7_Keep_Edge_Size : MonoBehaviour
{
	/// notre graph
	public Graph mGraph = null;

	/// la longueur de chaque edge du graph au depart
	/// elle doit etre remplie dans le Start() et ensuite on l'utilise pour maintenir la contrainte
	public List<float> mEdgeSizes = null;

	// Use this for initialization
	void Start () 
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("Un Graph");
		mGraph.initAsRandomPoints(8, -25, 25, -18, 18);
		mGraph.link3ClosestPoints();

		//TOREMOVEFOREXERCISES_BEGIN
		mEdgeSizes = new List<float>();
		for(int e = 0; e < mGraph.mEdges.Count; ++e)
		{
			var edge = mGraph.mEdges[e];
			var A = mGraph.mPoints[edge.a];
			var B = mGraph.mPoints[edge.b];
			mEdgeSizes.Add( (A-B).length() );
		}
		//TOREMOVEFOREXERCISES_END

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

		// ici la contrainte est tres FORTE. 
		// elle va creer des SAUTS dans les valeurs.
		//
		// precedemment on a fait les calculs en 2 etapes :
		//  - 1 boucle : calcul des mouvements
		//	- 1 boucle : appliquer les mouvements
		// cependant cela cree des instabilites car la contrainte est FORTE cette fois.
		//
		// donc : 
		//   - soit on ne fait pas la decomposition en 2 etapes
		//   - soit on ajoute un *0.1 pour reduire la force de la contrainte, mais cela nuit a l'experience!
		int nbPoints = mGraph.mPoints.Count;
		for(int e = 0; e < mGraph.mEdges.Count; ++e)
		{
			// recuperer notre lien entre 2 sommets
			Edge lEdge = mGraph.mEdges[e];

			// recuperer les sommets
			int a = lEdge.a;
			int b = lEdge.b;

			var A = mGraph.mPoints[a];
			var B = mGraph.mPoints[b];

			float lNewEdgeSize = (A-B).length();
			float lOriginalEdgeSize = mEdgeSizes[e];

			// gestion du cas particulier
			if( lNewEdgeSize < 0.0001f )
			{
				if( lOriginalEdgeSize > 0.0001f)
				{
					// on les ecarte artificiellement
					mGraph.mPoints[a] += new GVector3(0.01f, 0.0f, 0.0f);
				}
				continue;
			}

			// calculer notre nouvelle position ideale
			var centerAB = (A + B) *0.5f;

			float sizeChange = lOriginalEdgeSize / lNewEdgeSize;
			Vector3 lNewPositionA = centerAB + (A - centerAB) * sizeChange;
			Vector3 lNewPositionB = centerAB + (B - centerAB) * sizeChange;

			mGraph.mPoints[a] = lNewPositionA; 
			mGraph.mPoints[b] = lNewPositionB;
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

		if( GUILayout.Button("Changer les liens"))
		{
			mGraph.mEdges.Clear();
			for(int i = 0; i < mGraph.mPoints.Count; ++i)
			{
				for(int j = i+1; j < mGraph.mPoints.Count; ++j)
				{
					if( Random.Range(0, 3) == 0)
					{
						mGraph.addLinkIfNotThereBetween( i, j, false);
					}
				}
			}

			mEdgeSizes = new List<float>();
			for(int e = 0; e < mGraph.mEdges.Count; ++e)
			{
				var edge = mGraph.mEdges[e];
				var A = mGraph.mPoints[edge.a];
				var B = mGraph.mPoints[edge.b];
				mEdgeSizes.Add( (A-B).length() );
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
