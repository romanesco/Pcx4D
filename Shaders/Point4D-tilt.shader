﻿// 4D extension of Pcx by Hiroyuki Inou:
// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

Shader "Point Cloud/Point4D-tilt"
{
    Properties
    {
        _Tint("Tint", Color) = (0.5, 0.5, 0.5, 1)
        _PointSize("Point Size", Float) = 0.05
        [Toggle] _Distance("Apply Distance", Float) = 1
        _Translation4D("4D Translation", Vector) = (0, 0, 0, 0)
        [Toggle] _Chiral("Chirality (invert w coordinate)", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #pragma multi_compile_fog
            #pragma multi_compile _ UNITY_COLORSPACE_GAMMA
            #pragma multi_compile _ _DISTANCE_ON
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #include "Common.cginc"

            struct Attributes
            {
                float4 position : POSITION;
                half3 color : COLOR;
                float2 uv2 : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct Varyings
            {
                float4 position : SV_Position;
                half3 color : COLOR;
                float psize : PSIZE;
                UNITY_FOG_COORDS(0)
            };

            struct Point4D
            {
                float4 position;
                uint color;
            };

            half4 _Tint;
            float4x4 _Transform;
            float _PointSize;
            float _Chiral;

            int _RightEye;

            float4x4 _Rotation4D, _Tilt4D_LeftEye, _Tilt4D_RightEye;
            float4 _Translation4D;

        #if _COMPUTE_BUFFER
            StructuredBuffer<Point4D> _PointBuffer;
        #endif

        #if _COMPUTE_BUFFER
            Varyings Vertex(uint vid : SV_VertexID)
        #else
            Varyings Vertex(Attributes input)
        #endif
            {
            #if _COMPUTE_BUFFER
                Point4D pt = _PointBuffer[vid];
                float4 pos4d = mul(_Rotation4D, pt.position) + _Translation4D;
                float4 pos = mul(_Transform, float4(pos4d.xyz, 1));
                half3 col = PcxDecodeColor(pt.color);
            #else
                UNITY_SETUP_INSTANCE_ID(input);

                float4 pos4d = float4(input.position.xyz, _Chiral ? -input.uv2.x : input.uv2.x);
                pos4d = mul(_Rotation4D, pos4d) + _Translation4D;
                //pos4d = pos4d + _Translation4D;
                float4 pos = float4(pos4d.xyz, 1);
                half3 col = input.color;
            #endif

            #ifdef UNITY_COLORSPACE_GAMMA
                col *= _Tint.rgb * 2;
            #else
                col *= LinearToGammaSpace(_Tint.rgb) * 2;
                col = GammaToLinearSpace(col);
            #endif

                Varyings o;

				//o.position = UnityObjectToClipPos(pos);
                //float4 pos2 = float4(UnityObjectToViewPos(pos),pos4d.w);
                float4 pos2 = float4(mul(UNITY_MATRIX_MV, float4(pos.xyz, 0)).xyz, pos4d.w); // without translation
                float3 trans = UnityObjectToViewPos(float4(0,0,0,1));
                if ( (unity_StereoEyeIndex != 0) || (_RightEye != 0) ){
                    o.position = mul(UNITY_MATRIX_P, float4(mul(_Tilt4D_RightEye,pos2).xyz+trans,1));
                } else {
                    o.position = mul(UNITY_MATRIX_P, float4(mul(_Tilt4D_LeftEye,pos2).xyz+trans,1));
                }
                

                o.color = col;
            #ifdef _DISTANCE_ON
                o.psize = _PointSize / o.position.w * _ScreenParams.y;
            #else
                o.psize = _PointSize;
            #endif
                UNITY_TRANSFER_FOG(o, o.position);
                return o;
            }

            half4 Fragment(Varyings input) : SV_Target
            {
                half4 c = half4(input.color, _Tint.a);
                UNITY_APPLY_FOG(input.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
    CustomEditor "Pcx4D.Point4DMaterialInspector"
}
