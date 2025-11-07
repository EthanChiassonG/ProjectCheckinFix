Shader "Custom/ScrollingBottomWithOverlayWithLighting"
{
    Properties
    {
        _BaseTex ("Base Tex", 2D) = "white" {}
        _BaseTileX ("Base Tile (x)", Float) = 1
        _BaseTileY ("Base Tile (y)", Float) = 1
        _BasePanX  ("Base Pan (x)", Float) = 0
        _BasePanY  ("Base Pan (y)", Float) = 0

        _OverTex ("Overlay Tex", 2D) = "white" {}
        _OverTileX ("Overlay Tile (x)", Float) = 1
        _OverTileY ("Overlay Tile (y)", Float) = 1
        _OverPanX  ("Overlay Pan (x)", Float) = 0
        _OverPanY  ("Overlay Pan (y)", Float) = 0
        _OverOp    ("Overlay Opacity (0-1)", Range(0,1)) = 1
        _OverInt   ("Overlay Intensity (0-1)", Range(0,1)) = 1

        _AmbientColor ("Ambient Color", Color) = (0.2, 0.2, 0.2, 1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
            "UniversalMaterialType"="Unlit"
        }

        Pass
        {
            Name "ForwardUnlit"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite On
            Cull Back   

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseTex); SAMPLER(sampler_BaseTex);
            TEXTURE2D(_OverTex); SAMPLER(sampler_OverTex);

            float _BaseTileX, _BaseTileY, _BasePanX, _BasePanY;
            float _OverTileX, _OverTileY, _OverPanX, _OverPanY;
            float _OverOp, _OverInt;
            float4 _AmbientColor;

            struct appdata
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.worldPos = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                
                float2 uvBase = i.uv * float2(_BaseTileX, _BaseTileY) + float2(_BasePanX, _BasePanY) * _Time.y;
                float4 baseColor = SAMPLE_TEXTURE2D(_BaseTex, sampler_BaseTex, uvBase);

                float2 uvOverlay = i.uv * float2(_OverTileX, _OverTileY) + float2(_OverPanX, _OverPanY) * _Time.y;
                float4 overlayColor = SAMPLE_TEXTURE2D(_OverTex, sampler_OverTex, uvOverlay);

                float3 ambient = _AmbientColor.rgb * baseColor.rgb;

                float3 litBaseColor = ambient;

                float overlayAlpha = saturate(overlayColor.a * _OverOp);
                float3 finalColor = lerp(litBaseColor, overlayColor.rgb * _OverInt, overlayAlpha);

                return float4(finalColor, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
