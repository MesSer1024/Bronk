Shader "Custom/Ambient Occlusion" 
{	
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent"}
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest LEqual
		ZWrite Off
		Offset -1, -1
		
		Pass 
		{			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			float4 _Color;
						
			struct v2f 
			{
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.color = _Color;
				o.color.a *=  v.color.r;		
				
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR0 
			{				
				return i.color;
			}
			ENDCG
		}
	} 
}