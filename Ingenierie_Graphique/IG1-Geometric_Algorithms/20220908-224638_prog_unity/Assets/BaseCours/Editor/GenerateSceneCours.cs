using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// generer les GameObjects organises comme les repertoires de code
/// 1 GO par type de MonoBehaviour qui m'interesse.
public class GenerateSceneCours
{
	[MenuItem("Cours/Generer la scene")]
	public static void doIt()
	{
		// "as" signifie "antislash"
		List<string> directories_as = new List<string>();
		directories_as.Add("Assets\\100_forces");
		directories_as.Add("Assets\\200_meshes");
		directories_as.Add("Assets\\300_anims");
		directories_as.Add("Assets\\400_sdf");

		foreach(string dir_as in directories_as )
		{ 
			// creer la categorie
			GameObject lCategory = new GameObject();
			lCategory.SetActive(false); 
			lCategory.name = dir_as.Substring(dir_as.LastIndexOf("_") + 1);

			// recup les fichiers .cs
			List<string> files = new List<string>( Directory.GetFiles(dir_as) );
			for(int f = 0; f < files.Count; ++f)
			{
				files[f] = Path.GetFileName( files[f] );
			}

			// trier pour que les dizaines soient dans l'ordre
			files.Sort( (a,b)=>{
				int indexOfUnderscoreA = a.IndexOf('_');
				int indexOfUnderscoreB = b.IndexOf('_');
				if( indexOfUnderscoreA == indexOfUnderscoreB )
				{
					return a.CompareTo(b);
				}else{
					return indexOfUnderscoreA.CompareTo(indexOfUnderscoreB);
				}
			});

			// creer les go correspondants a ces fichiers
			foreach( string file in files )
			{
				if( !file.ToUpper().EndsWith(".CS") )
				{
					// on evite les meta notamment
					continue;
				}
				string lFileNoExt = Path.GetFileNameWithoutExtension( file );
				System.Type lType = sGetTypeByName( lFileNoExt );
				if( lType == null )
				{
					Debug.LogError("impossible de trouver le type : " + lFileNoExt  );
					continue;
				}

				GameObject lElement = new GameObject( lFileNoExt );
				lElement.transform.parent = lCategory.transform;

				lElement.AddComponent( lType );
				lElement.SetActive(false);
			}
		}
	}

	/// trouver un type grace au nom
	public static Type sGetTypeByName(string pTypeName)
	{
		var lAssemblies = System.AppDomain.CurrentDomain.GetAssemblies();
		Type result = null;
		for(int a = 0 ; result == null && a < lAssemblies.Length; ++a)
		{
			try
			{
				//result = Types.GetType( pInput, lAssemblies[i].FullName);
				var lTypes = lAssemblies[a].GetTypes();
				for(int t = 0; t < lTypes.Length; ++t)
				{
					try
					{
						Type lType = lTypes[t];
						if( lType.Name == pTypeName)
						{
							result = lType;
						}
					}
					catch(Exception)
					{
						// certaines assemblies sont protegees
					}
				}
			}
			catch(Exception)
			{
				
			}
		}
		return result;
	}

}
