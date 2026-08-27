// Reconstructed replacement for the original Xbox 360 main.fx (bytecode not
// portable to PC). Applies a red-damage-flash tint, a grayscale mix, and a
// vertical top-to-bottom color gradient (map mood lighting), matching the
// "red"/"gray"/"tR,tG,tB"/"bR,bG,bB" parameters Game1.cs sets on this effect.
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
float tR;
float tG;
float tB;
float bR;
float bG;
float bB;

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 tex = tex2D(TextureSampler, texCoord);
    float luminance = dot(tex.rgb, float3(0.299, 0.587, 0.114));
    float3 grayed = lerp(tex.rgb, luminance.xxx, saturate(gray));

    float3 topColor = float3(tR, tG, tB);
    float3 bottomColor = float3(bR, bG, bB);
    float3 gradient = lerp(topColor, bottomColor, texCoord.y);

    float3 result = grayed * gradient + float3(red, 0, 0);
    return float4(result, tex.a) * color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
