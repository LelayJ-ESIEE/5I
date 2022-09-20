using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// BUT : trouver 2 points tres eloignes sur le maillage
///   1/ prendre un point P0
///   2/ trouver le point le plus eloigne de P0 : appelons-le P1
///	  3/ trouver le point le plus eloigne de P1 : appelons-le P2
///   4/ trouver le chemin qui va de P1 a P2.
/// 
/// Il y a plus d'explications au dessus de la fonction a completer : sGetPathBetween2MostOppositeVerticesOnMesh
public class Step9_findMostDistantPoints_incomplet : MonoBehaviour
{
	Mesh3D mMesh;

	// Use this for initialization
	void Start()
	{
		// positionner camera
		var lCamera = CameraHlp.setup3DCamera();
		lCamera.transform.position = new Vector3(0,34,-68);
		lCamera.backgroundColor = Color.cyan;


		var lGo = Resources.Load("GermanShephardLowPoly") as GameObject;
		if( null == lGo )
		{
			Debug.LogError("Impossible de trouver le modele 3D a charger");
			enabled = false;
			return;
		}

		MeshFilter meshFilter = lGo.GetComponentInChildren<MeshFilter>();
		if( null == meshFilter || null == meshFilter.sharedMesh)
		{
			Debug.LogError("L'objet n'a pas de mesh ou de meshfilter");
			enabled = false;
			return;
		}

		mMesh = new Mesh3D();
		mMesh.mName = "MyMesh";
		mMesh.initFrom( meshFilter.sharedMesh );
		mMesh.mergePointsAtSamePositions();
		mMesh.scale( 5 );

		var lVerticesOnPath = sGetPathBetween2MostOppositeVerticesOnMesh( mMesh, mMesh.getEdges() );

		mMesh.visual_update();
		// colorie differemment les sommets du trou (bordure)
		for(int i = 0; i < lVerticesOnPath.Count; ++i)
		{
			mMesh.visual_setPointColor( lVerticesOnPath[i], Color.red); 
		}

	}

	void Update()
	{
		mMesh.visual_updateFromScene();
		mMesh.visual_update();
	}

	void OnGUI()
	{
		if( GUILayout.Button("changer taille * 2" ))
		{
			mMesh.scale(2);
			mMesh.visual_update();
		}
		if( GUILayout.Button("changer taille * 0.5" ))
		{
			mMesh.scale(0.5f);
			mMesh.visual_update();
		}
	}

	/// 1/ trouve les 2 indexes des vertices les plus eloignes (en nombre d'edge) sur pMesh
	/// 2/ renvoie un chemin le plus court entre ces 2 index
	/// par ailleurs, pEdges peut etre obtenu avec pMesh.getEdges(), mais si on veut le reutiliser apres la fonction
	///   alors c'est plus simple de le passer en parametre (puisque le calcul est assez couteux)
	public static List<int> sGetPathBetween2MostOppositeVerticesOnMesh(Mesh3D pMesh, List<Edge> pEdges)
	{
		// trouver les 2 points les plus eloignes (en nombre de sommets)
		// cette AdjacencyMatrix ne contient que des '1' en distances (et des 0 s'il n'y a pas d'edges)
		float[,] lAdjacencyMatrix = pMesh.getAdjacencyMatrix_1_1();

		// algo : 
		// 1/ prenez un point au hasard. par exemple le premier du graph.
		// 2/ trouver le point le plus eloigne de ce point (appelons le P1)
		// 3/ trouver le point le plus eloigne de P1 : appelons le P2.
		// le chemin entre P1 et P2 est le chemin le plus long!
		//
		// Comme toutes les distances sont de '1' , et que vous avez toutes les distances depuis P1, vous pouvez parcourir la liste
		// des sommets en cherchant a chaque fois le "plus loin -1" qui est voisin du precedent.
		// 
		// il faut cependant tester a chaque fois que c'est bien un voisin.
		//
		// utilisez la fonction "GraphDistance.dijkstra" pour calculer les distances entre 1 point et tous les autres du graph.

		// TODO COMPLETEZ ICI
 return new List<int>();
	}
}

