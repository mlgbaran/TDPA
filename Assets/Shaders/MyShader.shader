Shader "Unlit/MyShader"
{   

    // The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}   //Her pixel'in r ve g değerinin U ve V koordinatı belirttiği "Color Map" Texture'ı
        _CharSkin ("Character Skin Texture", 2D) = "white" {}   //Color map'teki U ve V koordinatlarının işaret ettiği pixellerin
                                                                //bulunduğu texture. (Karakterin Asıl Görünüşü)
        

    }
    // The SubShader block containing the Shader code.
    SubShader
    {   

        Tags { 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True" 
            "Queue" = "Transparent" 
            "RenderType" = "Transparent"
        }
        ZWrite On
        
        Tags{ "RenderType"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
        
            CGPROGRAM

            

            #pragma vertex vertexFunc
            #pragma fragment fragmentFunc

            #define texture_size 32.0
            #define half_texel_size 0.5/32.0

            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"

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

            v2f vertexFunc(appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;

            }
            
            float4 fragmentFunc(v2f i) : COLOR
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


            ENDCG
        }
        
        
    }
}
