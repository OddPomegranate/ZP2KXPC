// Reconstructed replacement for the original Xbox 360 blur.fx (bytecode not
// portable to PC). Approximates a soft additive glow: a few offset samples
// averaged together, then tinted by the brite-gradient color the game sets.
// Parameter names/usage inferred from Game1.cs's Parameters[...] calls.
#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler TextureSampler : register(s0);

float v;
float briteGradientR;
float briteGradientG;

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 sum = tex2D(TextureSampler, texCoord);
    sum += tex2D(TextureSampler, texCoord + float2(v, 0));
    sum += tex2D(TextureSampler, texCoord - float2(v, 0));
    sum += tex2D(TextureSampler, texCoord + float2(0, v));
    sum += tex2D(TextureSampler, texCoord - float2(0, v));
    sum /= 5.0;

    float3 tint = float3(briteGradientR, briteGradientG, briteGradientG);
    return float4(sum.rgb * tint, sum.a) * color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
