Shader "Spine/SkeletonShadow" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		_MainTex ("Texture to blend", 2D) = "white" {}
		_AlphaTex("AlphaTex",2D) = "white"{}
		_Offset("Offset", vector) = (-0.2, -0.2, 0, 0)
	}
	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		V2F_SHADOW_CASTER;
		float2  uv : TEXCOORD1;
	};
	uniform float4 _MainTex_ST;
	uniform sampler2D _MainTex;
	uniform sampler2D _AlphaTex;
	uniform fixed _Cutoff;
	float4 _Offset;

	v2f vert_offset(appdata_base v) {
		v2f o;
		float4 pos = mul(unity_ObjectToWorld, v.vertex);
		o.pos = mul(UNITY_MATRIX_VP, pos + _Offset);
		o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}
	float4 frag_color(v2f i) : COLOR{
		float4 c;
		c = tex2D(_MainTex, i.uv);
		c.w = tex2D(_AlphaTex ,i.uv)*c.w;
		if (c.w >= 0.5)
		{
			c.r = 0;
			c.g = 0;
			c.b = 0;
			c.w = 0.5f;
		}

		return c;
	}
	float4 frag(v2f i) : COLOR {
		fixed4 texcol = tex2D(_MainTex, i.uv);
		return texcol;
	}
	
	v2f vert(appdata_base v) {
		v2f o;
		TRANSFER_SHADOW_CASTER(o)
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
		return o;
	}

	ENDCG
	
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Fog { Mode Off }
		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass{
			ColorMaterial AmbientAndDiffuse
			SetTexture[_MainTex]{
				Combine texture * primary
			}
		}

		Pass{
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert_offset
			#pragma fragment frag_color

			ENDCG
		}

		Pass{
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag


			ENDCG
		}

	}
}
