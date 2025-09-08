Shader "Outline"
{
    Properties
    {
        _OutLineWidth("Width", float) = 1
        _OutLineColor("OutLineColor", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull front

            CGPROGRAM 
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata 
            {
                float4 position : POSITION;
                half3 normal : NORMAL;
                half4 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 position : SV_POSITION;
            };
            
            
            float _OutLineWidth; // 设置变量

            v2f vert(appdata input)
            {
                v2f output;

                half4 positionCS = UnityObjectToClipPos(input.position);

                // 将法线变换到NDC空间
                half3 viewNormal = mul((half3x3)UNITY_MATRIX_IT_MV, input.normal.xyz);
                half2 ndcNormal = normalize(mul((half2x2)UNITY_MATRIX_P, viewNormal.xy));

                // 将近裁剪面右上角位置的顶点变换到观察空间
                half4 nearUpperRight = mul(unity_CameraInvProjection, half4(1, 1, UNITY_NEAR_CLIP_VALUE, _ProjectionParams.y));

                // 求得屏幕宽高比
                half aspect = abs(nearUpperRight.y / nearUpperRight.x);
                ndcNormal.x *= aspect;

                half4 posSV = positionCS;
                posSV.xy += _OutLineWidth * 0.01 * ndcNormal;

                output.position = posSV;
                return output;
            }
            
            float4 _OutLineColor;
            
            fixed4 frag(v2f i) :SV_Target
            {
                return _OutLineColor;
            }
            ENDCG
        }
    }
    FallBack "VertexLit"
}