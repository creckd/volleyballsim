Shader "Dani/TapIndicator"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_InnerColor("Inner Color",Color) = (0,0,0,0.5)
		_OutlineColor("Outline Color",Color) = (0,0,0,1)
		_OutlineWidth("Outline Width",Range(0,1)) = 0.1
		_CircleSize("CircleSize",Range(0,1)) = 1
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			half4 _InnerColor;
			half4 _OutlineColor;
			float _OutlineWidth;
			float _CircleSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				half4 color = _InnerColor;
				float dst = sqrt(pow(i.uv.x - 0.5,2) + pow(i.uv.y - 0.5,2));
				float circle = saturate((dst / (0.5 * _CircleSize)));
				circle = saturate((1-circle) / fwidth(dst * 10));
				color.a *= circle;
				float outline = dst / ((_CircleSize * 0.5) - _OutlineWidth);
				outline = saturate((1 - outline) / fwidth(dst * 10));
				color.a += saturate(color.a * (1-outline));
				color.a = saturate(color.a);
				float gradient = sin(radians(frac((i.uv.y * 0.75) + _Time.x * 5) * 360));
				color.rgb = lerp(saturate(_OutlineColor.rgb - pow(gradient * 0.5,2)),color.rgb,outline);
                return color;
            }
            ENDCG
        }
    }
}
