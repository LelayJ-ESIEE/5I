using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// trouver un trou dans un maillage ("trou" est equivalent a "bordure qui boucle").
/// 
/// 1/ ce qui est deja fait c'est qu'au debut on cree un CUT.
///     c'est une liste d'edge qui va ouvrir le maillage, comme une fermeture eclair (cf sticky fingers de buccelatti)
/// 
/// 2/ sachant qu'il y a un tel trou dans le maillage, on cherche maintenant le trou.
///     il est compose d'EDGE qui ne touchent que 1 triangle.
/// 
/// 3/ une fois qu'on a tous les edges qui participent a des trous, on en prend 1.
///     puis on cherche le sommet suivant sans jamais retourner en arriere (il faut une liste des edges deja visites pour cela)
/// 	et ainsi de suite dans une while loop.
///    si on retombe sur le sommet du debut ou que l'on ne trouve plus rien, arret de la recherche.
/// 
/// Exercice : completez la fonction sFindHole en bas du fichier
public class Step11_findHole_geometryImage_incomplet : MonoBehaviour {

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
		mMesh.initFrom( meshFilter.sharedMesh );
		mMesh.mergePointsAtSamePositions();
		mMesh.scale( 5 );

		// faire un cut
		{
			// definir l'ensemble des edges du cut
			var lCut = new List<Edge>();
			lCut.Add( new Edge( 192, 197) );
			lCut.Add( new Edge( 197, 199) );
			lCut.Add( new Edge( 199, 200) );
			lCut.Add( new Edge( 200, 92 ) );
			lCut.Add( new Edge( 92,  5)   );

			// effectuer le cut, en doublonnant des sommets, et donc creant un trou dans le mesh ferme
			Step10_createCut_geometryImage.createCut( lCut, mMesh );
		}

		// trouver la succession de sommet qui forme une boucle
		do
		{
			var lHole = sFindHole( mMesh );

			if( lHole == null || lHole.Count == 0 )
			{
				Debug.Log("pas de trou obtenu");
				break;
			}

			// afficher un petit message a ce sujet
			string message = "Le hole contient les sommets : (";
			for(int h = 0; h < lHole.Count; ++h)
			{
				message += (" "+lHole[h].ToString() );
			}
			message += ")";
			Debug.Log(message);

			// optionnellement : supprimer les vertex qui ne servent plus
			// Step10_createCut_geometryImage.sRemoveUnusedVertex( mMesh );

			// changer la couleur des sommets non utilises
			mMesh.visual_update();
			mMesh.getUnusedVertices().ForEach( delegate( int pIndexVertex ){  mMesh.visual_setPointColor( pIndexVertex, Color.magenta); });
			lHole.ForEach( delegate( int pIndexVertex ){  mMesh.visual_setPointColor( pIndexVertex, Color.blue); });

			// aide visuelle
			// reduire un petit peu la taille de tous les triangles qui touchent le hole, de maniere 
			// a voir mieux le trou apparaitre
			for(int t = 0; t < mMesh.mFaces.Count; ++t)
			{
				var lTriangle = mMesh.mFaces[t];
				foreach( int h in lHole )
				{
					if( lTriangle.has( h ))
					{
						Vector3 A3D = mMesh.mPoints[ lTriangle.a ];
						Vector3 B3D = mMesh.mPoints[ lTriangle.b ];
						Vector3 C3D = mMesh.mPoints[ lTriangle.c ];
						Vector3 center = (A3D + B3D + C3D) / 3.0f;
						Vector3 newA = center + (A3D - center) * 0.3f;
						Vector3 newB = center + (B3D - center) * 0.3f;
						Vector3 newC = center + (C3D - center) * 0.3f;

						mMesh.mPoints[lTriangle.a] = newA;
						mMesh.mPoints[lTriangle.b] = newB;
						mMesh.mPoints[lTriangle.c] = newC;
						break;
					}
				}
			}
		}
		while(false);//breakable

		// afficher notre modele 3D
		mMesh.visual_update();
	}

	void Update()
	{
		// permettre de prendre en compte les mouvements eventuels faits dans la vue scene
		mMesh.visual_updateFromScene();
		mMesh.visual_update();
	}


	/// renvoie le premier trou dans le mesh (qui etait manifold avant le cut) ou vide
	/// 
	/// les int sont les numeros des sommets concernes.
	/// le premier numero doit etre different du dernier (meme si ils sont supposes boucler).
	/// 
	/// renvoi vide en cas de probleme
	public static List<int> sFindHole( Mesh3D pMesh)
	{
		List<int> verticesHole = new List<int>();
		// TODO COMPLETEZ ICI

		return verticesHole;
	}
}



