// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/ArrowShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_ColorInside ("Color Inside", Color) = (0.5,0.5,0.5,1)
		_ColorBorder ("Color Border", Color) = (0.2,0.2,0.2,1)
		_Angle ("Angle", Range(0,360)) = 90
		_Alpha ("Alpha", Range(0,1)) = 0.2
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ColorChooseTex("Color Choose", 2D) = "white" {}
		_Border("Border", Range(0,0.5)) = 0.1
	}
	SubShader {
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		
		// From: Transparent shader with depth writes
		// http://docs.unity3d.com/Manual/SL-CullAndDepth.html
		// "Usually semitransparent shaders do not write into the depth buffer.
		// However, this can create draw order problems, especially with complex
		// non-convex meshes. If you want to fade in & out meshes like that, then
		// using a shader that fills in the depth buffer before rendering transparency
		// might be useful."
		
		// Following is an extra pass that renders to depth buffer only
		Pass{
			ZWrite On
			ColorMask 0

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			static const float PI = 3.14159265359;

			float _Angle;

			struct v2f {
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = float2(v.vertex.x, v.vertex.z);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target{

				float angle = atan2(i.uv.x,-i.uv.y) + PI;

				clip(sign(radians(_Angle) - angle + 0.00001));

				return fixed4(1.0,0.0,0.0,1.0);
			}

			ENDCG
		}

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input {
			float2 uv_MainTex : TEXCOORD0;
			float2 uv2_ColorChooseTex : TEXCOORD1;
			float2 localPosXZ;			
		};
		
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.localPosXZ = float2(v.vertex.x,v.vertex.z);			
		}

		fixed4 _Color;
		fixed4 _ColorInside;
		fixed4 _ColorBorder;
		float _Angle;
		float _Alpha;
		float _Border;
		
		static const float PI = 3.14159265359;

		void surf (Input IN, inout SurfaceOutputStandard o) {
		
			// COLOR

			fixed4 c = _Color;
			fixed4 ci = _ColorInside;
			fixed4 cb = _ColorBorder;
			fixed4 cMain = c * IN.uv2_ColorChooseTex.x + ci * IN.uv2_ColorChooseTex.y;
			fixed4 cFinal;
			fixed borderUpper = 1 - _Border;
			fixed borderLower = _Border;
			
			if (IN.uv_MainTex.y < borderLower) {
				cFinal = lerp(cb, cMain, smoothstep(0,1,IN.uv_MainTex.y/ _Border));
			} else if (IN.uv_MainTex.y > borderUpper) {
				cFinal = lerp(cMain, cb, smoothstep(0, 1, (IN.uv_MainTex.y - borderUpper) / _Border));
			} else if (IN.uv_MainTex.x < borderLower) {
				cFinal = lerp(cb, cMain, smoothstep(0, 1, IN.uv_MainTex.x / _Border));
			} else if (IN.uv_MainTex.x > borderUpper) {
				cFinal = lerp(cMain, cb, smoothstep(0, 1, (IN.uv_MainTex.x - borderUpper) / _Border));
			} else {
				cFinal = cMain;
			}
					
			o.Albedo = cFinal;

			// WHETHER ON OR OFF (i.e. transparency)

			float angle = atan2(IN.localPosXZ.x,-IN.localPosXZ.y) + PI;
			
			clip(sign(radians(_Angle) - angle + 0.00001));

			//fixed ifOpaque = clamp(sign(radians(_Angle) - angle + 0.00001), 0, 1);

			// This is a shader-friendly if-statement
			// ifTrans will be 0 if angle < _Angle
			
			//fixed ifTrans = clamp(sign(angle - radians(_Angle)),0,1);
			//o.Alpha = _Alpha * ifTrans + c.a * (1 - ifTrans);
			//o.Alpha = _Color.a * ifOpaque + _Alpha * (1 - ifOpaque);
			o.Alpha = _Color.a;
			
			o.Metallic = 0.0;
			o.Smoothness = 0.5;			
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
