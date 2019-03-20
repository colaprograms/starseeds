/* from https://halisavakis.com/my-take-on-shaders-glitch-image-effect/ */

Shader "Custom/glitchshader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ChromAberrAmountX("Chromatic aberration amount X", float) = 0
        _ChromAberrAmountY("Chromatic aberration amount Y", float) = 0
        _RightStripesAmount("Right stripes amount", float) = 1
        _RightStripesFill("Right stripes fill", range(0, 1)) = 0.7
        _LeftStripesAmount("Left stripes amount", float) = 1
        _LeftStripesFill("Left stripes fill", range(0, 1)) = 0.7
        _DisplacementAmount("Displacement amount", vector) = (0, 0, 0, 0)
        _WavyDisplFreq("Wavy displacement frequency", float) = 10
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
 
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
 
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
             
            sampler2D _MainTex;
            float _ChromAberrAmountX;
            float _ChromAberrAmountY;
            fixed4 _DisplacementAmount;
            float _DesaturationAmount;
            float _RightStripesAmount;
            float _RightStripesFill;
            float _LeftStripesAmount;
            float _LeftStripesFill;
            float _WavyDisplFreq;
 
            float rand(float2 co){
                return frac(sin( dot(co ,float2(12.9898,78.233))) * 43758.5453 );
            }
 
            fixed4 frag (v2f i) : SV_Target {
                fixed2 _ChromAberrAmount = fixed2(_ChromAberrAmountX, _ChromAberrAmountY);
 
 
                //Stripes section
                float stripesRight = floor(i.uv.y * _RightStripesAmount);
                stripesRight = step(_RightStripesFill, rand(float2(stripesRight, stripesRight)));
 
                float stripesLeft = floor(i.uv.y * _LeftStripesAmount);
                stripesLeft = step(_LeftStripesFill, rand(float2(stripesLeft, stripesLeft)));
                //Stripes section
 
                fixed4 wavyDispl = lerp(fixed4(1,0,0,1), fixed4(0,1,0,1), (sin(i.uv.y * _WavyDisplFreq) + 1) / 2);
 
                //Displacement section
                fixed2 displUV = (_DisplacementAmount.xy * stripesRight) - (_DisplacementAmount.xy * stripesLeft);
                displUV += (_DisplacementAmount.zw * wavyDispl.r) - (_DisplacementAmount.zw * wavyDispl.g);
                //Displacement section
 
                //Chromatic aberration section
                float chromR = tex2D(_MainTex, i.uv + displUV + _ChromAberrAmount).r;
                float chromG = tex2D(_MainTex, i.uv + displUV).g;
                float chromB = tex2D(_MainTex, i.uv + displUV - _ChromAberrAmount).b;
                //Chromatic aberration section
                 
                fixed4 finalCol = fixed4(chromR, chromG, chromB, 1);
                 
                return finalCol;
            }
            ENDCG
        }
    }
}