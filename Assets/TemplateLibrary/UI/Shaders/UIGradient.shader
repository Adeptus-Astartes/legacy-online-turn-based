// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UIGradient"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		_Color2 ("Color Gradient", Color) = (1,1,1,1)
    	_Center ("Center", Range(-10,10)) = 1
    	
    	_RotationAngle("Rotation Angle", Range(0,360)) = 0
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			fixed4 _Color2;
			float _Center;
			uniform float _RotationAngle;

			float2 rotatePoint(float2 p, float angle)
			{
				float redRotAngle = angle*0.0174533;
				float sinX = sin(redRotAngle);
				float cosX = cos(redRotAngle);
				float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);

				float2 result = mul(p, rotationMatrix);
				return result;
			}
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
#endif
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 point1 = float2(0,0);
				float2 point2 = float2(1,0);
				point2 = rotatePoint(point2, _RotationAngle);

				float2 dir = point1 - point2;
				float2 pointQ = float2(dir.x,dir.y);

				float2 a = point1;
				float2 b = pointQ;
				float2 c = IN.texcoord * _Center;

				float d = (c.x - a.x)*(b.y-a.y) - (c.y-a.y)*(b.x-a.x);

			
				half4 color = tex2D(_MainTex, IN.texcoord) * lerp(_Color, _Color2, d);
				clip (color.a - 0.01);
				return color;
			}
		ENDCG
		}
	}
}
