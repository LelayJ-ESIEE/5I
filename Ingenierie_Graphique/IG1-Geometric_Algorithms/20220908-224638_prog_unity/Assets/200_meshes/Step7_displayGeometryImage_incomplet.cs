using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// but : dessiner le contenu de gim_3D_map.png
/// qui est une geometry image qui a deja ete calculee
/// (source : http://hhoppe.com/proj/gim/)
/// 
/// 
/// note : c'est la base de la techno NANITE utilisee dans la demo de l'UE5 sur la PS5.
/// 		source : https://twitter.com/briankaris/status/1260590413003362305?lang=en
///         source: http://graphicrants.blogspot.com/2009/01/virtual-geometry-images.html
/// 
/// 
/// Vous devez completer la fonction "createFromImage" en creant la grille. 
/// Il faut remplir le tableau de sommets et d'indices notamment.
/// 
public class Step7_displayGeometryImage_incomplet : MonoBehaviour
{
	/// nom du fichier de la texture d'albedo dans le dossier 'Resources'
	public string resources_file_base = "gim_3D_map";
	/// nom du fichier de la texture de normale dans le dossier 'Resources'
	public string resources_file_normal = "gim_normal_map";

	// Use this for initialization
	void Start()
	{
		// positionner camera
		var lCamera = CameraHlp.setup3DCamera();
		lCamera.transform.SetPositionAndRotation( new Vector3(0.427f, 0.435f, 2.046f), Quaternion.AngleAxis(176.0f,Vector3.up)); 
		lCamera.backgroundColor = Color.cyan;

		// creer notre objet 3D
		GameObject go = new GameObject("GeometryImage");
		var lMeshFilter = go.AddComponent<MeshFilter>();
		var lMeshRenderer = go.AddComponent<MeshRenderer>();
		//lMeshRenderer.material = new Material( Shader.Find("Standard"));
		lMeshRenderer.material = new Material( Shader.Find("CoursVertexColorDoubleFace"));

		Debug.Log("Attention, prenez soin d'importer la texture en mode NON COMPRESSEE. C'est s\u00E9lectionnable dans les options d'import d'Unity3D.");

		var lImage = Resources.Load( resources_file_base ) as Texture2D;
		if( lImage == null )
		{
			Debug.LogError("Impossible de charger l'image des Resources : "+resources_file_base);
			return;
		}

		var lImageNormal = Resources.Load( resources_file_normal ) as Texture2D;
		if( lImage == null )
		{
			Debug.LogError("Impossible de charger l'image des Resources " +resources_file_normal);
			return;
		}

		lMeshFilter.mesh = createFromImage( lImage, lImageNormal );
		//lMeshFilter.mesh = createFromImageV2( lImage, lImageNormal );
	}

	/// fonction qui recoit une image, et en fait un mesh, en utilisant le principe des "geometry images" de Hugues Hoppe
	/// 
	/// chaque pixel de pTexture contient la position 3D d'un sommet ( RGB devient donc X Y Z du sommet). 
	/// on trace une grille reguliere entre les sommets.
	/// on suppose qu'il n'y a pas de trou dans la texture (note : c'est faux, cela se verra lors de l'affichage).
	///
	public static Mesh createFromImage( Texture2D pTexture, Texture2D pNormal, float scale = 200.0f )
	{
		Mesh lNewMesh = new Mesh();
		lNewMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // mettre en 32 bits pour avoir plus de 65535 elements
		lNewMesh.MarkDynamic();

		// dimension texture
		int sizeX = pTexture.width;
		int sizeY = pTexture.height;
		int lNbPointsInTexture = sizeX * sizeY;

		var lListVertices = new List<Vector3>(lNbPointsInTexture);
		var lNormals = new List<Vector3>(lNbPointsInTexture);
		var lColors = new List<Color>(lNbPointsInTexture);
		List<int> lIndices = new List<int>( 2 * 3 * lNbPointsInTexture); // en moyenne il y aura 6 triangles par sommet

		// pour acceder a un pixel situe en i,j : pTexture.GetPixel(i,j)

		// TODO COMPLETEZ ICI
 

		lNewMesh.SetVertices( lListVertices );
		lNewMesh.SetNormals( lNormals );
		lNewMesh.SetColors( lColors );
		// Tres utile pour debugguer : juste dessiner des points. 
		// Il faut toutefois transmettre une liste qui contient [0 1 2 3 ... Nb points-1]
		// lNewMesh.SetIndices( lIndices, MeshTopology.Points, 0);

		// dessiner des triangles
		lNewMesh.SetTriangles( lIndices, 0);

		return lNewMesh;
	}

	/// fonction qui recoit une image, et en fait un mesh, en utilisant le principe des "geometry images" de Hugues Hoppe
	/// amelioration : on met la diagonale des quads selon la distance la plus courte.
	public static Mesh createFromImageV2( Texture2D pTexture, Texture2D pNormal )
	{
		Mesh lNewMesh = new Mesh();
		lNewMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // mettre en 32 bits pour avoir plus de 65535 elements
		lNewMesh.MarkDynamic();

		int sizeX = pTexture.width;
		int sizeY = pTexture.height;
		int lNbPointsInTexture = pTexture.height * pTexture.width;

		// TODO COMPLETEZ ICI
 return null;
	}

}

