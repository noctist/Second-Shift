
#define numRaySamplesHigh 50;
#define numRaySamplesMed 40;
#define numRaySamplesLow 30;
float godRayAlphaMultiplyer = 0.75;
float godRayFinalMultiplyer = 5;
float godRayPower = 0.75;
float blendScale = 1;
float godRayScale = 1;
float godRayAlpha = 1;
float distortion = 0;
float bloomWidth = 0.05;
float bloomScale = 1;
float bloomIntensity = 1;
float smokeAlpha = 1;
float aspect = 1.777778;
float depth = 0;

float blur = 1;
float blurStart = 0.1;
float blurEnd = 0.2;

float bloom = 1;
float lightBloom = 1;
float objBloom = 0;
float bloomRadius = 0.03f;
float4 bloomColor = float4(1, 1, 1, 1);
float minBlur = 0;
float4 contrastColor = float4(0.5, 0.5, 0.5, 0.5);
float2 rayDelta = float2(0, 0);
float flareAlpha = 1;

float3 bevelColor = float3(1,1,1);
float2 bevelDelta = float2(0, 0);
float bevelGlow = 1;
float zoomLevel = 0.05f;

float3 objPos = float3(0, 0, 0);
float3 cameraPos = float3(0, 0, 0);
float cameraDepthSize = 500;
float3 cameraDepthPower = 1;
float2 cameraLookDirection = float2(0, 0);
float cameraLookDirectionW = 1.5;
float nearZPlane = 1000;
float farZPlane = -10000000;
float2 textureScale = float2(1, 1);

float2 depthMaskTexStart = float2(0.5, 0);
float2 depthMaskTexEnd = float2(1, 1);
float2 depthMaskDepthStart = 0.5f;
float2 depthMaskDepthEnd = 0.4f;

/*float farDepth = 50000;
float closeDepth = 0;

float farBlur = 0.1;
float closeBlur = 0;

float closeFog = 0;
float farFog = 0.1;
float fogWeight = 0.2;*/

SamplerState gSampler : register(s0)
{

};

Texture2D<float4> depthTexture : register(t1);
SamplerState depthSampler : register(s1) = sampler_state
{
	//Texture = (depthTexture);
};
/*Texture2D<float4> depthTexture2 : register(t2);
SamplerState depthSampler2 : register(s2) = sampler_state
{
	Texture = (depthTexture2);
};*/
Texture2D<float4> blendTexture : register(t3);
SamplerState blendSampler : register(s3) = sampler_state
{
	Texture=(blendTexture);
};

struct mrt { float4 col0 : COLOR0; float4 col1 : COLOR1; float4 col2 : COLOR2; };
struct VSOutput
{
	//float Depth : COLOR1;
	float4 Color : COLOR0;
	float2 Tex : TEXCOORD0;
	float4 Position : POSITION0;
	float Depth : DEPTH;
};
struct VSDepthBlendOutput
{
	//float Depth : COLOR1;
	float4 Color : COLOR0;
	float2 Tex : TEXCOORD0;
	float2 Tex2 : TEXCOORD1;
	float4 Position : POSITION0;
};
struct VSInput
{
	//float Depth : COLOR1;
	float4 Color : COLOR0;
	float2 Tex : TEXCOORD0;
	float4 Position : POSITION;
};

float4x4 MatrixTransform : register(vs, c0);
float4x4 ObjectTransform;

float getBlurLevel(float red)
{
	return clamp((red - blurStart) / (blurEnd - blurStart), 0, 1);
}
float length(float3 val1, float3 val2)
{
	float3 val3 = val1 - val2;
	return sqrt((val3.x * val3.x) + (val3.y * val3.y) + (val3.z * val3.z));
}
float between(float val1, float val2, float between)
{
	return val1 + ((val2 - val1) * between);
}
float2 between(float2 val1, float2 val2, float2 between)
{
	return val1 + ((val2 - val1) * between);
}
float4 between(float4 val1, float4 val2, float between)
{
	return val1 + ((val2 - val1) * between);
}

float betweenValue(float val1, float val2, float between)
{
	return (between - val1) / (val2 - val1);
}
float2 betweenValue(float2 val1, float2 val2, float2 between)
{
	return (between - val1) / (val2 - val1);
}
float4 betweenValue(float4 val1, float4 val2, float4 between)
{
	return (between - val1) / (val2 - val1);
}
/*VSOutput SpriteVertexShader(in float4 color    : COLOR0,  
                        in float2 texCoord : TEXCOORD0,  
                        in float4 position : SV_Position)  
{  
	VSOutput output;
	output.Position = mul(position, MatrixTransform);
	output.Color = color;
	output.Tex = texCoord;
    //output.Depth = (position.z - closeDepth) / (farDepth - closeDepth);
	return output;
}  */
float GetZDiff(float4 pos)
{
	return (pos.z - cameraPos.z) + cameraDepthSize;
	//return (length(pos.xyz, cameraPos.xyz + float3(0, 0, cameraDepthSize)));
}

float GetZFactor(float zdiff)
{
	return pow(cameraDepthSize / zdiff, 1);
}

float4 GetScreenPosition(float4 pos, float4 origPos)
{
	float zdiff = GetZDiff(origPos);
	float zFactor = GetZFactor(zdiff);
	
	//pos.x -= cameraPos.x;
	//pos.y -= cameraPos.y;
	//pos.w = zFactor;
	pos.w = 1 / zFactor;
	return pos;
}
void DepthBlendVertexShader(inout float4 color : COLOR0, inout float2 texCoord : TEXCOORD0, out float2 texCoord2 : TEXCOORD1, inout float4 position : SV_Position)
{
	position = mul(position, MatrixTransform);
	texCoord2 = between(depthMaskTexStart, depthMaskTexEnd, texCoord);
}
void SpriteVertexShader(inout float4 color : COLOR0, inout float2 texCoord : TEXCOORD0, inout float4 position : SV_Position, out float zDepth : DEPTH, out float blurDepth : DEPTH1)
{
	//position = mul(position, MatrixTransform);
	//position = GetScreenPosition(position);

	float z = position.z;
	texCoord *= textureScale;
	position = mul(position, ObjectTransform);
	z = position.z;
	float4 origPos = position;
	position = mul(position, MatrixTransform);
	position.z = z;
	position = GetScreenPosition(position, origPos);
	position.z = betweenValue(-nearZPlane, -farZPlane, z - cameraPos.z);
	position.xy += cameraLookDirection * (position.w - cameraLookDirectionW);
	//position.x += 0.5;
	zDepth = betweenValue(-nearZPlane, -farZPlane, z - cameraPos.z);
	blurDepth = getBlurLevel(zDepth);
	//position.z = between(nearZPlane, farZPlane, z);
}

void PolygonVertexShader(inout float4 color    : COLOR0,
	inout float2 texCoord : TEXCOORD0,
	inout float4 position : POSITION)
{
	position.xyz += objPos;
	float z = position.z;
	//color.r = position.z / 1000;
	float4 origPos = position;
	position = mul(position, MatrixTransform);
	//position.z = z;
	position = GetScreenPosition(position, origPos);
	position.z = 0;
}

void BasicVertexShader(inout float4 color    : COLOR0,
						inout float2 texCoord : TEXCOORD0,
						inout float4 position : SV_Position)
{
	position = mul(position, MatrixTransform);
}
float4 contrast(float4 col, float4 concol)
{
	return float4(concol.a + ((col.r - concol.a) * concol.r), 
		concol.a + ((col.g - concol.a) * concol.g), 
		concol.a + ((col.b - concol.a) * concol.b), 
		col.a);
}
float4 performScreenFinal(float4 col, float2 tex)
{
	col = contrast(col, contrastColor);
	//col = between(col, float4(1, 0.5, 0.2, 1), clamp(betweenValue(closeFog, farFog, depthTexture.Sample(depthSampler, tex).r), 0 , fogWeight));

	return col;
}
float4 AverageShader(float4 color : COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float4 col = (float4)0;
	float num = 0;
	for (float i = 0; i <= 1; i += 0.1)
	{
		for (float o = 0; o <= 1; o += 0.1)
		{
			col += tex2D(gSampler, float2(i, o));
			num++;
		}
	}
	return col / num;
}
mrt BevelShader(float4 color : COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float light = 0;
	float2 tex2 = tex;
    float num = 0;
	for (float i = -1; i < 1; i += 0.2)
	{
		light += (1 - tex2D(gSampler, tex + (bevelDelta * i)).a) * i;
		num++;
	}
	light /= num;
	light = clamp(light, -0.2, 50);
	mrt o = (mrt)0;
	o.col0 = tex2D(gSampler, tex) * color;
	o.col0 += float4(bevelColor * light, 0) * o.col0.a;
	o.col1 = float4(depth, objBloom + (bevelGlow * light), distortion, 1) * o.col0.a;
	return o;
}
mrt BevelShaderLow(float4 color : COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float light = 0;
	float2 tex2 = tex;
		float num = 0;
	for (float i = -1; i < 1; i += 0.25)
	{
		light += (1 - tex2D(gSampler, tex + (bevelDelta * i)).a) * i;
		num++;
	}
	light /= num;
	light = clamp(light, -0.2, 50);
	mrt o = (mrt)0;
	o.col0 = tex2D(gSampler, tex) * color;
	o.col0 += float4(bevelColor * light, 0) * o.col0.a;
	o.col1 = float4(depth, objBloom + (bevelGlow * light), distortion, 1) * o.col0.a;
	return o;
}
float4 ScreenBloomX(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	//float4 bloomLevel = depthTexture.Sample(depthSampler, tex);
	float4 col = 0;
	float intensity = 0;
	float4 tempCol;
	float num = 0;
	for (float i = -1; i <= 1; i+= 0.25)
	{
		intensity = 1 - abs(i);
	    tempCol = tex2D(gSampler, tex + float2(i * bloomRadius, 0));
		col += tempCol * intensity;
		num += intensity;
	}
	return (col / num) * bloom * color;
}
float4 ScreenBloomY(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 bloomLevel = depthTexture.Sample(depthSampler, tex);
	float4 col = 0;
	float intensity = 0;
	float4 tempCol;
	float num = 0;
	for (float i = -1; i <= 1; i+= 0.25)
	{
		intensity = 1 - abs(i);
	    tempCol = tex2D(gSampler, tex + float2(0, i * bloomRadius));
		col += tempCol * intensity;
		num += intensity;
	}
	return (col / num) * bloom * color;
}
float4 ScreenObjBloomX(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = 0;
	float intensity = 1;
	float4 tempCol;
	float num = 0;
	float intens = 1;
	float num2 = 0;
	float baseBloom = bloom + depthTexture.Sample(depthSampler, tex).g;
	float averageBloom = 0;
	for (float i = -1; i <= 1; i += 0.2f)
	{
		intensity = (1 - abs(i));
		intens = bloom + (depthTexture.Sample(depthSampler, tex + float2(i * bloomRadius, 0)).g);
		averageBloom += intens;
		tempCol = (tex2D(gSampler, tex + float2(i * bloomRadius, 0))) * intens * intensity;
		col += tempCol;
		num++;
		num2++;
	}
	averageBloom /= num;
	col /= num;
	float bloomMul = 1;
	if (baseBloom > averageBloom)
	{
		bloomMul = baseBloom / averageBloom;
	}
	return col * color;
}
float4 ScreenObjBloomY(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = 0;
	float intensity = 1;
	float4 tempCol;
	float num = 0;
	float intens = 1;
	float baseBloom = bloom + depthTexture.Sample(depthSampler, tex).g;
	float num2 = 0;
	float averageBloom = 0;
	//return tex2D(gSampler, tex) * 1000;
	for (float i = -1; i <= 1; i += 0.2f)
	{
		intensity = (1 - abs(i));

		intens = bloom + depthTexture.Sample(depthSampler, tex + float2(0, i * bloomRadius * aspect)).g;
		tempCol = tex2D(gSampler, tex + float2(0, i * bloomRadius * aspect)) * intensity;
		averageBloom += intens;
		//intens = 0;

		col += tempCol;
		num++;
		num2++;
	}
	averageBloom /= num;
	float bloomMul = 1;
	if (baseBloom > averageBloom)
	{
		bloomMul = baseBloom / averageBloom;
	}
	return ((pow(col / num, 2) * float4(1, 1, 1, 10 / 255.0f))) * 1000 * color;
}

float4 ScreenObjBloomXHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = 0;
	float intensity = 1;
	float4 tempCol;
	float num = 0;
	float intens = 1;
	float num2 = 0;
	float baseBloom = bloom + depthTexture.Sample(depthSampler, tex).g;
	float averageBloom = 0;
	for (float i = -1; i <= 1; i+= 0.1f)
	{
		intensity = (1 - abs(i));
		intens = bloom + (depthTexture.Sample(depthSampler, tex + float2(i * bloomRadius,0)).g);
		averageBloom += intens;
		tempCol = (tex2D(gSampler, tex + float2(i * bloomRadius, 0))) * intens * intensity;
		col += tempCol;
		num ++;
		num2 ++;
	}
	averageBloom /= num;
	col /= num;
	float bloomMul = 1;
	if (baseBloom > averageBloom)
	{
		bloomMul = baseBloom / averageBloom;
	}
	return col * color;
}
float4 ScreenObjBloomYHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = 0;
	float intensity = 1;
	float4 tempCol;
	float num = 0;
	float intens = 1;
	float baseBloom = bloom + depthTexture.Sample(depthSampler, tex).g;
	float num2 = 0;
	float averageBloom = 0;
	for (float i = -1; i <= 1; i+= 0.1f)
	{
		intensity = (1 - abs(i));
		
		intens = bloom + depthTexture.Sample(depthSampler, tex + float2(0, i * bloomRadius * aspect)).g;
		tempCol = tex2D(gSampler, tex + float2(0, i * bloomRadius * aspect)) * intensity;
		averageBloom += intens;
		//intens = 0;
	    
		col += tempCol;
		num ++;
		num2 ++;
	}
	averageBloom /= num;
	float bloomMul = 1;
	if (baseBloom > averageBloom)
	{
		bloomMul = baseBloom / averageBloom;
	}
	return ((pow(col / num, 2) * float4(1, 1, 1, 10 / 255.0f))) * 1000 * color;
}

float4 BlurX(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{ 
	float blurLevel = depthTexture.Sample(depthSampler, tex).r;
	//return tex2D(depthSampler2, tex);
	//blurLevel.r = (blurLevel.r - 0) / (1 - 0);
	float4 col = 0;
		float num = 0;
	float b2 = 1;
	float intensity = 0;
	float4 tempCol;
	float tempI;
	for (float i = -1; i <= 1; i += 0.2f)
	{
		intensity = 1 - abs(i);
		b2 = depthTexture.Sample(depthSampler, tex + float2(i * blurLevel * blur, 0)).r;
		//b2 *= 1 - ddx(b2);
		tempCol = tex2D(gSampler, tex + float2(i * blur * b2, 0));
		tempI = ((tempCol.r + tempCol.g + tempCol.b + 1) * intensity);
		//tempI = tempI * tempI;
		col += tempCol * tempI;
		num += tempI;
	}
	col /= num;
	col.a = 1;
	return col * color;
}

float4 BlurY(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float blurLevel = depthTexture.Sample(depthSampler, tex).r;
	//blurLevel.r = 1;
	float4 col = 0;
		float num = 0;
	float b2 = 1;
	float blurAlpha = clamp(((blurLevel.r * blur) - 0.002) / (0.0015), 0, 0.9999);
	float intensity = 0;
	float4 tempCol;
	float tempI;
	float mul;
	blurLevel *= aspect;
	//[branch]
	//if (blurAlpha > 0)
	{
		for (float i = -1; i <= 1; i += 0.2f)
		{
			intensity = 1 - abs(i);
			b2 = depthTexture.Sample(depthSampler, tex + float2(0, i * blurLevel * blur)).r;
			tempCol = tex2D(gSampler, tex + float2(0, i * blur * b2));
			tempI = (tempCol.r + tempCol.g + tempCol.b + 1) * intensity;
			//tempI = tempI * tempI;
			col += tempCol * tempI;
			num += tempI;
		}
		col /= num;
		return col * blurAlpha * color;
	}
}

float4 BlurXHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	
	float blurLevel = getBlurLevel(depthTexture.Sample(depthSampler, tex).r);
	//blurLevel.r = (blurLevel.r - 0) / (1 - 0);
    float4 col = 0;
	float num = 0;
	float b2 = 1;
	float intensity = 0;
	float4 tempCol;
	float tempI;
	for (float i = -1; i <= 1; i+= 0.1f)
	{
		intensity = 1 - abs(i);
		b2 = getBlurLevel(depthTexture.Sample(depthSampler, tex + float2(i * blurLevel * blur, 0)).r);
		//b2 *= 1 - ddx(b2);
		tempCol = tex2D(gSampler, tex + float2(i * blur * b2, 0));
		tempI = ((tempCol.r + tempCol.g + tempCol.b + 1) * intensity);
		//tempI = tempI * tempI;
		col += tempCol * tempI;
		num += tempI;
	}
	col /= num;
	col.a = 1;
	return col * color;
	
	/*float4 col = tex2D(gSampler, tex);
	return col;*/
}
float4 BlurYHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float blurLevel = getBlurLevel(depthTexture.Sample(depthSampler, tex).r);
	//blurLevel.r = 1;
    float4 col = 0;
	float num = 0;
	float b2 = 1;
	float blurAlpha = clamp(((blurLevel.r * blur) - 0.004) / (0.003), 0, 0.9999);
	float intensity = 0;
	float4 tempCol;
	float tempI;
	float mul;
	blurLevel *= aspect;
	//[branch]
	//if (blurAlpha > 0)
	{
		for (float i = -1; i <= 1; i+= 0.1f)
		{
			intensity = 1 - abs(i);
			b2 = getBlurLevel(depthTexture.Sample(depthSampler, tex + float2(0, i * blurLevel * blur)).r);
			tempCol = tex2D(gSampler, tex + float2(0, i * blur * b2));
			tempI = (tempCol.r + tempCol.g + tempCol.b + 1) * intensity;
			//tempI = tempI * tempI;
			col += tempCol * tempI;
			num += tempI;
		}
		col /= num;
		return col * blurAlpha * color;
	}
}

float4 LightBloomX(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 tempCol = tex2D(gSampler, tex + float2(0, 0));
	float minAlpha;
	float alpha = minAlpha = tempCol.r + tempCol.g + tempCol.b;
	float num = 0;
	float intensity = 1;
	for (float i = -1; i <= 1; i += 0.2f)
	{
		intensity = 1 - abs(i);
		tempCol = tex2D(gSampler, tex + float2(i * bloomWidth, 0)) * intensity;
		alpha += tempCol.r + tempCol.g + tempCol.b;
		num += intensity;
	}
	return (clamp(alpha / num, minAlpha, 100) * num / 3) * color / num;
}
float4 LightBloomY(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 tempCol = tex2D(gSampler, tex + float2(0, 0));
	float minAlpha;
	float alpha = minAlpha = tempCol.r + tempCol.g + tempCol.b;
	float num = 0;
	float intensity = 1;

	for (float i = -1; i <= 1; i += 0.2f)
	{
		intensity = 1 - abs(i);
		tempCol = tex2D(gSampler, tex + float2(0, i * bloomWidth)) * intensity;
		alpha += tempCol.r + tempCol.g + tempCol.b;
		num += intensity;
	}
	tex -= float2(0.5, 0.5);
	tex *= bloomScale;
	return (clamp(alpha / num, minAlpha, 100) * num / 3) * color * bloomIntensity * 7 * (1 - pow((tex.x * tex.x) + (tex.y * tex.y), 0.3)) / num;
}

float4 LightBloomXHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 tempCol = tex2D(gSampler, tex + float2(0, 0));
	float minAlpha;
	float alpha = minAlpha = tempCol.r + tempCol.g + tempCol.b;
	float num = 0;
	float intensity;
	for (float i = -1; i <= 1; i+= 0.1f)
	{
		intensity = 1 - abs(i);
		tempCol = tex2D(gSampler, tex + float2(i * bloomWidth, 0)) * intensity;
		alpha += tempCol.r + tempCol.g + tempCol.b;
		num += intensity;
	}
	return (clamp(alpha / num, minAlpha, 100) * num / 3) * color / num;
}
float4 LightBloomYHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 tempCol = tex2D(gSampler, tex + float2(0, 0));
	float minAlpha;
	float alpha = minAlpha = tempCol.r + tempCol.g + tempCol.b;
	float num = 0;
	float intensity;
		
	for (float i = -1; i <= 1; i+= 0.1f)
	{
		intensity = 1 - abs(i);
		tempCol = tex2D(gSampler, tex + float2(0, i * bloomWidth)) * intensity;
		alpha += tempCol.r+ tempCol.g + tempCol.b;
		num += intensity;
	}
	tex -= float2(0.5, 0.5);
	tex *= bloomScale;
	return (clamp(alpha / num, minAlpha, 100) * num / 3) * color * bloomIntensity * 7 * (1 - pow((tex.x * tex.x) + (tex.y * tex.y), 0.3)) / num;
}

float4 GodRayShader(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 tempCol;
	float4 col = 0;
		float2 texPos = 0.5f.xx + ((tex - 0.5f.xx) * godRayScale);
		float2 delta = ((0.5f.xx - tex) + rayDelta) * godRayScale / numRaySamplesMed
		float alAdd = 1;
	for (int i = 0; i < numRaySamplesMed i++)
	{
		//if (texPos.x < 0.9 && texPos.x > 0.1 &&texPos.y < 0.9 && texPos.y > 0.1)
		{
			tempCol = tex2D(gSampler, texPos);
			alAdd += (tempCol.r + tempCol.g + tempCol.b);
				col += tempCol;
		}

		texPos += delta;
	}
	alAdd *= godRayAlphaMultiplyer / numRaySamplesMed
	col /= numRaySamplesMed
	tex = (tex - float2(0.5f, 0.5f)) * 2;
	float al = pow(1 - clamp((tex.x * tex.x) + (tex.y * tex.y), 0, 1), godRayPower);
	return color * godRayFinalMultiplyer * al * pow(alAdd, 0.5);
}
float4 GodRayShaderHigh(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	//color.a *= godRayAlpha;
	float4 tempCol;
	float4 col = 0;
	float2 texPos = 0.5f.xx + ((tex - 0.5f.xx) * godRayScale);
	float2 delta = ((0.5f.xx - tex) + rayDelta) * godRayScale / numRaySamplesHigh
	float alAdd = 1;
	for (int i = 0; i < numRaySamplesHigh i++)
	{
		//if (texPos.x < 0.9 && texPos.x > 0.1 &&texPos.y < 0.9 && texPos.y > 0.1)
		{
			tempCol = tex2D(gSampler, texPos);
			alAdd += (tempCol.r + tempCol.g + tempCol.b);
			col += tempCol;
		}
		
		texPos += delta;	
	}
	alAdd *= godRayAlphaMultiplyer / numRaySamplesHigh
	col /= numRaySamplesHigh
	tex = (tex - float2(0.5f, 0.5f)) * 2;
	float al = pow(1 - clamp(  (tex.x * tex.x) + (tex.y * tex.y), 0, 1 ), godRayPower);
	return color * godRayFinalMultiplyer * al * pow(alAdd, 0.5);
}

float4 GodRayShaderLow(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 tempCol;
	float4 col = 0;
	float2 texPos = 0.5f.xx + ((tex - 0.5f.xx) * godRayScale);
	float2 delta = ((0.5f.xx - tex) + rayDelta) * godRayScale / numRaySamplesLow
	float alAdd = 0;
	for (int i = 0; i < numRaySamplesLow i++)
	{
		//if (texPos.x < 0.9 && texPos.x > 0.1 &&texPos.y < 0.9 && texPos.y > 0.1)
		{
			tempCol = tex2D(gSampler, texPos);
			alAdd += (tempCol.r + tempCol.g + tempCol.b);
			col += tempCol;
		}

		texPos += delta;
	}
	alAdd *= godRayAlphaMultiplyer / numRaySamplesLow
	col /= numRaySamplesLow
	tex = (tex - float2(0.5f, 0.5f)) * 2;
	float al = pow(1 - clamp((tex.x * tex.x) + (tex.y * tex.y), 0, 1), godRayPower);
	return color * godRayFinalMultiplyer * al * pow(alAdd, 0.5);
}

mrt SmokeShader(float4 color : COLOR, float2 tex : TEXCOORD0, float depth : DEPTH) : COLOR0
{
	mrt o = (mrt)0;
	float al = color.a;
	color /= color.a;
	float4 col = tex2D(gSampler, tex) * al;
	float coll = pow(clamp((col.a - 0.4f) * 10, 0, 1), 0.2);
	//if (col.a < 0.4f)
		//coll = 0;
	o.col0 = color * coll;
	o.col1 = float4(depth, objBloom, distortion * col.a, 1) * o.col0.a;
	return o;
}
float4 NormalShader(float4 color : COLOR, float2 tex : TEXCOORD0) : COLOR
{
	//return float4(1 * tex.x * color.e, 0, 0, 1);
	return tex2D(gSampler, tex) * color;
}
mrt NormalDepthShader(float4 color : COLOR, float2 tex : TEXCOORD0, float depth : DEPTH, float blurDepth : DEPTH1) : COLOR0
{
	mrt o = (mrt)0;
	o.col0 = tex2D(gSampler, tex) * color;
	o.col1 = float4(depth, objBloom, distortion, 1) * o.col0.a;
	o.col2 = float4(blurDepth, 0, 0, 1) * o.col0.a;
	return o;
}
mrt SlowDownShader(float4 color : COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	mrt o = (mrt)0;
	float4 col = tex2D(gSampler, tex + float2(-0.01f, 0.01f));
	col += tex2D(gSampler, tex + float2(-0.01f, -0.01f));
	col += tex2D(gSampler, tex + float2(0.01f, -0.01f));
	col += tex2D(gSampler, tex + float2(0.01f, 0.01f));
	o.col0 = (float4(0, 1, 1, 1) * color.a * ((abs(ddx(col.a)) + abs(ddy(col.a)) * 3)));
	o.col1 = float4(depth, objBloom, distortion, 1) * o.col0.a;
	return o;
}
float4 SelectedShader(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float4 col = tex2D(gSampler, tex);
	float high = 0;
	float high2 = 0;
	float num = 0;
	for (float i = -0.1; i <= 0.1; i+= 0.1f)
	{
		for (float o = -0.1; o <= 0.1; o+= 0.1f)
		{
		    float2 poss = tex + float2(i, o);
			high += tex2D(gSampler, poss).a;
			float xx = clamp(max((0.1 - poss.x) * 10,(poss.x -0.9) * 10), 0, 1);
			float yy = clamp(max((0.1 - poss.y) * 10,(poss.y -0.9) * 10), 0, 1);
			high2 += xx + yy;
			num ++;
		}
	}
    high /= num / 2;
	high2 /= num;
	high *= 1 - col.a;
	high2 *= col.a;
	return (float4)(high + high2);
}
float4 MaskShader(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float2 tex2 = 0.5.xx + ((tex - 0.5.xx) * blendScale);
	float4 col = tex2D(gSampler, tex2);
#if SM4
	float4 al = blendTexture.Sample(blendSampler, tex);
#else
		float4 al = tex2D(blendSampler, tex);
#endif
    //col.a = al.a;
	//float4 al = 1;
	return col * al.a * color * ((col.r + col.g + col.b) / 3);
}
float4 DepthMaskShader(float4 color: COLOR, float2 tex : TEXCOORD0, float2 tex2 : TEXCOORD1) : COLOR0
{
	float4 col = tex2D(gSampler, tex);
	float al = depthTexture.Sample(depthSampler, tex2).r;
	al = betweenValue(depthMaskDepthStart, depthMaskDepthEnd, al);
	return color * (float4(al, al, al, 1));
}
float4 Distortion(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float2 distort = (float2)0;
	float4 distortTimes = depthTexture.Sample(depthSampler, tex);
	float4 d = depthTexture.Sample(depthSampler, tex + float2(0.01f, 0));
	distort.x += d.b;
	d = depthTexture.Sample(depthSampler, tex + float2(-0.01f, 0));
	distort.x -= d.b;

	d = depthTexture.Sample(depthSampler, tex + float2(0, 0.01f));
	distort.y += d.b;
	d = depthTexture.Sample(depthSampler, tex + float2(0, -0.01f));
	distort.y -= d.b;

	//float2 dist = float2(ddx(distort), ddy(distort)) * distort * 10;
	return performScreenFinal(tex2D(gSampler, tex - (distort * distortTimes.b * (1 - distortTimes.r))) * color, tex);
}

float4 ZoomShader(float4 color : COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float4 col = (float4)0;
	float add = 0;
	for (float i = -1; i <= 1; i += 0.2)
	{
		col += tex2D(gSampler, tex + ((tex - float2(0.5f, 0.5f)) * i * zoomLevel));
		add++;
	}
	return performScreenFinal(col / add, tex);
}
float4 DistortionLow(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	float distort = depthTexture.Sample(depthSampler, tex).b;
	float2 dist = float2(distort, -distort * 0.5) * 0.1;
	return (tex2D(gSampler, tex + dist.xy) + tex2D(gSampler, tex + dist.yx)) / 2;
}

float4 GetAverageColorShader(float4 color: COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float4 col = (float4)0;
	float div = 0;
	for (float i = 0; i <= 1; i += 0.1)
	{
		for (float o = 0; o <= 1; o += 0.1)
		{
			col += tex2D(gSampler, (tex * 0.0001) + float2(i, o));
			div++;
		}
	}
	return col * color / div;
}

float4 GetAverageColorMediumShader(float4 color: COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float4 col = (float4)0;
	float div = 0;
	for (float i = 0.44; i <= 0.55; i += 0.05)
	{
		for (float o = 0.44; o <= 0.55; o += 0.05)
		{
			col += tex2D(gSampler, (tex * 0.0001) + float2(i, o));
			div++;
		}
	}
	return col * color / div;
}

float4 GetAverageColorSmallShader(float4 color: COLOR, float2 tex : TEXCOORD0) : COLOR0
{
	float4 col = (float4)0;
	float div = 0;
	for (float i = 0.48; i <= 0.52; i += 0.02)
	{
		for (float o = 0.48; o <= 0.52; o += 0.02)
		{
			col += tex2D(gSampler, (tex * 0.0001) + float2(i, o));
			div++;
		}
	}
	return col * color / div;
}

float4 ScreenFinalShader(float4 color: COLOR, float2 tex: TEXCOORD0) : COLOR0
{
	return performScreenFinal(tex2D(gSampler, tex) * color, tex);
}


technique Normal
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 NormalShader();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 NormalShader();
#endif
	}
}

technique PlainNormal
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 NormalShader();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 NormalShader();
#endif
	}
}
technique Polygon
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 PolygonVertexShader();
		PixelShader = compile ps_4_0_level_9_3 NormalShader();
#else
		VertexShader = compile vs_3_0 PolygonVertexShader();
		PixelShader = compile ps_3_0 NormalShader();
#endif
	}
}
technique GetAverageColor
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 GetAverageColorShader();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 GetAverageColorShader();
#endif
	}
}
technique GetAverageColorMedium
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 GetAverageColorMediumShader();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 GetAverageColorMediumShader();
#endif
	}
}
technique GetAverageColorSmall
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 GetAverageColorSmallShader();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 GetAverageColorSmallShader();
#endif
	}
}
technique NormalDepth
{
	pass p1
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_1 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_1 NormalDepthShader();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 NormalDepthShader();
#endif
	}
}
technique BlurMedium
{
	pass xPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 BlurX();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 BlurX();
#endif
	}
	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 BlurY();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 BlurY();
#endif
	}
}
technique BlurHigh
{
	pass xPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 BlurXHigh();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 BlurXHigh();
#endif
	}
	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 BlurYHigh();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 BlurYHigh();
#endif
	}
}

technique Bloom
{
	pass xPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_1 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_1 ScreenBloomX();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenBloomX();
#endif
	}

	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_1 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_1 ScreenBloomY();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenBloomY();
#endif
	}
}

technique ObjBloom
{
	pass xPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 ScreenObjBloomX();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenObjBloomX();
#endif
	}

	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 ScreenObjBloomY();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenObjBloomY();
#endif
	}
	
}

technique ObjBloomHigh
{
	pass xPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 ScreenObjBloomXHigh();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenObjBloomXHigh();
#endif
	}

	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 ScreenObjBloomYHigh();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenObjBloomYHigh();
#endif
	}
}

technique LightBloom
{
		pass xPass
	{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 LightBloomX();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 LightBloomX();
#endif
	}

	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 LightBloomY();
#else
		VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 LightBloomY();
#endif
	}
}

technique LightBloomHigh
{
		pass xPass
	{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 LightBloomXHigh();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 LightBloomXHigh();
#endif
	}

	pass yPass
	{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 LightBloomYHigh();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 LightBloomYHigh();
#endif
	}
}

technique Smoke
	{
		pass smokePass
		{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 SmokeShader();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 SmokeShader();
#endif
		}
	}
technique Selected
	{
		pass selectPass
		{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 SelectedShader();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 SelectedShader();
#endif
		}
	}
technique Distort
	{
		pass selectPass
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 Distortion();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 Distortion();
#endif
		}
	}
technique DistortLow
	{
		pass selectPass
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 DistortionLow();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 DistortionLow();
#endif
		}
	}
	technique Mask
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 MaskShader();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 MaskShader();
#endif
		}
	}
	technique DepthMask
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 DepthBlendVertexShader();
			PixelShader = compile ps_4_0_level_9_3 DepthMaskShader();
#else
			VertexShader = compile vs_3_0 DepthBlendVertexShader();
			PixelShader = compile ps_3_0 DepthMaskShader();
#endif
		}
	}
	technique GodRays
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 GodRayShader();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 GodRayShader();
#endif
		}
	}
	technique GodRaysHigh
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 GodRayShaderHigh();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 GodRayShaderHigh();
#endif
		}
	}
	technique GodRaysLow
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 GodRayShaderLow();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 GodRayShaderLow();
#endif
		}
	}
	technique SlowDown
	{
		pass p1
		{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 SlowDownShader();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 SlowDownShader();
#endif
		}
	}
	technique ScreenFinal
	{
		pass p1
		{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 BasicVertexShader();
		PixelShader = compile ps_4_0_level_9_3 ScreenFinalShader();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
		PixelShader = compile ps_3_0 ScreenFinalShader();
#endif
		}
	}
	technique Bevel
	{
		pass p1
		{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 BevelShader();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 BevelShader();
#endif
		}
	}
	technique BevelLow
	{
		pass p1
		{
#if SM4
		VertexShader = compile vs_4_0_level_9_3 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_3 BevelShaderLow();
#else
		VertexShader = compile vs_3_0 SpriteVertexShader();
		PixelShader = compile ps_3_0 BevelShaderLow();
#endif
		}
	}
	technique Average
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_1 BasicVertexShader();
			PixelShader = compile ps_4_0_level_9_1 NormalShader();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
			PixelShader = compile ps_3_0 AverageShader();
#endif
		}
	}
	/*technique Fake
	{
		pass p1
		{
#if SM4
			VertexShader = compile vs_4_0_level_9_1 BasicVertexShader();
			PixelShader = compile ps_4_0_level_9_1 FakeShader();
#else
			VertexShader = compile vs_3_0 BasicVertexShader();
			PixelShader = compile ps_3_0 FakeShader();
#endif
		}
	}*/