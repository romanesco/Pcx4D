﻿// 4D extension of Pcx by Hiroyuki Inou:
// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

Shader "Point Cloud/Disk4D-Perspective"
{
	Properties
	{
		_Tint("Tint", Color) = (0.5, 0.5, 0.5, 1)
		_PointSize("Point Size", Float) = 0.05
		[Toggle] _Distance("Apply Distance", Float) = 1
		_Translation4D("4D Translation", Vector) = (0, 0, 0, 0)
        [Toggle] _Chiral("Chirality (invert w coordinate)", Float) = 0
		_Camera4D("4D Camera", Vector) = (0, 0, 0, 5)
		_FoV("FoV", Float) = 90
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		Cull Off
		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex Vertex
			#pragma geometry Geometry
			#pragma fragment Fragment
			#pragma multi_compile_fog
			#pragma multi_compile _ UNITY_COLORSPACE_GAMMA
			#pragma multi_compile _ _COMPUTE_BUFFER
			#include "Disk4D-Perspective.cginc"
			ENDCG
		}
		Pass
		{
			Tags { "LightMode" = "ShadowCaster" }
			CGPROGRAM
			#pragma vertex Vertex
			#pragma geometry Geometry
			#pragma fragment Fragment
			#pragma multi_compile _ _COMPUTE_BUFFER
			#define PCX_SHADOW_CASTER 1
			#include "Disk4D-Perspective.cginc"
			ENDCG
		}
	}
		CustomEditor "Pcx4D.Disk4DMaterialInspector"
}
