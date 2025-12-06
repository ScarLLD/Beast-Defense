Shader "Custom/BulletAlwaysOnTop"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0,0,0,0.5)
        _ShadowOffset ("Shadow Offset", Vector) = (0.1, -0.1, 0, 0)
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _Specular ("Specular", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent+5000"  // Максимально высокий Queue
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        
        LOD 300
        
        // Проход для захвата фона (чтобы рисовать поверх)
        GrabPass
        {
            "_BackgroundTexture"
        }
        
        // Первый проход: рисуем тень
        Pass
        {
            Name "SHADOW"
            Tags { "LightMode" = "ForwardBase" }
            
            ZTest LEqual
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Offset -1, -1  // Смещаем тень немного назад
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                SHADOW_COORDS(3)  // Для теней
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ShadowColor;
            float4 _ShadowOffset;
            
            v2f vert (appdata v)
            {
                v2f o;
                
                // Смещаем вершины для создания тени
                float4 shadowVertex = v.vertex;
                shadowVertex.x += _ShadowOffset.x;
                shadowVertex.y += _ShadowOffset.y;
                
                o.pos = UnityObjectToClipPos(shadowVertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                TRANSFER_SHADOW(o);  // Передаем данные для теней
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Тень всегда темная и полупрозрачная
                fixed4 shadowCol = _ShadowColor;
                
                // Мягкость тени в зависимости от расстояния от центра
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float softness = 1.0 - smoothstep(0.0, 0.5, dist);
                shadowCol.a *= softness * 0.7;
                
                // Учитываем реальные тени сцены
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                shadowCol.rgb *= atten;
                
                return shadowCol;
            }
            ENDCG
        }
        
        // Второй проход: сама пуля (всегда поверх)
        Pass
        {
            Name "BULLET"
            Tags { "LightMode" = "ForwardBase" }
            
            ZTest Always  // Всегда рисуем поверх
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                SHADOW_COORDS(4)
            };
            
            sampler2D _MainTex;
            sampler2D _BackgroundTexture;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _Gloss;
            float _Specular;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                
                UNITY_TRANSFER_FOG(o, o.pos);
                TRANSFER_SHADOW(o);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Базовая текстура
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = tex * _Color;
                
                // Если текстура прозрачная - отбрасываем пиксель
                clip(col.a - 0.1);
                
                // Простая модель освещения
                float3 normal = normalize(i.worldNormal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfDir = normalize(lightDir + viewDir);
                
                // Диффузное освещение
                float ndotl = max(0, dot(normal, lightDir));
                float3 diffuse = ndotl * _LightColor0.rgb;
                
                // Спекулярное освещение
                float ndoth = max(0, dot(normal, halfDir));
                float specular = pow(ndoth, _Gloss * 128) * _Specular;
                
                // Тени
                UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
                
                // Итоговый цвет
                col.rgb *= diffuse * atten + specular;
                col.rgb += unity_AmbientSky.rgb * 0.1;
                
                // Добавляем блик в центре для 3D эффекта
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float highlight = 1.0 - smoothstep(0.0, 0.3, dist);
                col.rgb += highlight * 0.2;
                
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
        
        // Проход для отбрасывания тени (чтобы пуля отбрасывала тень на другие объекты)
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            Cull Back
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                return o;
            }
            
            float4 frag(v2f i) : SV_Target
            {
                // Проверяем альфа-канал текстуры
                fixed4 tex = tex2D(_MainTex, i.uv);
                clip(tex.a - 0.1);  // Отбрасываем прозрачные пиксели
                
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    
    FallBack "VertexLit"  // Фолбэк для теней
}