using UnityEngine;
using System.Collections;
using System.Collections.Generic;


// exemple : numerics.mathdotnet.com
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;


/// effectue une "cinematique inverse", cad qu'on va controler une "chaine de joints" par autre chose que ses angles.
/// ici, on va la controler via un "point" cible que l'extermite de la "chaine de joints" doit atteindre.
/// 
/// Algo : voir cours
public class Step4_ik_jacobien_sansFormule : MonoBehaviour
{
	/// le but que l'on souhaite suivre.
	public Transform mTargetToFollow;

	/// chaine cinematique, du root vers les fils.
	/// elle est initialisee lors du PLAY par la fonction Start().
	/// MISE PUBLIC juste pour du debug par le biais de l'inspecteur de Unity3D
	public List<Transform> mChain;

	/// Temps ecoule en secondes depuis le dernier calcul de minimisation.
	public float mTimeSinceLastStep_s;

	/// Temps a attendre en secondes entre chaque calcul de minimisation.
	public float mTimeBetweenAutomaticUpdates_s;

	/// Initialisation : creer la chaine
	void Start ()
	{
		mTimeBetweenAutomaticUpdates_s = 0.2f;

		// positionner la camera 3D
		CameraHlp.setup3DCamera();
		Camera.main.gameObject.transform.Translate(0,0,-7.0f);

		mChain = new List<Transform>();

		// creer une pyramide
		float widthPyramide = 0.3f;
		float lengthPyramide = 2.0f;
		Vector3 p1 = new Vector3( 0,-widthPyramide,-widthPyramide);
		Vector3 p2 = new Vector3( 0, 0,             widthPyramide);
		Vector3 p3 = new Vector3( 0, widthPyramide,-widthPyramide);
		Vector3 p4 = new Vector3( lengthPyramide,0,0);
		Mesh lMesh = CreateMeshUnity.sCreateMeshFromTriangles("Tetrahedre", new List<Triangle3D>( new Triangle3D[]{
			new Triangle3D(p1,p4,p2),
			new Triangle3D(p3,p4,p1),
			new Triangle3D(p2,p4,p3),
			new Triangle3D(p2,p3,p1)
		} ) );

		mChain.Add( this.transform );
		for(int i = 0; i < 8; ++i)
		{
			var lChild = new GameObject("Fils_"+i.ToString());
			var lChildTrans = lChild.transform;

			lChildTrans.parent = mChain[mChain.Count -1];
			lChildTrans.localPosition = Vector3.right * 2.0f;
			lChild.AddComponent<MeshFilter>().sharedMesh = lMesh;
			var lRenderer = lChild.AddComponent<MeshRenderer>();
			lRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

			mChain.Add( lChildTrans );
		}
		mChain.RemoveAt(0);
		mChain[mChain.Count-1].localScale = Vector3.one * 0.1f; // on se moque de l'orientation du dernier element


		mTargetToFollow = GameObject.CreatePrimitive( PrimitiveType.Cube).transform;
		mTargetToFollow.Translate( 5,5,5);
	}
	
	/// Update() est appelee a chaque frame.
	/// On l'utilise pour executer la boucle de minimisation une fois de temps en temps.
	void Update()
	{
		mTimeSinceLastStep_s += Time.deltaTime;
		if( mTimeSinceLastStep_s > mTimeBetweenAutomaticUpdates_s)
		{
			// on effectue le calcul toutes les 0.2 secondes
			doOneCycle(); // juste une boucle, pas une infinite
			mTimeSinceLastStep_s = 0;
		}
	}

	/// une interface avec des boutons pour faciliter les tests
	public void OnGUI()
	{
		if(GUILayout.Button("repositionner target"))
		{
			mTargetToFollow.position = Random.insideUnitSphere*15.0f;
		}
		if(GUILayout.Button("effectuer le cycle de minimisation 1 fois"))
		{
			doOneCycle();
		}
		if(GUILayout.RepeatButton("effectuer le cycle de minimisation"))
		{
			doOneCycle();
		}

		if( mTimeBetweenAutomaticUpdates_s < 100000.0f)
		{
			if( GUILayout.Button( "Stopper les calculs automatiques" ))
			{
				mTimeBetweenAutomaticUpdates_s = 100000.0f;
			}else{
				GUILayout.Label( "Dur\u00E9e entre chaque calcul automatique" );
				mTimeBetweenAutomaticUpdates_s = GUILayout.HorizontalSlider(mTimeBetweenAutomaticUpdates_s, 0.0f, 3.0f );
			}

		}else{
			if( GUILayout.Button( "Activer les calculs automatiques" ))
			{
				mTimeBetweenAutomaticUpdates_s = 0.2f;
			}
		}
	}

	/// renvoie l'extremite de la chaine
	public Transform getLastBone()
	{
		if( mChain == null || mChain.Count == 0)
		{
			return null;
		}
		return mChain[mChain.Count-1];
	}

	// effectue 1 iteration de minimisation
	public void doOneCycle()
	{
		// sans utiliser la formule, on refabrique les derivees nous meme.

		//TOREMOVEFOREXERCISES_BEGIN

		// remplissage des axes de rotations et des bones associes (autant dans chaque vecteur)
		List<Vector3> axes_de_rotations = new List<Vector3>();
		List<Transform> bones_pour_chaque_axeDeRot = new List<Transform>();
		for(int i = 0; i < mChain.Count; ++i)
		{
			var lBone = mChain[i];

			Vector3 axe_rotation_1 = Vector3.right;
			Vector3 axe_rotation_2 = Vector3.up;

			axes_de_rotations.Add( axe_rotation_1 );
			axes_de_rotations.Add( axe_rotation_2 );

			bones_pour_chaque_axeDeRot.Add( lBone );
			bones_pour_chaque_axeDeRot.Add( lBone );
		}

		// calculs des derivees partielles pour chaque axe (cinematique DIRECTE, pas inverse)
		List<float[]> derivees_partielles = new List<float[]>();
		for(int i = 0; i < axes_de_rotations.Count; ++i)
		{
			var lBone = bones_pour_chaque_axeDeRot[i];
			var lAxeDeRot_w = axes_de_rotations[i];

			// calculer les derivees partielles en cinematique DIRECTE
			// (ca permettra de remplir le jacobien)
			//
			//   si on fait une rotation de 0.01, et que l'on bouge de (5,7,11)
			//   on en deduit qu'une rotation de '1' bougerait de (500,700,1100)
			//
			//   donc sur X la derivee partielle serait de 500
			//   et   sur Y la derivee partielle serait de 700
			//   et   sur Z la derivee partielle serait de 1100
			//
			// on considere que notre chaine est "fixe" sauf pour le juste l'axe de rotation qu'on etudie.
			// donc on a comme un "bras solide fixe" qui part de this et qui va jusqu'au bout.
			//
			// on veut savoir de combien l'extremite de ce bras bouge lorsqu'on fait une rotation (sur son axe de rotation).
			// instinctivement : 
			//   s'ils sont tous les deux dans le meme sens (l'axe de rotation et notre "bras solide fixe") alors ca bougera pas du tout!
			//   s'ils sont perpendiculaires, alors on se deplace sur un cercle parfait qui fait la longueur du "bras solide fixe".
			// 
			// on pourrait par exemple projeter sur le plan perpendiculaire a notre axe de rotation,
			//      puis effectuer la rotation avec des cos et sin.
			//
			// cependant les quaternions permettent d'effectuer cette rotation facilement aussi


			// notre bras fixe
			Vector3 toExtremity_w = getLastBone().position - lBone.position;

			// notre bras fixe apres un mouvement de 0.01f sur son angle
			Vector3 toExtremity_rotated_w = Quaternion.AngleAxis( 0.01f, lAxeDeRot_w ) * toExtremity_w ;
			Vector3 mouvement_for001 = toExtremity_rotated_w - toExtremity_w;

			// j'en deduis le "coeff directeur"
			Vector3 mouvement_for1 = mouvement_for001 * 100.0f;

			derivees_partielles.Add( new float[]{ mouvement_for1.x, mouvement_for1.y, mouvement_for1.z });
		}

		// fabriquer le jabobien
		DenseMatrix lJacobian = DenseMatrix.OfColumnArrays( derivees_partielles );

		// fabriquer sa pseudo inverse
		Matrix<float> lJacobianInverse = null;
		if( lJacobian.RowCount == lJacobian.ColumnCount)
		{
			lJacobianInverse = lJacobian.Inverse();
		}else{
			var lPseudoInverse_Jacobian = lJacobian.PseudoInverse();
			lJacobianInverse = lPseudoInverse_Jacobian; 
		}

		// dire quel est le mouvement que l'on veut faire
		Vector3 fromExtremiyToTarget_w = mTargetToFollow.position - getLastBone().position;

		// on le converti
		DenseVector lMovementVector = DenseVector.Create( 3, 0.0f);
		lMovementVector[0] = fromExtremiyToTarget_w.x;
		lMovementVector[1] = fromExtremiyToTarget_w.y;
		lMovementVector[2] = fromExtremiyToTarget_w.z;

		// effectuer le calcul avec l'inverse jacobian, qui nous donne les angles que l'on va devoir faire
		var lDeltaAngles_rad = lJacobianInverse * lMovementVector;

		// et on applique tout cela
		for(int i = axes_de_rotations.Count -1 ; i >= 0; --i)
		{
			var lBone = bones_pour_chaque_axeDeRot[i];

			float lDeltaAngle_deg = lDeltaAngles_rad[i] * Mathf.Rad2Deg ;

			// note : on n'applique pas le calcul complet
			float lDeltaAngleReduced_deg = lDeltaAngle_deg * 0.001f;

			lBone.Rotate( axes_de_rotations[i], lDeltaAngleReduced_deg , Space.World );
		}

		//TOREMOVEFOREXERCISES_END
	}
}

