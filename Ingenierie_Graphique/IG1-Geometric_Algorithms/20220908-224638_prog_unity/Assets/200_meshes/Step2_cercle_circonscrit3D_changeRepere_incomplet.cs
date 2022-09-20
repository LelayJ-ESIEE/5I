using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// Ici on va calculer si des points sont dans le cercle circonscrit d'un triangle 3D.
/// 
/// Pour cela, au lieu d'utiliser la formule (qui marche tres bien), on va faire un changement de repere :
/// 1/ on calcule un repere pour ABC
/// 2/ on exprime notre point dans ce repere
/// 3/ on appelle Step1_cercle_circonscrit2D.isInCircumcircle_XY dans ce nouveau repere.
/// 
/// fonctions a completer : 
///    - getNewRepere
///    - isInCircumcircle_3D
/// 
public class Step2_cercle_circonscrit3D_changeRepere_incomplet : MonoBehaviour {

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
		for (int i = 0; i < mBackGround.mPoints.Count; ++i)
		{
			var lVisualPoint = mBackGround.getVisualPoint(i);
			var lText = lVisualPoint.GetComponentInChildren<TextMesh>();
			Destroy( lText );
			lVisualPoint.Translate(0,0,-3.0f);
			lVisualPoint.localScale *= 0.2f;
			mBackGround.visual_changePointColor( lVisualPoint, Color.yellow );
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		//----------------------------------------------------------------------
		//----------------- mise a jour du graph (cote logique) ----------------
		//----------------------------------------------------------------------

		// si on a bouge des objets de la scene (via l'inspector), on prend cela en compte pour mettre a jour le graph
		mGraph.updateFromScene();
		mBackGround.updateFromScene();

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

		// tester si on est dans le cercle circonscrit
		{
			// triangle
			var A = mGraph.mPoints[0];
			var B = mGraph.mPoints[1];
			var C = mGraph.mPoints[2];
			for (int i = 0; i < mBackGround.mPoints.Count; ++i)
			{
				bool inCircle = isInCircumcircle_3D(A,B,C, mBackGround.mPoints[i], Camera.main.transform.position );
				mBackGround.visual_changePointColor( i, inCircle ? Color.yellow : Color.blue );
			}
		}

		/// dessiner notre repere sous forme de lignes
		{
			var A = mGraph.mPoints[0];
			var B = mGraph.mPoints[1];
			var C = mGraph.mPoints[2];
			var lRepere = getNewRepere(A,B,C,Camera.main.transform.position);


			Debug.DrawLine(A, A + lRepere[0],Color.red);
			Debug.DrawLine(A, A + lRepere[1],Color.green);
			Debug.DrawLine(A, A + lRepere[2],Color.blue);
		}

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();

		mBackGround.updateScene();
	}


	/// renvoie les 3 vecteurs qui forment un repere direct orthonorme:
	///   - le premier vecteur est oriente selon AB
	///   - le deuxieme vecteur est dans le plan ABC, perpendiculaire a AB
	///   - le troisieme vecteur est la normale au plan ABC
	/// 
	/// pointOutside est un point qui doit etre situe hors de notre objet 3D.
	///               pour faire simple : pointOutside sera la position de la camera
	/// 			cela permettra de flip le plan si jamais on est du mauvais cote d'orientation.
	/// 
	public static Vector3[] getNewRepere( Vector3 A, Vector3 B, Vector3 C, Vector3 pointOutside)
	{
		// vous pouvez utiliser la classe "Plane" de Unity pour obtenir un plan.
		// vous pouvez utiliser la fonction "Vector3.Cross" pour faire le produit vectoriel.

		// TODO COMPLETEZ ICI
  return new Vector3[]{ Vector3.right, Vector3.up, Vector3.forward};
	}

	/// note : il y a une formule qui existe, mais ce n'est pas interessant.
	/// 
	/// ici on va resoudre ce calcul en exprimant dans un nouveau repere nos points,
	/// puis on appellera Step1_cercle_circonscrit2D.isInCircumcircle_XY dans ce nouveau repere.
	/// 
	/// pointOutside est un point qui se trouve du 'cote de la normale' du plan.
	///   pour faire simple : pointOutside sera la position de la camera
	public static bool isInCircumcircle_3D(Vector3 A, Vector3 B, Vector3 C, Vector3 P, Vector3 pointOutside)
	{
		// 1/ calculer le plan
		Plane plan = new Plane(A,B,C);

		// 2/ trouver la bonne orientation du plan
		if( !plan.GetSide( pointOutside ))
		{
			// changer le sens du plan
			plan.Flip();
		}

		// 3/ exprimer A,B,C,P dans un nouveau repere, dont le nouveau plan XY est confondu avec le plan ABC
		var lNewRepere = getNewRepere( A,B,C, pointOutside );

		// TODO COMPLETEZ ICI
 return false;
	}

}


