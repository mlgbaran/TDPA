Shader "Custom/NewShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}   //Her pixel'in r ve g değerinin U ve V koordinatı belirttiği "Color Map" Texture'ı
        _CharSkin ("Character Skin Texture", 2D) = "white" {}   //Color map'teki U ve V koordinatlarının işaret ettiği pixellerin
                                                                //bulunduğu texture. (Karakterin Asıl Görünüşü)
    }
    SubShader
    {
        Tags { 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True" 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }


            HLSLPROGRAM

            #include "UnityCG.cginc"


            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE

            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON

            #pragma multi_compile_instancing
            #pragma vertex vert
            #pragma fragment frag

            //#include "UnityCG.cginc"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl" 
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata {
            
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;

            };

            struct v2f {

                float4 pos: POSITION;

                //float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;

            };
            sampler2D _CharSkin;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;

            }


           float4 frag(v2f i) : COLOR
            {   

                float4 col = tex2D(_MainTex, i.uv);

                //Extract X and Y coordinates from R and G channels
                float xCoord = col.r*255.0;     //col.r [0,1] arasında bir değer döndürüyor. Rengin içine koyduğum U,V değerine
                float yCoord = col.g*255.0;     //ulaşmak için [0,255] aralığına döndürüyorum.

                

                float u = saturate(xCoord/32.0);          //UV koordinatları da [0,1] aralığında olduğu için, elde ettiğim koordinatları 
                float v = saturate(yCoord/32.0);          //texture'ın boyutlarına (32x32) bölüyorum.

                



                float4 result = tex2D(_CharSkin, float2(u,v));
              
                //float4 result = tex2D(_CharSkin, i.uv);

                return float4(result.r,result.g,result.b,col.a);     //Skin texture'dan u ve v koordinatlarındaki pixel'i (güya) alıyorum

            }
            ENDHLSL
        }
    }
}