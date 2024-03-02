Shader "Kadzu/CellShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FrameData("FrameData", Vector) = (0.0, 0.0, 0.0, 0.0)
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
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
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float4, _FrameData)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert(appdata v, uint vid : SV_VertexID)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 frame_data = UNITY_ACCESS_INSTANCED_PROP(Props, _FrameData);
                float2 scaledUV = v.uv;
                scaledUV.x = frame_data.x + (v.uv.x * frame_data.z);
                scaledUV.y = frame_data.y + (v.uv.y * frame_data.w);
                o.uv = scaledUV;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
