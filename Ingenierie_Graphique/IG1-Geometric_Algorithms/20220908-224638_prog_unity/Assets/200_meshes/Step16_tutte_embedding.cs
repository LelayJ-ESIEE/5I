using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// exemple : numerics.mathdotnet.com
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;


/// Algorithme de "tutte embedding" qui sert en general pour avoir un premier jet de "deploiement des Uvs" sur un mesh
/// la page wikipedia a ce sujet est malheureusement tres mal foutue.
public class Step16_tutte_embedding : MonoBehaviour 
{
	/// notre maillage
	Mesh3D mMesh;

	/// servira pour tutte : liste des sommets du mesh, il est rempli uniquement a la fin de l'init
	List<Edge> mEdges;

	/// le trou dans le maillage que l'on va etaler en cercle
	List<int> mHole;

	/// ceci ne vous servira pas dans l'algo, c'est juste pour stocker les positions originales des points,
	/// avant d'ecrire sur le disque.
	List<Vector3> mTemporaryForWritingImage;


	// Use this for initialization
	void Start()
	{
		// positionner camera
		var lCamera = CameraHlp.setup3DCamera();
		lCamera.transform.position = new Vector3(0,34,-68);
		lCamera.backgroundColor = Color.cyan;

		bool useSimpleMesh = false;

		// charger maillage
		var lGo = Resources.Load( useSimpleMesh ? "Simple3D" : "GermanShephardLowPoly") as GameObject;
		//var lGo = Resources.Load( "bosse") as GameObject;
		//var lGo = GameObject.CreatePrimitive(PrimitiveType.Quad);

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
		mMesh.scale( 20 );
		if( useSimpleMesh )
		{
			mMesh.rotate( 0, -90, 0);
		}

		// definir l'ensemble des edges qui vont servir pour couper
		var lCut = new List<Edge>();

		if( !useSimpleMesh)
		{
			// faire un cut sur le chien
			bool useLongCut = true;
			
			// definir l'ensemble des edges du cut
			if( !useLongCut )
			{
				lCut.Add( new Edge( 20, 19) );
				lCut.Add( new Edge( 19, 18) );
				lCut.Add( new Edge( 18, 24) );
				lCut.Add( new Edge( 24, 25) );
				//lCut.Add( new Link( 24, 23) );
			}else{

				var lEdges = mMesh.getEdges();
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

			// effectuer le cut, en doublonnant des sommets, et donc creant un trou dans le mesh ferme
			Step10_createCut_geometryImage.createCut( lCut, mMesh );
		}else{
			// faire un cut sur la 3D simple
			// definir l'ensemble des edges du cut
			lCut.Add( new Edge( 6, 7) );
			lCut.Add( new Edge( 7, 8) );
			lCut.Add( new Edge( 8, 9) );
			lCut.Add( new Edge( 8, 2) );

			// effectuer le cut, en doublonnant des sommets, et donc creant un trou dans le mesh ferme
			Step10_createCut_geometryImage.createCut( lCut, mMesh );
		}

		// trouver la succession de sommets, c-a-dire ceux qui forment une boucle
		mHole = Step11_findHole_geometryImage.sFindHole( mMesh );


		// optionnellement : supprimer les vertex qui ne servent plus
		bool removeLoneVertices = false;
		if( removeLoneVertices )
		{
			List<int> removedVertices = Step10_createCut_geometryImage.sRemoveUnusedVertex( mMesh );

			// les vertex supprimes ont crees des decallages dans nos indices de hole et de cut, il faut le prendre en compte, et les corriger
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
				for(int c = 0; c < mHole.Count; ++c)
				{
					int previousCutVertex = lCut[c].a;
					if( removedVertex < previousCutVertex )
					{
						lCut[c].a = previousCutVertex - 1;
					}

					previousCutVertex = lCut[c].b;
					if( removedVertex < previousCutVertex )
					{
						lCut[c].b = previousCutVertex - 1;
					}
				}
			}
		}

		Debug.Log("nombre de sommets dans le mesh : " + mMesh.mPoints.Count );

		// afficher un petit message sur le hole
		{
			string message = "Le hole contient les sommets : (";
			for(int h = 0; h < mHole.Count; ++h)
			{
				message += (" "+mHole[h].ToString() );
			}
			message += ")";
			Debug.Log(message);
		}

		// recuperer nos edges (qui serviront pour le dessin)
		mEdges = mMesh.getEdges();

		mMesh.visual_update();
		for(int i = 0; i < mHole.Count; ++i)
		{
			mMesh.visual_setPointColor( mHole[i], Color.red); 
		}

		for(int i = 0; i < lCut.Count; ++i)
		{
			mMesh.visual_setPointColor( lCut[i].a, Color.magenta);
			mMesh.visual_setPointColor( lCut[i].b, Color.magenta);
		}
	}

	void Update()
	{
		// si on veut dessiner le mesh en 3D.
		mMesh.visual_updateFromScene();
		mMesh.visual_update();

		// affichage sous forme de lignes colorees
		{
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
	}

	void OnGUI()
	{
		if(GUILayout.Button("compter voisins de la selection"))
		{
			var lGO = UnityEditor.Selection.activeGameObject;
			if( lGO != null )
			{
				int numeroSommet = lGO.transform.GetSiblingIndex();
				Debug.Log( "nombre de voisins du sommet "+ numeroSommet+" est : "+countNeighbours( numeroSommet, mEdges) );
			}
		}

		if(GUILayout.Button("noter la position des points.. pour creation image plus tard, et translate pour etre > 0"))
		{
			// on force aussi le mesh a etre avec des valeurs positives only sur X,Y,Z.
			Vector3 lMinimum = mMesh.getMinMaxOfTriangles()[0];
			mMesh.translate( -lMinimum.x, -lMinimum.y, -lMinimum.z );
			mMesh.visual_update();

			// on effectue notre sauvegarde des positions
			mTemporaryForWritingImage = new List<Vector3>( mMesh.mPoints.Count );
			for(int i = 0; i < mMesh.mPoints.Count; ++i)
			{
				var lPoint = mMesh.mPoints[i];
				mTemporaryForWritingImage.Add( new Vector3(lPoint.x, lPoint.y, lPoint.z) );
			}
			Debug.Log("temporary contient maintenant : " + mTemporaryForWritingImage.Count+" elements.");

		}

		if(GUILayout.Button("mettre le trou sur un cercle"))
		{
			deployHoleCircle();
			mMesh.visual_update();
		}

		if(GUILayout.Button("mettre le trou sur un carre"))
		{
			deployHoleSquare();
			mMesh.visual_update();
		}

		if(GUILayout.Button("Tutte embedding"))
		{
			tutte_embedding_XY(mMesh, mHole, mEdges);
			mMesh.visual_update();
		}

		if(GUILayout.Button("generer notre geometry image grace aux points qu'on a enregistre"))
		{
			int sizeTexX = 512;
			float sizeTexX_f = (float)sizeTexX;
			Vector3[] lMinMax = mMesh.getMinMaxOfTriangles();
			Vector3 lMinMesh = lMinMax[0];
			Vector3 lMaxMesh = lMinMax[1];
			Vector3 lSizeMesh = lMaxMesh - lMinMesh;

			Vector3 lMaximum_originalValues = GVector3.sGetMaximumOf( mTemporaryForWritingImage );
			float maxValueOnSizeMesh = Mathf.Max( lMaximum_originalValues.x, lMaximum_originalValues.y, lMaximum_originalValues.z );

			Texture2D lTexture = new Texture2D( sizeTexX, sizeTexX, TextureFormat.RGBAFloat, false );

			// dira pour chaque pixel si on l'a calcule ou non. A cause d'approximation, cela peut arriver qu'on en manque
			// avec la technique du rater, donc on fera le calcul des derniers pixels differemment.
			bool[] calculatedPixels = new bool[sizeTexX * sizeTexX];

			for(int f = 0; f < mMesh.mFaces.Count; ++f)
			{
				var lFace = mMesh.mFaces[f]; 
				Vector3 A = mMesh.mPoints[lFace.a];
				Vector3 B = mMesh.mPoints[lFace.b];
				Vector3 C = mMesh.mPoints[lFace.c];
				Vector3 original_A = mTemporaryForWritingImage[lFace.a];
				Vector3 original_B = mTemporaryForWritingImage[lFace.b];
				Vector3 original_C = mTemporaryForWritingImage[lFace.c];

				// valeurs du triangle : je veux que les limites soient entre 0 et la taille de la texture
				//   pour l'ensemble des triangles.
				Triangle3D ABC = new Triangle3D(A,B,C);
				Triangle3D ABC_0_XXX = ABC.getTranslated(-lMinMesh.x, -lMinMesh.y, -lMinMesh.z);
				Triangle3D ABC_0_512 = ABC_0_XXX.getMultiplied( sizeTexX_f/lSizeMesh.x, sizeTexX_f/lSizeMesh.y, 0 );

				// jusque la cela semble OK

				// rasteriser ce triangle, et dessine donc les pixels dans la texture.
				Vector3 lMinABC_0_512 = ABC_0_512.getMinimum();
				Vector3 lMaxABC_0_512 = ABC_0_512.getMaximum();

				Vector2 A2D_0_512 = new Vector2( ABC_0_512.A.x, ABC_0_512.A.y );
				Vector2 B2D_0_512 = new Vector2( ABC_0_512.B.x, ABC_0_512.B.y );
				Vector2 C2D_0_512 = new Vector2( ABC_0_512.C.x, ABC_0_512.C.y );

				for(float y = lMinABC_0_512.y; y<=lMaxABC_0_512.y+1; ++y)
				{
					for(float x = lMinABC_0_512.x; x<=lMaxABC_0_512.x+1; ++x)
					{
						if( IntersectionHlp.isPoint2DInTriangle2D( new Vector2(x,y), A2D_0_512, B2D_0_512, C2D_0_512 ) )
						{
							int xi = (int)x;
							int yi = (int)y;
							if( xi >= 0 && xi < sizeTexX && yi >=0 && yi < sizeTexX )
							{
								calculatedPixels[xi + yi * sizeTexX] = true;

								// obtenir les coord bary
								float u = 0;
								float v = 0;
								float w = 0;
								
								IntersectionHlp.Barycentric( new Vector3(x,y,0), ABC_0_512.A, ABC_0_512.B, ABC_0_512.C, ref u, ref v, ref w);

								Vector3 original3Dpos = 
									original_A * u+ 
									original_B * v+ 
									original_C * w;


								lTexture.SetPixel(xi,yi, new Color(
									original3Dpos.x/maxValueOnSizeMesh,
									original3Dpos.y/maxValueOnSizeMesh,
									original3Dpos.z/maxValueOnSizeMesh));
								
								//lTexture.SetPixel(xi,yi, new Color(original_A.x / maxValueOnSizeMesh, original_A.y / maxValueOnSizeMesh, original_A.z / maxValueOnSizeMesh ));
							}
						}
					}
				}
			}

			// on finit les quelques points qui ont ete rates (ce calcul est beaucoup plus lent!)
			{
				Bounds boundsMesh = new Bounds();
				boundsMesh.SetMinMax( - Vector3.one * 1.1f, Vector3.one * 1.1f); 
				var lVertices = mMesh.getVerticesAsArray();
				var lIndices = mMesh.getFacesAsInts();
				float sizeStepX = 1.0f/sizeTexX_f;
				for(float j = 0; j < sizeTexX; ++j)
				{
					for(float i = 0; i < sizeTexX; ++i)
					{
						if( !calculatedPixels[((int)i)+((int)j)*sizeTexX] )
						{
							// tout est entre 1 et -1
							Vector3 position = new Vector3( -1 + 2 * i * sizeStepX, -1 + 2 * j* sizeStepX, 0);

							// trouver le triangle qui le contient (dans notre mesh etendu)
							// et les coord barycentriques.
							int foundIndexInTri;
							Vector3 bary;
							float rayDist;
							if(!IntersectionHlp.RayMeshIntersection( new Ray(position - Vector3.forward, Vector3.forward), boundsMesh,
								lVertices, lIndices, 
								out foundIndexInTri, 
								out rayDist, 
								out bary)) 
							{
								// probleme, on ne va pas y toucher pour l'instant,
								// mais on est probablement sur le bord... il faudrait prendre le point le plus proche tout simplement.
								continue;
							}

							// en deduire la position 3D correspondante, en prenant dans ce qu'on avait enregistre precedemment
							Vector3 correspondingOriginal3Dpos = 
								bary.x * mTemporaryForWritingImage[lIndices[foundIndexInTri]] + 
								bary.y * mTemporaryForWritingImage[lIndices[foundIndexInTri+1]] + 
								bary.z * mTemporaryForWritingImage[lIndices[foundIndexInTri+2]];

							// mettre cette valeur dans notre texture
							lTexture.SetPixel( (int)i, (int)j, new Color(
								correspondingOriginal3Dpos.x, 
								correspondingOriginal3Dpos.y, 
								correspondingOriginal3Dpos.z));
						}
					}
				}
			}

			// supprimer les imperfections (c'est juste un cours, on fait cela a "la grosse maille")
			TextureHlp.filtreMedian( lTexture );
			TextureHlp.doubleLeftBorder( lTexture );
			TextureHlp.doubleBottomBorder( lTexture );

			// dans unity il faut appeler Apply pour que la texture soit envoyee mise a jour a la carte graphique.
			lTexture.Apply();

			var bytesGeometryImage1 = lTexture.EncodeToPNG();
			System.IO.File.WriteAllBytes("GeneratedGeometryImage.png", bytesGeometryImage1);
			var bytesGeometryImage2 = lTexture.EncodeToEXR();
			System.IO.File.WriteAllBytes("GeneratedGeometryImage.exr", bytesGeometryImage2);
		}

		if(GUILayout.Button("scale mesh * 2"))
		{
			mMesh.scale(2);
			mMesh.visual_update();
		}

		if(GUILayout.Button("scale mesh * 0.5"))
		{
			mMesh.scale(0.5f);
			mMesh.visual_update();
		}
	}

	/// compte le nombre de voisins de v.
	/// (on suppose pas de doublons dans pEdge, ni d'edge de type "v-v")
	public static int countNeighbours(int v, List<Edge> pEdges)
	{
		int result = 0;
		foreach( var e in pEdges)
		{
			if( e.a == v || e.b == v )
			{
				++result;
			}
		}
		return result;
	}

	/// deploie les elements du mesh (si possible) autour du hole.
	/// le calcul est fait pour les coordonnees XY.
	/// en general on va disposer border sur une forme de CERCLE ou de CARRE
	public static void tutte_embedding_XY(Mesh3D pMesh, List<int> pBorder, List<Edge> pEdges)
	{
		int nbPointsTotal = pMesh.mPoints.Count;
		int nbPointsOutsideBorder = nbPointsTotal - pBorder.Count;

		var lUnusedVertices = pMesh.getUnusedVertices();

		// chaque ligne de Ax sera 1 list<float>
		// (note : ce constructeur fait juste une alloc memoire, mais ne cree aucune case dans la liste)
		List<List<float>> A = new List<List<float>>(nbPointsOutsideBorder );
			
		// colonne resultat
		// (note : ce constructeur fait juste une alloc memoire, mais ne cree aucune case dans la liste)
		List<float> Bx = new List<float>(nbPointsOutsideBorder );
		List<float> By = new List<float>(nbPointsOutsideBorder );


		//TOREMOVEFOREXERCISES_BEGIN

		for(int pL = 0; pL < nbPointsTotal; ++pL)
		{
			if( lUnusedVertices.Contains( pL))
			{
				// ce sommet ne nous servira pas du tout de toute facon
				continue;
			}
			if( pBorder.Contains( pL ))
			{
				// pas besoin d'ajouter cette ligne
				// car les bordures ne verifient pas l'equation
				continue;
			}

			// prepare la ligne de la matrice
			List<float> newLineA = new List<float>();

			// Bx et By etant des colonnes, la ligne ne contient qu'une seule valeur.
			float newLineBx = 0;
			float newLineBy = 0;

			// remplit la ligne de A.
			for(int pC = 0; pC < nbPointsTotal; ++pC)
			{
				if( lUnusedVertices.Contains( pC))
				{
					// ce sommet ne nous servira pas du tout de toute facon
					continue;
				}

				bool isOnBorder = pBorder.Contains( pC );
				bool areNeighbours = null != pEdges.Find( (e)=>{ return e.hasBothVertices( pL,pC);});

				if( isOnBorder )
				{
					if( areNeighbours )
					{
						// sur le bord, on n'ajoute pas de colonne dans Ax.
						// par contre on met a jour la valeur de Bx
						float xBord = (pMesh.mPoints[pC].x);
						float yBord = (pMesh.mPoints[pC].y);
						newLineBx = newLineBx + xBord;
						newLineBy = newLineBy + yBord;
					}
				}else{
					
					if( pL == pC )
					{
						int nbNeighbours = countNeighbours( pL, pEdges);
						newLineA.Add( nbNeighbours );
					}else{
						// sont-ils voisins ?
						if( areNeighbours )
						{
							newLineA.Add(-1);
						}else{
							// en general c'est 0
							newLineA.Add( 0 );
						}
					}
				}
			}

			A.Add( newLineA );
			Bx.Add( newLineBx );
			By.Add( newLineBy );
		}

		//TOREMOVEFOREXERCISES_END 

		// a ce niveau A et les B sont remplis.


		// je convertis A en matrice, et je convertis les B en vecteurs solutions.
		// j'utilise la bibliotheque logicielle "Math.Net" pour resoudre ce systeme d'equation.
		// https://numerics.mathdotnet.com/LinearEquations.html
		Matrix<float> A_Matrix = DenseMatrix.OfRows(A);
		Vector<float> B_Vector_X = DenseVector.OfEnumerable( Bx );
		Vector<float> B_Vector_Y = DenseVector.OfEnumerable( By );

		// on effectue la resolution de l'equation
		Vector<float> Result_Xs = A_Matrix.Solve( B_Vector_X );
		Vector<float> Result_Ys = A_Matrix.Solve( B_Vector_Y );

		// maintenant j'applique les resultats sur mon mesh
		//TOREMOVEFOREXERCISES_BEGIN
		int indexInResult = 0;
		for(int pL = 0; pL < nbPointsTotal; ++pL)
		{
			if( lUnusedVertices.Contains( pL))
			{
				// ce sommet ne nous servira pas du tout de toute facon
				continue;
			}

			if( pBorder.Contains( pL ))
			{
				// les bordures ne font pas partie des choses que l'on a cherche
				continue;
			}

			// on recupere les valeurs qu'on vient de calculer et on les applique au mesh pour ce point
			float newX = Result_Xs[ indexInResult ];
			float newY = Result_Ys[ indexInResult ];
			pMesh.mPoints[pL] = new Vector3( newX, newY, 0.0f );

			++indexInResult;
		}

		//TOREMOVEFOREXERCISES_END 
	}


	/// deploie en cercle les sommets qui forment le trou du maillage
	public void deployHoleCircle()
	{
		// creons un cercle a partir de notre hole
		for(int h = 0; h < mHole.Count; ++h)
		{
			int indexVertex = mHole[h];

			float angle_deg = ((float)h/(float)mHole.Count) * 360.0f;
			float angle_rad = angle_deg * Mathf.Deg2Rad; 
			Vector3 lNewPoint = new Vector3( Mathf.Cos( angle_rad ), Mathf.Sin( angle_rad), 0.0f) ;

			mMesh.mPoints[ indexVertex ] = lNewPoint;
		}
	}


	/// deploie en carre les sommets qui forment le trou du maillage
	public void deployHoleSquare()
	{
		// en 2 etapes : 
		// 1/ c'est comme un deploiement en cercle de rayon 1, mais on prolonge notre rayon jusqu'au bords du carre.
		// 2/ on repositionne les 4 points les plus proches des coins: on les met chacun bien precisemment chacun sur son coin.

		//TOREMOVEFOREXERCISES_BEGIN


		// 1/ c'est comme un deploiement en cercle de rayon 1, mais on prolonge notre rayon jusqu'au bords du carre.
		for(int h = 0; h < mHole.Count; ++h)
		{
			float angle_deg = ((float)h/(float)mHole.Count) * 360.0f + 45.0f; // comme cela on commence pile sur un coin
			float angle_rad = angle_deg * Mathf.Deg2Rad; 
			Vector3 lNewPoint_dir = new Vector3( Mathf.Cos( angle_rad ), Mathf.Sin( angle_rad), 0.0f) ;

			// les plans qui limitent le carre
			Plane p1 = new Plane(-Vector3.right,Vector3.one);
			Plane p2 = new Plane(-Vector3.up,Vector3.up);
			Plane p3 = new Plane(-Vector3.left,Vector3.left);
			Plane p4 = new Plane(-Vector3.down,Vector3.down);

			// on cherche l'intersection la plus proche, elle est forcement a moins de 2.0f en distance.
			Ray r = new Ray( Vector3.zero, lNewPoint_dir);
			float d1 = 2.0f;
			float d2 = 2.0f;
			float d3 = 2.0f;
			float d4 = 2.0f;
			if(!p1.Raycast( r, out d1)){ d1 = 2.0f;	}
			if(!p2.Raycast( r, out d2)){ d2 = 2.0f;	}
			if(!p3.Raycast( r, out d3)){ d3 = 2.0f;	}
			if(!p4.Raycast( r, out d4)){ d4 = 2.0f;	}

			float minDist1 = Mathf.Min( d1, d2);
			float minDist2 = Mathf.Min( d3, d4);
			float minDist3 = Mathf.Min( minDist1, minDist2);

			Vector3 lNewPoint = r.GetPoint( minDist3 );

			int indexVertex = mHole[h];
			mMesh.mPoints[ indexVertex ] = lNewPoint;
		}


		// 2/ on mets les points les plus proches des coins bien sur les coins

		// fonction locale, trouver l'index (dans le mesh) du point du hole le plus proche de pPoint
		System.Func<Vector3,int> func_getClosestInHole = (Vector3 pCornerPoint) =>
		{
			int best = -1;
			float distance = float.MaxValue;
			for(int h = 0; h < mHole.Count; ++h)
			{
				int indexVertex = mHole[h];
				Vector3 lPoint = mMesh.mPoints[ indexVertex ];
				float calcDist = (lPoint - pCornerPoint).sqrMagnitude;
				if( distance > calcDist )
				{
					distance = calcDist;
					best = indexVertex;
				}
			}
			return best;
		};


		var corner3D_1 = new Vector3( 1, 1,0);
		var corner3D_2 = new Vector3(-1, 1,0);
		var corner3D_3 = new Vector3(-1,-1,0);
		var corner3D_4 = new Vector3( 1,-1,0);

		int corner_1 = func_getClosestInHole( corner3D_1 );
		int corner_2 = func_getClosestInHole( corner3D_2 );
		int corner_3 = func_getClosestInHole( corner3D_3 );
		int corner_4 = func_getClosestInHole( corner3D_4 );

		mMesh.mPoints[corner_1] = corner3D_1;
		mMesh.mPoints[corner_2] = corner3D_2;
		mMesh.mPoints[corner_3] = corner3D_3;
		mMesh.mPoints[corner_4] = corner3D_4;

		//TOREMOVEFOREXERCISES_END Debug.Log("a coder!");
	}


}
