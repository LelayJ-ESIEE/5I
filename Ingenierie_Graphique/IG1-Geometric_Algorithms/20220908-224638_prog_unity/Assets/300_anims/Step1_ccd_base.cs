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
/// 3/ on considère "l'avant dernier de la chaine" bout de la chaine, et on calcule l'angle qui minimise la distance entre "le bout" et "la cible".
///      on applique cet angle sur "l'avant dernier de la chaine"
/// 
/// 4/ on refait comme 3 mais en remplacant "l'avant dernier de la chaine" par "l'avant-avant dernier de la chaine".
/// 	c'est a dire qu'on remonte d'1 cran, et à chaque fois on calcule l'angle qui minimise la distance entre "le bout" et "la cible".
/// 
/// on refait l'étape 4/ en remontant tous les joints. Quand on a fait une fois le cycle, ce n'est pas terminé. On peut recommencer en 3/.
/// on arrete quand on estime que "le bout" et "la cible" sont suffisamment proches.
/// 
/// Remarque : 
/// Pour le "calcul de l'angle qui minimise la distance entre le bout et la cible pour le bone B" qui a lieu en 3/ et 4/: 
///       	Calculer le vecteur "depuis BONE vers BOUT"  : on l'appelle "versBOUT"
/// 		Calculer le vecteur "depuis BONE vers CIBLE" : on l'appelle "versCIBLE"
/// 		Calculer le quaternion qui passe de "versBOUT" à "versCIBLE" 
/// 			(il y a des fonctions sur étagères pour cela dans Quaternion avec Quaternion.FromToRotation). 
/// 		Appliquer ce quaternion à notre BONE.
/// 
/// 
/// USAGE : il est utilise dans les jeux videos pro, notamment pour faire 
///   - que les pieds touchent le sol au bon moment.
///   - que les mains qui clappent se touchent.
///   - que les epees frappent au bon endroit.
///   - que les mains prennent correctement les objets poses sur des tables, etc...
///   - que les personnages se regardent.
/// 
/// Il est tres flexible car on peut facilement ajouter des contraintes.
/// 
public class Step1_ccd_base : MonoBehaviour
{
	/// le but que l'on souhaite suivre.
	public Transform mTargetToFollow;

	/// chaine cinematique, du root vers les fils.
	/// elle est initialisee lors du PLAY par la fonction Start().
	/// le "public" permet de la voir dans l'inspecteur unity.
	/// (ce n'est pas une bonne idee en general de bosser ainsi, c'est pour vous faciliter le debug)
	public List<Transform> mChain;

	/// Temps ecoule en secondes depuis le dernier calcul de minimisation.
	public float mTimeSinceLastStep_s;

	/// Temps a attendre en secondes entre chaque calcul de minimisation.
	public float mTimeBetweenAutomaticUpdates_s;


	/// Initialisation : 
	///   recuperer tous les "non-Cylinder" fils qui forment la chaine.
	void Start ()
	{
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

	/// fonction que l'on cherche a minimiser
	/// doit renvoyer une valeur qui est 0 quand on a parfaitement reussi, et sinon une valeur plus grande que 0.
	public float fonctionDeCout()
	{
		Vector3 end_rs = getLastBone().position;
		Vector3 target_rs = mTargetToFollow.position;
		float longueur = (end_rs - target_rs).magnitude;
		return longueur;
	}

	/// Execution du CCD juste 1 fois pour chaque joint de la chaine cinematique,
	/// en commencant par la derniere (extremite), et en remontant a la racine de la chaine.
	public void doOneCycle()
	{
		// Quaternion.FromToRotation vous donnera la rotation pour passer d'une rotation a l'autre

		// "getLastBone()" est le dernier element de la chaine

		// ".position" vous donne la position world dans unity.

		//TOREMOVEFOREXERCISES_BEGIN

		// part de la fin, et tente de faire pointer vers extremite.
		for(int i = mChain.Count -1 ; i >= 0; --i)
		{
			var lBone = mChain[i];

			Vector3 end_w = getLastBone().position;
			Vector3 bone_w = lBone.position;
			Vector3 target_w = mTargetToFollow.position;

			// obtenir les 2 directions qui m'interessent
			Vector3 dirBone2End_w = (end_w - bone_w).normalized;
			Vector3 dirBone2Target_w = (target_w - bone_w).normalized;

			// obtenir l'angle de passage d'une direction a l'autre
			Quaternion rotationToApply_w = Quaternion.FromToRotation(dirBone2End_w, dirBone2Target_w);

			// si on veut ne pas l'appliquer a 100%, mais juste entamer le bon mouvement,
			// alors on peut faire une petite interpolation. ici seulement de "20% vers le mouvement maximum".
			// rotationToApply_w = Quaternion.Lerp( Quaternion.identity, rotationToApply_w, 0.20f);

			// on peut l'appliquer. 

			// V1 : fonctionne, lisible mais un peu plus couteuse car 2 manip de world
			// lBone.rotation =  rotationToApply_w * lBone.rotation;

			// V2 : fonctionne, moins couteuse, car juste 1 manip world.
			float angle_d_rootSpace;
			Vector3 axis_rootSpace;
			rotationToApply_w.ToAngleAxis(out angle_d_rootSpace, out axis_rootSpace);  
			lBone.Rotate( axis_rootSpace, angle_d_rootSpace, Space.World);
		}

		//TOREMOVEFOREXERCISES_END
	}
}

