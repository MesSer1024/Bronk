Shader "Custom/Unlit" 
{	
	Properties {
		_Color ("Color", Color) = (0.5,0.5,0.5,0.5) 
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Tags { "Queue"="Geometry" "RenderType"="Opaque"}
		
		ZTest LEqual
		ZWrite On
		
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
				float4 color : COLOR;
			};

			struct appdata_unlit
			{
			    float4 vertex : POSITION;
			    float4 texcoord : TEXCOORD0;
			    fixed4 color : COLOR;
			};
			
			v2f vert (appdata_unlit v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);	
				o.texcoord = v.texcoord;	

				o.color = v.color;

				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{
				float4 tex = tex2D(_MainTex, i.texcoord);
				
				tex.rgb *= i.color.rgb * _Color;
				
				return tex;
			}
			ENDCG
		}
	} 
}