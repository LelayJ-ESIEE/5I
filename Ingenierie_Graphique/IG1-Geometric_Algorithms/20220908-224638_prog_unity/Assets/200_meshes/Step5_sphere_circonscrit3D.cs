using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// But : savoir si des points sont dans la sphere circonscrite au triangle.
/// 
/// utilisation de la formule du cours
///  https://gamedev.stackexchange.com/questions/60630/how-do-i-find-the-circumcenter-of-a-triangle-in-3d
/// pour resoudre le probleme
public class Step5_sphere_circonscrit3D : MonoBehaviour
{
	/// notre graph
	public Graph mGraph = null;

	public Graph mBackGround = null;

	// Use this for initialization
	void Start () {
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("Triangle");
		mGraph.initAsRandomPoints(3, -25, 25, -18, 18);
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

		// dessiner la sphere
		{
			var lSphereInfo = getCircomcircleSphere( mGraph.mPoints[0], mGraph.mPoints[1], mGraph.mPoints[2]);
			Step4_dessiner_sphere.drawSphereAsLines( lSphereInfo.center, lSphereInfo.radius, Color.green, 30);
		}

		// tester si on est dans le cercle circonscrit
		{
			// triangle
			var A = mGraph.mPoints[0];
			var B = mGraph.mPoints[1];
			var C = mGraph.mPoints[2];
			for (int i = 0; i < mBackGround.mPoints.Count; ++i)
			{
				bool inCircle = isInCircomcircleSphere_3D(A,B,C, mBackGround.mPoints[i] );
				mBackGround.visual_changePointColor( i, inCircle ? Color.yellow : Color.blue );
			}
		}

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();

		mBackGround.updateScene();
	}

	/// decrit une sphere : centre + rayon.
	public struct SphereDescriptor
	{
		/// le centre
		public Vector3 center;
		/// le rayon
		public float radius;

		public SphereDescriptor(Vector3 pCenter, float pRadius)
		{
			center = pCenter;
			radius = pRadius;
		}
	}

	/// obtenir la sphere circoncise au triangle ABC
	/// utilisation de la formule du cours
	public static SphereDescriptor getCircomcircleSphere(Vector3 A, Vector3 B, Vector3 C)
	{
		//TOREMOVEFOREXERCISES_BEGIN
		Vector3 ac = C - A ;
		Vector3 ab = B - A ;
		Vector3 abXac = Vector3.Cross( ab, ac ) ;

		// this is the vector from "point A" TO the circumsphere center
		Vector3 toCircumsphereCenter = ( Vector3.Cross( abXac, ab )*ac.sqrMagnitude + Vector3.Cross( ac, abXac )*ab.sqrMagnitude) / (2.0f*abXac.sqrMagnitude) ;
		float circumsphereRadius = toCircumsphereCenter.magnitude;

		// The 3 space coords of the circumsphere center then:
		Vector3 ccs = A  +  toCircumsphereCenter ; // now this is the actual 3space location

		SphereDescriptor result = new SphereDescriptor();
		result.center = ccs;
		result.radius = circumsphereRadius;
		return result;
		//TOREMOVEFOREXERCISES_END return new SphereDescriptor(Vector3.zero, 1.0f);
	}

	/// utilisation de la formule du cours
	public static bool isInCircomcircleSphere_3D( Vector3 A, Vector3 B, Vector3 C, Vector3 P)
	{
		var infoSphere = getCircomcircleSphere( A,B,C );
		return (P - infoSphere.center).magnitude < infoSphere.radius - 0.001f; // on ajoute un petit ecart pour ne pas prendre en compte ceux qui sont piles sur le cercle
	}
}
