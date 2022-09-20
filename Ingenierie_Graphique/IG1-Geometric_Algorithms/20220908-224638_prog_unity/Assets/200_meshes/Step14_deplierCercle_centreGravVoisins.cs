using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// BUT : deplier le maillage sur un cercle.
/// 
///   1/ on accroche quelques points sur un cercle. (c'est deja code)
/// 
///   2/ on sait que le but c'est que 
/// 		- les points du bord reste a leur place.
/// 		- chaque point se retrouve au centre de gravite de ses voisins (et pas du triangle, mais du FAN complet que forme ses voisins)
/// 
///   3/ donc a chaque instant on essaie de bouger un peu ces points vers ce centre de gravite
/// 
/// Exercice : completer la fonction updateCentreGravVoisins()
/// 
public class Step14_deplierCercle_centreGravVoisins : MonoBehaviour
{
	Mesh3D mMesh;

	/// indice des sommets qui se trouvent a la bordure du "trou dans le maillage"
	List<int> mHole;
	/// position3D des sommets qui se trouvent a la bordure du "trou dans le maillage"
	Vector3[] mHole3D;
	public float mDeployedRadius = 1.0f;
	/// liste des sommets du maillage
	public List<Edge> mEdges;

	/// pour chaque sommet, sa distance jusqu'a chaque point de mHole
	List<float[]> mVertexDistancesFromHoles;

	/// (distance average, numero sommet)
	List<Vector2> mAverageDistanceToBorders;

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
		mMesh.mName = "MeshThatWillBeCut";
		mMesh.initFrom( meshFilter.sharedMesh );
		mMesh.mergePointsAtSamePositions();
		mMesh.scale( 5 );

		// faire un cut
		{
			// definir l'ensemble des edges du cut
			bool useLongCut = true;
			var lCut = new List<Edge>();
			if(useLongCut)
			{
				var lEdges = mMesh.getEdges();
				// recuperer les 2 points les plus eloignes
				var lVerticesOnPath = Step9_findMostDistantPoints.sGetPathBetween2MostOppositeVerticesOnMesh( mMesh, lEdges );
				// et on met tout cela dans le cut
				for(int v = 0; v < lVerticesOnPath.Count -1; ++v)
				{
					var lEdge = new Edge( lVerticesOnPath[v], lVerticesOnPath[v+1] );
					bool alreadyExists = null != lEdges.Find( (e) =>{ return e.isSame_u(lEdge);} );
					if( alreadyExists )
					{
						lCut.Add( lEdge );
					}else{
						Debug.LogError("impossible de trouver un edge entre "+ lEdge.a +" et "+ lEdge.b);
						break;
					}
				}
			}

			if( lCut.Count == 0 )
			{	
				// si la premiere n'est pas codee, c'est pas grave
				// on va mettre quelques valeurs en dur
				lCut.Add( new Edge( 20, 19) );
				lCut.Add( new Edge( 19, 18) );

				lCut.Add( new Edge( 18, 24) );
				lCut.Add( new Edge( 24, 25) );
				lCut.Add( new Edge( 24, 23) );
			}

			// effectuer le cut, en doublonnant des sommets, et donc creant un trou dans le mesh ferme
			Step10_createCut_geometryImage.createCut( lCut, mMesh );
		}

		// trouver la succession de sommet qui forme une boucle
		{
			mHole =  Step11_findHole_geometryImage.sFindHole( mMesh );

			// afficher un petit message a ce sujet
			string message = "Le hole contient les sommets : (";
			for(int h = 0; h < mHole.Count; ++h)
			{
				message += (" "+mHole[h].ToString() );
			}
			message += ")";
			Debug.Log(message);
		}


		// optionnellement : supprimer les vertex qui ne servent plus
		{
			List<int> removedVertices = Step10_createCut_geometryImage.sRemoveUnusedVertex( mMesh );

			// les vertex supprimes ont crees des decallages dans nos indices de hole, il faut le prendre en compte
			for(int i = 0; i < removedVertices.Count; ++i)
			{
				int removedVertex = removedVertices[i];
				for(int h = 0; h < mHole.Count; ++h)
				{
					int previousHoleVertex = mHole[h];
					if( removedVertex < previousHoleVertex )
					{
						mHole[h] = previousHoleVertex - 1;
					}
				}
			}
		}

		mEdges = mMesh.getEdges();

		// calculer pour chaque hole, la distance de celui-ci a chaque point du graph.
		{
			List<float[]> distancesFromHoles = new List<float[]>();
			float[,] lAdjacencyMatrix = mMesh.getAdjacencyMatrix();
			foreach( int indexVertexHole in mHole)
			{
				int nbVertices = mMesh.mPoints.Count;

				float[] lDistancesFromHole = GraphDistance.dijkstra( nbVertices, lAdjacencyMatrix, indexVertexHole );
				distancesFromHoles.Add( lDistancesFromHole );
			}
			mVertexDistancesFromHoles = distancesFromHoles;
		}

		// calculer pour chaque vertex, la distance minimum a une bordure du graph
		{
			float[,] lAdjacencyMatrix = mMesh.getAdjacencyMatrix();

			// on va mettre juste des 1 et des 0 dans cette version de la matrice, pas des vrai distances
			for(int i = 0; i < mMesh.mPoints.Count; ++i)
			{
				for(int j = 0; j < mMesh.mPoints.Count; ++j)
				{
					if( lAdjacencyMatrix[i,j] != 0.0f)
					{
						lAdjacencyMatrix[i,j] = 1.0f;
					}
				}
			}

			List<float[]> eachHoleDistanceToVertex = new List<float[]>( mHole.Count );
			foreach( int indexVertexHole in mHole)
			{
				int nbVertices = mMesh.mPoints.Count;

				float[] lDistancesFromHole = GraphDistance.dijkstra( nbVertices, lAdjacencyMatrix, indexVertexHole );
				eachHoleDistanceToVertex.Add( lDistancesFromHole );
			}

			// trouver la distance average : 
			//  [distance, numeroSommet]
			List<Vector2> distanceToBordersAverage = new List<Vector2>();
			for(int v= 0; v < mMesh.mPoints.Count; ++v)
			{
				float average_distance = 0;
				for(int h = 0; h < eachHoleDistanceToVertex.Count; ++h)
				{
					float distanceToThatHole = eachHoleDistanceToVertex[h][v];
					average_distance += distanceToThatHole; 
				}
				distanceToBordersAverage.Add( new Vector2(average_distance / (float)eachHoleDistanceToVertex.Count, (float)v) );
			}
			mAverageDistanceToBorders = distanceToBordersAverage;
		}

		mMesh.visual_update();
		// colorie differemment les sommets du trou (bordure)
		for(int i = 0; i < mHole.Count; ++i)
		{
			mMesh.visual_setPointColor( mHole[i], Color.red); 
		}

	}

	void Update()
	{
		// affichage sous forme de lignes colorees
		System.Random rand = new System.Random(55);
		foreach( var lEdge in mEdges )
		{
			var lLine3D = lEdge.getLine3D( mMesh.mPoints );
			float col_r = ((float)(rand.Next(255)))/255.0f;
			float col_g = ((float)(rand.Next(255)))/255.0f;
			float col_b = ((float)(rand.Next(255)))/255.0f;
			Debug.DrawLine( lLine3D.p1, lLine3D.p2, new Color( col_r, col_g, col_b ));
		}

	}

	void OnGUI()
	{
		if( GUILayout.RepeatButton("Disposer le trou sur un cercle"))
		{
			// met le hole sur un cercle
			deployHoleCircle();
			mMesh.visual_update();
		}

		if( GUILayout.RepeatButton("Tenter de deployer avec les barycentres des 'distances jusqu au bord' "))
		{
			deployBaryDistance();
			mMesh.visual_update();
		}

		if( GUILayout.RepeatButton("Mettre en grille les sommets qui ne sont pas sur le trou"))
		{
			int nodeCount = mMesh.mPoints.Count;
			int sqrtNodeCount = (int) Mathf.Sqrt( (float)nodeCount);
			for(int n = 0; n < nodeCount; ++n)
			{
				mMesh.mPoints[n] = new Vector3( n % sqrtNodeCount, n /sqrtNodeCount, 0) 
					- new Vector3(sqrtNodeCount/2,sqrtNodeCount/2,0); // pour le centrer sur 0
			}

			for(int h = 0; h < mHole.Count; ++h)
			{
				int indexVertexHole = mHole[h];
				mMesh.mPoints[ indexVertexHole ] = mHole3D[h];
			}

			mMesh.visual_update();
		}

		if( GUILayout.RepeatButton("Bouger grace au centre de gravite des voisins immediats" ))
		{
			updateCentreGravVoisins();
			mMesh.visual_update();
		}

		if( GUILayout.RepeatButton("changer taille * 2" ))
		{
			mMesh.scale(2);
			updateHole3Dvalues();
			mMesh.visual_update();
		}
		if( GUILayout.RepeatButton("changer taille * 0.5" ))
		{
			mMesh.scale(0.5f);
			updateHole3Dvalues();
			mMesh.visual_update();
		}
	}

	/// met a jour mHole3D pour qu'il soit pareil que les valeurs dans mMesh.
	/// typiquement, il faut l'appeler apres avoir bouge des sommets (ou translate, scale, rotate du mesh...).
	public void updateHole3Dvalues()
	{
		for(int h = 0; h < mHole.Count; ++h)
		{
			mHole3D[h] = mMesh.mPoints[ mHole[h] ];
		}
	}

	// met les sommets du trou sur un cercle
	public void deployHoleCircle()
	{
		// creons un cercle a partir de notre hole
		mHole3D = new Vector3[mHole.Count];
		for(int h = 0; h < mHole.Count; ++h)
		{
			int indexVertex = mHole[h];

			float angle_deg = ((float)h/(float)mHole.Count) * 360.0f;
			float angle_rad = angle_deg * Mathf.Deg2Rad; 
			Vector3 lNewPoint = new Vector3( Mathf.Cos( angle_rad ), Mathf.Sin( angle_rad), 0.0f)* mDeployedRadius;

			mMesh.mPoints[ indexVertex ] = lNewPoint;
			mHole3D[h] = lNewPoint;
		}
	}

	/// calculer pour chaque hole, la distance de celui-ci a chaque point du graph.
	/// puis, exprime la position de chaque point du mesh dans le repere barycentrique du polygone forme par le hole
	public void deployBaryDistance()
	{
		// calcule les positions des points du graph avec des barycentres, dont les poids sont lies a nos distances
		// cela permettra un premier jet interessant
		for(int v = 0; v < mMesh.mPoints.Count; ++v)
		{
			if( mHole.Contains(v))
			{
				// lui reste lui meme, c'est deja un truc du bord
				continue;
			}

			float sumQuantity = 0.0f;
			Vector3 lVector3D = Vector3.zero;
			for(int h = 0; h < mVertexDistancesFromHoles.Count; ++h)
			{
				float distance = mVertexDistancesFromHoles[h][v];
				if( distance == float.MaxValue || distance == 0)
				{
					// vertex seul ou pb.
					sumQuantity = 1.0f;
					break;
				}
				float quantity = 1 / distance;
				Vector3 lHole3D = mHole3D[h];
				lVector3D = lVector3D + quantity * lHole3D;
				sumQuantity += quantity;
			}

			mMesh.mPoints[v] = lVector3D / sumQuantity;
		}
	}

	/// rapproche chaque vertex du centre de gravite forme par la "boucle des sommets auxquels il est rattache"
	public void updateCentreGravVoisins()
	{
		//TOREMOVEFOREXERCISES_BEGIN

		// pour chaque sommet
		int nbSommets = mMesh.mPoints.Count;
		for(int s = 0; s < nbSommets; ++s)
		{
			// si ce sommet appartient a la boundary, alors on passe au suivant
			if( mHole.Contains( s ))
			{
				continue;
			}

			// on va calculer la somme de tous les voisins et faire la moyenne ensuite.
			int nombreDeVoisins = 0;
			Vector3 positionsVoisins = Vector3.zero;

			// parcourir tous les edges
			for(int e = 0; e < mEdges.Count; ++e)
			{
				// a chaque fois qu'il existe un voisin, accumuler les positions des voisins et leur nombre.
				var lEdge = mEdges[e];
				if( lEdge.a == s)
				{
					positionsVoisins += mMesh.mPoints[lEdge.b];
					++nombreDeVoisins;
				}else if( lEdge.b == s)
				{
					positionsVoisins += mMesh.mPoints[lEdge.a];
					++nombreDeVoisins;
				}
			}

			if( nombreDeVoisins == 0)
			{
				continue;
			}

			// calculer la moyenne
			Vector3 positionsMoyenneDesVoisins = positionsVoisins * 1.0f/(float)nombreDeVoisins;
			mMesh.mPoints[s] = positionsMoyenneDesVoisins;
		}

		//TOREMOVEFOREXERCISES_END Debug.Log(" a coder !");
	}
}
