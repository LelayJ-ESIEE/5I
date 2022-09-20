using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// signed distance fields
public class Step20_SDF : MonoBehaviour 
{
	// Use this for initialization
	void Start()
	{
		// positionner camera
		var lCamera = CameraHlp.setup3DCamera();
		lCamera.transform.position = new Vector3(0,0,-20);
		lCamera.backgroundColor = Color.cyan;
		lCamera.nearClipPlane = 0.1f;
		lCamera.fieldOfView = 60.0f;

		GameObject lGoSurface = GameObject.CreatePrimitive(PrimitiveType.Quad);
		lGoSurface.name = "objet_sdf";
		lGoSurface.transform.localScale = new Vector3(20,20,20);
		lGoSurface.transform.localPosition = lCamera.transform.position + Vector3.forward;

		// chargement shader et son application sur notre objet.
		{
			var lRenderer = lGoSurface.GetComponent<Renderer>();
			string lShaderName = "sdf1";
			var lShader = Resources.Load( lShaderName ) as Shader;
			if( lShader == null)
			{
				Debug.LogError("Impossible de trouver le shader : ");
				return;
			}
			lRenderer.sharedMaterial = new Material( lShader);
		}

		// creer 4 bords pour bien les voir...
		{
			float surfaceScale = lGoSurface.transform.localScale.x;
			for(int i = 1; i < 5; ++i)
			{
				GameObject lGoBord = GameObject.CreatePrimitive(PrimitiveType.Cube);
				lGoBord.name = "bord_"+i;
				var lTransform = lGoBord.transform;
				lTransform.parent = lGoSurface.transform;

				lTransform.localScale = i < 3 ? new Vector3(1, 1/surfaceScale, 1/surfaceScale) : new Vector3( 1/surfaceScale, 1, 1/surfaceScale);

				float side = Mathf.Sign( (float)(i % 2)-0.5f );
				Vector3 axis = i < 3 ? Vector3.up : Vector3.right;
				lTransform.localPosition = Vector3.zero;
				lTransform.Translate( axis * side * surfaceScale * 0.5f);
			}
		}

		// creer un vrai cube juste pour que les gens voient bien la difference
		{
			GameObject lGoComparaison = GameObject.CreatePrimitive(PrimitiveType.Cube);
			lGoComparaison.name ="justePourComparer";
			lGoComparaison.transform.Translate( -1.2f, 0, - 3.2f );
			lGoComparaison.transform.Rotate( -45, 45, 0);
			lGoComparaison.transform.localScale = 5 * Vector3.one;
			var lMaterial = new Material( Shader.Find("Standard"));
			lMaterial.color = Color.yellow; 
			lGoComparaison.GetComponent<Renderer>().sharedMaterial = lMaterial; 
		}

	}


}

