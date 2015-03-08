Shader "Robots/Self-Illumination" {
    Properties {
        _MainTex ("Base (RGB) Self-Illumination (A)", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)
   }

    SubShader {
		Tags { "RenderType" = "Opaque" }
		Tags { "Queue" = "Geometry" } 
		
		
      CGPROGRAM
      #pragma surface surf Lambert addshadow
      struct Input {
          float2 uv_MainTex;
      };
      
      sampler2D _MainTex;
      
      void surf (Input IN, inout SurfaceOutput o) {
      	half4 c = tex2D (_MainTex, IN.uv_MainTex).rgba;
          o.Albedo = c.rgb;
          o.Emission = c.rgb * c.a;
      }
      ENDCG
    }

    SubShader {
		Tags { "RenderType" = "Opaque" }
		Tags { "Queue" = "Geometry" } 
		
		/* Upgrade NOTE: commented out, possibly part of old style per-pixel lighting: Blend AppSrcAdd AppDstAdd */
		
		
		Pass {
			// no lights
			// Tags { "LightMode" = "VertexOrNone" }
			
			Material {
                Diffuse [_Color]
                Ambient [_Color]
            }
			Lighting On
			Cull Back

            SetTexture [_MainTex] {
                combine texture * primary
            }

            SetTexture [_MainTex] {
                combine texture lerp(texture) previous double
            }
        }
    }
}