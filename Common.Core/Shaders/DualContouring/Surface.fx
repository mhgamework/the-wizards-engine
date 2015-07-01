
#include <Deferred\DeferredMesh.fx>


technique10 DCSurface
{
    pass Pass1
    {
		SetGeometryShader( NULL );
		SetVertexShader( CompileShader( vs_4_0, VertexShaderFunction() ) );
		SetPixelShader( CompileShader( ps_4_0, PixelShaderFunction() ) );
    }

}
