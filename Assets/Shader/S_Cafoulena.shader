// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "New Amplify Shader"
{
	Properties
	{
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_Shadow_Color("Shadow_Color", Color) = (0.3207547,0.03782484,0.03782484,0)
		_Tilling("Tilling", Float) = 0.5
		_T_MaskShadow_M("T_MaskShadow_M", 2D) = "bump" {}
		_TextureShadow("TextureShadow", Float) = -2.68
		_SmoothShadowMin("SmoothShadowMin", Float) = 0
		_SmoothShadowMax("SmoothShadowMax", Float) = 0
		_Emlissive("Emlissive", Color) = (0,0,0,0)
		_Noise("Noise", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _Emlissive;
		uniform float4 _Shadow_Color;
		uniform sampler2D _TextureSample0;
		uniform float _Tilling;
		uniform float4 _Color;
		uniform float _SmoothShadowMin;
		uniform float _SmoothShadowMax;
		uniform sampler2D _T_MaskShadow_M;
		uniform float4 _T_MaskShadow_M_ST;
		uniform float _TextureShadow;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 temp_cast_1 = (_Tilling).xx;
			float2 uv_TexCoord15 = i.uv_texcoord * temp_cast_1;
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult2 = dot( ase_vertexNormal , ase_worldlightDir );
			float temp_output_5_0 = ( ( ( dotResult2 + 1.0 ) / 2.0 ) * ase_lightAtten );
			float2 uv_T_MaskShadow_M = i.uv_texcoord * _T_MaskShadow_M_ST.xy + _T_MaskShadow_M_ST.zw;
			float temp_output_34_0 = pow( UnpackNormal( tex2D( _T_MaskShadow_M, uv_T_MaskShadow_M ) ).r , _TextureShadow );
			float smoothstepResult41 = smoothstep( 0.0 , 1.0 , temp_output_5_0);
			float lerpResult42 = lerp( temp_output_5_0 , pow( temp_output_5_0 , temp_output_34_0 ) , smoothstepResult41);
			float lerpResult38 = lerp( 0.0 , lerpResult42 , temp_output_34_0);
			float smoothstepResult9 = smoothstep( _SmoothShadowMin , _SmoothShadowMax , lerpResult38);
			float4 lerpResult10 = lerp( _Shadow_Color , ( tex2D( _TextureSample0, uv_TexCoord15 ) * _Color ) , smoothstepResult9);
			float2 uv_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			c.rgb = ( lerpResult10 * pow( tex2D( _Noise, uv_Noise ) , 2.0 ) ).rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Emission = _Emlissive.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15900
0;1;1907;992;161.7802;-216.6374;1;True;True
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-987.7658,133.1658;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalVertexDataNode;3;-965.7659,-88.83423;Float;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;2;-614.7654,126.1658;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;-457.4015,142.1355;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-423.6789,788.2147;Float;False;Property;_TextureShadow;TextureShadow;5;0;Create;True;0;0;False;0;-2.68;-6.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-523.2411,565.1475;Float;True;Property;_T_MaskShadow_M;T_MaskShadow_M;4;0;Create;True;0;0;False;0;7bcc85eb883818e449900b4d3b6eb8ae;302951faffe230848aa0d3df7bb70faa;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;8;-330.7609,139.9111;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;1;-544.1928,307.9657;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;34;-167.3433,603.6484;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-321.7938,266.9853;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;37;8.906128,340.9834;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;41;12.17755,123.0029;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;183.4562,-463.0165;Float;False;Property;_Tilling;Tilling;3;0;Create;True;0;0;False;0;0.5;4.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;324.2642,-501.5394;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;42;282.3329,267.2026;Float;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;307.8087,682.6575;Float;False;Property;_SmoothShadowMax;SmoothShadowMax;7;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;585.1501,-384.1938;Float;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;False;0;9a4a55d8d2e54394d97426434477cdcf;a8de9c9c15d9c7e4eaa883c727391bee;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;38;427.1776,435.0029;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;403.9079,-257.3876;Float;False;Property;_Color;Color;1;0;Create;True;0;0;False;0;1,1,1,0;0.2358491,0.2358491,0.2358491,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;32;296.8351,604.7458;Float;False;Property;_SmoothShadowMin;SmoothShadowMin;6;0;Create;True;0;0;False;0;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;911.4842,82.86207;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;11;8.831351,-193.3925;Float;False;Property;_Shadow_Color;Shadow_Color;2;0;Create;True;0;0;False;0;0.3207547,0.03782484,0.03782484,0;0.06666667,0.1254902,0.2392157,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;9;578.3668,537.2515;Float;True;3;0;FLOAT;0;False;1;FLOAT;0.48;False;2;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;603.2197,776.6375;Float;True;Property;_Noise;Noise;9;0;Create;True;0;0;False;0;ab01ff7db01e28443a168c181da9555a;ab01ff7db01e28443a168c181da9555a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;10;779.0823,250.3132;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;48;911.2197,754.6375;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;2;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;44;1157.502,187.0994;Float;False;Property;_Emlissive;Emlissive;8;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1131.22,442.6375;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1412.799,262.5921;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;New Amplify Shader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;3.17;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;3;0
WireConnection;2;1;4;0
WireConnection;7;0;2;0
WireConnection;8;0;7;0
WireConnection;34;0;19;1
WireConnection;34;1;24;0
WireConnection;5;0;8;0
WireConnection;5;1;1;0
WireConnection;37;0;5;0
WireConnection;37;1;34;0
WireConnection;41;0;5;0
WireConnection;15;0;16;0
WireConnection;42;0;5;0
WireConnection;42;1;37;0
WireConnection;42;2;41;0
WireConnection;13;1;15;0
WireConnection;38;1;42;0
WireConnection;38;2;34;0
WireConnection;25;0;13;0
WireConnection;25;1;18;0
WireConnection;9;0;38;0
WireConnection;9;1;32;0
WireConnection;9;2;33;0
WireConnection;10;0;11;0
WireConnection;10;1;25;0
WireConnection;10;2;9;0
WireConnection;48;0;46;0
WireConnection;45;0;10;0
WireConnection;45;1;48;0
WireConnection;0;2;44;0
WireConnection;0;13;45;0
ASEEND*/
//CHKSM=D0E713974AA380DA62D81FF1AB467EB44AEE1D50