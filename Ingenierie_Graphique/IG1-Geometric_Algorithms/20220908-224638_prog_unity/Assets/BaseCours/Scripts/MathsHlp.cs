using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathsHlp
{
	// extensions pour les vector3
	public static float longueur(this Vector3 pVector)
	{
		return pVector.magnitude;
	}

	// extensions pour les vector3
	public static float length(this Vector3 pVector)
	{
		return pVector.magnitude;
	}

	// source : https://answers.unity.com/questions/11363/converting-matrix4x4-to-quaternion-vector3.html
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{ 
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	/// multiplie un vecteur 3 par matrice 4, en passant temporairement par des coordonnees homogenes
	/// puis retourne en coordonnees cartesienne en divisant par la quatrieme composante
	public static Vector3 multiplication_homogene( Matrix4x4 pMatrix, Vector3 pVector)
	{
		// remarque :  pMatrix.MultiplyPoint( pVector); fait la meme chose, je l'ai verifie avec des multiplications de nombres premiers.


		// passage en coordonnees homogenes
		var lV4 = new Vector4( pVector.x, pVector.y, pVector.z, 1);

		// multiplication d'elements de la meme taille
		var lNewV4 = pMatrix * lV4;

		// passage en coordonnees cartesiennes
		if( lNewV4.w == 0)
		{
			return new Vector3(lNewV4.x, lNewV4.y, lNewV4.z);
		}else{
			float div = 1.0f / lNewV4.w;
			return new Vector3(lNewV4.x * div, lNewV4.y * div, lNewV4.z * div);
		}
	}

	/// return true si les deux sont quasiment identiques
	public static bool approximately(Vector3 a, Vector3 b)
	{
		return Mathf.Approximately(a.x, b.x)
			&& Mathf.Approximately(a.y, b.y)
			&& Mathf.Approximately(a.z, b.z);
	}


	/// pour une ligne A->B.
	/// dit si le point pPointToTest est du "meme cote de la ligne" que ne l'est le point pSideA 
	public static bool isSameSideAs(Vector3 pA_fromLine, Vector3 pB_fromLine, Vector3 pSideA, Vector3 pPointToTest)
	{
		var lLineDir = pB_fromLine - pA_fromLine;
		var lPerp1 = Vector3.Cross( lLineDir, pSideA-pA_fromLine );
		var lPerp2 = Vector3.Cross( lLineDir, pPointToTest-pA_fromLine );
		return Vector3.Dot( lPerp1, lPerp2 ) > 0;
	}

	/// renvoie true si P est dans le triangle ABC. Ceci renverra false si les points ne sont pas sur le meme plan.
	/// attention : si jamais P est pile sur un bord (ou sur A,B,C), alors il est souvent considere comme HORS du triangle.
	///				pour eviter ce probleme, on peut fabriquer un triangle un tout petit peu plus gros pour compenser,
	///				puis le passer en parametre de cette fonction.
	public static bool isInsideTriangle(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
	{
		var PA = (A-P).normalized;
		var PB = (B-P).normalized;
		var PC = (C-P).normalized;

		// obtenir les produits en croix, perpendiculaires au triangle
		var PAPB = Vector3.Cross(PA, PB);
		var PBPC = Vector3.Cross(PB, PC);
		var PCPA = Vector3.Cross(PC, PA);

		// si on veut debug
		//var rand1 = Random.onUnitSphere * 0.1f;
		//var rand2 = Random.onUnitSphere * 0.15f;
		//var rand3 = Random.onUnitSphere * 0.2f;
		//Debug.DrawLine(P + rand1, P + PAPB+ rand1, Color.red);
		//Debug.DrawLine(P + rand2, P + PBPC+ rand2, Color.green);
		//Debug.DrawLine(P + rand3, P + PCPA+ rand3, Color.blue);

		PAPB.Normalize();
		PBPC.Normalize();
		PCPA.Normalize();

		float dot1 = Vector3.Dot(PAPB, PBPC);
		float dot2 = Vector3.Dot(PBPC, PCPA);
		float dot3 = Vector3.Dot(PCPA, PAPB);

		// si l'angle entre les perpendiculaire est un petit peu different, alors 
		// le dot product est < 1 et donc
		//		- soit P n'est pas dans le triangle en 2D
		//      - soit on n'est pas sur du plat
		float limit = 0.9999f; // acos(0.9999) * 180/pi => 0.81 donc angle de moins de 1 degres

		return dot1 > limit && dot2 > limit && dot3 > limit;
	}

	/// renvoie l'aire du triangle
	public static float calculateAreaTriangle(Vector3 A, Vector3 B, Vector3 C)
	{
		var BA = A-B;
		var CA = A-C;
		var BC = C-B;

		float a = BA.magnitude;
		float b = BC.magnitude;
		float c = CA.magnitude;

		float s = (a + b + c) * 0.5f;  
		return Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
	}

	public static float calculateRayonCercleCircomscrit(Vector3 A, Vector3 B, Vector3 C)
	{
		// calcul de l'aire
		var BA = A-B;
		var CA = A-C;
		var BC = C-B;

		float a = BA.magnitude;
		float b = BC.magnitude;
		float c = CA.magnitude;

		float s = (a + b + c) * 0.5f;  
		float area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));

		// calcul du rayon : R = (abc) / (4 * A)
		float rayon = a*b*c / (4.0f * area);
		return rayon;
	}

	/// distance entre un point et un segment (pas une ligne infinie)
	/// https://stackoverflow.com/questions/849211/shortest-distance-between-a-point-and-a-line-segment
	public static float distancePointToSegment2D(Vector3 p, Vector3 segment_p1,  Vector3 segment_p2)
	{
		float x = p.x;
		float y = p.y;

		float x1 = segment_p1.x;
		float y1 = segment_p1.y;
		float x2 = segment_p2.x;
		float y2 = segment_p2.y;

		var A = x - x1;
		var B = y - y1;
		var C = x2 - x1;
		var D = y2 - y1;

		var dot = A * C + B * D;
		var len_sq = C * C + D * D;
		float param = -1.0f;
		if (len_sq != 0.0f) //in case of 0 length line
			param = dot / len_sq;


		float xx = 0;
		float yy = 0;
		if (param < 0) {
			xx = x1;
			yy = y1;
		}
		else if (param > 1) {
			xx = x2;
			yy = y2;
		}
		else {
			xx = x1 + param * C;
			yy = y1 + param * D;
		}

		var dx = x - xx;
		var dy = y - yy;
		return Mathf.Sqrt(dx * dx + dy * dy);
	}


	/// Renvoie true si le rayon pRay touche le triangle ABC.
	/// pRay, A,B,C doivent etre dans le meme repere (ex: en world).
	/// 
	/// oDistance est modifie par la fonction, et est mise a la distance de contact le long du rayon pRay.
	///   apres l'appel on peut obtenir le point de contact en faisant :  Vector3 point_contact = pRay.GetPoint( oDistance );
	/// 
	/// tire de mon cours c++
	public static bool rayTriangleIntersect( Ray pRay, Vector3 A, Vector3 B, Vector3 C, out float oDistance)
	{
		oDistance = float.MaxValue;

		// https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
		float kEpsilon = 0.000001f;

		Vector3 O = pRay.origin;
		Vector3 D = pRay.direction;

		//Find vectors for two edges sharing V1
		Vector3 e1 = B - A;
		Vector3 e2 = C - A;

		//Begin calculating determinant - also used to calculate u parameter
		Vector3 P = Vector3.Cross(D,e2);

		//if determinant is near zero, ray lies in plane of triangle or ray is parallel to plane of triangle
		float det = Vector3.Dot(e1,P);

		//NOT CULLING
		if (det > -kEpsilon && det < kEpsilon)
		{
			return false;
		}

		float inv_det = 1.0f / det;

		//calculate distance from V1 to ray origin
		Vector3 T = O - A;

		//Calculate u parameter and test bound
		float u = Vector3.Dot(T,P) * inv_det;

		//The intersection lies outside of the triangle
		if (u < 0.0f || u > 1.0f)
		{
			return false;
		}

		//Prepare to test v parameter
		Vector3 Q = Vector3.Cross(T,e1);

		//Calculate V parameter and test bound
		float v = Vector3.Dot( D, Q) * inv_det;
		//The intersection lies outside of the triangle
		if (v < 0.0f || u + v  > 1.0f)
		{
			return false;
		}

		float t = Vector3.Dot(e2,Q) * inv_det;
		if (t > kEpsilon)
		{ 
			//ray intersection
			oDistance = t;
			return true;
		}

		// No hit, no win
		return false;
	}

	/// connaitre la position barycentrique de p, dans le triangle ABC. 
	/// 
	/// Les coordonnees barycentriques, c'est "le pourcentage de chaque sommets du triangle dans P"
	/// https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
	/// 
	/// exemple d'usage, si on connait deja P,A,B,C,uvA,uvB,uvC et qu'on cherche l'uv de P : 
	/// float percentA01 = 0;
	/// float percentB01 = 0;
	/// float percentC01 = 0;
	/// barycentricCoords( P, A,B,C, ref percentA01, ref percentB01, ref percentC01);
	/// Vector2 uvP = uvA * percentA01 + uvB * percentB01 + uvC * percentC01;
	public static void barycentricCoords(Vector3 p, Vector3 a, Vector3 b, Vector3 c, ref float u, ref float v, ref float w)
	{
		Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
		float d00 = Vector3.Dot(v0, v0);
		float d01 = Vector3.Dot(v0, v1);
		float d11 = Vector3.Dot(v1, v1);
		float d20 = Vector3.Dot(v2, v0);
		float d21 = Vector3.Dot(v2, v1);
		float denom = d00 * d11 - d01 * d01;
		v = (d11 * d20 - d01 * d21) / denom;
		w = (d00 * d21 - d01 * d20) / denom;
		u = 1.0f - v - w;
	}

	/// envoie un rayon pRay, et cherche la premiere intersection avec le mesh compose de pVertices et pTriangles.
	/// 
	/// Attention : n'oubliez pas de faire le changement de repere du rayon, pour qu'il soit dans le meme repere que les sommets ! idem pour aabb!
	/// 
	/// Il faut que la boite englobante aabb contienne le mesh et soit dans le meme repere que les pVertices et le pRay (ex: tous en repere local).
	/// 	En cas de doute, vous pouvez mettre un aabb gigantesque a la place, cela supprimera juste une optimisation.
	/// 
	/// Cette fonction renvoie en sortie :
	///   - oIndexInTri : l'indice du sommet du premier des 3 points du triangle dans pTriangles (donc les 2 autres index sont oIndexInTri+1 et oIndexInTri+2)
	///   - oRayDistance : la distance le long du rayon jusqu'au contact
	///   - oBarycentricCoords : les coordonnees barycentriques a l'emplacement du contact.
	/// 
	/// note : on suppose pas de scale. 
	/// 
	/// return false si pas de contact
	/// 
	/// exemple :
	/// 	Ray pRay_l;
	/// 	int sommetA = 0;
	/// 	float distanceContactSurRayon = 0;
	/// 	Vector3 baryCoordinates = new Vector3();
	/// 	if( rayMeshIntersection( pRay_l, aabb_l, mesh.vertices, mesh.triangles, out sommetA, out distanceContactSurRayon, out baryCoordinates))
	/// 	{
	/// 		Vector3 lPointContact_l = pRay_l.GetPoint(distanceContactSurRayon);
	/// 		etc.
	/// 	}
	/// 
	public static bool rayMeshIntersection( 
		Ray pRay, Bounds aabb, Vector3[] pVertices, int[] pTriangles,
		out int oIndexInTri, out float oRayDistance, out Vector3 oBarycentricCoords)
	{
		oIndexInTri = -1;
		oRayDistance = 0;
		oBarycentricCoords = Vector3.zero;

		if(! aabb.IntersectRay( pRay ))
		{
			return false;
		}

		float closestDist = float.MaxValue;
		Vector3 closestBary = Vector3.zero;
		int closestIndex = -1;

		// il touche peut-etre l'un des triangles ?
		// attention, il ne faut pas forcement prendre le premier trouve.
		for(int i = 0; i < pTriangles.Length; i += 3)
		{
			int iA = pTriangles[i];
			int iB = pTriangles[i+1];
			int iC = pTriangles[i+2];
			Vector3 A = pVertices[iA];
			Vector3 B = pVertices[iB];
			Vector3 C = pVertices[iC];

			float dist;
			if(!rayTriangleIntersect(pRay,A,B,C, out dist))
			{
				continue;
			}

			if( dist < closestDist )
			{
				closestDist = dist;
				closestIndex = i;

				Vector3 point = pRay.origin + pRay.direction * dist;
				Vector3 bary = new Vector3();
				barycentricCoords( point, A,B,C, ref bary.x, ref bary.y, ref bary.z);
				closestBary = bary;
			}
		}

		if( closestIndex == -1 )
		{
			return false;
		}

		oIndexInTri = closestIndex;
		oRayDistance = closestDist;
		oBarycentricCoords = closestBary;

		return true;
	}

}


