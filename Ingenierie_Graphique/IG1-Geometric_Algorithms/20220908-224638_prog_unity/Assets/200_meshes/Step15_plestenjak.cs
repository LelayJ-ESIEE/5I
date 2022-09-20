using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 2 algorithmes de depliage par iteration :
/// 
/// 
/// 1/ Algorithm decrit par Bor Plestenjak dans "An Algorithm for Drawing Planar Graph" de University of Ljubljana
/// https://www.fmf.uni-lj.si/~plestenjak/Papers/schlegel.pdf
///    note : cet algo est bof.
/// 
/// 
/// 2/ algorithme base sur les centre de gravite. que l'on fait par petites iterations.
///    note : cet algo est pas mal du tout.
/// 
public class Step15_plestenjak : MonoBehaviour 
{
	Mesh3D mMesh;
	List<int> mHole;
	Vector3[] mHole3D;
	public float mDeployedRadius = 1.0f;
	public List<float> mOriginalLineLengths;
	public List<Edge> mEdges;
	public float mProgressiveForce = 1.0f;
	List<float[]> mVertexDistancesFromHoles;
	List<float> mMinimumDistanceToBorders;

	/// (distance average, numero sommet)
	List<Vector2> mAverageDistanceToBorders;

	// un systeme masse ressort
	SM_Graph mSMGraph;

	public float mSM_kCoeff = 0.01f;
	public float mSM_kDamping = 0.01f;
	public float mSM_kDampingNode = 0.01f;

	public int mPlastenjakIteration = 0;

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
			var lCut = new List<Edge>();
			lCut.Add( new Edge( 20, 19) );
			lCut.Add( new Edge( 19, 18) );

			lCut.Add( new Edge( 18, 24) );
			lCut.Add( new Edge( 24, 25) );
			//lCut.Add( new Link( 24, 23) );

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


		// noter la taille originale des liens
		mEdges = mMesh.getEdges();
		mOriginalLineLengths = new List<float>();
		for(int l = 0; l<mEdges.Count; ++l)
		{
			var lLine3D = mEdges[l].getLine3D( mMesh.mPoints );
			mOriginalLineLengths.Add( lLine3D.getLength() );
		}

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
			List<float> distanceToBorders = new List<float>( mMesh.mPoints.Count );

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

			// trouver la distance mini
			for(int v= 0; v < mMesh.mPoints.Count; ++v)
			{
				float min_distance = 10000;
				for(int h = 0; h < eachHoleDistanceToVertex.Count; ++h)
				{
					float distanceToThatHole = eachHoleDistanceToVertex[h][v];
					if( min_distance < distanceToThatHole )
					{
						min_distance = distanceToThatHole;
					}
				}
				distanceToBorders.Add( min_distance );
			}
			mMinimumDistanceToBorders = distanceToBorders;

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

		// met le hole sur un cercle
		deployHoleCircle();

		deployBaryDistance();


		// creer notre graph de spring
		mSMGraph = new SM_Graph();
		mSMGraph.initNodesFrom( mMesh.mPoints, null ); 
		mSMGraph.initSpringsFrom( mEdges, null);


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
			Debug.DrawLine( lLine3D.p1 * 500.0f, lLine3D.p2 * 500.0f, new Color( col_r, col_g, col_b ));
		}

	}

	void OnGUI()
	{
		if( GUILayout.RepeatButton("mettre en 0"))
		{
			for(int n = 0; n < mSMGraph.mNodes.Count; ++n)
			{
				mSMGraph.mNodes[n].pos = Vector3.zero;
			}

			for(int h = 0; h < mHole.Count; ++h)
			{
				int indexVertexHole = mHole[h];
				var lNodeHode = mSMGraph.mNodes[ indexVertexHole ];
				lNodeHode.accel = Vector3.zero;
				lNodeHode.vitesse = Vector3.zero;
				lNodeHode.pos = mHole3D[h];
			}

			mSMGraph.applyTo( mMesh );
		}

		if( GUILayout.RepeatButton("mettre en grille"))
		{
			int nodeCount =mSMGraph.mNodes.Count;
			int sqrtNodeCount = (int) Mathf.Sqrt( (float)nodeCount);
			for(int n = 0; n < mSMGraph.mNodes.Count; ++n)
			{
				mSMGraph.mNodes[n].pos = new Vector3( n % sqrtNodeCount, n /sqrtNodeCount, 0) 
					- new Vector3(sqrtNodeCount/2,sqrtNodeCount/2,0); // pour le centrer sur 0
			}

			for(int h = 0; h < mHole.Count; ++h)
			{
				int indexVertexHole = mHole[h];
				var lNodeHode = mSMGraph.mNodes[ indexVertexHole ];
				lNodeHode.accel = Vector3.zero;
				lNodeHode.vitesse = Vector3.zero;
				lNodeHode.pos = mHole3D[h];
			}

			mSMGraph.applyTo( mMesh );
		}


		if( GUILayout.RepeatButton("Platenjak iteration (with new iter) " + mPlastenjakIteration ))
		{
			updatePlestenjak( mPlastenjakIteration );
			++mPlastenjakIteration;
		}

		if( GUILayout.RepeatButton("Platenjak iteration (keep iter) " + mPlastenjakIteration ))
		{
			updatePlestenjak( mPlastenjakIteration );
		}

		if( GUILayout.RepeatButton("technique des centre de gravite des voisins" ))
		{
			updateCentreGravVoisins();
		}
	}

	/// rapproche chaque vertex du centre de gravite forme par la "boucle des sommets auxquels il est rattache"
	public void updateCentreGravVoisins()
	{
		//TOREMOVEFOREXERCISES_BEGIN

		// pour chaque edge, du plus proche du bord au plus lointain
		// 		trouver ses voisins et calculer la position moyenne
		mAverageDistanceToBorders.Sort( (a,b)=>{return a.x.CompareTo(b.x);} );

		for(int i = 0; i< mAverageDistanceToBorders.Count; ++i)
		{
			float averageDist = mAverageDistanceToBorders[i].x;
			int indexVertex = (int)mAverageDistanceToBorders[i].y;
			if( mHole.Contains(indexVertex))
			{
				continue;
			}

			Vector3 averagePositionOfNeighbours = mMesh.mPoints[indexVertex];
			float nbNeighbours = 1.0f;

			// notre multiplicateur de force
			float quantiteParVoisin = Mathf.Pow(1.0f + averageDist, 10.0f);
			foreach( var lEdge in mEdges)
			{
				if( lEdge.a == indexVertex )
				{
					averagePositionOfNeighbours += mMesh.mPoints[lEdge.b] * quantiteParVoisin;
					nbNeighbours += quantiteParVoisin;
				}else if( lEdge.b == indexVertex )
				{
					averagePositionOfNeighbours += mMesh.mPoints[lEdge.a] * quantiteParVoisin;
					nbNeighbours += quantiteParVoisin;
				}
			}

			if( nbNeighbours == 0)
			{
				continue;
			}

			var lNewPoint = (averagePositionOfNeighbours / nbNeighbours);
			var lPreviousPos = mMesh.mPoints[ indexVertex ];

			mMesh.mPoints[ indexVertex ] = lPreviousPos + (lNewPoint - lPreviousPos) * 1.5f;
		}

		//TOREMOVEFOREXERCISES_END Debug.Log(" a coder !");
	}


	/// algorithm decrit par bor Plestenjak dans "An Algorithm for Drawing Planar Graph" de University of Ljubljana
	public void updatePlestenjak( int pCurrentIteration )
	{
		int nbVertices = mSMGraph.mNodes.Count ;

		// step (a)
		List<Vector3> verticeForce = new List<Vector3>( nbVertices );
		for(int n = 0; n < mSMGraph.mNodes.Count; ++n)
		{
			verticeForce.Add( Vector3.zero );
		}

		// step (b)
		float C = Mathf.Sqrt( (float)nbVertices / Mathf.PI ); // original

		float maxPeriphericity = max(mMinimumDistanceToBorders);
		float APeriph = 2.5f;
		float powerUV = 7.0f; // original etait 3

		for(int l = 0; l < mEdges.Count; ++l)
		{
			var link = mEdges[l];
			var lNodeU = mSMGraph.mNodes[link.a]; 
			var lNodeV = mSMGraph.mNodes[link.b]; 


			// periphericity : on change C
			float uPeriph = mMinimumDistanceToBorders[link.a];
			float vPeriph = mMinimumDistanceToBorders[link.b];
			C = (Mathf.Sqrt( ((float)nbVertices) / Mathf.PI)) * Mathf.Exp( APeriph * ((2 * maxPeriphericity - uPeriph - vPeriph )/maxPeriphericity) );


			Vector3 UV = (lNodeV.pos - lNodeU.pos); 
			//Vector3 UV_AuCube = new Vector3( Mathf.Pow( UV.x, powerUV ), Mathf.Pow( UV.y, powerUV ), Mathf.Pow( UV.z, powerUV ) );
			Vector3 UV_AuCube = Mathf.Pow(UV.magnitude, powerUV ) * UV.normalized;
			Vector3 ForceUV = UV_AuCube * C;

			verticeForce[link.a] += ForceUV;
			verticeForce[link.b] -= ForceUV;
		}


		// step (c)
		float cool_numerator = (Mathf.Sqrt( Mathf.PI/ (float)nbVertices) );
		float cool_denominator = 1 + ((Mathf.PI/ (float)nbVertices)) * Mathf.Pow( (float) pCurrentIteration, 1.5f );
		float cool = cool_numerator / cool_denominator ;

		for(int n = 0; n < mSMGraph.mNodes.Count; ++n)
		{
			if( mHole.Contains( n ) )
			{
				continue;
			}

			var lNode = mSMGraph.mNodes[n];
			var lForce = verticeForce[n];
			float lForceLength = lForce.magnitude;

			lNode.pos = lNode.pos + (Mathf.Min( lForceLength, cool) * (lForce / lForceLength));
		}

		mSMGraph.applyTo( mMesh );
	}


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

	// le maximum
	public float max(float[] p)
	{
		float res = float.MinValue;
		for(int i = 0; i < p.Length; ++i)
		{
			res = Mathf.Max(res, p[i]);
		}
		return res;
	}

	public float max(List<float> p)
	{
		float res = float.MinValue;
		for(int i = 0; i < p.Count; ++i)
		{
			res = Mathf.Max(res, p[i]);
		}
		return res;
	}

	public float min(List<float> p)
	{
		float res = float.MaxValue;
		for(int i = 0; i < p.Count; ++i)
		{
			res = Mathf.Min(res, p[i]);
		}
		return res;
	}


	// calcul grossier de la moyenne
	public float average(float[] p)
	{
		return sum(p)/(float)p.Length; 
	}
	public float average(List<float> p)
	{
		return sum(p)/(float)p.Count; 
	}


	public float sum(float[] p)
	{
		float res = 0;
		for(int i = 0; i < p.Length; ++i)
		{
			res += p[i];
		}
		return res; 
	}


	public float sum(List<float> p)
	{
		float res = 0;
		for(int i = 0; i < p.Count; ++i)
		{
			res += p[i];
		}
		return res; 
	}


}
