// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "S_Dieu"
{
	Properties
	{
		_Albedo("Albedo", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,0)
		_Emissive("Emissive", Color) = (0,0,0,0)
		_Shadow_Color("Shadow_Color", Color) = (0.3207547,0.03782484,0.03782484,0)
		_Tilling("Tilling", Float) = 0.5
		_ShadowMask("ShadowMask", 2D) = "gray" {}
		_Noise("Noise", 2D) = "white" {}
		_ShadowStep("ShadowStep", Range( 0 , 1)) = 0.1
		_ShadowOpacity("ShadowOpacity", Range( 0 , 1)) = 0
		_ShadowMaskIntensity("ShadowMaskIntensity", Range( 0 , 3)) = 0
		_DisolveColorMask("DisolveColorMask", 2D) = "white" {}
		_DisolveMask("DisolveMask", 2D) = "white" {}
		_DisolveColor("DisolveColor", Color) = (1,0.848044,0.4009434,0)
		_MaskPower("MaskPower", Range( 0 , 200)) = 1.71463
		_GradientMax("GradientMax", Range( -2 , 2)) = 0
		_GradientMin("GradientMin", Range( -2 , 2)) = -0.2355881
		_DisolveSpeed("DisolveSpeed", Vector) = (0,0,0,0)
		_DisplacementMap("DisplacementMap", 2D) = "white" {}
		_DistanceDisplacement("Distance Displacement", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Overlay"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		
		AlphaToMask On
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float4 vertexColor : COLOR;
			float2 uv_texcoord;
			float3 worldNormal;
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

		uniform sampler2D _DisplacementMap;
		uniform float4 _DisplacementMap_ST;
		uniform float3 _DistanceDisplacement;
		uniform float4 _Emissive;
		uniform float4 _DisolveColor;
		uniform sampler2D _DisolveColorMask;
		uniform float2 _DisolveSpeed;
		uniform float _MaskPower;
		uniform float _GradientMin;
		uniform float _GradientMax;
		uniform sampler2D _DisolveMask;
		uniform float4 _Shadow_Color;
		uniform sampler2D _Albedo;
		uniform float _Tilling;
		uniform float4 _Color;
		uniform float _ShadowStep;
		uniform sampler2D _ShadowMask;
		uniform float4 _ShadowMask_ST;
		uniform float _ShadowMaskIntensity;
		uniform float _ShadowOpacity;
		uniform sampler2D _Noise;
		uniform float4 _Noise_ST;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float2 uv_DisplacementMap = v.texcoord * _DisplacementMap_ST.xy + _DisplacementMap_ST.zw;
			v.vertex.xyz += ( tex2Dlod( _DisplacementMap, float4( uv_DisplacementMap, 0, 0.0) ) * float4( _DistanceDisplacement , 0.0 ) ).rgb;
		}

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
			float temp_output_140_0 = ( 1.0 - i.vertexColor.r );
			float smoothstepResult78 = smoothstep( _GradientMin , _GradientMax , temp_output_140_0);
			float3 ase_worldPos = i.worldPos;
			float2 panner83 = ( _Time.y * _DisolveSpeed + ase_worldPos.xy);
			float2 temp_cast_3 = (_Tilling).xx;
			float2 uv_TexCoord15 = i.uv_texcoord * temp_cast_3;
			float temp_output_128_0 = ( 1.0 - step( ( pow( tex2D( _DisolveColorMask, panner83 ).r , _MaskPower ) * smoothstepResult78 ) , temp_output_140_0 ) );
			float4 lerpResult117 = lerp( ( tex2D( _Albedo, uv_TexCoord15 ) * _Color ) , _DisolveColor , temp_output_128_0);
			float3 ase_worldNormal = i.worldNormal;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult55 = dot( ase_worldNormal , ase_worldlightDir );
			float temp_output_5_0 = ( dotResult55 * ase_lightAtten );
			float2 uv_ShadowMask = i.uv_texcoord * _ShadowMask_ST.xy + _ShadowMask_ST.zw;
			float blendOpSrc60 = ( temp_output_5_0 * ( tex2D( _ShadowMask, uv_ShadowMask ).r + _ShadowMaskIntensity ) );
			float blendOpDest60 = step( 0.1 , temp_output_5_0 );
			float4 lerpResult10 = lerp( _Shadow_Color , lerpResult117 , ( 1.0 - ( ( 1.0 - step( _ShadowStep , ( saturate( min( blendOpSrc60 , blendOpDest60 ) )) ) ) * _ShadowOpacity ) ));
			float2 uv_Noise = i.uv_texcoord * _Noise_ST.xy + _Noise_ST.zw;
			c.rgb = ( lerpResult10 * pow( tex2D( _Noise, uv_Noise ) , 2.0 ) ).rgb;
			c.a = step( ( smoothstepResult78 * pow( tex2D( _DisolveMask, panner83 ).r , _MaskPower ) ) , temp_output_140_0 );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float3 ase_worldPos = i.worldPos;
			float2 panner83 = ( _Time.y * _DisolveSpeed + ase_worldPos.xy);
			float temp_output_140_0 = ( 1.0 - i.vertexColor.r );
			float smoothstepResult78 = smoothstep( _GradientMin , _GradientMax , temp_output_140_0);
			float temp_output_128_0 = ( 1.0 - step( ( pow( tex2D( _DisolveColorMask, panner83 ).r , _MaskPower ) * smoothstepResult78 ) , temp_output_140_0 ) );
			float4 lerpResult131 = lerp( _Emissive , _DisolveColor , temp_output_128_0);
			o.Emission = lerpResult131.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

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
				half4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
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
				surfIN.vertexColor = IN.color;
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
133;506;901;441;-735.2546;32.66413;1.405336;True;False
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;53;-1770.566,355.0718;Float;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;54;-1794.541,200.9426;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightAttenuation;1;-1373.617,546.6259;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;55;-1448.032,317.3958;Float;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-1358.477,50.93072;Float;False;Property;_ShadowMaskIntensity;ShadowMaskIntensity;10;0;Create;True;0;0;False;0;0;0.77;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;133;-322.7063,-1168.903;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;90;-416.6007,-496.567;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-1372.308,-145.3764;Float;True;Property;_ShadowMask;ShadowMask;6;0;Create;True;0;0;False;0;7bcc85eb883818e449900b4d3b6eb8ae;7bcc85eb883818e449900b4d3b6eb8ae;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;98;-431.0656,-927.2956;Float;False;Property;_DisolveSpeed;DisolveSpeed;17;0;Create;True;0;0;False;0;0,0;0.5,0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;83;-182.8861,-664.2195;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-816.1655,612.1749;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;139;-147.3083,-986.6544;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;-1108.559,322.988;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-1007.214,-46.31028;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;95;-50.04045,-732.769;Float;False;Property;_GradientMax;GradientMax;15;0;Create;True;0;0;False;0;0;0.99;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;124;43.97942,-1186.121;Float;True;Property;_DisolveColorMask;DisolveColorMask;11;0;Create;True;0;0;False;0;07d0b1a345caff34a91b9825ef411d37;07d0b1a345caff34a91b9825ef411d37;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;94;-24.62883,-802.989;Float;False;Property;_GradientMin;GradientMin;16;0;Create;True;0;0;False;0;-0.2355881;-2;-2;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;58;-613.6542,604.1548;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;9.725008,-390.7159;Float;False;Property;_MaskPower;MaskPower;14;0;Create;True;0;0;False;0;1.71463;1.8;0;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-695.1037,124.4477;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;140;131.5667,-961.1447;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;60;-381.6092,394.5597;Float;True;Darken;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;78;276.2044,-810.0916;Float;True;3;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;62;-368.2079,683.6812;Float;False;Property;_ShadowStep;ShadowStep;8;0;Create;True;0;0;False;0;0.1;0.056;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-669.7698,-105.8657;Float;False;Property;_Tilling;Tilling;5;0;Create;True;0;0;False;0;0.5;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;125;410.0048,-1171.147;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;61;-70.11243,667.2832;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;15;-528.962,-144.3885;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;126;729.4451,-1053.021;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;67;260.8686,516.6865;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;13;-268.076,-27.04289;Float;True;Property;_Albedo;Albedo;1;0;Create;True;0;0;False;0;9a4a55d8d2e54394d97426434477cdcf;c7ba312121d29c141b0360e6b32e1a42;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;69;147.8686,740.6866;Float;False;Property;_ShadowOpacity;ShadowOpacity;9;0;Create;True;0;0;False;0;0;0.602;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;127;1064.977,-961.0922;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;18;-240.5298,189.7029;Float;False;Property;_Color;Color;2;0;Create;True;0;0;False;0;1,1,1,0;1,0.7333058,0.3726408,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;80;-15.58956,-594.059;Float;True;Property;_DisolveMask;DisolveMask;12;0;Create;True;0;0;False;0;c05e6ee3f8ad8904da34cfa44f108069;c05e6ee3f8ad8904da34cfa44f108069;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;87.16753,101.1333;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;128;1200.286,-56.43445;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;485.1325,409.6455;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;115;207.6953,-226.5511;Float;False;Property;_DisolveColor;DisolveColor;13;0;Create;True;0;0;False;0;1,0.848044,0.4009434,0;0.7830188,0.3804289,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;91;299.0689,-458.7914;Float;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;46;179.5223,999.8689;Float;True;Property;_Noise;Noise;7;0;Create;True;0;0;False;0;ab01ff7db01e28443a168c181da9555a;e986fe073d211414c95bc96f2beb772f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;72;697.1325,363.6455;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;117;674.9589,-66.29325;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;11;519.0669,195.6355;Float;False;Property;_Shadow_Color;Shadow_Color;4;0;Create;True;0;0;False;0;0.3207547,0.03782484,0.03782484,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;48;526.6812,747.1732;Float;True;2;0;COLOR;0,0,0,0;False;1;FLOAT;2;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;600.7017,-458.99;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;137;961.6573,1191.226;Float;False;Property;_DistanceDisplacement;Distance Displacement;19;0;Create;True;0;0;False;0;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;10;890.4738,253.6255;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;44;930.3017,-370.9129;Float;False;Property;_Emissive;Emissive;3;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;134;918.4442,971.562;Float;True;Property;_DisplacementMap;DisplacementMap;18;0;Create;True;0;0;False;0;ab01ff7db01e28443a168c181da9555a;e986fe073d211414c95bc96f2beb772f;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;131;1424.72,-260.602;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1075.428,460.6016;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;135;1271.106,1009.629;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WorldPosInputsNode;138;-146.3206,-1262.195;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StepOpNode;99;878.3161,-755.5256;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1633.084,127.0549;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;S_Dieu;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Overlay;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0.2509802,0.1490194,0.1960782,0;VertexScale;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;True;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;55;0;54;0
WireConnection;55;1;53;0
WireConnection;83;0;133;0
WireConnection;83;2;98;0
WireConnection;83;1;90;0
WireConnection;5;0;55;0
WireConnection;5;1;1;0
WireConnection;73;0;19;1
WireConnection;73;1;74;0
WireConnection;124;1;83;0
WireConnection;58;0;59;0
WireConnection;58;1;5;0
WireConnection;51;0;5;0
WireConnection;51;1;73;0
WireConnection;140;0;139;1
WireConnection;60;0;51;0
WireConnection;60;1;58;0
WireConnection;78;0;140;0
WireConnection;78;1;94;0
WireConnection;78;2;95;0
WireConnection;125;0;124;1
WireConnection;125;1;92;0
WireConnection;61;0;62;0
WireConnection;61;1;60;0
WireConnection;15;0;16;0
WireConnection;126;0;125;0
WireConnection;126;1;78;0
WireConnection;67;0;61;0
WireConnection;13;1;15;0
WireConnection;127;0;126;0
WireConnection;127;1;140;0
WireConnection;80;1;83;0
WireConnection;25;0;13;0
WireConnection;25;1;18;0
WireConnection;128;0;127;0
WireConnection;68;0;67;0
WireConnection;68;1;69;0
WireConnection;91;0;80;1
WireConnection;91;1;92;0
WireConnection;72;0;68;0
WireConnection;117;0;25;0
WireConnection;117;1;115;0
WireConnection;117;2;128;0
WireConnection;48;0;46;0
WireConnection;79;0;78;0
WireConnection;79;1;91;0
WireConnection;10;0;11;0
WireConnection;10;1;117;0
WireConnection;10;2;72;0
WireConnection;131;0;44;0
WireConnection;131;1;115;0
WireConnection;131;2;128;0
WireConnection;45;0;10;0
WireConnection;45;1;48;0
WireConnection;135;0;134;0
WireConnection;135;1;137;0
WireConnection;99;0;79;0
WireConnection;99;1;140;0
WireConnection;0;2;131;0
WireConnection;0;9;99;0
WireConnection;0;13;45;0
WireConnection;0;11;135;0
ASEEND*/
//CHKSM=0E4C856BACE0646E916DBA06679CDB56E3750F77