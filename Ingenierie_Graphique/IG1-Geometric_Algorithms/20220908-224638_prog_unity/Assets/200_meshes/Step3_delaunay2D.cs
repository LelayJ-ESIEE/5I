using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// completez la fonction "delaunay2D" qui fabrique une triangulation 2D de delaunay,
///  a partir d'un ensemble de points.
public class Step3_delaunay2D : MonoBehaviour
{
	/// notre graph
	public Graph mGraph = null;

	/// faciliter le dessin des triangles
	public TriangleDrawer mTriangleDrawer = null;

	public List<Triangle3D> mLastTriangulation = null;

	// Use this for initialization
	void Start()
	{
		/// je veux que l'aleatoire s'execute toujours pareil
		System.Random random = new System.Random(6779);

		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		mTriangleDrawer = new TriangleDrawer();
		mTriangleDrawer.setDoubleFace( true );

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("PointCloud");

		int variation = 1;
		int limit = 18;
		// je cree des points
		for(int i = -limit; i < limit; i+= 3)
		{
			for(int j = -limit; j < limit; j+=3)
			{
				if(random.Next( 0, 3) == 1)
				{
					continue;
				}

				float lMoveX = (float)random.Next(-variation,variation);
				float lMoveY = (float)random.Next(-variation,variation);
				float lMoveZ = (float)random.Next(-variation,variation);

				Vector3 lXYZ = new Vector3( i+lMoveX, j+lMoveY, lMoveZ ) * 0.8f;
				mGraph.mPoints.Add( lXYZ );
			}
		}

		// afficher le graph dans la 3D, en creant des objets si besoin : cote visuel
		mGraph.updateScene();
		for(int i = 0; i < mGraph.mPoints.Count; ++i)
		{
			mGraph.getVisualPoint(i).transform.localScale = Vector3.one * 0.5f;
		}


		Debug.Log("Touche espace pour lancer le calcul de Delaunay.");
	}

	void OnGUI()
	{
		GUILayout.Label("touche espace pour lancer le calcul de Delaunay");
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
		if( Input.GetMouseButton(0))
		{
			// obtenir la position 3D de la souris (projection sur le plan Z = 0)
			var lMousePosition3D = MouseHlp.getPositionOnZ();

			// trouver le numero du point le plus proche
			int indexPoint = mGraph.getClosestPointFrom( lMousePosition3D );

			// le mettre a la meme position que la souris 3D
			mGraph.mPoints[indexPoint] = lMousePosition3D;
		}


		// si on appuie sur espace => on effectue une triangulation de delaunay
		if( Input.GetKeyDown(KeyCode.Space))
		{
			// on effectue le calcul de delaunay
			var newTriangles = delaunay2D( mGraph );
			if( newTriangles != null && newTriangles.Count > 0)
			{
				mLastTriangulation = newTriangles;

				mTriangleDrawer.beginAddingTri();
				for(int t = 0; t < newTriangles.Count; ++t)
				{
					newTriangles[t].color = Random.ColorHSV();
					mTriangleDrawer.addTriangle( newTriangles[t].getReduced(0.2f) );
				}
				mTriangleDrawer.updateMesh();
				Debug.Log("fin de la triangulation de delaunay");


				// petite verification du cercle circonscrit au triangle
				for(int p = 0; p < mGraph.mPoints.Count; ++p)
				{
					var P = mGraph.mPoints[p];
					foreach( var T in newTriangles)
					{
						if( T.hasVertex( P ))
						{
							continue;
						}
						if( Step1_cercle_circonscrit2D.isInCircumcircle_XY( T.A, T.B, T.C, P) )
						{
							int indexP = mGraph.getClosestPointFrom(P);
							int indexA = mGraph.getClosestPointFrom(T.A);
							int indexB = mGraph.getClosestPointFrom(T.B);
							int indexC = mGraph.getClosestPointFrom(T.C);

							Debug.Log("Pas bien : le sommet "+ indexP +	" est dans le cercle circonscrit au triangle ("+ indexA+", "+indexB+", "+indexC+")");
						}
					}
				}
			}
		}


		/// un exemple d'usage de l'algorithme du flipping
		if( Input.GetKeyDown(KeyCode.T) && mLastTriangulation != null)
		{
			bool anyBreak = false;
			// petite verification du cercle circonscrit au triangle pour chaque element du graph
			for(int p = 0; p < mGraph.mPoints.Count; ++p)
			{
				Vector3 P = mGraph.mPoints[p];
				for(int t = 0; t < mLastTriangulation.Count; ++t)
				{
					var T = mLastTriangulation[t];
					if( T.hasVertex( P ))
					{
						continue;
					}
					if( Step1_cercle_circonscrit2D.isInCircumcircle_XY( T.A, T.B, T.C, P) )
					//if( Step1_cercle_circonscrit3D.isInCircumcircle_3D( T.A, T.B, T.C, P, Camera.main.transform.position) )
					{
						int indexP = mGraph.getClosestPointFrom(P);
						int indexA = mGraph.getClosestPointFrom(T.A);
						int indexB = mGraph.getClosestPointFrom(T.B);
						int indexC = mGraph.getClosestPointFrom(T.C);

						Debug.Log("Pas bien : le sommet "+ indexP +	" est dans le cercle circonscrit au triangle ("+ indexA+", "+indexB+", "+indexC+")");
						Debug.Log("Les 4 points ont les coordonnees   P:"+ P+"    ABC : "+T.A+" "+T.B+" "+T.C);

						// on effectue un flip si possible.
						int foundABP = 	mLastTriangulation.FindIndex( (tri)=>{ return tri.isSameAs( T.A, T.B, P );});
						int foundBCP = 	mLastTriangulation.FindIndex( (tri)=>{ return tri.isSameAs( T.B, T.C, P );});
						int foundCAP = 	mLastTriangulation.FindIndex( (tri)=>{ return tri.isSameAs( T.C, T.A, P );});

						if( foundABP != -1 )
						{
							mLastTriangulation.Add( new Triangle3D( T.A, P, T.C ));
							mLastTriangulation.Add( new Triangle3D( T.B, P, T.C ));
							if( t > foundABP )
							{
								mLastTriangulation.RemoveAt( t );
								mLastTriangulation.RemoveAt( foundABP );
							}else{
								mLastTriangulation.RemoveAt( foundABP );
								mLastTriangulation.RemoveAt( t );
							}
							
							anyBreak  = true;
							Debug.Log("On effectue un flip selon les points AB : "+ mGraph.getClosestPointFrom(T.A) +" " +mGraph.getClosestPointFrom(T.B) );
							break;
						}

						if( foundBCP != -1 )
						{
							mLastTriangulation.Add( new Triangle3D( T.A, P, T.B ));
							mLastTriangulation.Add( new Triangle3D( T.A, P, T.C ));

							if( t > foundBCP )
							{
								mLastTriangulation.RemoveAt( t );
								mLastTriangulation.RemoveAt( foundBCP );
							}else{
								mLastTriangulation.RemoveAt( foundBCP );
								mLastTriangulation.RemoveAt( t );
							}

							anyBreak  = true;
							Debug.Log("On effectue un flip selon les points BC : "+ mGraph.getClosestPointFrom(T.B) +" " +mGraph.getClosestPointFrom(T.C) );
							break;
						}

						if( foundCAP != -1 )
						{
							mLastTriangulation.Add( new Triangle3D( T.C, P, T.B ));
							mLastTriangulation.Add( new Triangle3D( T.A, P, T.B ));

							if( t > foundCAP )
							{
								mLastTriangulation.RemoveAt( t );
								mLastTriangulation.RemoveAt( foundCAP );
							}else{
								mLastTriangulation.RemoveAt( foundCAP );
								mLastTriangulation.RemoveAt( t );
							}

							anyBreak  = true;
							Debug.Log("On effectue un flip selon les points CA : "+ mGraph.getClosestPointFrom(T.C) +" " +mGraph.getClosestPointFrom(T.A) );
							break;
						}
					}
				}
				if( anyBreak )
				{
					break;
				}
			}

			// MAJ du dessin
			mTriangleDrawer.beginAddingTri();
			for(int t = 0; t < mLastTriangulation.Count; ++t)
			{
				//mLastTriangulation[t].color = Random.ColorHSV();
				mTriangleDrawer.addTriangle( mLastTriangulation[t].getReduced(0.2f) );
			}
			mTriangleDrawer.updateMesh();

			Debug.Log("fin de la recherche de diamants a flipper");
		}


		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}

	/// algo de bowyer-watson
	/// wiki : https://en.wikipedia.org/wiki/Bowyer%E2%80%93Watson_algorithm
	/// video : https://www.youtube.com/watch?v=GctAunEuHt4
	/// 
	///  1/ creer un gros "triangle geant" qui contient tout (il sera supprime a la fin)
	/// 
	///  2/ pour chaque point
	/// 
	///     3/ trouver tous les triangles pour lesquels P est dans leur cercle circonscrit.
	///        les noter dans un coin (badTriangles), et les supprimer de la liste des bons triangles.
	///
	///     4/ trouver tous les edges des badTriangles qui ne sont pas partages par des badTriangles. 
	///        (c'est le "pourtours" du trou que l'on veut garder, on peut les appeler 'goodEdges')
	/// 
	///     5/ pour chaque edge a garder, creer un triangle avec notre point P.
	/// 
	///     remarque: pour chaque point on aura donc une nouvelle liste de "mauvais triangles" et une nouvelle liste de "bons edges"
	/// 
	///  6/ quand la boucle est finie, supprimer les triangles qui utilisent des sommets du "triangle geant" du depart
	/// 
	public static List<Triangle3D> delaunay2D(Graph pGraph)
	{
		List<Triangle3D> created_triangles = new List<Triangle3D>();

		// TODO ici etudiants, il faut completer la fonction

		//TOREMOVEFOREXERCISES_BEGIN

		// 1/ creer un gros "triangle geant" qui contient tout (il sera supprime a la fin)
		Triangle3D tri_geant = new Triangle3D();
		tri_geant.A = new Vector3(-500,-500,0);
		tri_geant.B = new Vector3(0, 500,0);
		tri_geant.C = new Vector3(500,-500,0);
		tri_geant.color = Color.red;

		created_triangles.Add( tri_geant );

		// 2/ pour tous les triangles deja obtenus
		for(int p = 0; p < pGraph.mPoints.Count; ++p)
		{
			var lPoint = pGraph.mPoints[p];

			// 3/ trouver tous les triangles pour lesquels P est dans leur cercle circonscrit.
			List<Triangle3D> badTriangles = new List<Triangle3D>();
			for(int t = 0; t < created_triangles.Count; ++t)
			{
				Triangle3D toTest = created_triangles[t];

				if(Step1_cercle_circonscrit2D.isInCircumcircle_XY( toTest.A, toTest.B, toTest.C, lPoint))
				{
					badTriangles.Add( toTest );
					// je supprime le mauvais triangle
					created_triangles.RemoveAt( t );
					--t;
				}
			}

			// 4/ trouver tous les edges des badTriangles qui ne sont pas partages par des badTriangles.
			List<Line3D> border_edges = new List<Line3D>();

			for(int t = 0; t < badTriangles.Count; ++t)
			{
				for(int i = 0; i < 3; ++i)
				{
					Line3D lEdgeBadTriangle = badTriangles[t].getEdge(i);

					// si l'edge n'est pas deja present dans edges.
					// et dans ce cas ne garder que les good edges. => beaucoup + efficace
					int lFoundIndex = border_edges.FindIndex( 
						(edge_de_la_liste) => { return edge_de_la_liste.isSame_u(lEdgeBadTriangle); } );

					if( -1 == lFoundIndex)
					{
						border_edges.Add( lEdgeBadTriangle );
					}else{
						border_edges.RemoveAt( lFoundIndex );
					}
				}
			}

			// 5 creer un triangle par edge obtenu avec P au centre
			for(int e = 0; e < border_edges.Count; ++e)
			{
				Line3D edge = border_edges[e];
				Triangle3D lNewTriangle = new Triangle3D();
				lNewTriangle.A = edge.p1 ; // extremite du segment
				lNewTriangle.B = edge.p2 ; // extremite du segment
				lNewTriangle.C = lPoint ;

				created_triangles.Add( lNewTriangle );
			}
		}

		// 6/ supprimer les triangles dont 1 sommet appartient a tri_geant
		for(int t = created_triangles.Count-1; t >=0 ; --t)
		{
			var lTri = created_triangles[t];
			if( 	lTri.hasVertex( tri_geant.A ) 
				|| 	lTri.hasVertex( tri_geant.B )
				|| 	lTri.hasVertex( tri_geant.C )
			)
			{
				created_triangles.RemoveAt( t );
			}
		}

		//TOREMOVEFOREXERCISES_END 

		return created_triangles;
	}




}

