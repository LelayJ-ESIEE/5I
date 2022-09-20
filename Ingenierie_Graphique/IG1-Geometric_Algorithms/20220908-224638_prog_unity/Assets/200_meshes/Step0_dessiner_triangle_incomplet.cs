using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// exercice : completez la fonction updateMesh() en bas du fichier.
/// Elle remplit les listes de sommets, faces, ... d'un Mesh unity3D a partir d'une liste de Triangle3D.
/// Cette liste de triangles 3D est : List<Triangle3D> mTriangles;
///    Chaque triangle possede les attributs : 
///          - A, B, C qui sont des Vector3.
/// 		 - color qui est une color.
/// 
public class Step0_dessiner_triangle_incomplet : MonoBehaviour
{
	/// classe qui fait le dessin de triangle
	private Step0_TriangleDrawer mTriangleDrawer;

	private System.Random random = null;

	// Use this for initialization
	void Start()
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// init du random (avec une graine prise au hasard, mais qui sera la meme chez tout le monde)
		random = new System.Random(46780);

		mTriangleDrawer = new Step0_TriangleDrawer();
		mTriangleDrawer.setDoubleFace(true);

		// exemple d'usage pour 2 triangles
		{
			mTriangleDrawer.beginAddingTri(); // nettoie sa 3D

			mTriangleDrawer.addTriangle( new Vector3(0,2,0), new Vector3(1,2,0), new Vector3(1,1,0), Color.blue );
			mTriangleDrawer.addTriangle( new Triangle3D());

			mTriangleDrawer.updateMesh();// mets a jour la 3D
		}

		Debug.Log("appuyez sur espace pour (re)generer des triangles");
	}

	
	// Update is called once per frame
	void Update()
	{

		// si on appuie sur la touche espace...
		if( Input.GetKeyDown(KeyCode.Space))
		{
			reGenerateTriangles();
		}
	}

	void OnGUI()
	{
		if(GUILayout.Button("(re)generer des triangles"))
		{
			reGenerateTriangles();
		}
	}

	/// met a jour les infos de mTriangleDrawer.
	void reGenerateTriangles()
	{
		// on nettoie notre liste de triangles
		mTriangleDrawer.beginAddingTri();

		// on ajoute des triangles (sans mettre a jour la 3D correspondante)
		for(int i = -4; i < 4; ++i)
		{
			for(int j = -4; j < 4; ++j)
			{
				if( random.Next(0,3) == 1)
				{
					// on choisit aleatoirement de ne pas en mettre partout
					continue;
				}

				Vector3 A = new Vector3( i, j, 0) * 3;
				Vector3 B = new Vector3( i+0.5f, j, 0) * 3;
				Vector3 C = new Vector3( i+0.25f, j-0.5f, 0) * 3;
				Color color = new Color( 
					((float)random.Next(0,255))/255.0f,
					((float)random.Next(0,255))/255.0f,
					((float)random.Next(0,255))/255.0f,
					1.0f);

				mTriangleDrawer.addTriangle( A,B,C, color );
			}
		}

		// on mets a jour le maillage
		mTriangleDrawer.updateMesh();
	}


	/// sert a dessiner de nombreux triangles
	/// faire dans l'ordre : 
	///    beginAddingTri()
	///    addTriangle( mon triangle )
	///    updateMesh()
	public class Step0_TriangleDrawer
	{
		/// la liste des triangles a afficher
		private List<Triangle3D> mTriangles; 

		/// le necessaire pour faire l'affichage dans unity3D
		private GameObject mGo;
		private Mesh mMesh;

		public Step0_TriangleDrawer()
		{
			mTriangles = new List<Triangle3D>();

			mMesh = new Mesh();
			mMesh.MarkDynamic();

			mGo = new GameObject("TriangleDrawer");
			var lFilter = mGo.AddComponent<MeshFilter>();
			lFilter.mesh = mMesh;

			var lRenderer = mGo.AddComponent<MeshRenderer>();
			lRenderer.material = new Material( Shader.Find("CoursVertexColor") );
		}

		/// pour dessiner tous les triangles en double face ou non
		public void setDoubleFace(bool doubleFace)
		{
			string lShaderName = doubleFace ? "CoursVertexColorDoubleFace" : "CoursVertexColor";

			var lRenderer = mGo.GetComponent<MeshRenderer>();
			lRenderer.material.shader = Shader.Find( lShaderName );
		}

		public MeshRenderer getRenderer()
		{
			return mGo.GetComponent<MeshRenderer>();
		}

		public int getNbTriangles()
		{
			return mTriangles.Count;
		}

		/// reducedQuantity : reduit un petit peu la taille de chaque triangle independamment
		public void clearAndDrawThese(List<Triangle3D> pTriangles, int color_rand_seed = -1 , float reducedQuantity = 0.0f)
		{
			mTriangles.Clear();

			if( color_rand_seed != -1 )
			{
				System.Random rand = new System.Random(color_rand_seed);
				for(int i = 0; i < pTriangles.Count; ++i)
				{
					var lTri = pTriangles[i];

					var lColor = new Color( 
						((float)rand.Next(0,255))/255.0f, 
						((float)rand.Next(0,255))/255.0f, 
						((float)rand.Next(0,255))/255.0f, 
						1.0f) ;

					if( reducedQuantity != 0.0f)
					{
						mTriangles.Add( new Triangle3D(lTri.A, lTri.B, lTri.C, lColor ).getReduced(reducedQuantity) );
					}else{
						mTriangles.Add( new Triangle3D(lTri.A, lTri.B, lTri.C, lColor ) );
					}
				}
			}else{
				if( reducedQuantity == 0.0f )
				{
					mTriangles.AddRange( pTriangles );
				}else{
					for(int i = 0; i < pTriangles.Count; ++i)
					{
						mTriangles.Add( new Triangle3D( pTriangles[i] ).getReduced(reducedQuantity) );
					}
				}
			}
			updateMesh();
		}


		/// vide la liste interne de triangle.
		/// il faudra faire des addTriangle puis un updateMesh pour que le resultat devienne visible
		public void beginAddingTri()
		{
			mTriangles.Clear();
		}

		public void addTriangle(Triangle3D t)
		{
			mTriangles.Add( t );
		}

		public void addTriangle(Vector3 a, Vector3 b, Vector3 c)
		{
			mTriangles.Add( new Triangle3D(a,b,c) );
		}

		public void addTriangle(Vector3 a, Vector3 b, Vector3 c, Color pColour)
		{
			mTriangles.Add( new Triangle3D(a,b,c, pColour) );
		}

		/// cette fonction parcourt mTriangles et met a jour le mesh unity3D correspondant.
		public void updateMesh()
		{
			mMesh.Clear();

			int nbVertices =  mTriangles.Count * 3 ;

			if( nbVertices == 0)
			{
				return;
			}

			Vector3[] newVertices = new Vector3[  nbVertices ];
			Vector3[] newNormals = new Vector3[  nbVertices ];
			Color[] newColors = new Color[  nbVertices ];
			int[] newTriangles = new int[ nbVertices ];

			for(int t = 0; t < mTriangles.Count; ++t)
			{
				Triangle3D tri = mTriangles[t];

				// Remplir newVertices, newNormals, newColors et newTriangles
				// note : tri.color donne la couleur du triangle
				//        tri.A,  tri.B,   tri.C   sont des Vector3 et sont les positions 3D des sommets du triangle.

				// TODO COMPLETEZ ICI

			}

			mMesh.vertices = newVertices;
			mMesh.normals = newNormals;
			mMesh.colors = newColors;
			mMesh.triangles = newTriangles;

		}
	}
}


