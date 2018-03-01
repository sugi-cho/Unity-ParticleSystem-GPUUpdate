Shader "Hidden/MeshDataWriter"
{
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.5

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				uint vid : SV_VertexID;
			};
			struct vData {
				float3 pos;
				float3 normal;
				float2 uv;
			};
			RWStructuredBuffer<vData> _VDataOut : register(u1);

			void vert(appdata v)
			{
				vData data = (vData)0;
				data.pos = mul(unity_ObjectToWorld, v.vertex);
				data.normal = UnityObjectToWorldNormal(v.normal);
				data.uv = v.uv;
				_VDataOut[v.vid] = data;
			}

			void frag(){}

			ENDCG
		}
	}
}