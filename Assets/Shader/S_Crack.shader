// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "S_ToonOpaque"
{
	Properties
	{
		_ASEOutlineWidth( "Outline Width", Float ) = 3.17
		_ASEOutlineColor( "Outline Color", Color ) = (0.2509804,0.1490196,0.1960784,0)
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_Shadow_Color("Shadow_Color", Color) = (0.3207547,0.03782484,0.03782484,0)
		_Tilling("Tilling", Float) = 0.5
		_T_MaskShadow_M("T_MaskShadow_M", 2D) = "gray" {}
		_Emlissive("Emlissive", Color) = (0,0,0,0)
		_Noise("Noise", 2D) = "white" {}
		_ShadowStep("ShadowStep", Range( 0 , 1)) = 0.1
		_ShadowOpacity("ShadowOpacity", Range( 0 , 1)) = 0
		_ShadowMaskIntensity("ShadowMaskIntensity", Range( 0 , 3)) = 0
		_DissolveMap("DissolveMap", 2D) = "white" {}
		_Dissovle("Dissovle", Float) = 0
		_Crack("Crack", 2D) = "white" {}
		_CrackIntensity("CrackIntensity", Range( 0 , 2)) = 0.8941177
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		uniform half4 _ASEOutlineColor;
		uniform half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

		Tags{ "RenderType" = "Overlay"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		AlphaToMask On
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

		uniform float _CrackIntensity;
		uniform float4 _Emlissive;
		uniform sampler2D _Crack;
		uniform sampler2D _DissolveMap;
		uniform float _Dissovle;
		uniform float4 _Shadow_Color;
		uniform sampler2D _TextureSample0;
		uniform float _Tilling;
		uniform float4 _Color;
		uniform float _ShadowStep;
		uniform sampler2D _T_MaskShadow_M;
		uniform float4 _T_MaskShadow_M_ST;
		uniform float _ShadowMaskIntensity;
		uniform float _ShadowOpacity;
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
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult55 = dot( ase_worldNormal , ase_worldlightDir );
			float temp_output_5_0 = ( dotResult55 * ase_lightAtten );
			float2 uv_T_MaskShadow_M = i.uv_texcoord * _T_MaskShadow_M_ST.xy + _T_MaskShadow_M_ST.zw;
			float blendOpSrc60 = ( temp_output_5_0 * ( UnpackNormal( tex2D( _T_MaskShadow_M, uv_T_MaskShadow_M ) ).r + _ShadowMaskIntensity ) );
			float blendOpDest60 = step( 0.1 , temp_output_5_0 );
			float4 lerpResult10 = lerp( _Shadow_Color , ( tex2D( _TextureSample0, uv_TexCoord15 ) * _Color ) , ( 1.0 - ( ( 1.0 - step( _ShadowStep , ( saturate( min( blendOpSrc60 , blendOpDest60 ) )) ) ) * _ShadowOpacity ) ));
			float2 uv_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			c.rgb = ( lerpResult10 * pow( tex2D( _Noise, uv_Noise ) , 2.0 ) ).rgb;
			c.a = pow( tex2D( _DissolveMap, i.uv_texcoord ).r , _Dissovle );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float temp_output_95_0 = ( 1.0 - _CrackIntensity );
			float smoothstepResult93 = smoothstep( temp_output_95_0 , temp_output_95_0 , ( _Emlissive.r + pow( tex2D( _Crack, i.uv_texcoord ).r , 0.8 ) ));
			float3 temp_cast_0 = (smoothstepResult93).xxx;
			o.Emission = temp_cast_0;
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
			AlphaToMask Off
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
			sampler3D _DitherMaskLOD;
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
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
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
518;471;901;374;-136.3609;1060.455;1;True;False
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;-1736.433,-236.5646;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;54;-1760.408,-390.6938;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightAttenuation;1;-1339.484,-45.01038;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-1338.175,-737.0128;Float;True;Property;_T_MaskShadow_M;T_MaskShadow_M;5;0;Create;True;0;0;False;0;7bcc85eb883818e449900b4d3b6eb8ae;302951faffe230848aa0d3df7bb70faa;True;0;False;gray;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DotProductOpNode;55;-1413.899,-274.2406;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1324.344,-540.7057;Float;False;Property;_ShadowMaskIntensity;ShadowMaskIntensity;10;0;Create;True;0;0;False;0;0;0;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-973.0807,-637.9467;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-782.0327,20.53882;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1074.426,-268.6484;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-660.9709,-467.1887;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;58;-579.5214,12.51866;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;60;-347.4763,-197.0767;Float;True;Darken;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-334.075,92.04506;Float;False;Property;_ShadowStep;ShadowStep;8;0;Create;True;0;0;False;0;0.1;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;61;-35.97956,75.64713;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-635.637,-697.5021;Float;False;Property;_Tilling;Tilling;4;0;Create;True;0;0;False;0;0.5;4.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-494.8291,-736.0249;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;69;207.1325,646.6455;Float;False;Property;_ShadowOpacity;ShadowOpacity;9;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;67;320.1325,422.6455;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;81;443.3953,-813.255;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;464.8312,419.7961;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-233.9431,-618.6793;Float;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;False;0;9a4a55d8d2e54394d97426434477cdcf;a8de9c9c15d9c7e4eaa883c727391bee;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;18;-206.3969,-401.9335;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;1,1,1,0;0.2358491,0.2358491,0.2358491,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;80;695.9588,-829.7897;Float;True;Property;_Crack;Crack;13;0;Create;True;0;0;False;0;fb9827773dffa7844b52bacd45d0a344;fb9827773dffa7844b52bacd45d0a344;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;11;225.1998,-779.0009;Float;False;Property;_Shadow_Color;Shadow_Color;3;0;Create;True;0;0;False;0;0.3207547,0.03782484,0.03782484,0;0.06666667,0.1254902,0.2392157,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;44;799.827,-585.2116;Float;False;Property;_Emlissive;Emlissive;6;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;78;652.7903,-205.5146;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;82;1038.624,-769.3594;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.8;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;218.6814,769.1732;Float;True;Property;_Noise;Noise;7;0;Create;True;0;0;False;0;ab01ff7db01e28443a168c181da9555a;ab01ff7db01e28443a168c181da9555a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;72;566.8656,279.0566;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;121.3004,-490.5031;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;94;1279.502,-302.8307;Float;False;Property;_CrackIntensity;CrackIntensity;14;0;Create;True;0;0;False;0;0.8941177;0.8941177;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;77;829.8755,5.925495;Float;False;Property;_Dissovle;Dissovle;12;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;79;1064.192,-509.6414;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;75;880.6525,-206.2512;Float;True;Property;_DissolveMap;DissolveMap;11;0;Create;True;0;0;False;0;4c6c41bb026f22a448fe36d01c870f58;4c6c41bb026f22a448fe36d01c870f58;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;95;1364.894,-422.4825;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;10;745.2289,111.8807;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;48;526.6812,747.1732;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;2;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;76;1082.808,19.02339;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1075.428,460.6016;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;93;1569.811,-457.9068;Float;True;3;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;0.11;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1382.084,175.0549;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;S_ToonOpaque;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Overlay;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;3.17;0.2509804,0.1490196,0.1960784,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;55;0;54;0
WireConnection;55;1;53;0
WireConnection;73;0;19;1
WireConnection;73;1;74;0
WireConnection;5;0;55;0
WireConnection;5;1;1;0
WireConnection;51;0;5;0
WireConnection;51;1;73;0
WireConnection;58;0;59;0
WireConnection;58;1;5;0
WireConnection;60;0;51;0
WireConnection;60;1;58;0
WireConnection;61;0;62;0
WireConnection;61;1;60;0
WireConnection;15;0;16;0
WireConnection;67;0;61;0
WireConnection;68;0;67;0
WireConnection;68;1;69;0
WireConnection;13;1;15;0
WireConnection;80;1;81;0
WireConnection;82;0;80;1
WireConnection;72;0;68;0
WireConnection;25;0;13;0
WireConnection;25;1;18;0
WireConnection;79;0;44;1
WireConnection;79;1;82;0
WireConnection;75;1;78;0
WireConnection;95;0;94;0
WireConnection;10;0;11;0
WireConnection;10;1;25;0
WireConnection;10;2;72;0
WireConnection;48;0;46;0
WireConnection;76;0;75;1
WireConnection;76;1;77;0
WireConnection;45;0;10;0
WireConnection;45;1;48;0
WireConnection;93;0;79;0
WireConnection;93;1;95;0
WireConnection;93;2;95;0
WireConnection;0;2;93;0
WireConnection;0;9;76;0
WireConnection;0;13;45;0
ASEEND*/
//CHKSM=79173D717F06400D4842CB2AC3E6DCE09F4241CD