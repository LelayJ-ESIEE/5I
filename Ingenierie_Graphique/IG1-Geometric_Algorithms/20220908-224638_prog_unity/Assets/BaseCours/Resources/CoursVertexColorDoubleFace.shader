Shader "CoursVertexColorDoubleFace"
{
    Properties
    {

    }
    SubShader 
    {
        Tags { "RenderType"="Opaque"}       
		pass
		{

			Cull Off

			CGPROGRAM
			#pragma vertex vert_function
			#pragma fragment frag_function
			#include "UnityCG.cginc"

			struct VertIn
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			struct VertOut
			{
				float4 position : POSITION;
				float3 normal : NORMAL;
				float4 color : COLOR;
			};

			VertOut vert_function(VertIn input, float3 normal : NORMAL)
			{
				VertOut output;
				output.position = UnityObjectToClipPos(input.vertex);
				output.color = input.color;
				output.normal = input.normal ;
				return output;
			}

			struct FragOut
			{
				float4 color : COLOR;
			};

			FragOut frag_function(VertOut input)
			{
				FragOut output;
				output.color = input.color;
				return output;
			}

			ENDCG
        }
	}
}
