#include <TestHelper.fx>

float ConvertToLinearDepth(float depth, float4x4 projection)
{
	return projection._43 / (depth - projection._33);
}
float ConvertToLinearDepth(float depth, float zNear, float zFar)
{
	float Projection43 = -zNear * zFar / (zFar-zNear);
	float Projection33 = zFar / (zFar - zNear);
	return Projection43 / (depth - Projection33);
}
