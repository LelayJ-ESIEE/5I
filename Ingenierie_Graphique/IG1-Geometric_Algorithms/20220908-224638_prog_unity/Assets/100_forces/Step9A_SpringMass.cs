using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// 
/// Il est possible de creer des petits systemes de ressorts.
/// On appelle cela des "spring-mass systems".
/// Ce sont des graphs avec des sommets et des edges.
/// 
/// Les sommets jouent le role des "Masses" et les edges le role des "Ressorts" ("spring" en anglais).
/// 
/// Les sommets ont chacun : position, vitesse, acceleration, masse (ce sont les "Mass" du systeme).
/// Les edges ont chacun : longueur au repos (comme une "longueur ideale"), coefficient de ressort, coefficient d'amortissement.
/// 
/// Choisir les bons coefficients est souvent fait "a vue de nez" et peut prendre beaucoup de temps.
/// 
/// Exercice : 	Il faut finaliser l'operation qui met sous la souris une node.
/// 			lors de cette operation on remet a 0 l'acceleration et la vitesse de la node.
/// 			mSpringMassGraph contient des nodes qui ont les positions, la vitesse et l'acceleration.
/// 
/// Voici un code qui utilise des spring mass pour simuler les vetements de cette maniere, c'est tres clair aussi.
/// https://github.com/Relfos/ClothSimSharp/blob/master/clothSim/ClothSimulation.cs
/// 
public class Step9A_SpringMass : MonoBehaviour
{
	/// notre graph
	public Graph mGraph = null;
	public SM_Graph mSpringMassGraph = null;
	public float mSpringCoeff_Stiffness = 0.01f;
	public float mDamping = 0.5f;

	// Use this for initialization
	void Start () 
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// creer notre graph avec ses points : cote logique
		mGraph = new Graph("Un Graph");
		mGraph.initAsCirclePoints(15, 0, 0, 20, true);

		// afficher le graph dans la 3D, en creant des objets si besoin : cote visuel
		mGraph.updateScene();

		mSpringMassGraph = new SM_Graph();


		//-------------------------------------------------
		//-----initialiser les masses du systeme-----------
		//-------------------------------------------------

		// je mets le meme poids que "indice +1" (pour eviter d'avoir une masse de 0)
		float[] lPoids = new float[mGraph.mPoints.Count];
		for(int p = 0; p < mGraph.mPoints.Count; ++p)
		{
			lPoids[p] = (p + 1);
			//lPoids[p] = 2.0f;
		}
		mSpringMassGraph.initNodesFrom( mGraph.mPoints, lPoids );

		//-------------------------------------------------
		//-----initialiser les ressorts du systeme---------
		//-------------------------------------------------
		float[] lLongueursRessortsAuRepos = new float[mGraph.mEdges.Count];
		for(int e = 0; e < mGraph.mEdges.Count; ++e)
		{
			var lEdge = mGraph.mEdges[e];
			GVector3 BA = (mGraph.mPoints[lEdge.a] - mGraph.mPoints[lEdge.b]);
			lLongueursRessortsAuRepos[e] = BA.length();
		}
		mSpringMassGraph.initSpringsFrom( mGraph.mEdges, lLongueursRessortsAuRepos );
		mSpringMassGraph.setCoeffStiffnessForAll( mSpringCoeff_Stiffness );

	}

	// Update is called once per frame
	void Update()
	{
		//----------------------------------------------------------------------
		//----------------- mise a jour du graph (cote logique) ----------------
		//----------------------------------------------------------------------

		// si on appui sur espace, on vient mettre 'le sommet le plus proche du clic' sous le curseur de la souris
		if( Input.GetKey( KeyCode.Space ) )
		{
			// obtenir la position 3D de la souris (envoi un rayon sur le plan Z == 0)
			Vector3 lMousePosition3D = MouseHlp.getPositionOnZ();

			// trouver le numero du point le plus proche
			int indexPoint = mGraph.getClosestPointFrom( lMousePosition3D );

			// le mettre a la meme position que la souris 3D
			mGraph.mPoints[indexPoint] = lMousePosition3D;


			// ici, on applique dans mSpringMassGraph la nouvelle position sur la node indexPoint aussi

			//TOREMOVEFOREXERCISES_BEGIN
			var lNode = mSpringMassGraph.mNodes[indexPoint];
			lNode.accel = Vector3.zero;
			lNode.vitesse = Vector3.zero;
			lNode.pos = lMousePosition3D;
			//TOREMOVEFOREXERCISES_END
		}

		// mise a jour du spring mass system, je mets un temps fixe pour la stabilite.
		mSpringMassGraph.update(0.1f);

		// mise a jour du graph selon ce que le spring mass system a fait
		mSpringMassGraph.applyTo( mGraph );

		//----------------------------------------------------------------------
		//----------------- mise a jour de la scene (cote visuel)---------------
		//----------------------------------------------------------------------
		// bouger les objets 3D
		mGraph.updateScene();
	}

	/// afficher un bouton notamment pour remettre les points au hasard
	void OnGUI()
	{
		if( GUILayout.Button("Repositionner les points au hasard"))
		{
			for(int v = 0; v < mGraph.mPoints.Count; ++v)
			{
				var lPoint = mGraph.mPoints[v];
				lPoint.x = (2.0f*UnityEngine.Random.value - 1.0f) * 20.0f;
				lPoint.y = (2.0f*UnityEngine.Random.value - 1.0f) * 20.0f;

				// affecter aussi le "systeme masses-ressorts", et on peut choisir de mettre la vitesse et l'acceleration a 0
				var lNode = mSpringMassGraph.mNodes[v];
				lNode.pos = lPoint;
				lNode.vitesse = Vector3.zero;
				lNode.accel = Vector3.zero;
			}

			mGraph.updateScene();
		}

		// changer le coefficient des ressorts
		{
			GUILayout.Space(25);
			GUILayout.Label("Coefficient K des ressorts (en general entre 0 et 0.3): " + mSpringCoeff_Stiffness);
			float newSpringCoeff = GUILayout.HorizontalSlider(mSpringCoeff_Stiffness,0.00001f,0.5f);
			if( newSpringCoeff != mSpringCoeff_Stiffness)
			{
				mSpringCoeff_Stiffness = newSpringCoeff;
				mSpringMassGraph.setCoeffStiffnessForAll( mSpringCoeff_Stiffness );
			}
		}

		// changer l'amortissement
		{
			GUILayout.Space(25);
			GUILayout.Label("Amortissement des masses : " + mDamping);
			float newCoeff = GUILayout.HorizontalSlider(mDamping, 0.00001f, 1.0f);
			if( newCoeff  != mDamping)
			{
				mDamping = newCoeff;
				mSpringMassGraph.setDampingForAllNodes( mDamping );
			}
		}
	}

}



