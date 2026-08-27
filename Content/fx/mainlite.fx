// Reconstructed replacement for the original Xbox 360 mainlite.fx (bytecode
// not portable to PC). Applies a red-damage-flash tint and a grayscale mix,
// matching the "red"/"gray" parameters Game1.cs sets on this effect.
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler TextureSampler : register(s0);

float red;
float gray;

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 tex = tex2D(TextureSampler, texCoord);
    float luminance = dot(tex.rgb, float3(0.299, 0.587, 0.114));
    float3 grayed = lerp(tex.rgb, luminance.xxx, saturate(gray));
    float3 reddened = grayed + float3(red, 0, 0);
    return float4(reddened, tex.a) * color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
