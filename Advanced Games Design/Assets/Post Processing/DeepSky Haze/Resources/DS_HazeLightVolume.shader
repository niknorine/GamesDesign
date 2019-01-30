// Upgrade NOTE: replaced '_CameraToWorld' with 'unity_CameraToWorld'

Shader "Hidden/DS_HazeLightVolume"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 300

		Pass // 0 - Point light.
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SHADOWS_CUBE
			#pragma multi_compile __ POINT_COOKIE
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define POINT
			#define SHADOWS_NATIVE
			#include "DS_LightVolumeLib.cginc"

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv.xy / i.uv.w;

				float2 uv_offset = uv * (_ScreenParams.xy / 2 / _SamplingOffsets_TexelSize.z);
				uv_offset += kSampleOffsetCoords * _SamplingOffsets_TexelSize.x;
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth(depth);
				float sampleOffset = tex2D(_SamplingOffsets, uv_offset).r;

				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float4 vpos = float4(i.ray * depth, 1);
				float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
				float3 eyeray = wpos - _WorldSpaceCameraPos;
				float3 ray = normalize(eyeray);
				float ws_depth = depth / dot(_DS_HazeCameraDirection.xyz, ray);

				float t0 = 0;				
				float t1 = 0;

				IntersectRaySphere(_DS_HazeLightVolumeParams0, ray, t0, t1);

				if (t0 * _ProjectionParams.w > ws_depth)
				{
					discard;
				}

				if (t1 * _ProjectionParams.w > ws_depth)
				{
					t1 = ws_depth * _ProjectionParams.z;
				}

				float3 rayNear = _WorldSpaceCameraPos + ray * t0;
				float3 traceRay = ray * (t1 - t0);

				float3 rayDt = traceRay / kVolumeSamples;
				float dT = length(rayDt);
				float3 sampleWS = rayNear + rayDt * sampleOffset;

				float radiance = 0;

				UNITY_UNROLL
				for (int si = 0; si < kVolumeSamples; si++)
				{
					float3 ld = sampleWS - _DS_HazeLightVolumeParams0.xyz;
					float atten = ComputeAttenuation(ld, sampleWS, 0, uv);
					float scatter = ComputeScattering(sampleWS, normalize(-ld), ray);
					radiance += atten * dT * scatter;

					sampleWS += rayDt;
				}

				return float4(_DS_HazeLightVolumeColour.rgb * radiance, 1);
			}
			ENDCG
		}

		Pass // 1 - Spot light (Inside the cone)
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SPOT_COOKIE			
			#pragma multi_compile __ SHADOWS_DEPTH			
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define SPOT
			#define SHADOWS_NATIVE
			#include "DS_LightVolumeLib.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv.xy / i.uv.w;
				float2 uv_offset = uv * (_ScreenParams.xy / 2 / _SamplingOffsets_TexelSize.z);
				uv_offset += kSampleOffsetCoords * _SamplingOffsets_TexelSize.x;
				float sampleOffset = tex2D(_SamplingOffsets, uv_offset).r;

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth(depth);

				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float4 vpos = float4(i.ray * depth, 1);
				float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
				float3 eyeray = wpos - _WorldSpaceCameraPos;
				float3 ray = normalize(eyeray);
				float ws_depth = depth / dot(_DS_HazeCameraDirection.xyz, ray);

				float3 light_to_cam = _WorldSpaceCameraPos - kSpotLightApex;
				bool plane_ptv = false;
				if (dot(light_to_cam, kSpotLightAxis) > kLightRange)
				{
					plane_ptv = true;
				}

				float2 tC = float2(0, 0);
				float tP = 0.0f;
				float dt0 = 0.0f;
				float dt1 = 0.0f;
				bool hitC = IntersectRayCone(ray, tC);
				bool hitP = IntersectRayPlane(ray, tP);

				if (!hitC)
				{
					discard;
				}

				if (plane_ptv)
				{
					dt0 = tP;
					dt1 = min(tC.x, tC.y);
				}
				else
				{
					dt1 = min(tP, min(tC.x, tC.y));
				}

				if (dt0 * _ProjectionParams.w > ws_depth)
				{
					discard;
				}

				dt1 = min(dt1, ws_depth * _ProjectionParams.z);

				float3 rayNear = _WorldSpaceCameraPos + ray * dt0;
				float3 traceRay = ray * (dt1 - dt0);

				float3 rayDt = traceRay / kVolumeSamples;
				float dT = length(rayDt);
				float3 sampleWS = rayNear + rayDt * sampleOffset;

				float radiance = 0;

				UNITY_UNROLL
				for (int si = 0; si < kVolumeSamples; si++)
				{
					float3 ld = kSpotLightApex - sampleWS;
					float atten = ComputeAttenuation(ld, sampleWS, 0, uv);
					float scatter = ComputeScattering(sampleWS, normalize(ld), ray);
					radiance += atten * dT * scatter;

					sampleWS += rayDt;
				}

				return fixed4(_DS_HazeLightVolumeColour.rgb * radiance, 1);
			}
			ENDCG
		}

		Pass // 2 - Spot light (Outside the cone)
		{
			Cull Front
			ZTest Always
			ZWrite Off
			Blend One One

			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile __ SPOT_COOKIE
			#pragma multi_compile __ SHADOWS_DEPTH
			#pragma multi_compile __ DENSITY_TEXTURE
			#pragma multi_compile SAMPLES_4 SAMPLES_8 SAMPLES_16 SAMPLES_32

			#define SPOT
			#define SHADOWS_NATIVE
			#include "DS_LightVolumeLib.cginc"

			fixed4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv.xy / i.uv.w;
				float2 uv_offset = uv * (_ScreenParams.xy / 2 / _SamplingOffsets_TexelSize.z);
				uv_offset += kSampleOffsetCoords * _SamplingOffsets_TexelSize.x;
				float sampleOffset = tex2D(_SamplingOffsets, uv_offset).r;

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth(depth);
				i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
				float4 vpos = float4(i.ray * depth, 1);
				float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
				float3 eyeray = wpos - _WorldSpaceCameraPos;
				float3 ray = normalize(eyeray);
				float ws_depth = depth / dot(_DS_HazeCameraDirection.xyz, ray);

				float3 light_to_cam = _WorldSpaceCameraPos - kSpotLightApex;
				bool plane_ptv = false;
				if (dot(light_to_cam, kSpotLightAxis) > kLightRange)
				{
					plane_ptv = true;
				}

				float2 tC = float2(0, 0);
				float tP = 0.0f;
				float dt0 = 0.0f;
				float dt1 = 0.0f;
				bool hitC = IntersectRayCone(ray, tC);
				bool hitP = IntersectRayPlane(ray, tP);

				if (!hitC)
				{
					discard;
				}

				if (plane_ptv)
				{
					dt0 = min(max(tP, tC.x), max(tP, tC.y));
					dt1 = max(tC.x, tC.y);
				}
				else
				{
					dt0 = min(tP, min(tC.x, tC.y));
					dt1 = max(min(tP, tC.x), min(tP, tC.y));
				}

				if (dt0 * _ProjectionParams.w > ws_depth)
				{
					discard;
				}

				dt1 = min(dt1, ws_depth * _ProjectionParams.z);

				float3 rayNear = _WorldSpaceCameraPos + ray * dt0;
				float3 traceRay = ray * (dt1 - dt0);

				float3 rayDt = traceRay / kVolumeSamples;
				float dT = length(rayDt);
				float3 sampleWS = rayNear + rayDt * sampleOffset;

				float radiance = 0;

				UNITY_UNROLL
				for (int si = 0; si < kVolumeSamples; si++)
				{
					float3 ld = kSpotLightApex - sampleWS;
					float atten = ComputeAttenuation(ld, sampleWS, 0, uv);
					float scatter = ComputeScattering(sampleWS, normalize(ld), ray);
					radiance += atten * dT * scatter;

					sampleWS += rayDt;
				}
				return fixed4(_DS_HazeLightVolumeColour.rgb * saturate(radiance), 1);
			}
			ENDCG
		}
	}
}
