using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Ici on va calculer si des points sont dans le cercle circonscrit 2D d'un triangle.
/// Ce triangle est sur le plan Z = 0, c'est a dire le plan XY.
/// 
/// On affiche plein de points, et selon les valeurs renvoyees par les fonctions  
///    isTriangleTrigo_XY ( a completer)
///     et 
///    isInCircumcircle_XY (a completer),
/// les elements sont colories ou non.
/// 
/// Par ailleurs, vous pouvez calculer le centre du cercle circonscrit en completant la fonction calcCircumcircle_center_v2.
///   Dans ce cas, le 4eme point du graph se positionnera a l'emplacement renvoye par cette fonction.
public class Step1_cercle_circonscrit2D_incomplet : MonoBehaviour 
{
	/// notre graph
	public Graph mGraph = null;

	/// notre fond qui est bleu et jaune
	public Graph mBackGround = null;

	// Use this for initialization
	void Start()
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("Triangle");

		// on cree 4 points : les trois premiers forment le triangle. le dernier sera le centre du cercle circonscrit.
		mGraph.initAsRandomPoints(4, -25, 25, -18, 18);


		mGraph.mEdges.Add( new Edge(0,1) );
		mGraph.mEdges.Add( new Edge(1,2) );
		mGraph.mEdges.Add( new Edge(2,0) );

		// afficher le graph dans la 3D, en creant des objets si besoin : cote visuel
		mGraph.updateScene();

		// ceci servira a afficher les points dans le cercle circonscrit ou non.
		mBackGround = new Graph("Background");

		// je cree un background regulier avec plein de points
		for(int i = -25; i < 25; ++i)
		{
			for(int j = -18; j < 18; ++j)
			{
				mBackGround.mPoints.Add( new GVector3(i,j,0));
			}
		}

		// j'ameliore le visuel de ces points
		mBackGround.updateScene();
		mBackGround.visual_removeAllTexts();
		mBackGround.visual_scaleAllPoints(0.2f);
		mBackGround.visual_changeAllPointsColor( Color.yellow );
		mBackGround.translateAllPoints(0,0,-3.0f);
	}
	
	// Update is called once per frame
	void Update () {
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


		// tester si on est dans le cercle circonscrit, pour chaque point de mBackGround
		{
			// triangle
			var A = mGraph.mPoints[0];
			var B = mGraph.mPoints[1];
			var C = mGraph.mPoints[2];
			for (int i = 0; i < mBackGround.mPoints.Count; ++i)
			{
				bool inCircle = isInCircumcircle_XY(A,B,C, mBackGround.mPoints[i] );
				mBackGround.visual_changePointColor( i, inCircle ? Color.yellow : Color.blue );
			}

			// positionner le centre
			mGraph.mPoints[3] = calcCircumcircle_center_v2( A,B,C );
		}


		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();

		mBackGround.updateScene();
	}

	void OnGUI()
	{
		GUILayout.Button("Notre Triangle est-il dans le sens trigo ? "+ isTriangleTrigo_XY( mGraph.mPoints[0], mGraph.mPoints[1], mGraph.mPoints[2]).ToString() );
	}

	/// dans le plan XY (X positif a droite, Y positif vers le haut) :
	/// true si le triangle tourne dans le sens trigo
	/// faux sinon.
	/// source : https://algs4.cs.princeton.edu/91primitives/#:~:text=Use%20the%20following%20determinant%20formula,b%2D%3Ec%20are%20collinear.
	public static bool isTriangleTrigo_XY(Vector3 p0, Vector3 p1, Vector3 p2)
	{
		// TODO ici etudiants, completez la fonction, vous appuyant sur le cours.

		// TODO COMPLETEZ ICI
 return false;
	}

	/// il existe plusieurs formules tres proches
	public static Vector3 calcCircumcircle_center_v1( Vector3 A, Vector3 B, Vector3 C)
	{
		Vector3 AB = B-A; 
		Vector3 AC = C-A;
		Vector3 crossABAC = Vector3.Cross(AB,AC);
		Vector3 numerator_center = 
			Vector3.Dot(AC,AC) * Vector3.Cross( crossABAC, AB) 
			+ Vector3.Dot(AB,AB) * Vector3.Cross(Vector3.Cross(AC,AB), AC);

		Vector3 center = A + (numerator_center / (2.0f * Vector3.Dot(crossABAC,crossABAC)));
		return center;
	}

	/// renvoie le centre du cercle circonscrit au triangle
	public static Vector3 calcCircumcircle_center_v2( Vector3 A, Vector3 B, Vector3 C)
	{
		// calculer le centre circonscrit au triangle avec la formule du cours.
		// la fonction "Vector3.Cross(.." sert a faire le produit vectoriel
		// n'hesitez pas a lire la doc unity3D sur la classe Vector3.

		// TODO COMPLETEZ ICI
 return Vector3.zero;
	}

	/// true si P est dans le cercle circonscrit en 2D (avec ABC triangle qui tourne dans le sens trigo) dans le plan XY
	public static bool isInCircumcircle_XY(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
	{
		// TODO ici etudiants, completez la fonction.

		// formule de wikipedia
		// https://en.wikipedia.org/wiki/Delaunay_triangulation#Algorithms


		// TODO COMPLETEZ ICI
 return false;
	}

}


