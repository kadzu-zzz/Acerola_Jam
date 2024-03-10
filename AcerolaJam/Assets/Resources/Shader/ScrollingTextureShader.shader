Shader "Kadzu/ScrollingTextureShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TileSize("Tile Size", Float) = 1.0
        _ScrollSpeedX("Scroll Speed X", Float) = 0.5
        _ScrollSpeedY("Scroll Speed Y", Float) = 0.5
        _Colour("Colour", Color) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Cull off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _TileSize;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            float4 _Colour;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldPosition = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.uv = worldPosition.xy / _TileSize;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            { 
                float2 scrolledUV = i.uv + float2(_Time.y * _ScrollSpeedX, _Time.y * _ScrollSpeedY);
                return tex2D(_MainTex, scrolledUV) * _Colour;
            }
            ENDCG
        }
    }
}