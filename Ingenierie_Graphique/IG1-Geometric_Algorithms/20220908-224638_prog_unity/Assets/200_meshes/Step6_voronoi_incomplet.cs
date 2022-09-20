using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// le diagramme de voronoi est le graph dual de delaunay
/// voici le principe :
/// 1/ faire delaunay
/// 2/ trouver les centres des cercles circonscrits chaque triangle
/// 3/ connecter chaque paire de "centre de cercles circonscrits" dont les triangles se partagent un cote
/// 
public class Step6_voronoi_incomplet : MonoBehaviour
{

	/// notre graph
	public Graph mGraph = null;

	public Graph mVoronoi = null;

	/// faciliter le dessin des triangles
	public TriangleDrawer mTriangleDrawer = null;

	// Use this for initialization
	void Start()
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		mTriangleDrawer = new TriangleDrawer();
		mTriangleDrawer.setDoubleFace( true );

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("PointCloud");
		mGraph.initAsRandomPoints( 9, -10, 10,-10,10);

		// afficher le graph dans la 3D, en creant des objets si besoin : cote visuel
		mGraph.updateScene();
		mGraph.visual_scaleAllPoints( 0.5f);

		Debug.Log("Touche espace pour lancer le calcul.");
		Debug.Log("Touche T pour ajouter/enlever 4 sommets extremes.");
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
			var newTriangles = Step3_delaunay2D.delaunay2D( mGraph );
			if( newTriangles != null && newTriangles.Count > 0)
			{
				mTriangleDrawer.beginAddingTri();
				for(int t = 0; t < newTriangles.Count; ++t)
				{
					newTriangles[t].color = new Color( Random.value, 1.0f - Random.value * 0.5f, Random.value);
					mTriangleDrawer.addTriangle( newTriangles[t].getReduced(0.2f) );
				}
				mTriangleDrawer.updateMesh();
				Debug.Log("fin de la triangulation de delaunay");

				Debug.Log("Calcul du graph de voronoi");
				if( mVoronoi != null )
				{
					mVoronoi.destroy();
				}

				mVoronoi = voronoi_from_delaunay( newTriangles);
				mVoronoi.visual_setEdgeTransparent(false);
				mVoronoi.updateScene();

				// on change les couleurs pour mieux voir : je met a chaque 'centre circonscrit' la meme couleur que son 'triangle associe'
				for(int i = 0; i < mVoronoi.mPoints.Count; ++i)
				{
					var lVoronoiVertex = mVoronoi.getVisualPoint(i);
					mVoronoi.visual_changePointColor(i, newTriangles[i].color);
					lVoronoiVertex.GetComponentInChildren<TextMesh>().text = "\u263C"; // soleil
				}
			}
		}

		/// ajouter dans notre graph 4 points tres eloignes pour avoir les mediatrices de edges externes visibles sur notre voronoi
		if( Input.GetKeyDown(KeyCode.T))
		{
			if( mGraph.mPoints[mGraph.mPoints.Count-1].x != -100.0f )
			{
				mGraph.mPoints.Add( new GVector3( -100,-100,0));
				mGraph.mPoints.Add( new GVector3(  100,-100,0));
				mGraph.mPoints.Add( new GVector3(  100, 100,0));
				mGraph.mPoints.Add( new GVector3( -100, 100,0));
			}else{
				mGraph.mPoints.RemoveAt( mGraph.mPoints.Count -1 );
				mGraph.mPoints.RemoveAt( mGraph.mPoints.Count -1 );
				mGraph.mPoints.RemoveAt( mGraph.mPoints.Count -1 );
				mGraph.mPoints.RemoveAt( mGraph.mPoints.Count -1 );
			}
		}

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}

	/// le diagramme de voronoi est le graph dual de delaunay
	/// 
	/// 1/ ici on a deja fait delaunay, et vous recevez en entree un maillage de delaunay
	/// 2/ trouver les centres des cercles circonscrits chaque triangle avec Step5_sphere_circonscrit3D.getCircomcircleSphere
	/// 3/ connecter chaque paire de "centre de cercles circonscrits" dont les triangles se partagent un cote
	///
	public static Graph voronoi_from_delaunay(List<Triangle3D> pDelaunay)
	{
		Graph voronoi_result = new Graph("voronoi");

		// TODO COMPLETEZ ICI


		return voronoi_result;
	}
}

