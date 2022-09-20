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
public class Step4_ik_jacobien_sansFormule_incomplet : MonoBehaviour
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

		// TODO COMPLETEZ ICI

	}
}


