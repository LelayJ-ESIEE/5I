using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class MergeOneMesh
{
	[MenuItem("Cours/Mettre tous les triangles dans 1 mesh")]
	public static void doIt()
	{
		if( Selection.activeGameObject == null )
		{
			Debug.LogError("Vous devez selectionner exactement 1 objet concerne.");
			return;
		}


		var lMeshFilter = Selection.activeGameObject.GetComponent<MeshFilter>();
		if( null == lMeshFilter )
		{
			Debug.LogError("Il n'y a pas de meshfilter sur l'objet selectionne.");
			return;
		}

		if( null == lMeshFilter.sharedMesh)
		{
			Debug.LogError("Le mesh est null dans le meshfilter de l'objet selectionne.");
			return;
		}

		var lMesh = lMeshFilter.sharedMesh;

		List<int> lAllTriangles = new List<int>();
		for(int s = 0; s < lMesh.subMeshCount; ++s)
		{
			var lTriangles = lMesh.GetTriangles( s );
			lAllTriangles.AddRange( lTriangles ); 
		}


		var lNewMesh = new Mesh();
		lNewMesh.name = lMesh.name + "_mergedTri";
		lNewMesh.vertices = lMesh.vertices;
		lNewMesh.normals = lMesh.normals;
		lNewMesh.uv = lMesh.uv;
		lNewMesh.uv2 = lMesh.uv2;
		lNewMesh.bindposes = lMesh.bindposes;
		lNewMesh.boneWeights = lMesh.boneWeights; 
		lNewMesh.triangles = lAllTriangles.ToArray();

		AssetDatabase.CreateAsset( lNewMesh, "Assets/Resources/"+lNewMesh.name+".asset");
		AssetDatabase.SaveAssets();
	}


}

