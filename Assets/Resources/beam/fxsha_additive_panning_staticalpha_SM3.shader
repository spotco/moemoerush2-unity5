Shader "VFX/fxsha_additive_panning_staticalpha"
{
	Properties 
	{
		_MainTex("_MainTex", 2D) = "black" {}
		_DistortionTexture("_DistortionTexture", 2D) = "black" {}
		_StaticAlpha("_StaticAlpha", 2D) = "black" {}
		_ColorTint("_ColorTint", Color) = (1,1,1,1)
		_Speed("_Speed", Vector) = (1,0,0,0)
		_IntensityBoost("_IntensityBoost", Float) = 2
		_DistortionAmt("_DistortionAmt", Float) = 0.5

	}
	
	SubShader 
	{
	LOD 300
		Tags
		{
			"Queue"="Transparent+1"
			"IgnoreProjector"="True"
			"RenderType"="Additive"

		}

		Blend One One
		Cull Back
		ZWrite Off
		ZTest LEqual
		ColorMask RGBA
		Fog { Mode Off } 

		
		Pass {
			Name "BASE"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord: TEXCOORD0;
			};
			
			struct v2f {
				float4 vertex : POSITION;
				float2 uvmain : TEXCOORD0;
				float2 uvalpha : TEXCOORD1;
				float2 uvdistort : TEXCOORD2;
			};
			
			float4 _MainTex_ST;
			float4 _StaticAlpha_ST;
			float4 _DistortionTexture_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.uvalpha = TRANSFORM_TEX( v.texcoord, _StaticAlpha );
				o.uvdistort = TRANSFORM_TEX( v.texcoord, _DistortionTexture );
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _DistortionTexture;
			sampler2D _StaticAlpha;
			float4 _ColorTint;
			float4 _Speed;
			float _IntensityBoost;
			float _DistortionAmt;
			
			half4 frag( v2f i ) : COLOR
			{
				float2 uv_StaticAlpha = i.uvalpha;
				float2 uv_DistortionTexture = i.uvdistort;
				float2 uv_MainTex = i.uvmain;
				
				float4 Tex2D0=tex2D(_StaticAlpha,(uv_StaticAlpha.xyxy).xy);
				float4 Multiply0=_Speed * _Time;
				float4 Multiply2=Multiply0 * float4( 2.718285,2.718285,2.718285,2.718285 );
				float4 Add1=(uv_DistortionTexture.xyxy) + Multiply2;
				float4 Tex2D3=tex2D(_DistortionTexture,Add1.xy);
				float4 Multiply5=_DistortionAmt.xxxx * Tex2D3;
				float4 Multiply1=(uv_MainTex.xyxy) * Multiply5;
				float4 Add3=(uv_MainTex.xyxy) + Multiply1;
				float4 Tex2D2=tex2D(_MainTex,Add3.xy);
				float4 Add2=_ColorTint + Tex2D2;
				float4 Multiply4=Add2 * Tex2D2;
				float4 Multiply3=_IntensityBoost.xxxx * Multiply4;
				float4 Multiply6=Tex2D0 * Multiply3;

				return half4(Multiply6);
			}
			ENDCG
		}
	}
	
SubShader 
	{
	LOD 200
		Tags
		{
			"Queue"="Transparent+1"
			"IgnoreProjector"="True"
			"RenderType"="Additive"

		}

		Blend One One
		Cull Back
		ZWrite On
		ZTest Always
		ColorMask RGBA
		Fog { Mode Off } 

		
		Pass {
			Name "BASE"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			
			struct appdata_t {
				fixed4 vertex : POSITION;
				half2 texcoord: TEXCOORD0;
			};
			
			struct v2f {
				fixed4 vertex : POSITION;
				half2 uvmain : TEXCOORD0;
				half2 uvalpha : TEXCOORD1;
				half2 uvdistort : TEXCOORD2;
			};
			
			fixed4 _MainTex_ST;
			fixed4 _StaticAlpha_ST;
			fixed4 _DistortionTexture_ST;
			fixed4 _Speed;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
				o.uvalpha = TRANSFORM_TEX( v.texcoord, _StaticAlpha );
				o.uvdistort = TRANSFORM_TEX( v.texcoord, _DistortionTexture );
				//o.uvmain += _Speed * _Time * fixed4(2.718285, 2.718285, 2.718285, 2.718285); 
				
				
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _DistortionTexture;
			sampler2D _StaticAlpha;
			fixed4 _ColorTint;
			
			half _IntensityBoost;
			half _DistortionAmt;
			
			half4 frag( v2f i ) : COLOR
			{
				half2 uv_StaticAlpha = i.uvalpha;
				half2 uv_DistortionTexture = i.uvdistort;
				float2 uv_MainTex = i.uvmain;
				
				//fixed4 Tex2D0=tex2D(_StaticAlpha,uv_StaticAlpha.xy);
				fixed4 Tex2D0=tex2D(_StaticAlpha,(uv_StaticAlpha.xyxy).xy);
				fixed4 Multiply0=_Speed * _Time;
				fixed4 Multiply2=Multiply0 * float4( 2.718285,2.718285,2.718285,2.718285 );
				fixed4 Add1=(uv_DistortionTexture.xyxy) + Multiply2;
				fixed4 Tex2D3=tex2D(_DistortionTexture,Add1.xy);
				fixed4 Multiply5=_DistortionAmt.xxxx * Tex2D3;
				fixed4 Multiply1=(uv_MainTex.xyxy) * Multiply5;
				fixed4 Add3=(uv_MainTex.xyxy) + Multiply1;
				fixed4 Tex2D2=tex2D(_MainTex,Add3.xy);
				fixed4 Add2=_ColorTint + Tex2D2;
				fixed4 Multiply4=Add2 * Tex2D2;
				fixed4 Multiply3=_IntensityBoost.xxxx * Multiply4;
				fixed4 Multiply6=Tex2D0 * Multiply3;

				return half4(Multiply6);
			}
			ENDCG
		}
	}
	
	Fallback "Particles/Additive"
}