Shader "Custom/ScrollOverlayNoiseandShader_Float"
{
    Properties
    {
        _BaseTex ("Base Tex", 2D) = "white" {}
        _TileX   ("Tile (x)", Float) = 1
        _TileY   ("Tile (y)", Float) = 1
        _PanX    ("Pan (x)", Float) = 0
        _PanY    ("Pan (y)", Float) = 0

        _OverTex ("Overlay Tex", 2D) = "white" {}
        _OverOp  ("Overlay Opacity (0-1)", Range(0,1)) = 1
        _OverInt ("Overlay Intensity (0-1)", Range(0,1)) = 1

        _NoiseAmp   ("Noise Amp (0-1)", Range(0,1)) = 0.1
        _NoiseScale ("Noise Scale", Float) = 3
        _NoiseSpeed ("Noise Speed", Float) = 0.2
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" "UniversalMaterialType"="Unlit" } 

        HLSLINCLUDE
        #pragma target 2.0
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_BaseTex); SAMPLER(sampler_BaseTex);
        TEXTURE2D(_OverTex);  SAMPLER(sampler_OverTex);

        CBUFFER_START(UnityPerMaterial)
            float _TileX, _TileY, _PanX, _PanY;
            float _OverOp, _OverInt;
            float _NoiseAmp, _NoiseScale, _NoiseSpeed;
        CBUFFER_END

        struct appdata 
        { 
            float4 positionOS : POSITION; 
            float2 uv : TEXCOORD0; 
        };

        struct v2f     
        { 
            float4 positionHCS : SV_POSITION; 
            float2 uv : TEXCOORD0; 
        };

        v2f vert(appdata v)
        { 
            v2f o; 
            o.positionHCS = TransformObjectToHClip(v.positionOS.xyz); 
            o.uv = v.uv; 
            return o; 
        }

        float h21(float2 p)
        { 
            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453); 
        }

        float vnoise(float2 p)
        {
            float2 i = floor(p), f = frac(p);
            float a = h21(i);
            float b = h21(i + float2(1,0));
            float c = h21(i + float2(0,1));
            float d = h21(i + float2(1,1));
            float2 u = f*f*(3.0 - 2.0*f);
            return lerp(lerp(a,b,u.x), lerp(c,d,u.x), u.y);
        }

        float fbm3(float2 p)
        { 
            float n0 = vnoise(p) * 0.5;
            float n1 = vnoise(p*2.0) * 0.25;
            float n2 = vnoise(p*4.0) * 0.125;
            return n0 + n1 + n2;
        }

        float4 frag(v2f i) : SV_Target
        {
            float t = _Time.y;
            float2 uv = i.uv * float2(_TileX, _TileY) + float2(_PanX, _PanY) * t;

            float2 p = uv * _NoiseScale + t * _NoiseSpeed;
            float nA = fbm3(p);
            float nB = fbm3(p + float2(17.2, 9.2));

            float sx = 1.0 + (nA - 0.5) * (_NoiseAmp * 0.5);
            float sy = 1.0 + (nB - 0.5) * (_NoiseAmp * 0.5);
            float2 uvc  = (uv - 0.5) * float2(sx, sy) + 0.5;
            float2 disp = (float2(nA, nB) - 0.5) * (_NoiseAmp * 0.5);
            float2 uvw  = uvc + disp;

            float4 b = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, uvw);
            float4 o = SAMPLE_TEXTURE2D(_OverTex, sampler_OverTex, i.uv);

            float a = saturate(o.a * _OverOp);
            float3 c = lerp(b.rgb, o.rgb * _OverInt, a);
            return float4(c, 1);
        }
        ENDHLSL

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }

    FallBack Off
}
