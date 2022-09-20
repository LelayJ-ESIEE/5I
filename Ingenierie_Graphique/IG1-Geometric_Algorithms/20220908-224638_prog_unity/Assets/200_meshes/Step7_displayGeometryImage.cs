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
public class Step7_displayGeometryImage : MonoBehaviour
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

		//TOREMOVEFOREXERCISES_BEGIN

		// on renseigne nos sommets
		for(int j = 0; j < sizeY; ++j)
		{
			for(int i = 0; i < sizeX; ++i)
			{
				Color lNormal = Color.green;
				if( pNormal != null )
				{
					float percentI_01 = ((float)i) / (float)sizeX;
					float percentJ_01 = ((float)j) / (float)sizeY;
					lNormal =  pNormal.GetPixel( (int)((float)pNormal.width * percentI_01), (int)((float)pNormal.height* percentJ_01) );
				}

				Color lPixel = pTexture.GetPixel( i, j );

				// offsets dans la texture de base
				//int offset_I0_J0 = i     +(j*sizeX);

				// notre sommet (situe en top left sur le mini quad)
				lListVertices.Add( new Vector3(lPixel.r,  lPixel.g,  lPixel.b) * scale );
				lNormals.Add( new Vector3(lNormal.r * 2.0f - 1.0f, lNormal.g * 2.0f - 1.0f, lNormal.b * 2.0f - 1.0f) );
				lColors.Add( lPixel );
			}
		}

		// on cree nos triangles.
		for(int j = 0; j < sizeY-1; ++j)
		{
			for(int i = 0; i < sizeX-1; ++i)
			{
				// offsets dans la texture de base
				int offset_I0_J0 = i     +(j*sizeX);
				int offset_I1_J0 = (i+1) +(j*sizeX);
				int offset_I1_J1 = (i+1) +((j+1)*sizeX);
				int offset_I0_J1 = i     +((j+1)*sizeX);

				// premier triangle
				lIndices.Add( offset_I0_J0 );
				lIndices.Add( offset_I1_J0 );
				lIndices.Add( offset_I1_J1 );

				// deuxieme triangle
				lIndices.Add( offset_I0_J0 );
				lIndices.Add( offset_I1_J1 );
				lIndices.Add( offset_I0_J1 );
			}
		}


		//TOREMOVEFOREXERCISES_END 

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

		//TOREMOVEFOREXERCISES_BEGIN

		var lVertices = new Vector3[lNbPointsInTexture];
		var lNormals = new Vector3[lNbPointsInTexture];
		var lColors = new Color[lNbPointsInTexture];
		var lIndices = new int[lNbPointsInTexture  * 2 * 3];

		// on renseigne nos sommets
		for(int j = 0; j < sizeY; ++j)
		{
			for(int i = 0; i < sizeX; ++i)
			{
				float percentI_01 = ((float)i) / (float)sizeX;
				float percentJ_01 = ((float)j) / (float)sizeY;

				Color lPixel = pTexture.GetPixel( i, j);
				Color lNormal = pNormal.GetPixel( (int)((float)pNormal.width * percentI_01), (int)((float)pNormal.height* percentJ_01) );

				// offsets dans la texture de base
				int offset_I0_J0 = i     +(j*sizeX);

				// notre sommet (situe en top left sur le mini quad)
				lVertices[ offset_I0_J0 ] = new Vector3(lPixel.r,  lPixel.g,  lPixel.b);
				lNormals[  offset_I0_J0 ] = new Vector3(lNormal.r * 2.0f - 1.0f, lNormal.g * 2.0f - 1.0f, lNormal.b * 2.0f - 1.0f);
				lColors[ offset_I0_J0 ] = lPixel;
			}
		}

		// on cree nos triangles.
		int currentIndex = 0;
		for(int j = 0; j < sizeY-1; ++j)
		{
			for(int i = 0; i < sizeX-1; ++i)
			{
				// offsets dans la texture de base
				int offset_I0_J0 = i     +(j*sizeX);
				int offset_I1_J0 = (i+1) +(j*sizeX);
				int offset_I1_J1 = (i+1) +((j+1)*sizeX);
				int offset_I0_J1 = i     +((j+1)*sizeX);


				Vector3 v00 = lVertices[ offset_I0_J0 ];
				Vector3 v01 = lVertices[ offset_I0_J1 ];
				Vector3 v11 = lVertices[ offset_I1_J1 ];
				Vector3 v10 = lVertices[ offset_I1_J0 ];

				bool cutIsfrom_00_to_11 = (v00-v11).sqrMagnitude < (v01 - v10).sqrMagnitude;

				if( cutIsfrom_00_to_11 )
				{
					// premier triangle
					lIndices[currentIndex] = offset_I0_J0;
					++currentIndex;
					lIndices[currentIndex] = offset_I1_J0;
					++currentIndex;
					lIndices[currentIndex] = offset_I1_J1;
					++currentIndex;

					// deuxieme triangle
					lIndices[currentIndex] = offset_I0_J0;
					++currentIndex;
					lIndices[currentIndex] = offset_I1_J1;
					++currentIndex;
					lIndices[currentIndex] = offset_I0_J1;
					++currentIndex;
				}else{
					// premier triangle
					lIndices[currentIndex] = offset_I0_J0;
					++currentIndex;
					lIndices[currentIndex] = offset_I1_J0;
					++currentIndex;
					lIndices[currentIndex] = offset_I0_J1;
					++currentIndex;

					// deuxieme triangle
					lIndices[currentIndex] = offset_I0_J1;
					++currentIndex;
					lIndices[currentIndex] = offset_I1_J0;
					++currentIndex;
					lIndices[currentIndex] = offset_I1_J1;
					++currentIndex;
				}
			}
		}

		lNewMesh.vertices = lVertices;
		lNewMesh.normals = lNormals;
		lNewMesh.colors = lColors;
		lNewMesh.triangles = lIndices;

		return lNewMesh;
		//TOREMOVEFOREXERCISES_END return null;
	}

}
