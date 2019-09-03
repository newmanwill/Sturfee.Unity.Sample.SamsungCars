// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/GlowLightsBlink" {

Properties {
	_MainTex ("Base texture", 2D) = "white" {}
	_Multiplier("Color multiplier", float) = 1
	_TimeOnDuration("ON duration",float) = 0.5
	_TimeOffDuration("OFF duration",float) = 0.5
	_BlinkingTimeOffsScale("Blinking time offset scale (seconds)",float) = 5
	_NoiseAmount("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
	_Color("Color", Color) = (1,1,1,1)
}

	
SubShader {
	
	
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
	Blend One One
	Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
	
	LOD 100
	
	CGINCLUDE	
	#include "UnityCG.cginc"
	sampler2D _MainTex;
	
	float _Multiplier;
	float _TimeOnDuration;
	float	_TimeOffDuration;
	float _BlinkingTimeOffsScale;
	float _MaxGrowSize;
	float _NoiseAmount;
	float4 _Color;
	
	struct v2f {
		float4	pos	: SV_POSITION;
		float2	uv		: TEXCOORD0;
		fixed4	color	: TEXCOORD1;
	};

	
	v2f vert (appdata_full v)
	{
		v2f 		o;
		
		float		time 			= _Time.y + _BlinkingTimeOffsScale * v.color.b;		
		float3		viewPos		= mul(UNITY_MATRIX_MV,v.vertex);
		float		fracTime	= fmod(time,_TimeOnDuration + _TimeOffDuration);
		float		wave			= smoothstep(0,_TimeOnDuration * 0.25,fracTime)  * (1 - smoothstep(_TimeOnDuration * 0.75,_TimeOnDuration,fracTime));
		float		noiseTime	= time *  (6.2831853f / _TimeOnDuration);
		float		noise			= sin(noiseTime) * (0.5f * cos(noiseTime * 0.6366f + 56.7272f) + 0.5f);
		float		noiseWave	= _NoiseAmount * noise + (1 - _NoiseAmount);
		
		wave = _NoiseAmount < 0.01f ? wave : noiseWave;
		
		float4	mdlPos = v.vertex;
			
		o.uv		= v.texcoord.xy;
		o.pos	= UnityObjectToClipPos(mdlPos);
		o.color	= _Color * _Multiplier * wave;
						
		return o;
	}
	ENDCG


	Pass {
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma fragmentoption ARB_precision_hint_fastest		
		fixed4 frag (v2f i) : COLOR
		{		
			return tex2D (_MainTex, i.uv.xy) * i.color;
		}
		ENDCG 
	}	
}


}

