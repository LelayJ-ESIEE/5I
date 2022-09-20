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
public class Step2_cercle_circonscrit3D_changeRepere : MonoBehaviour {

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

		//TOREMOVEFOREXERCISES_BEGIN
		// 1/ calculer le plan
		Plane plan = new Plane(A,B,C);

		// 2/ projeter P sur le plan
		if( !plan.GetSide( pointOutside ))
		{
			// changer le sens du plan
			plan.Flip();
		}

		// 3/ exprimer A,B,C,P dans un nouveau repere, dont le nouveau plan XY est confondu avec le plan ABC
		// trouver la perpendiculaire a AB dans le plan ABC, passant par A : AO
		// exprimer ABCP dans ce repere (normalize(AB), normalize(AO))
		var AB = B-A;
		var AB1 = AB.normalized;

		// AB1 sera notre nouvel axe 'X'
		// la normal du plan sera notre nouvel axe 'Z'.
		var lPlanNormal = plan.normal.normalized;

		// produit vectoriel donne quelque chose perpendiculaire aux 2 premieres
		var lPerpendiculaireDe_AB_dansLePlan_ABC = (Vector3.Cross( AB1, lPlanNormal)).normalized;

		// je dois avoir un repere direct, donc je dois corrige cet axe
		var lPerpendiculaireDe_AB_dansLePlan_ABC_bienOrientee = -lPerpendiculaireDe_AB_dansLePlan_ABC; 

		var newbase_X = AB1;
		var newbase_Y = lPerpendiculaireDe_AB_dansLePlan_ABC_bienOrientee;
		var newbase_Z = lPlanNormal;

		return new Vector3[]{ newbase_X, newbase_Y, newbase_Z};

		//TOREMOVEFOREXERCISES_END  return new Vector3[]{ Vector3.right, Vector3.up, Vector3.forward};
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

		//TOREMOVEFOREXERCISES_BEGIN

		// preparer mon changement de repere! Ici seuls ces nouveaux X et Y m'interesse en l'occurence.
		// remarque : ils sont deja de taille 1 (sinon il aurait fallu les normaliser).
		var newbase_X = lNewRepere[0];
		var newbase_Y = lNewRepere[1];
		var newbase_Z = lNewRepere[2];

		// je fabrique ma matrice de transformation grace a cela.
		// note : j'aurai pu me contenter d'une matrice de rotation, car dans le cadre 
		//         du calcul "isInCircumcircle" si tout est Translate cela ne change rien au fait que le point est dedans/dehors
		var lColumn1 = new Vector4( newbase_X.x, newbase_X.y, newbase_X.z, 0);
		var lColumn2 = new Vector4( newbase_Y.x, newbase_Y.y, newbase_Y.z, 0);
		var lColumn3 = new Vector4( newbase_Z.x, newbase_Z.y, newbase_Z.z, 0);
		var lColumn4 = new Vector4( A.x, A.y, A.z, 1); // rappel : la derniere colonne est la translation (position dans l'espace par rapport au parent)
		var lTRS = new Matrix4x4( lColumn1, lColumn2, lColumn3, lColumn4);

		// rappel : la TRS permet de passer de notre repere nouveau VERS le repere world.
		// il se trouve qu'on veut passer du WORLD vers le nouveau repere.
		// c'est dans le sens inverse! donc il faut inverser la matrice.
		var lInverseTRS = lTRS.inverse;

		// calcul du changement de repere lui meme
		var P_projeteSurPlan = plan.ClosestPointOnPlane( P );

		// mon nouveau plan est centre sur A donc 0 0 pour x et y (et je me moque de Z)
		var newBase_A = MathsHlp.multiplication_homogene( lInverseTRS, A); // evidemment cela devrait etre 0,0,0 ici
		var newBase_B = MathsHlp.multiplication_homogene( lInverseTRS, B);
		var newBase_C = MathsHlp.multiplication_homogene( lInverseTRS, C);
		var newBase_P_projeteSurPlan = MathsHlp.multiplication_homogene( lInverseTRS, P_projeteSurPlan);

		// 4/ faire notre calcul precedent en 2D
		return Step1_cercle_circonscrit2D.isInCircumcircle_XY( newBase_A, newBase_B, newBase_C, newBase_P_projeteSurPlan);

		//TOREMOVEFOREXERCISES_END return false;
	}

}

