using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// usage : dessiner la sphere dans la vue scene
/// completez : drawSphereAsLines
public class Step4_dessiner_sphere_incomplet : MonoBehaviour {

	// centre de la sphere
	public Vector3 mCenter;

	// rayon de la sphere
	public float mRadius = 10.0f;
	public int mNbSteps = 20;

	// Use this for initialization
	void Start()
	{
		// positionner et orienter la camera
		CameraHlp.setup2DCamera();

		// nettoyer nos parametres
		mCenter = Vector3.zero;
		mRadius = Mathf.Abs(mRadius);
		if( mNbSteps < 5 )
		{
			mNbSteps = 5;
		}

	}
	
	// Update is called once per frame
	void Update () {
		drawSphereAsLines( mCenter, mRadius, new Color(1,1,1,0.75f), mNbSteps);
	}

	void OnGUI()
	{
		GUILayout.Button("La sphere n'est visible que dans la vue scene");
	}

	/// dessiner une sphere d'apres son centre et son rayon, a l'aide de petites lignes.
	/// cela nous servira pour du debug plus tard des centre des spheres circonscrites en 3D.
	/// 
	/// formule de wikipedia pour passer de "Coord Spherique (sciences physiques" ==> "Coord cartesiennes")
	///   rayon : r
	///   phi : entre 0 et 2 PI (en radians) c'est le YAW.
	///   theta : entre 0 et PI (en radians) 
	///            c'est un peu comme le pitch, mais en 0 il correspond a un pitch de +90 deg, et en pi il correspond a un PItch de -90deg
	/// 
	/// alors la formule est : 
	///   x = r * sin theta * cos phi
	///   y = r * sin theta * sin phi
	///   z = r * cos theta
	public static void drawSphereAsLines( Vector3 pCenter, float pRadius, Color pColor, int pNbSteps = 15 )
	{
		/// Utiliser la fonction suivante pour dessiner une ligne en vue scene : "Debug.DrawLine( Vector3, Vector3, couleur)"
		/// Dessinez plein de lignes pour faire la sphere.

		// TODO COMPLETEZ ICI


	}
}

