using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionHlp 
{
	public static float Epsilon = 0.00000001f;

	/// https://answers.unity.com/questions/861719/a-fast-triangle-triangle-intersection-algorithm-fo.html
	/// 
	/// <summary>
	/// Checks if the specified ray hits the triagnlge descibed by p1, p2 and p3.
	/// Möller–Trumbore ray-triangle intersection algorithm implementation.
	/// </summary>
	/// <param name="p1">Vertex 1 of the triangle.</param>
	/// <param name="p2">Vertex 2 of the triangle.</param>
	/// <param name="p3">Vertex 3 of the triangle.</param>
	/// <param name="ray">The ray to test hit for.</param>
	/// <returns><c>true</c> when the ray hits the triangle, otherwise <c>false</c></returns>
	public static bool rayTriangleIntersect0(Vector3 p1, Vector3 p2, Vector3 p3, Ray ray)
	{
		// Vectors from p1 to p2/p3 (edges)
		Vector3 e1, e2;  

		Vector3 p, q, t;
		float det, invDet, u, v;

		//Find vectors for two edges sharing vertex/point p1
		e1 = p2 - p1;
		e2 = p3 - p1;

		// calculating determinant 
		p = Vector3.Cross(ray.direction, e2);

		//Calculate determinat
		det = Vector3.Dot(e1, p);

		//if determinant is near zero, ray lies in plane of triangle otherwise not
		if (det > -Epsilon && det < Epsilon) { return false; }
		invDet = 1.0f / det;

		//calculate distance from p1 to ray origin
		t = ray.origin - p1;

		//Calculate u parameter
		u = Vector3.Dot(t, p) * invDet;

		//Check for ray hit
		if (u < 0 || u > 1) { return false; }

		//Prepare to test v parameter
		q = Vector3.Cross(t, e1);

		//Calculate v parameter
		v = Vector3.Dot(ray.direction, q) * invDet;

		//Check for ray hit
		if (v < 0 || u + v > 1) { return false; }

		if ((Vector3.Dot(e2, q) * invDet) > Epsilon)
		{ 
			//ray does intersect
			return true;
		}

		// No hit at all
		return false;
	}

	/// tire de mon cours c++
	public static bool rayTriangleIntersect( Ray pRay, Vector3 V1, Vector3 V2, Vector3 V3, out float oDistance)
	{
		oDistance = float.MaxValue;

		// https://en.wikipedia.org/wiki/M%C3%B6ller%E2%80%93Trumbore_intersection_algorithm
		float kEpsilon = 0.000001f;

		Vector3 O = pRay.origin;
		Vector3 D = pRay.direction;

		//Find vectors for two edges sharing V1
		Vector3 e1 = V2 - V1;
		Vector3 e2 = V3 - V1;

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
		Vector3 T = O - V1;

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
	/// Les coordonnees barycentriques, c'est "le pourcentage de chaque sommets du triangle dans P"
	/// https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
	public static void Barycentric(Vector3 p, Vector3 a, Vector3 b, Vector3 c, ref float u, ref float v, ref float w)
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

	/// pour obtenir : 
	///   - l'indice du sommet du premier des 3 points du triangle dans pTriangles
	///   - la distance le long du rayon
	///   - les coordonnees barycentriques
	/// note : on suppose pas de scale. n'oubliez pas de faire le changement de repere du rayon!
	/// return false si pas touche
	public static bool RayMeshIntersection( 
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
				Barycentric( point, A,B,C, ref bary.x, ref bary.y, ref bary.z);
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

	public static bool isPoint2DInTriangle2D( Vector2 P, Vector2 A, Vector2 B, Vector2 C)
	{
		Vector2 one = A-P;
		Vector2 two = B-P;
		Vector2 thr = C-P;

		bool sign1 = one.x * two.y - one.y * two.x > 0;
		bool sign2 = two.x * thr.y - two.y * thr.x > 0;
		bool sign3 = thr.x * one.y - thr.y * one.x > 0;
		return sign1 == sign2 && sign2 == sign3;

		/*
		// une autre solution.
		bool sign1 = tri_orientation(p1,p2,*this)>0;
		bool sign2 = tri_orientation(p2,p3,*this)>0;
		bool sign3 = tri_orientation(p3,p1,*this)>0;
		return sign1 == sign2 && sign2 == sign3;
		*/
	}

	/*
	/// renvoie le signe de CA cross CB
	public static float crossProductSign(Vector2 A, Vector2 B, Vector2 C)
	{
		return (A.x - C.x) * (B.y - C.y) - (B.x - C.x) * (A.y - C.y);
	}

	/// true si le point pt est dans ABC.
	public static bool isPoint2DInTriangle2D(Vector2 pt, Vector2 A, Vector2 B, Vector2 C)
	{
		float d1, d2, d3;
		bool has_neg, has_pos;

		//d1 = crossProductSign(A, B, pt);
		//d2 = crossProductSign(B, C, pt);
		//d3 = crossProductSign(C, A, pt);

		d1 = crossProductSign(pt, A, B);
		d2 = crossProductSign(pt, B, C);
		d3 = crossProductSign(pt, C, A);


		has_neg = (d1 <= 0) || (d2 <= 0) || (d3 <= 0);
		has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

		return !(has_neg && has_pos);
	}*/

	/* EPIC FAIL je le laisse ici pour pas que quelqu'un essaie de l'utiliser.
	// https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/barycentric-coordinates
	public static bool rayTriangleIntersect( 
		Ray pRay, 
		Vector3 v0, Vector3 v1, Vector3 v2,
		out float rayDistance,
		out Vector3 baryCoordinates) 
	{ 
		baryCoordinates = new Vector3();
		Vector3 orig = pRay.origin;
		Vector3 dir = pRay.direction;
		rayDistance = 0;

		// compute plane's normal
		Vector3 v0v1 = v1 - v0; 
		Vector3 v0v2 = v2 - v0; 
		// no need to normalize
		Vector3 N = Vector3.Cross(v0v1,v0v2); // N 
		float denom = Vector3.Dot(N, N); 

		// Step 1: finding P

		// check if ray and plane are parallel ?
		float NdotRayDirection = Vector3.Dot(N,dir); 
		if (Mathf.Abs(NdotRayDirection) < Epsilon) // almost 0 
			return false; // they are parallel so they don't intersect ! 

		// compute d parameter using equation 2
		float d = Vector3.Dot(N, v0); 

		// compute t (equation 3), notre premiere barycoordinate
		float t = (Vector3.Dot(N, orig) + d) / NdotRayDirection;
		float u;
		float v;
		// check if the triangle is in behind the ray
		if (t < 0) return false; // the triangle is behind 

		// compute the intersection point using equation 1
		Vector3 P = orig + t * dir; 

		// Step 2: inside-outside test
		Vector3 C; // vector perpendicular to triangle's plane 

		// edge 0
		Vector3 edge0 = v1 - v0; 
		Vector3 vp0 = P - v0; 
		C = Vector3.Cross(edge0,vp0); 
		if (Vector3.Dot(N,C) < 0) return false; // P is on the right side 

		// edge 1
		Vector3 edge1 = v2 - v1; 
		Vector3 vp1 = P - v1; 
		C = Vector3.Cross(edge1,vp1); 
		if ((u = Vector3.Dot(N,C)) < 0)  return false; // P is on the right side 

		// edge 2
		Vector3 edge2 = v0 - v2; 
		Vector3 vp2 = P - v2; 
		C = Vector3.Cross(edge2,vp2); 
		if ((v = Vector3.Dot(N,C)) < 0) return false; // P is on the right side; 

		u /= denom; 
		v /= denom;

		rayDistance = t;
		baryCoordinates.x = u;
		baryCoordinates.y = v;
		baryCoordinates.z = 1-u-v;

		return true; // this ray hits the triangle 
	}*/
}
