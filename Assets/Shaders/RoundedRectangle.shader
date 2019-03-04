Shader "Dani/RoundedRectangle"
{
	Properties
	{
		_Width("Rect Width", Range(0,1)) = 1
		_Height("Rect Height", Range(0,1)) = 1
		_Radius("Roundness", Range(0,1)) = 1
		_OutlineWidth("Outline Width",Range(0,1)) = 0.2
		_OutlineColor("Outline Color",Color) = (0,0,0,0)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			float4 _MainColor;
			float4 _OutlineColor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.color = v.color;
				return o;
			}

			float _Width;
			float _Height;
			float _Radius;
			float _OutlineWidth;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float Width = _Width;
				float Height = _Height;
				float Radius = _Radius;
				Radius = max(min(min(abs(Radius * 2), abs(Width)), abs(Height)), 1e-5);
				float2 uv = abs(i.uv * 2 - 1) - float2(Width, Height) + Radius;
				float d = length(max(0, uv)) / Radius;
				float f = saturate((1 - d) / (fwidth(d) * 4));
				float4 finalColor;
				finalColor.a = 1;
				finalColor.rgb = i.color.rgb;
				finalColor.a *= i.color.a;
				finalColor.a *= f;
				float outline = 1 - saturate(saturate((1-d) - _OutlineWidth) / (fwidth(d) * 4));
				finalColor.rgb = lerp(finalColor.rgb, _OutlineColor,outline);
				finalColor.a = lerp(finalColor.a, saturate(finalColor.a * 2), outline);
				return finalColor;
			}
			ENDCG
		}
	}
}