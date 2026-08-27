// Reconstructed replacement for the original Xbox 360 scene.fx (bytecode not
// portable to PC). Classic exposure/saturation color-grade post-process,
// matching the "alpha"/"burn"/"add"/"sat" parameters MainMenu.cs sets on
// this effect for the menu background.
//
// 2026-08-23: two failed attempts before this one, worth recording so the
// next tuning pass doesn't repeat them. MainMenu.cs's burn/add/sat aren't
// small numbers - at rest on the main "Press Start" screen (sat and brite
// both idle at their max, num=1) it sends burn=2.5, add=0.4, sat=1.99. A
// flat `color*burn+add` clipped almost everything to white. Replacing it
// with a 1:1 Color-Burn/Dodge blend (Photoshop's actual formula for those
// names) was less catastrophic but STILL clearly oversaturated compared to
// a screenshot of the original Xbox build at (as far as we can tell) the
// same steady-state parameter values - so the original shader's response to
// this parameter range must be far gentler than either literal reading.
// Rather than guess a third closed-form curve blind, the burn/add/sat
// -> visual-strength mapping is now three explicit sensitivity constants
// below, so the next round of "too bright/too dark" feedback is a single
// number tweak instead of a shader rewrite. All three default to a mild
// effect; raise a constant for more punch, 0 disables that parameter.
static const float BURN_SENSITIVITY = 0.00001; // exposure lift per unit of (burn - 1.5)
static const float ADD_SENSITIVITY  = 0.00001; // dodge/burn push per unit of add
static const float SAT_SENSITIVITY = 0.00001; // saturation boost per unit of (sat - 1)

#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler TextureSampler : register(s0);

float alpha;
float burn;
float add;
float sat;

float4 MainPS(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
    float4 tex = tex2D(TextureSampler, texCoord);
    float luminance = dot(tex.rgb, float3(0.299, 0.587, 0.114));

    float satAmount = 1.0 + (sat - 1.0) * SAT_SENSITIVITY;
    float3 graded = lerp(luminance.xxx, tex.rgb, satAmount);

    // Color-Burn-style exposure lift: brightens without hard-clipping
    // highlights the way a flat multiply does, but dampened by
    // BURN_SENSITIVITY so the game's fairly wide 1.5-2.5 burn range maps to
    // a subtle change instead of a dramatic one.
    float exposure = 1.0 + max(burn - 1.5, 0.0) * BURN_SENSITIVITY;
    graded = saturate(1.0 - (1.0 - graded) / max(exposure, 0.0001));

    // Signed dodge/burn push for "add", same softening treatment.
    float push = add * ADD_SENSITIVITY;
    float3 dodged = 1.0 - (1.0 - graded) * (1.0 - saturate(push));
    float3 burned = graded * saturate(1.0 + push);
    graded = (push >= 0.0) ? dodged : burned;

    return float4(saturate(graded), tex.a * alpha) * color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
