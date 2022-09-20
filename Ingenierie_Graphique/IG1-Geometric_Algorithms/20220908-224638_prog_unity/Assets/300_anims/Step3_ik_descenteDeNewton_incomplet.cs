using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// effectue une "cinematique inverse", cad qu'on va controler une "chaine de joints" par autre chose que ses angles.
/// ici, on va la controler via un "point" cible que l'extermite de la "chaine de joints" doit atteindre.
/// 
/// Algo :
/// 
/// 0/ on a deja cree une succession de pere-fils qu'on appelle une "chaine cinematique" en general.
/// 
/// 1/ j'appelle le bout de la chaine "le bout". C'est le fils du fils du fils etc.... de la racine de la chaine. 
///    On appelle "bone" n'importe quel element de la chaine.
/// 
/// 2/ on a une cible (target en anglais) qui ne changera pas pendant tout l'algorithme.
/// 
/// 3/ on considère "l'avant dernier de la chaine" bout de la chaine, et on CHERCHE l'angle qui minimise la distance entre "le bout" et "la cible".
///      on applique cet angle sur "l'avant dernier de la chaine"
/// 
/// 4/ on refait comme 3 mais en remplacant "l'avant dernier de la chaine" par "l'avant-avant dernier de la chaine".
/// 	c'est a dire qu'on remonte d'1 cran, et à chaque fois on CHERCHE l'angle qui minimise la distance entre "le bout" et "la cible".
/// 
/// on refait l'étape 4/ en remontant tous les joints. Quand on a fait une fois le cycle, ce n'est pas terminé. On peut recommencer en 3/.
/// on arrete quand on estime que "le bout" et "la cible" sont suffisamment proches.
/// 
/// Remarque : 
/// Pour la "RECHERCHE de l'angle qui minimise la distance entre le bout et la cible pour le bone B" qui a lieu en 3/ et 4/: 
///   - pour chaque angle 
/// 		- faire varier l'angle dans 1 sens et evaluer la fonction de cout
/// 		- noter l'evolution de la fonction de cout en fonction de la taille de l'angle.
/// 		- en deduire quel angle il faut appliquer pour vraiment reduire au maximum.
/// 			ex: la fonction de cout passe de 4 a 6, lorsqu'on a juste change x de +0.2. 
/// 					Donc on s'est eloigne, et "1 mouvement de +0.2 sur x" nous a coute "+2" sur la fonction de cout.
/// 					Donc on s'est eloigne, et "1 mouvement de +1 sur x" nous aurait coute "+10" sur la fonction de cout.
/// 					Donc on s'est eloigne, et "1 mouvement de -0.4 sur x" nous aurait coute "-4" sur la fonction de cout.
/// 					-> donc on applique -4 (c'est une regle de 3).
/// 		
/// 		1/ EVIDEMMENT CELA VA ECHOUER AVEC DES ANGLES SI ON NE MET PAS DE LIMITES.
/// 		2/ C'EST PAS MEGA STABLE pour l'IK (ce n'est pas utilise pour de l'IK), 
/// 			mais quand ca marche, cela converge plus vite que la technique "gauche droite", et le code est presque identique.
/// 
/// 
public class Step3_ik_descenteDeNewton_incomplet : MonoBehaviour
{
	/// le but que l'on souhaite suivre.
	public Transform mTargetToFollow;

	/// chaine cinematique, du root vers les fils.
	/// elle est initialisee lors du PLAY par la fonction Start().
	/// MISE PUBLIC juste pour du debug par le biais de l'inspecteur de Unity3D
	public List<Transform> mChain;

	/// Temps ecoule en secondes depuis le dernier calcul de minimisation.
	private float mTimeSinceLastStep_s;

	/// Temps a attendre en secondes entre chaque calcul de minimisation.
	public float mTimeBetweenAutomaticUpdates_s;

	/// Initialisation : 
	///   recuperer tous les "non-Cylinder" fils qui forment la chaine.
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
	private Transform getLastBone()
	{
		if( mChain == null || mChain.Count == 0)
		{
			return null;
		}
		return mChain[mChain.Count-1];
	}

	/// fonction que l'on cherche a minimiser
	/// doit renvoyer une valeur qui est 0 quand on a parfaitement reussi, et sinon une valeur plus grande que 0.
	public float fonctionDeCout()
	{
		Vector3 end_rs = getLastBone().position;
		Vector3 target_rs = mTargetToFollow.position;
		float longueur = (end_rs - target_rs).magnitude;
		return longueur;
	}

	/// Execution du cycle de minimisation juste 1 fois pour chaque joint de la chaine cinematique,
	/// en commencant par la derniere (extremite), et en remontant a la racine de la chaine.
	public void doOneCycle()
	{
		// TODO COMPLETEZ ICI

	}

}


