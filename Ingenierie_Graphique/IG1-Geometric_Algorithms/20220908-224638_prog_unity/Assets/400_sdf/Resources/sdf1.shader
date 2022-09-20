Shader "Unlit/sdf1"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM

			// on dit a unity quel est le nom de notre vertex shader et de notre fragment shader
			#pragma vertex nom_fonction_vertex_shader
			#pragma fragment nom_fonction_fragment_shader


			// il s'agit de fonctions et de macros d'aides
			#include "UnityCG.cginc"
			#include "fundations.cginc"

			/// ceci est envoye par unity au vertex shader.
			/// appdata signifie "application data", c'est a dire que c'est envoye par l'application, c'est a dire le CPU
			struct appdata
			{
				// position du vertex dans le repere local de l'objet (c-a-d par rapport a son pivot)
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			/// ceci est ce qui va sortir du vertex shader, pour etre envoye au fragment shader.
			/// en general, il y a les uvs et la position du sommet (mais pas toujours)
			/// v2f signfie "vertex-to-fragment"
			struct v2f
			{
				float2 uv : TEXCOORD0;

				// position entre [-1,1][-1,1][-1,1] (modulo des differences sur le Z de temps en temps)
				float4 vertex : SV_POSITION;

				// position world du vertex
				float4 vertex_world : TEXCOORD1;
			};

			/// fonction qui projete en 2D un sommet. elle peut aussi bouger le sommet en 3D (deformation..),
			///   calculer des attributs supplementaires pour les ajouter dans le "vertex projete en sortie"
			///
			/// entree : 1 vertex 3D qui est dans le 'repere objet' (c'est a dire le repere "local" du pivot de l'objet qu'on dessine)
			///
			/// sortie : le meme vertex, mais projete entre [-1 1, -1 1, -1 1], 
			///                  et en plus on peut ajouter des donnees dedans (ex: precalculs pour alleger le fragment shader).
			///				attention : parfois la projection z n'est pas entre [-1 1], cela depend des gouts des developpeurs du moteur.
			v2f nom_fonction_vertex_shader(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;

				// pour voir les variables dispos : 
				// https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
				o.vertex_world = mul(unity_ObjectToWorld, v.vertex );
				return o;
			}

			// SDDF : copier/coller de inigo quilez, pour dessiner une box centree en 0,0,0 de taille b
			// source: https://iquilezles.org/www/articles/distfunctions/distfunctions.htm
			float sdBox( float3 p, float3 b )
			{
			  float3 q = abs(p) - b;
			  return length(max(q,0.0)) + min(max(q.x,max(q.y,q.z)),0.0);
			}

			/// SDF : ma fonction qui renvoie la distance jusqu'a mon objet
			/// par convention, elle s'appelle map(). tout le monde l'appelle map().
			float map(float3 p)
			{
				// distance a la sphere
				float d_sphere = sqrt(p.x*p.x + p.y*p.y + p.z*p.z) - 6.2;

				// distance a la box
				float d_box = sdBox( p + float3(0,3,0) , float3( 5,5,5));

				// faire l'un moins l'autre
				//float d_soustraction = max( -d_box, d_sphere);

				float d_addition = min( d_box, d_sphere);

				return d_addition ;
			}

			/// SDF : tente de deviner la normal grace a un point + grace a la fonction map
			float3 guess_normal(float3 p)
			{
			    const float d = 0.001;
			    return normalize( float3(
			        map(p+float3(  d,0.0,0.0))-map(p+float3( -d,0.0,0.0)),
			        map(p+float3(0.0,  d,0.0))-map(p+float3(0.0, -d,0.0)),
			        map(p+float3(0.0,0.0,  d))-map(p+float3(0.0,0.0, -d)) ));
			}

			/// SDF : l'algorithme de base, qui appelle regulierement map()
			//https://github.com/i-saint/RaymarchingOnUnity5/blob/master/Assets/Raymarching/Raymarcher.shader
			void raymarching(float3 ray_dir, float maxSteps, float max_distance, inout float o_total_distance, inout float3 o_raypos, out float o_num_steps)
			{
				//o_raypos = cam_pos + ray_dir * o_total_distance;
				o_num_steps = 0.0;
				for(float i=0.0; i<maxSteps; ++i)
				{
					// ici appelle de la fonction qui evalue la distance depuis o_raypos jusqu'au point le plus proche
					float last_distance = map( o_raypos );

					// on fait avancer notre rayon justement de cette quantite
					o_total_distance += last_distance;

					o_raypos += ray_dir * last_distance;
					o_num_steps += 1.0;

					if(last_distance < 0.001 || o_total_distance > max_distance) { break; }
				}
			}

			/// coloriage de notre fragment. est appele pour chaque "pixel" (mais le terme exact c'est fragment).
			/// en entree : ce qui est sorti de notre vertex_shader, mais qui a ete interpole pour le pixel courant.
			void nom_fonction_fragment_shader(v2f v, out float4 out_Color:COLOR, out float out_Depth:DEPTH)
			{
			    //------------------------------
			    // calcul position, et direction du rayon
			    //------------------------------
			    // passage en coordonnees homogenes.
			    // souvent la position de depart du rayon est nommee "ro", pour "ray origin"
			    // souvent la direction de depart du rayon est nommee "rd", pour "ray direction"
			    float3 rayStartPosition_w = v.vertex_world.xyz / v.vertex_world.w;
			    float3 camera2startRayPos_w = rayStartPosition_w - get_camera_position();

			    // normalizer mais j'aurai aussi besoin de la length apres, donc je stocke cette valeur
			    float camera2startRayPos_length = length(camera2startRayPos_w);
			    float3 rayDirection_w = camera2startRayPos_w / camera2startRayPos_length;

			    //------------------------------
			    // Appel au raymarcher
			    //------------------------------
			    float max_distance = _ProjectionParams.z; // farclip, cf doc : https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html
			    float max_steps = 100;
			    float total_distance = camera2startRayPos_length;
			    float3 rayFinalPos_w = rayStartPosition_w;
			    float num_steps;

			    			// inputs
			    raymarching(rayDirection_w, 
			    			max_steps,   
			    			max_distance,

			    			//in_out      
			    			total_distance,
			    			rayFinalPos_w,
			    			 
			    			// outputs
			    			num_steps
			    			);

			    //--------------------------------------------------------------------
			    // finaliser  : apres le raymarching.
			    //---------------------------------------------------------------------

			    //------------------------------
			    // annuler le dessin si on est arrive trop loin.
			    //------------------------------

			 	// V1 : sympa mais pas le plus efficace, a cause du "if"
			    //  if(total_distance > max_distance ) { discard; }

			    // V2 : la fonction clip() annule le dessin si le param est negatif. on evite le "if".
			    clip(max_distance - total_distance); 


			    //------------------------------
			    // calculer la profondeur
			    //------------------------------
			    {
			    	// on projette en 2D, depuis le world (donc on multiplie par la view(V) puis par la proj(P))
			    	float4 clip_pos_final = mul(UNITY_MATRIX_VP, float4(rayFinalPos_w, 1.0));
			    	// le Z dans le Zbuffer n'est pas toujours entre [-1,1] selon les implementations dans unity... 
			    	// cette fonction le calcule precisemment a partir de la position 2D.
			    	out_Depth = compute_depth( clip_pos_final );
			    }

			    //------------------------------
			    // calculer la normale. cette fonction appelle plusieurs fois map()!
			    //------------------------------
			    float3 normal = guess_normal(rayFinalPos_w);


			    //------------------------------
			    // colorier
			    //------------------------------

			    // exemple couleur : afficher la normale
			    //out_Color.xyz = normal.xyz * 0.5f + 0.5f;


			    // exemple couleur : afficher le nombre de calculs
			    //out_Color.xyz = float3(1,1,1) * (num_steps / max_steps);


			    // exemple couleur : simuler une lumiere, N dot L pour la partie diffuse, et j'y ajoute du speculaire
			    {
			    	// cote diffus
				    float3 L = normalize(float3(-0.3,1,-0.7));
				    float NdotL = dot(normal, L); // donne une valeur entre [-1 et 1], en general on la saturate ensuite.
				    float NdotL_valve = NdotL *0.5 + 0.5; // on le met entre [0 et 1]. cela donne un eclairage moins sombre dans les noirs.

				    // cote speculaire
				    float3 V = -rayDirection_w;
				    float3 H = normalize((V + L) *0.5f); // https://en.wikipedia.org/wiki/Blinn%E2%80%93Phong_reflection_model
				    float pow_NdotH = pow( saturate( dot(normal, H)) , 32 );

				    // resultat final
				    out_Color.xyz = NdotL_valve + pow_NdotH ;
			    }
			   
			    out_Color.a = 1;


			    // debug si on est arrive au max de calculs : c'est souvent tres proche des limites
			    if( num_steps >= max_steps-1)
			    {
			    	// magenta / rose fuscia
			    	out_Color.rgb = float3(1,0,1);
			    }
			}

		ENDCG
		}
	}
}



