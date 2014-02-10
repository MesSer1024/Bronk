Shader "Custom/BronkOutline" 
{	
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque"}
		
		ZWrite On
		
			
		Pass 
		{
			ZWrite On
			Cull Front

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
											
				float3 nyNormal = v.normal.xyz;
				//nyNormal.y = abs(nyNormal.y);
				

				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.pos.xy += float2(2,2) / _ScreenParams.xy * o.pos.w;


				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{				
				float4 tex = tex2D(_MainTex, i.texcoord) * _Color;											
				return 1;
			}
			ENDCG
		}
				
		Pass 
		{			
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{				
				float4 tex = tex2D(_MainTex, i.texcoord) * _Color;											
				return tex;
			}
			ENDCG
		}				
	} 
}