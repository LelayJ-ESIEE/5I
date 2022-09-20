using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// quelques fonctions pour aider la manipulation de textures.
public class TextureHlp
{
	/// Effectuer un filtre median (qui supprime les erreurs de type poivre et sel)
	/// pour cela on va faire pour plein de petits groupes : 
	///        "trie les couleurs avec un Sort()" et "garde celle du milieu de la liste triee".
	///
	/// important : il faut faire Apply() sur la texture apres cet appel pour voir le changement.
	public static void filtreMedian(Texture2D pTexture)
	{
		List<Color> lColors = new List<Color>(pTexture.width * pTexture.height);
		for(int j = 0; j < pTexture.height; ++j)
		{
			for(int i = 0; i < pTexture.width; ++i)
			{ 
				lColors.Add( pTexture.GetPixel(i,j));
			}
		}

		// supprime les traces noires, trous.. avec un filtre median
		int w = pTexture.width;
		int h = pTexture.height;

		// horizontal
		for(int j = 2; j < h-2; ++j)
		{
			for(int i = 0; i < w; ++i)
			{ 
				// lire les freres
				Color c1 = pTexture.GetPixel(i,j-2);
				Color c2 = pTexture.GetPixel(i,j-1);
				Color c3 = pTexture.GetPixel(i,j);
				Color c4 = pTexture.GetPixel(i,j+2);
				Color c5 = pTexture.GetPixel(i,j+1);

				List<float> toSort = new List<float>();
				toSort.Add( c1.r );
				toSort.Add( c2.r );
				toSort.Add( c3.r );
				toSort.Add( c4.r );
				toSort.Add( c5.r );
				toSort.Sort();
				float r = toSort[2];

				toSort.Clear();
				toSort.Add( c1.g );
				toSort.Add( c2.g );
				toSort.Add( c3.g );
				toSort.Add( c4.g );
				toSort.Add( c5.g );
				toSort.Sort();
				float g = toSort[2];

				toSort.Clear();
				toSort.Add( c1.b );
				toSort.Add( c2.b );
				toSort.Add( c3.b );
				toSort.Add( c4.b );
				toSort.Add( c5.b );
				toSort.Sort();
				float b = toSort[2];

				lColors[j *w + i] = new Color(r,g,b,1);
			}
		}
		for(int j = 0; j < h; ++j)
		{
			for(int i = 0; i < w; ++i)
			{ 
				pTexture.SetPixel(i,j, lColors[j * w + i]);
			}
		}

		// vertical
		for(int j = 0; j < h; ++j)
		{
			for(int i = 2; i < w-2; ++i)
			{ 
				// lire les freres
				Color c1 = pTexture.GetPixel(i-2,j);
				Color c2 = pTexture.GetPixel(i-1,j);
				Color c3 = pTexture.GetPixel(i,j);
				Color c4 = pTexture.GetPixel(i+1,j);
				Color c5 = pTexture.GetPixel(i+2,j);

				List<float> toSort = new List<float>();
				toSort.Add( c1.r );
				toSort.Add( c2.r );
				toSort.Add( c3.r );
				toSort.Add( c4.r );
				toSort.Add( c5.r );
				toSort.Sort();
				float r = toSort[2];

				toSort.Clear();
				toSort.Add( c1.g );
				toSort.Add( c2.g );
				toSort.Add( c3.g );
				toSort.Add( c4.g );
				toSort.Add( c5.g );
				toSort.Sort();
				float g = toSort[2];

				toSort.Clear();
				toSort.Add( c1.b );
				toSort.Add( c2.b );
				toSort.Add( c3.b );
				toSort.Add( c4.b );
				toSort.Add( c5.b );
				toSort.Sort();
				float b = toSort[2];

				lColors[j *w + i] = new Color(r,g,b,1);
			}
		}
		for(int j = 0; j < h; ++j)
		{
			for(int i = 0; i < w; ++i)
			{ 
				pTexture.SetPixel(i,j, lColors[j * w + i]);
			}
		}
	}

	/// pour camoufler les erreurs du cote gauche de la texture, on prend la colonne 1 et on la copie sur la colonne 0
	/// important : il faut faire Apply() sur la texture apres cet appel pour voir le changement.
	public static void doubleLeftBorder(Texture2D pTexture)
	{
		for(int j = 0; j < pTexture.height; ++j)
		{
			int i = 0;
			Color c = pTexture.GetPixel(i+1,j);
			pTexture.SetPixel(i,j,c);
		}
	}

	/// pour camoufler les erreurs du "bord bas" de la texture, on prend la ligne juste au dessus et on la copie sur ce bord.
	/// important : il faut faire Apply() sur la texture apres cet appel pour voir le changement.
	public static void doubleBottomBorder(Texture2D pTexture)
	{
		for(int i = 0; i < pTexture.width; ++i)
		{
			int j = 0;
			Color c = pTexture.GetPixel(i, j+1);
			pTexture.SetPixel(i,j,c);
		}
	}

}