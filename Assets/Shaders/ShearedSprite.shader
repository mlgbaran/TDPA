Shader "Custom/ShearedSprite"
{
    Properties
    {
        _MainTex("Sprite Texture (RGB)", 2D) = "white" {}
        _Color("Color Tint", Color) = (1, 1, 1, 1)
        _ShearX("Shear Factor X", Float) = 0.0
        _ShearY("Shear Factor Y", Float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "IsUI"="False"
        }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _ShearX;
            float _ShearY;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;

                // Local vertex position
                float4 pos = v.vertex;

                // Apply a simple shear in X and Y:
                // For example: pos.x += (pos.y * _ShearX)
                //             pos.y += (pos.x * _ShearY)

                float x = pos.x + (pos.y * _ShearX);
                float y = pos.y + (pos.x * _ShearY);

                pos.x = x;
                pos.y = y;

                // Convert to clip space
                o.vertex = UnityObjectToClipPos(pos);

                // Standard sprite UV transform
                o.uv = v.uv;

                // Multiply by the material's color
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.uv) * i.color;
                return c;
            }
            ENDCG
        }
    }
}
