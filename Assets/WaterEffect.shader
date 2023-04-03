Shader "Custom/WaterShader" {
    Properties{

        _Frequency("Frequency", Range(0, 50)) = 0.3
        _SingleWaveAmplitude("SingleWaveAmplitude", Range(1.1, 10)) = 10
        _SingleWaveOffset("SingleWaveOffset", Range(-2, 2)) = 0
        _SingleWaveWidth("SingleWaveWidth", Range(0, 5)) = 0.6
        _TimeScale("Time Scale", Range(0, 20)) = 1
        _CircleRadius("Circle Radius", Range(0, 5)) = 1

        _ParticleCount("Particle Count", Int) = 1


        _SimulationScale("Simulation Scale", Range(0, 100)) = 1
        _Ylevel("Y level", Range(-2, 2)) = 0
    }
        

        SubShader{
               Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
               LOD 100

               Pass {
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                // make fog work
                #pragma multi_compile_fog

                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                };

                struct v2f {
                    float3 objectPos : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                float _Frequency;
                float _SingleWaveAmplitude;
                float _SingleWaveOffset;
                float _SingleWaveWidth;
                float _TimeScale;
                float _CircleRadius;

                float rise;
                float _AngularVelocity;
                float _VelocityDelta;
                float singleWaveAmplitude;
                float singleWaveOffset;
                float singleWaveWidth;

                uniform float4 _ParticlePositions[128]; // 128 is the maximum array size
                uniform int _ParticleCount;

                float _SimulationScale;
                float _Ylevel;

                float4 _LightColor0;
                float3 _CustomLightPos; // Change the variable name here

                //float3 angularVelocity = _AngularVelocity.xyz;



                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    float4x4 localToWorldNoTranslation = unity_ObjectToWorld;
                    localToWorldNoTranslation._14_24_34 = float3(0, 0, 0);
                    o.objectPos = mul(localToWorldNoTranslation, v.vertex).xyz;
                    o.objectPos *= _SimulationScale;
                    o.objectPos.y -= _Ylevel;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target{


                    float3 lightDirection = normalize(float3(0, 1, 0) - i.objectPos); // Change the variable name here
                    float3 normalDirection = float3(0, 1, 0);
                    float diffuseIntensity = max(0, dot(normalDirection, lightDirection));


                    float a = _VelocityDelta;
                    float f = _Frequency;
                    float u = _AngularVelocity + rise;
                    float s = singleWaveAmplitude;
                    float o = -singleWaveOffset;
                    float h = singleWaveWidth;


                    float x = i.objectPos.x;
                    float t = _Time.y * _TimeScale;

                    float sin1 = sin(x * f + t);
                    float sin2 = sin((x*f*0.3) + (((t * 0.7 + 5.5) / 2)));
                    float I = ((sin1) + (sin2 * 2)) * a;

                    float sin3 = sin((x*f) + t + 3);
                    float sin4 = sin((x*f*-0.7));
                    float J = ((sin3 / 1.7) + (sin4 * 3)) * a;

                    float M = x * u;
                    float N = s * pow(s, -((x - o) * (x - o)) / (s * s * 0.1 / (h * h)))*0.1;
                    float G = N + M;
                    float K = (I + J) + M + G;

                    float colorValue = (i.objectPos.y > K) ? 0 : 1;
                     colorValue = (i.objectPos.y > K - 0.03 && i.objectPos.y < K) ? 1.5 : colorValue;


                    for (int j = 0; j < _ParticleCount; j++)
                    {
                        float2 circleCenter = _ParticlePositions[j];
                        float distanceFromCenter = distance(i.objectPos.xy, circleCenter);
                        if (distanceFromCenter <= _ParticlePositions[j].w && i.objectPos.y > K) {
                            colorValue = 1;
                            colorValue = (distanceFromCenter > _ParticlePositions[j].w - 0.02 && distanceFromCenter < _ParticlePositions[j].w) ? 1.5 : colorValue;
                        }

                    }


                    fixed4 col = fixed4(colorValue * 0.2, colorValue * 0.5, colorValue, colorValue);
                    col.rgb *= _LightColor0.rgb * diffuseIntensity;

                    return col;
                }
                ENDCG
            }
    }
}
