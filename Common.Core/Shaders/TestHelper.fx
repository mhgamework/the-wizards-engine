float4 t(float value)
{
	return float4(value,0,0,1);
}
float4 t(float2 value)
{
	return float4(value,0,1);
}
float4 t(float3 value)
{
	return float4(value,1);
}
float4 t(float4 value)
{
	return value;
}