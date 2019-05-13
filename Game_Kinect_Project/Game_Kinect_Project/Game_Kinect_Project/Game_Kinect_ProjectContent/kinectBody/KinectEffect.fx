texture ColorTexture;
texture PrevColorTexture;
texture DepthTexture;
texture UserTexture;
float k = 1;


sampler ColorSampler : register(s0) = sampler_state
{
    Texture = (ColorTexture);

};


sampler DepthSampler  = sampler_state
{
    Texture = (DepthTexture);
	MinFilter = POINT; 
    MagFilter = POINT; 
	mipfilter=POINT;
    AddressU = CLAMP; 
    AddressV = CLAMP; 
};


sampler PrevColorSampler = sampler_state
{
    Texture = (PrevColorTexture);

};

sampler UserSampler = sampler_state
{
    Texture = (UserTexture);

};


void RestoreBuffersPixelShader(in float2 texCoord : TEXCOORD0, 
								 out float4 color: COLOR0,
								 out float depth : DEPTH)
{
	color =  tex2D(ColorSampler, texCoord);
	if (color.a == 0)
		color = 0;
	
	
	float z = tex2D(DepthSampler, texCoord).r;
	float nearPlane = 1000;
	float farPlane = 10000;
	float p = z * 10000;
    if (p <= nearPlane)
		depth = 0;
	else if (p >= farPlane)
         depth = 1;
		 else
    depth  = ((p - nearPlane) * farPlane / (farPlane - nearPlane)) / p;
}




void DrawNoUser(in float2 texCoord : TEXCOORD0, 
				out float4 color: COLOR0)

{
	float z = tex2D(ColorSampler, texCoord).r;
	
	if (z < 1.0/254.0 &&
		z > 1.0/256.0)
		color = tex2D(PrevColorSampler, texCoord);
	else
		color = tex2D(UserSampler, texCoord);
	//color.z = 1;
	
}

void DrawDepth(in float2 texCoord : TEXCOORD0, 
				out float4 color: COLOR0)
{
	color.rgb = tex2D(ColorSampler, texCoord).r;
	color.a = 1;
}

void DrawDefault(in float2 texCoord : TEXCOORD0, 
				out float4 color: COLOR0)
{
	color = tex2D(ColorSampler, texCoord);	
}




technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 RestoreBuffersPixelShader();
    }
}



technique Technique2
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 DrawNoUser();
    }
}

technique DepthBW
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 DrawDepth();
    }
}


technique Default
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 DrawDefault();
    }
}
