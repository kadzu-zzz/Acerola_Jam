Shader "Kadzu/LevelShader"
{
    //Simplex Noise yoinked from https://github.com/keijiro/NoiseShader/blob/master/Packages/jp.keijiro.noiseshader/
    //MIT License.
    Properties
    {
        _Peak_Color ("Peak", Color) = (0.0, 0.0, 0.0, 0.0)
        _Pit_Color ("Pit", Color) = (0.0, 0.0, 0.0, 0.0)
        _ScrollSpeedX("Scroll Speed X", Float) = 0.5
        _ScrollSpeedY("Scroll Speed Y", Float) = 0.5
        _Tile_Size ("Tile", float) = 256
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            float2 wglnoise_mod289(float2 x)
            {
                return x - floor(x / 289) * 289;
            }

            float3 wglnoise_mod289(float3 x)
            {
                return x - floor(x / 289) * 289;
            }

            float2 wglnoise_permute(float2 x)
            {
                return wglnoise_mod289((x * 34 + 1) * x);
            }

            float3 wglnoise_permute(float3 x)
            {
                return wglnoise_mod289((x * 34 + 1) * x);
            }

            float3 SimplexNoiseGrad(float2 v)
            {
                const float C1 = (3 - sqrt(3)) / 6;
                const float C2 = (sqrt(3) - 1) / 2;

                // First corner
                float2 i = floor(v + dot(v, C2));
                float2 x0 = v - i + dot(i, C1);

                // Other corners
                float2 i1 = x0.x > x0.y ? float2(1, 0) : float2(0, 1);
                float2 x1 = x0 + C1 - i1;
                float2 x2 = x0 + C1 * 2 - 1;

                // Permutations
                i = wglnoise_mod289(i); // Avoid truncation effects in permutation
                float3 p = wglnoise_permute(i.y + float3(0, i1.y, 1));
                p = wglnoise_permute(p + i.x + float3(0, i1.x, 1));

                // Gradients: 41 points uniformly over a unit circle.
                // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
                float3 phi = p / 41 * 3.14159265359 * 2;
                float2 g0 = float2(cos(phi.x), sin(phi.x));
                float2 g1 = float2(cos(phi.y), sin(phi.y));
                float2 g2 = float2(cos(phi.z), sin(phi.z));

                // Compute noise and gradient at P
                float3 m = float3(dot(x0, x0), dot(x1, x1), dot(x2, x2));
                float3 px = float3(dot(g0, x0), dot(g1, x1), dot(g2, x2));

                m = max(0.5 - m, 0);
                float3 m3 = m * m * m;
                float3 m4 = m * m3;

                float3 temp = -8 * m3 * px;
                float2 grad = m4.x * g0 + temp.x * x0 +
                    m4.y * g1 + temp.y * x1 +
                    m4.z * g2 + temp.z * x2;

                return 99.2 * float3(grad, dot(m4, px));
            }

            float SimplexNoise(float2 v)
            {
                return SimplexNoiseGrad(v).z;
            }
            float4 _Peak_Color;
            float4 _Pit_Color;
            float _Tile_Size;
            float _ScrollSpeedX;
            float _ScrollSpeedY;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float3 worldPosition = _Time.x + mul(unity_ObjectToWorld, v.vertex).xyz;
                // Normalize and scale UVs based on world position and tile size
                o.uv = worldPosition.xy / _Tile_Size;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 scrollUV = ( i.uv - sin(_Time.x * float2(_ScrollSpeedX, _ScrollSpeedY)));

                // Sample the texture with the scrolled UVs
                fixed4 col = lerp(_Peak_Color, _Pit_Color, (SimplexNoise((scrollUV))));
                return col;
            }
            ENDCG
        }
    }
}
