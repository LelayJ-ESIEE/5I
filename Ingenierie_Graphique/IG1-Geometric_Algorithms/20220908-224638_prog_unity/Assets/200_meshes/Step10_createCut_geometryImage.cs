using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// but : 
///  Etant donne un "maillage manifold" ainsi qu'un cut (c'est un ensemble d'edge qui se touchent sans faire de boucle),
///    deplier le mesh sur un UV.
/// 
/// 
/// definition "mesh manifold" :
///  - pour faire simple, il n'a pas de bordure, il est ferme, et pas degenere
///  - en math : il n'y a pas de vertex seul, et chaque edge touche 2 triangles exactement.
/// 
/// 
public class Step10_createCut_geometryImage : MonoBehaviour
{
	Mesh3D mMesh;

	// Use this for initialization
	void Start()
	{
		// positionner camera
		var lCamera = CameraHlp.setup3DCamera();
		lCamera.transform.position = new Vector3(0,34,-68);
		lCamera.backgroundColor = Color.cyan;


		var lGo = Resources.Load("GermanShephardLowPoly") as GameObject;
		if( null == lGo )
		{
			Debug.LogError("Impossible de trouver le modele 3D a charger");
			enabled = false;
			return;
		}

		MeshFilter meshFilter = lGo.GetComponentInChildren<MeshFilter>();
		if( null == meshFilter || null == meshFilter.sharedMesh)
		{
			Debug.LogError("L'objet n'a pas de mesh ou de meshfilter");
			enabled = false;
			return;
		}

		mMesh = new Mesh3D();
		mMesh.initFrom( meshFilter.sharedMesh );
		mMesh.mergePointsAtSamePositions();
		mMesh.scale( 5 );


		// definir l'ensemble des edges du cut
		var lCut = new List<Edge>();
		lCut.Add( new Edge( 20, 19) );
		lCut.Add( new Edge( 19, 18) );

		// ci dessous un cut plus grand.
		//lCut.Add( new Edge( 192, 197) );
		//lCut.Add(new Edge(197, 199));
		//lCut.Add(new Edge(199, 200));
		//lCut.Add(new Edge(200, 92));
		//lCut.Add(new Edge(92, 5));

		// effectuer le cut, en doublonnant des sommets, et donc creant un trou dans le mesh ferme
		createCut( lCut, mMesh );

		mMesh.visual_update();
	}

	/// ajoute des sommets pour que l'on puisse ouvrir les edges de pCut "comme une fermeture eclair".
	public static void createCut(List<Edge> pCut, Mesh3D pMesh)
	{
		if( pCut.Count == 0)
		{
			Debug.LogError("cut vide, on ne cree pas de cut");
			return;
		}

		// message d'intro
		{
			string message = "";
			for(int c = 0; c < pCut.Count; ++c)
			{
				var lEdge = pCut[c];
				message += lEdge.ToString() +" ";
			}
			Debug.Log("Creation d'un cut utilisant les links : " + message );
		}

		// 1/ trouver les sommets concernes par davantage que 1 edge de cut
		var lCutChanging = new List<int>();
		for(int c = 0; c < pCut.Count; ++c)
		{
			var lEdge = pCut[c];
			for(int v = 0; v < 2; ++v)
			{
				int indexVertex = lEdge.getByIndex(v);
				if( lCutChanging.Contains( indexVertex ))
				{
					// pas besoin de l'ajouter, on sait deja qu'il est pris en compte
					continue;
				}

				int index1InCut = pCut.FindIndex( 0, (x) =>{ return x.hasVertexIndex( indexVertex); } );
				if( index1InCut == pCut.Count -1 )
				{
					continue;
				}

				// on regarde si on le trouve apres
				int index2InCut = pCut.FindIndex( index1InCut + 1, (x) =>{ return x.hasVertexIndex( indexVertex); } );
				if( index2InCut != -1 )
				{
					lCutChanging.Add( indexVertex);
				}
			}
		}

		// sort descending (juste pour du debug)
		// lCutChanging.Sort( (a,b)=>{ return b.CompareTo(a);} );


		// 2/ pour chacun de ces sommets, on va devoir creer des doublons, voire plus.
		// - a chaque fois, on va partir d'un triangle qui longe le cut, puis on va trouver le triangle suivant
		//  etc.. jusqu'a faire le tour.
		// - quand on arrive sur un nouvel edge qui fait parti du cut, alors :
		//        1/ creer un nouveau sommet.
		//        2/ relier tous les triangles depuis l'edge du cut precedent vers ce nouveau sommet au lieu de celui du cut.
		//        3/ si jamais c'etait le premier edge, alors on peut passer au traitement du sommet suivant.
		for(int cc = 0; cc < lCutChanging.Count; ++cc)
		{
			int originalVertex = lCutChanging[cc];
			//Debug.Log("Cut : traitement pour le sommet : "+ originalVertex );

			var lCutGroups_around1Vertex = sGetGroupsSeparatedByCut_arount1Vertex( pMesh.mFaces, originalVertex, pCut);
			if( lCutGroups_around1Vertex == null )
			{
				Debug.LogError("Cut : erreur de calcul?");
				continue;
			}
			if( lCutGroups_around1Vertex.Count < 2)
			{
				//Debug.Log("pas besoin de faire de changement pour le sommet original "+originalVertex+", car il ne touche qu'un seul 'groupe'. (pas de cut qui le separe completement)");
				continue;
			}

			// affichage debug des cut groups
			foreach( var lCutGroup_around1Vertex in lCutGroups_around1Vertex)
			{
				//Debug.Log("      cutgroup :" + Triplet.sToString( lCutGroup_around1Vertex ) + "    donc "+lCutGroup_around1Vertex.Count+" triplets." );
				if( lCutGroup_around1Vertex.Count == 0)
				{
					Debug.LogError("      ce cutgroup est vide! il fallait probablement le mettre a jour aussi.");	
				}
			}

			// maintenant on peut creer 1 new vertex pour chaque groupe, et relier les elements du groupe a ce new vertex.
			List<int> newVertexIndexes = new List<int>();
			for(int g = 0; g < lCutGroups_around1Vertex.Count; ++g)
			{
				var lTrianglesGroup = lCutGroups_around1Vertex[g];

				// creation du vertex
				Vector3 originalVertex3D = pMesh.mPoints[ originalVertex ];
				pMesh.mPoints.Add( originalVertex3D );
				int newVertex = pMesh.mPoints.Count - 1;

				newVertexIndexes.Add( newVertex);

				// changement des references dans les triangles du groupe
				for(int t = 0; t < lTrianglesGroup.Count; ++t)
				{
					var lTri = lTrianglesGroup[t];
					lTri.replace( originalVertex, newVertex );
				}
			}

			// mettre a jour le cut!
			for(int cutI = 0; cutI < pCut.Count; ++cutI)
			{
				var lEdge = pCut[cutI];
				if( !lEdge.hasVertexIndex( originalVertex ))
				{
					continue;
				}

				//string ajout_dans_cut = "{";

				// il pointait vers l'ancien sommet => on le met a jour
				foreach( var newVertIndex in newVertexIndexes)
				{
					var lNewEdge = new Edge( lEdge);
					lNewEdge.replace( originalVertex, newVertIndex);
					if( !lNewEdge.isInList_unordered( pCut ) )
					{
						pCut.Add( lNewEdge );
						//ajout_dans_cut += lNewEdge.ToString();
					}
				}
				//ajout_dans_cut +="}";

				//Debug.Log("      MAJ du Cut, on enleve "+lEdge.ToString()+",   et on ajoute "+ ajout_dans_cut); 

				pCut.RemoveAt(cutI);
				--cutI;
			}
		}
	}

	/// supprime les sommets qui sont seuls
	/// renvoie la liste des sommets supprimes, en ordre d'indice decroissant
	public static List<int> sRemoveUnusedVertex(Mesh3D pMesh)
	{
		List<int> removed = new List<int>();
		for(int v = pMesh.mPoints.Count-1; v >=0 ; --v)
		{
			bool used = false;
			for(int f = 0; f < pMesh.mFaces.Count; ++f)
			{
				if( pMesh.mFaces[f].has( v ))
				{
					used = true;
					break;
				}
			}
			if(!used)
			{
				pMesh.removePoint( v );
				removed.Add( v );
			}
		}
		return removed;
	}

	/// renvoie tous les triangles qui touchent vertexIndex.
	/// ils sont separes en plusieurs listes, qui correspondent a des CUT.
	/// Bref : s'il y a 2 edge de CUT qui arrivent sur un sommet , alors ceci renverra les 2 groupes separes par cette ligne brisee que forme ces "edges du cut"
	/// Bref : s'il y a 3 edge de CUT qui arrivent sur un sommet , alors ceci renverra les 3 groupes separes par ces 3 "edges du cut"
	///  (imaginer un camembert vu du dessus)
	/// renvoie null si gros probleme
	public static List<List<Triplet>> sGetGroupsSeparatedByCut_arount1Vertex(List<Triplet> pAllTriangles, int vertexIndex, List<Edge> pCut)
	{
		// une fonction qui va nous aider ensuite
		// (triplet, numero_vertice, Link) => renvoie l'autre Link qui touche numero_vertice
		System.Func<Triplet, int, Edge, Edge> func_findOtherLinkTouchingVertex = (ppTriangle, ppVertex, ppLink) =>
		{
			if( ppTriangle == null )
			{
				Debug.LogError("recu triangle null!");
				return new Edge(-100,-100);
			}
			for(int l = 0; l < 3; ++l)
			{
				Edge link = ppTriangle.getEdge(l);
				if( link.hasVertexIndex(ppVertex) && !link.isSame_u( ppLink ))
				{
					return link;
				}
			}
			Debug.LogError("on ne trouve pas de link satisfaisant dans notre triangle "+ppTriangle.ToString());
			// on cree une erreur
			return new Edge(-100,-100);
		};

		// trouver tous les triangles qui touchent v
		List<Triplet> lTrianglesToSearch = pAllTriangles.FindAll( (tri) =>{ return tri.has( vertexIndex ); } );
		//Debug.Log("         triangles qui touchent "+vertexIndex+" : "+ Triplet.sToString(lTrianglesToSearch)  );


		// groupes de triangles separes par les cut
		List<List<Triplet>> lGroupedByCuts = new List<List<Triplet>>();

		int maxIteration = 100; // on ne va quand meme pas avoir 100 edge sur 1 sommet!

		// dans un cas ideal on ne passe qu'une fois dans cette boucle.
		// cependant, si une operation precedente a Doublonne un EDGE,
		//            alors il arrive que la recherche n'arrive pas a faire le "tour du vertex"
		//
		// ce while permet dans ce cas de reprendre le calcul sur "les triangles restants a traiter".
		while( maxIteration > 0 && lTrianglesToSearch.Count > 0)
		{
			--maxIteration;

			// trouver parmi les triangles restant, un qui touche le cut avec vertexIndex.
			Triplet lTri = null;
			Edge edge_du_cut = null;

			//V2 
			foreach( var lTriToSearch in lTrianglesToSearch )
			{
				foreach( var lEdgeDuCut in pCut )
				{
					if( lEdgeDuCut.hasVertexIndex( vertexIndex ) && lTriToSearch.has_u( lEdgeDuCut ))
					{
						edge_du_cut = lEdgeDuCut;
						lTri = lTriToSearch;
						break;
					}
				}
			}

			if( lTri == null )
			{
				// message d'erreur
				string listTri_string = "";
				foreach( var lTriangleToSearch in lTrianglesToSearch)
				{
					listTri_string += lTriangleToSearch.ToString(); 
				}
				if (edge_du_cut != null) {
					Debug.LogError("impossible de trouver un triangle de depart parmi " + listTri_string + " contenant l'edge de depart" + edge_du_cut.a +" , "+ edge_du_cut.b+"]");
                }
                else
                {
					Debug.LogError("impossible de trouver un triangle de depart parmi " + listTri_string + " contenant l'edge de depart (null?!)");
				}
				return null;
			}

			// on peut enlever ce triangle de la liste de ceux a traiter
			lTrianglesToSearch.Remove( lTri );

			var lCurrentEdge = edge_du_cut;

			lGroupedByCuts.Add( new List<Triplet>()); 
			lGroupedByCuts[lGroupedByCuts.Count-1].Add( lTri );

			// ceci alternera entre 1 et 2.
			int nbEdgeDeCeTriangleDejaUtilises = 1;

			while( maxIteration > 0 )
			{
				//Debug.Log("Cut : Current edge va de :"+ lCurrentEdge.a+" vers " +lCurrentEdge.b);

				--maxIteration;

				// trouver l'autre edge dans ce triangle qui touche v (on "tourne autour de vertexIndex")
				Edge lNextLink = null;

				if( nbEdgeDeCeTriangleDejaUtilises == 1 )
				{
					lNextLink = func_findOtherLinkTouchingVertex( lTri, vertexIndex, lCurrentEdge );
					nbEdgeDeCeTriangleDejaUtilises = 2;
				}else if( nbEdgeDeCeTriangleDejaUtilises == 2)
				{
					// il faut passer au triangle suivant
					lTri = lTrianglesToSearch.Find( (x)=>{ return (x != lTri) &&  x.has_u( lCurrentEdge ); } );
					if( lTri == null )
					{
						// on n'a pas fait de tour complet.
						// on a probablement deja commence le cut a cote donc on a cree des bordures qui touchent le vide comme ici
						//Debug.Log("                on n'a pas fini de faire le 'tour du sommet' "+vertexIndex);
						break;
					}

					// on peut l'enlever de la liste des triangles
					lTrianglesToSearch.Remove( lTri );

					lGroupedByCuts[lGroupedByCuts.Count-1].Add( lTri );
					lNextLink = func_findOtherLinkTouchingVertex( lTri, vertexIndex, lCurrentEdge );
					nbEdgeDeCeTriangleDejaUtilises = 2;
				}

				// si c'est l'edge de depart, on a fini notre boucle.
				if( lNextLink.isSame_u( edge_du_cut ) )
				{
					//Debug.Log("                on a fini de faire le 'tour du sommet' "+vertexIndex);
					break;
				}

				bool isNextEdgeInCut = (null != pCut.Find( (x)=>{ return x.isSame_u( lNextLink ); } ));
				if( isNextEdgeInCut )
				{
					// on commence une nouvelle liste
					lGroupedByCuts.Add( new List<Triplet>() );
				}

				lCurrentEdge = lNextLink;
			}
		}


		if( maxIteration <= 0)
		{
			Debug.LogError("erreur lors de notre iteration, on est arrive au maximum, en tournant autour du vertex "+ vertexIndex);
			return null;
		}


		// supprimer les cutgroups vides
		for(int g = lGroupedByCuts.Count -1; g > 0;--g)
		{
			if( lGroupedByCuts[g].Count == 0)
			{
				lGroupedByCuts.RemoveAt( g );
			}
		}


		return lGroupedByCuts;
	}


}



