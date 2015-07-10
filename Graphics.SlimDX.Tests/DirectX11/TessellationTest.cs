//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using SlimDX;
//using SlimDX.D3DCompiler;
//using SlimDX.Direct3D11;
//using SlimDX.DXGI;
//using Buffer = SlimDX.Direct3D11.Buffer;
//using Device = SlimDX.Direct3D11.Device;

//namespace MHGameWork.TheWizards.Tests.DirectX11
//{
//    [TestFixture]
//    public class TessellationTest 
//    {
//        [Test]
//        public void TestSimpleTesselation()
//        {
            
//        }

//    struct SIMPLEVERTEX 
//{
//        float x, y, z;
//        float u, v;
//};

//struct EXTENDEDVERTEX 
//{
//        float x, y, z;
//        float nx, ny, nz;
//        float u, v;
//};

//struct TANGENTSPACEVERTEX 
//{
//        float x, y, z;
//        float u, v;
//        float nx, ny, nz;
//        float bx, by, bz;
//        float tx, ty, tz;
//};


//    //--------------------------------------------------------------------------------------
//// Constants
////--------------------------------------------------------------------------------------
//public static  float GRID_SIZE               = 200.0f;
//public static  float MAX_TESSELLATION_FACTOR = 11.0f;

////--------------------------------------------------------------------------------------
//// Structures
////--------------------------------------------------------------------------------------
//struct CONSTANT_BUFFER_STRUCT
//{
//    Matrix    mWorld;                         // World Matrix
//    Matrix    mView;                          // View Matrix
//    Matrix    mProjection;                    // Projection Matrix
//    Matrix    mWorldViewProjection;           // WVP Matrix
//    Matrix    mViewProjection;                // VP Matrix
//    Matrix    mInvView;                       // Inverse of view Matrix
//    Vector4      vMeshColor;                     // Mesh color
//    Vector4      vTessellationFactor;            // Edge, inside and minimum tessellation factor
//    Vector4      vDetailTessellationHeightScale; // Height scale for detail tessellation of grid surface
//    Vector4      vGridSize;                      // Grid size
//    Vector4      vDebugColorMultiply;            // Debug colors
//    Vector4      vDebugColorAdd;                 // Debug colors
//};                                                      
                                                        
//struct MATERIAL_CB_STRUCT
//{
//    Vector4     g_materialAmbientColor;  // Material's ambient color
//    Vector4     g_materialDiffuseColor;  // Material's diffuse color
//    Vector4     g_materialSpecularColor; // Material's specular color
//    Vector4     g_fSpecularExponent;     // Material's specular exponent

//    Vector4     g_LightPosition;         // Light's position in world space
//    Vector4     g_LightDiffuse;          // Light's diffuse color
//    Vector4     g_LightAmbient;          // Light's ambient color

//    Vector4     g_vEye;                  // Camera's location
//    Vector4     g_fBaseTextureRepeat;    // The tiling factor for base and normal map textures
//    Vector4     g_fPOMHeightMapScale;    // Describes the useful range of values for the height field

//    Vector4     g_fShadowSoftening;      // Blurring factor for the soft shadows computation

//    int             g_nMinSamples;           // The minimum number of samples for sampling the height field
//    int             g_nMaxSamples;           // The maximum number of samples for sampling the height field
//    int             uDummy1;
//    int             uDummy2;
//};

//struct DETAIL_TESSELLATION_TEXTURE_STRUCT
//{
//    WCHAR* DiffuseMap;                  // Diffuse texture map
//    WCHAR* NormalHeightMap;             // Normal and height map (normal in .Xyz, height in .w)
//    WCHAR* DisplayName;                 // Display name of texture
//    float  fHeightScale;                // Height scale when rendering
//    float  fBaseTextureRepeat;          // Base repeat of texture coordinates (1.0f for no repeat)
//    float  fDensityScale;               // Density scale (used for density map generation)
//    float  fMeaningfulDifference;       // Meaningful difference (used for density map generation)
//};

////--------------------------------------------------------------------------------------
//// Enums
////--------------------------------------------------------------------------------------
//public enum eTessellationMethod
//{
//    TESSELLATIONMETHOD_DISABLED,
//    TESSELLATIONMETHOD_DISABLED_POM,
//    TESSELLATIONMETHOD_DETAIL_TESSELLATION,
//} 

////--------------------------------------------------------------------------------------
//// Global variables
////--------------------------------------------------------------------------------------
//// DXUT resources
//CDXUTDialogResourceManager          g_DialogResourceManager;    // Manager for shared resources of dialogs
//CFirstPersonCamera                  g_Camera;
//CD3DSettingsDlg                     g_D3DSettingsDlg;           // Device settings dialog
//CDXUTDialog                         g_HUD;                      // Manages the 3D   
//CDXUTDialog                         g_SampleUI;                 // Dialog for sample specific controls
//CDXUTTextHelper*                    g_pTxtHelper = null;

//// Shaders
//VertexShader                 g_pPOMVS = null;
//PixelShader                  g_pPOMPS = null;
//VertexShader                 g_pDetailTessellationVS = null;
//VertexShader                 g_pNoTessellationVS = null;
//HullShader                   g_pDetailTessellationHS = null;
//DomainShader                 g_pDetailTessellationDS = null;
//PixelShader                  g_pBumpMapPS = null;
//VertexShader                 g_pParticleVS = null;
//GeometryShader               g_pParticleGS = null;
//PixelShader                  g_pParticlePS = null;

//// Textures
//public static int NUM_TEXTURES= 7;
////DETAIL_TESSELLATION_TEXTURE_STRUCT DetailTessellationTextures[NUM_TEXTURES + 1] =
////{
//////    DiffuseMap              NormalHeightMap                 DisplayName,    fHeightScale fBaseTextureRepeat fDensityScale fMeaningfulDifference
////    { "Textures\\rocks.jpg",    "Textures\\rocks_NM_height.dds",  "Rocks",       10.0f,       1.0f,              25.0f,        2.0f/255.0f },
////    { "Textures\\stones.bmp",   "Textures\\stones_NM_height.dds", "Stones",      5.0f,        1.0f,              10.0f,        5.0f/255.0f },
////    { "Textures\\wall.jpg",     "Textures\\wall_NM_height.dds",   "Wal",        8.0f,        1.0f,              7.0f,         7.0f/255.0f },
////    { "Textures\\wood.jpg",     "Textures\\four_NM_height.dds",   "Four shapes", 30.0f,       1.0f,              35.0f,        2.0f/255.0f },
////    { "Textures\\concrete.bmp", "Textures\\bump_NM_height.dds",   "Bump",        10.0f,       4.0f,              50.0f,        2.0f/255.0f },
////    { "Textures\\concrete.bmp", "Textures\\dent_NM_height.dds",   "Dent",        10.0f,       4.0f,              50.0f,        2.0f/255.0f },
////    { "Textures\\wood.jpg",     "Textures\\saint_NM_height.dds",  "Saints" ,     20.0f,       1.0f,              40.0f,        2.0f/255.0f },
////    { "",                    "",                            "Custom" ,     5.0f,        1.0f,              10.0f,        2.0f/255.0f },
////};
//int                               g_dwNumTextures = 0;
//ShaderResourceView           g_pDetailTessellationBaseTextureRV[NUM_TEXTURES+1];
//ShaderResourceView           g_pDetailTessellationHeightTextureRV[NUM_TEXTURES+1];
//ShaderResourceView           g_pDetailTessellationDensityTextureRV[NUM_TEXTURES+1];
//ShaderResourceView           g_pLightTextureRV = null;
//string[]                               g_pszCustomTextureDiffuseFilename[MAX_PATH] = "";
//string[]                               g_pszCustomTextureNormalHeightFilename[MAX_PATH] = "";
//bool[]                               g_bRecreateCustomTextureDensityMap  = false;

//// Geometry
//Buffer                       g_pGridTangentSpaceVB = null;
//Buffer                       g_pGridTangentSpaceIB = null;
//Buffer                       g_pMainCB = null;
//Buffer                       g_pMaterialCB = null;
//InputLayout                  g_pTangentSpaceVertexLayout = null;
//InputLayout                  g_pPositionOnlyVertexLayout = null;
//Buffer[]                       g_pGridDensityVB = new Buffer[NUM_TEXTURES+1];
//ShaderResourceView[]           g_pGridDensityVBSRV= new ShaderResourceView[NUM_TEXTURES+1];
//Buffer                       g_pParticleVB = null;

//// States
//RasterizerState              g_pRasterizerStateSolid = null;
//RasterizerState              g_pRasterizerStateWireframe = null;
//SamplerState                 g_pSamplerStateLinear = null;
//BlendState                   g_pBlendStateNoBlend = null;
//BlendState                   g_pBlendStateAdditiveBlending = null;
//DepthStencilState            g_pDepthStencilState = null;


//// Render settings
//int                                 g_nRenderHUD = 2;
//int                               g_dwGridTessellation = 50;
//bool                                g_bEnableWireFrame = false;
//float                               g_fTessellationFactorEdges = 7.0f;
//float                               g_fTessellationFactorInside = 7.0f;
//int                                 g_nTessellationMethod = TESSELLATIONMETHOD_DETAIL_TESSELLATION;
//int                                 g_nCurrentTexture = 0;
//bool                                g_bFrameBasedAnimation = false;
//bool                                g_bAnimatedCamera = false;
//bool                                g_bDisplacementEnabled = true;
//Vector3                         g_vDebugColorMultiply=new Vector3(1.0f, 1.0f, 1.0f);
//Vector3                         g_vDebugColorAdd = new Vector3(0.0f, 0.0f, 0.0f);
//bool                                g_bDensityBasedDetailTessellation = false;
//bool                                g_bDistanceAdaptiveDetailTessellation = false;
//bool                                g_bDetailTessellationShadersNeedReloading = false;
//bool                                g_bShowFPS = true;
//bool                                g_bDrawLightSource = true;

//// Frame buffer readback ( 0 means never dump to disk (frame counter starts at 1) )
//int                               g_dwFrameNumberToDump = 0; 


////--------------------------------------------------------------------------------------
//// Entry point to the program. Initializes everything and goes into a message processing 
//// loop. Idle time is used to render the scene.
////--------------------------------------------------------------------------------------
//int WINAPI wWinMain( HINSTANCE hInstance, HINSTANCE hPrevInstance, LPWSTR lpCmdLine, int nCmdShow )
//{

//    // DXUT will create and use the best device (either D3D10 or D3D11) 
//    // that is available on the system depending on which D3D callbacks are set below

//    InitApp();
//    DXUTInit( true, true );
//    DXUTSetCursorSettings( true, true ); // Show the cursor and clip it when in full screen
//    DXUTCreateWindow( "DetailTessellation11 v1.11" );
//    DXUTCreateDevice( D3D_FEATURE_LEVEL_11_0, true, 1024, 768);
//    DXUTMainLoop(); // Enter into the DXUT render loop

//    return DXUTGetExitCode();
//}



////--------------------------------------------------------------------------------------
//// Called right before creating a D3D9 or D3D10 device, allowing the app to modify the device settings as needed
////--------------------------------------------------------------------------------------
//bool CALLBACK ModifyDeviceSettings( DXUTDeviceSettings* pDeviceSettings, void* pUserContext )
//{
//    // For the first device created if its a REF device, optionally display a warning dialog box
//    static bool s_bFirstTime = true;
//    if( s_bFirstTime )
//    {
//        s_bFirstTime = false;
//        if( ( DXUT_D3D11_DEVICE == pDeviceSettings.ver &&
//              pDeviceSettings.d3d11.DriverType == D3D_DRIVER_TYPE_REFERENCE ) )
//        {
//            DXUTDisplaySwitchingToREFWarning( pDeviceSettings.ver );
//        }

//        // Enable 4xMSAA by default
//        DXGI_SAMPLE_DESC MSAA4xSampleDesc = { 4, 0 };
//        pDeviceSettings.d3d11.sd.SampleDesc = MSAA4xSampleDesc;
//    }

//    return true;
//}



////--------------------------------------------------------------------------------------
//// Handle key presses
////--------------------------------------------------------------------------------------
//void CALLBACK OnKeyboard( UINT nChar, bool bKeyDown, bool bAltDown, void* pUserContext )
//{
//    if( bKeyDown )
//    {
//        switch( nChar )
//        {
//            case 'V':           // Debug views
//                                if (g_vDebugColorMultiply.X==1.0)
//                                {
//                                    g_vDebugColorMultiply.X=0.0;
//                                    g_vDebugColorMultiply.Y=0.0;
//                                    g_vDebugColorMultiply.Z=0.0;
//                                    g_vDebugColorAdd.X=1.0;
//                                    g_vDebugColorAdd.Y=1.0;
//                                    g_vDebugColorAdd.Z=1.0;
//                                }
//                                else
//                                {
//                                    g_vDebugColorMultiply.X=1.0;
//                                    g_vDebugColorMultiply.Y=1.0;
//                                    g_vDebugColorMultiply.Z=1.0;
//                                    g_vDebugColorAdd.X=0.0;
//                                    g_vDebugColorAdd.Y=0.0;
//                                    g_vDebugColorAdd.Z=0.0;
//                                }
//                                break;


//        }
//    }
//}



////--------------------------------------------------------------------------------------
//// Create any D3D11 resources that aren't dependant on the back buffer
////--------------------------------------------------------------------------------------
//void CALLBACK OnD3D11CreateDevice( Device device, SurfaceDescription pBackBufferSurfaceDesc,
//                                      void* pUserContext )
//{

//    // Get device context
//    DeviceContext context = device.ImmediateContext;


//    //
//    // Compile shaders
//    //
//    DWORD dwShaderFlags = D3D10_SHADER_ENABLE_STRICTNESS;
//#if defined( DEBUG ) || defined( _DEBUG )
//    // Set the D3D10_SHADER_DEBUG flag to embed debug information in the shaders.
//    // Setting this flag improves the shader debugging experience, but still allows 
//    // the shaders to be optimized and to run exactly the way they will run in 
//    // the release configuration of this program.
//    dwShaderFlags |= D3D10_SHADER_DEBUG;
//#endif

//    // Detail tessellation shaders
//    ID3DBlob* pBlobVS_DetailTessellation = null;
    
//    DXUTFindDXSDKMediaFileCch(wcPath, 256, "DetailTessellation11.hls");

//    CreateVertexShaderFromFile( device, wcPath, pDetailTessellationDefines, null, "VS_NoTessellation", 
//                               "vs_5_0", dwShaderFlags, 0, null, &g_pNoTessellationVS );
//    CreateVertexShaderFromFile( device, wcPath, pDetailTessellationDefines, null, "VS",                
//                               "vs_5_0", dwShaderFlags, 0, null, &g_pDetailTessellationVS, &pBlobVS_DetailTessellation );
//    CreateHullShaderFromFile( device,   wcPath, pDetailTessellationDefines, null, "HS",                
//                               "hs_5_0", dwShaderFlags, 0, null, &g_pDetailTessellationHS );
//    CreateDomainShaderFromFile( device, wcPath, pDetailTessellationDefines, null, "DS",                
//                               "ds_5_0", dwShaderFlags, 0, null, &g_pDetailTessellationDS );
//    CreatePixelShaderFromFile( device,  wcPath, pDetailTessellationDefines, null, "BumpMapPS",
//                               "ps_5_0", dwShaderFlags, 0, null, &g_pBumpMapPS );


//    //
//    // Create vertex layouts
//    //
    
//    // Tangent space vertex input layout
//    InputElement[] tangentspacevertexlayout = new []
//    {
//        new InputElement( "POSITION", 0,Format.R32G32B32_Float  , 0, 0,  InputClassification.PerVertexData, 0 ),
//        new InputElement( "TEXCOORD", 0, Format.R32G32_Float,    0, 12, InputClassification.PerVertexData, 0 ),
//        new InputElement( "NORMAL",   0, Format.R32G32B32_Float, 0, 20, InputClassification.PerVertexData, 0 ),
//        new InputElement( "BINORMAL", 0, Format.R32G32B32_Float, 0, 32, InputClassification.PerVertexData, 0 ),
//        new InputElement( "TANGENT",  0, Format.R32G32B32_Float, 0, 44, InputClassification.PerVertexData, 0 ),
//    };

//    g_pTangentSpaceVertexLayout = new InputLayout(device,tangentspacevertexlayout,  pBlobVS_DetailTessellation.GetBufferPointer(), pBlobVS_DetailTessellation.GetBufferSize());

//    // Release blobs
//    pBlobVS_DetailTessellation.Dispose();

    
    
//    // Create main constant buffer
//    BufferDescription bd = new BufferDescription();
//    bd.Usage = ResourceUsage.Dynamic;
//    bd.ByteWidth = sizeof( CONSTANT_BUFFER_STRUCT );
//    bd.BindFlags = BindFlags.ConstantBuffer;
//    bd.CpuAccessFlags = CpuAccessFlags.Write;
//    bd.OptionFlags =  ResourceOptionFlags.None;
//    g_pMainCB = new Buffer(device, bd);

//    // Create grid geometry
//    FillGrid_Indexed_WithTangentSpace( device, g_dwGridTessellation, g_dwGridTessellation, GRID_SIZE, GRID_SIZE, 
//                                       &g_pGridTangentSpaceVB, &g_pGridTangentSpaceIB );

//    //
//    // Load textures
//    //
    
//    // Determine how many textures to load
//    g_dwNumTextures = NUM_TEXTURES;

//    // Is a custom texture specified?
//    if ( ( wcslen( DetailTessellationTextures[NUM_TEXTURES].DiffuseMap )!=0 ) && 
//         ( wcslen( DetailTessellationTextures[NUM_TEXTURES].NormalHeightMap )!=0 ) )
//    {
//        // Yes, add one to number of textures and update array
//        g_dwNumTextures += 1;
//    }

//    // Loop through all textures and load them
//    WCHAR str[256];
//    for ( int i=0; i<(int)g_dwNumTextures; ++i )
//    {
//        // Load detail tessellation base texture
//        DXUTFindDXSDKMediaFileCch( str, 256, DetailTessellationTextures[i].DiffuseMap );
//        hr = D3DX11CreateShaderResourceViewFromFile( device, str, 
//                                                     null, null, &g_pDetailTessellationBaseTextureRV[i], null );
//        if( FAILED( hr ) )
//            return hr;

//        // Load detail tessellation normal+height texture
//        DXUTFindDXSDKMediaFileCch( str, 256, DetailTessellationTextures[i].NormalHeightMap );
//        hr = D3DX11CreateShaderResourceViewFromFile( device, str, 
//                                                     null, null, &g_pDetailTessellationHeightTextureRV[i], null );
//        if( FAILED( hr ) )
//            return hr;

//        // Compute density map filename
//        WCHAR pszDensityMapFilename[256];
//        WCHAR *pExtensionPointer;

//        // Copy normal_height filename
//        DXUTFindDXSDKMediaFileCch(pszDensityMapFilename, 256, DetailTessellationTextures[i].NormalHeightMap );
//        pExtensionPointer = wcsrchr(pszDensityMapFilename, '.');
//        swprintf_s(pExtensionPointer, pExtensionPointer-pszDensityMapFilename, "_density.dds");
        
//        {
//            // Density file for this texture doesn't exist, create it

//            // Get description of source texture
//            ID3D11Texture2D* pHeightMap;
//            ID3D11Texture2D* pDensityMap;
//            g_pDetailTessellationHeightTextureRV[i].GetResource( (ID3D11Resource**)&pHeightMap );
            
//            // Create density map from height map
//            CreateDensityMapFromHeightMap( device, context, pHeightMap, 
//                                           DetailTessellationTextures[i].fDensityScale, &pDensityMap, null, 
//                                           DetailTessellationTextures[i].fMeaningfulDifference );

//            // Save density map to file
//            D3DX11SaveTextureToFile( context, pDensityMap, D3DX11_IFF_DDS, pszDensityMapFilename );
//            D3D11_TEXTURE2D_DESC d2ddsc;
//            pDensityMap.GetDesc(&d2ddsc); 
//            D3D11_SHADER_RESOURCE_VIEW_DESC dsrvd;
//            dsrvd.Format = d2ddsc.Format;
//            dsrvd.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
//            dsrvd.Texture2D.MipLevels = d2ddsc.MipLevels;
//            dsrvd.Texture2D.MostDetailedMip = 0;

//            device.CreateShaderResourceView( pDensityMap, &dsrvd, &g_pDetailTessellationDensityTextureRV[i] );



//            pDensityMap.Dispose();
//            pHeightMap.Dispose();
//        }


//        // Create density vertex stream for grid

//        // Create STAGING versions of VB and IB
//        Buffer pGridVBSTAGING = null;
//        CreateStagingBufferFromBuffer( device, context, g_pGridTangentSpaceVB, &pGridVBSTAGING );
        
//        Buffer pGridIBSTAGING = null;
//        CreateStagingBufferFromBuffer( device, context, g_pGridTangentSpaceIB, &pGridIBSTAGING );

//        ID3D11Texture2D* pDensityMap = null;
//        g_pDetailTessellationDensityTextureRV[i].GetResource( (ID3D11Resource **)&pDensityMap );


//        pGridIBSTAGING.Dispose();
//        pGridVBSTAGING.Dispose();

//    }

//    //
//    // Create state objects
//    //

//    // Create solid and wireframe rasterizer state objects
//    RasterizerStateDescription RasterDesc = new RasterizerStateDescription();
//    RasterDesc.FillMode =  FillMode.Solid;
//    RasterDesc.CullMode =  CullMode.Back;
//    RasterDesc.IsDepthClipEnabled = true;
//    g_pRasterizerStateSolid = RasterizerState.FromDescription(device, RasterDesc);

//    RasterDesc.FillMode =  FillMode.Wireframe;
//    g_pRasterizerStateWireframe = RasterizerState.FromDescription(device, RasterDesc);

//    // Create sampler state for heightmap and normal map
//    SamplerDescription SSDesc = new SamplerDescription();
//    SSDesc.Filter =          Filter.MinMagMipLinear;
//    SSDesc.AddressU =        TextureAddressMode.Wrap;
//    SSDesc.AddressV =       TextureAddressMode.Wrap;
//    SSDesc.AddressW =       TextureAddressMode.Wrap;
//    SSDesc.ComparisonFunction =  Comparison.Never;
//    SSDesc.MaximumAnisotropy =  16;
//    SSDesc.MinimumLod =         0;
//    SSDesc.MaximumLod =        float.MaxValue;
//    g_pSamplerStateLinear = SamplerState.FromDescription(device, SSDesc);

//    // Create a blend state to disable alpha blending
//    BlendStateDescription blendState = new BlendStateDescription();
//    blendState.IndependentBlendEnable =                 false;
//    blendState.RenderTargets[0].BlendEnable =            false;
//    blendState.RenderTargets[0].RenderTargetWriteMask =  ColorWriteMaskFlags.All;
    
//    g_pBlendStateNoBlend = BlendState .FromDescription(device, blendState);
//    blendState.RenderTargets[0].BlendEnable =            true;
//    blendState.RenderTargets[0].BlendOperation =                 BlendOperation.Add;
//    blendState.RenderTargets[0].SourceBlend =               BlendOption.One;
//    blendState.RenderTargets[0].DestinationBlend =              BlendOption.One;
//    blendState.RenderTargets[0].RenderTargetWriteMask =   ColorWriteMaskFlags.All;
//    blendState.RenderTargets[0].BlendOperationAlpha =           BlendOperation.Add;
//    blendState.RenderTargets[0].SourceBlendAlpha =           BlendOption.Zero;
//    blendState.RenderTargets[0].DestinationBlendAlpha =         BlendOption.Zero;
//    g_pBlendStateAdditiveBlending = BlendState.FromDescription(device, blendState);
    
//    // Create a depthstencil state
//    DepthStencilStateDescription DSDesc = new DepthStencilStateDescription();
//    DSDesc.IsDepthEnabled =        true;
//    DSDesc.DepthComparison =            Comparison.LessEqual;
//    DSDesc.DepthWriteMask =      DepthWriteMask.All;
//    DSDesc.IsStencilEnabled =      false;
//    g_pDepthStencilState = DepthStencilState.FromDescription(device, DSDesc);

//}



////--------------------------------------------------------------------------------------
//// Render the scene using the D3D11 device
////--------------------------------------------------------------------------------------
//void CALLBACK OnD3D11FrameRender( Device device, DeviceContext context, 
//                                  double fTime, float fElapsedTime, void* pUserContext )
//{
//    static DWORD                s_dwFrameNumber = 1;
//    Buffer               pBuffers[2];
//    ShaderResourceView   pSRV[4];
//    SamplerState         pSS[1];
//    D3DXVECTOR3                 vFrom;

//    // Check if detail tessellation shaders need reloading
//    if ( g_bDetailTessellationShadersNeedReloading )
//    {
//        // Yes, releasing existing detail tessellation shaders
//        g_pBumpMapPS.Dispose();
//        g_pDetailTessellationDS.Dispose();
//        g_pDetailTessellationHS.Dispose();
//        g_pDetailTessellationVS.Dispose();
//        g_pNoTessellationVS.Dispose();

//        // ... and reload them
//        D3D10_SHADER_MACRO    pDetailTessellationDefines[] = 
//            { "DENSITY_BASED_TESSELLATION", g_bDensityBasedDetailTessellation ? "1" : "0", 
//              "DISTANCE_ADAPTIVE_TESSELLATION", g_bDistanceAdaptiveDetailTessellation ? "1" : "0", 
//              null, null };
//        WCHAR wcPath[256];
//        DXUTFindDXSDKMediaFileCch( wcPath, 256, "DetailTessellation11.hls" );
//        CreateVertexShaderFromFile(device, wcPath, pDetailTessellationDefines, null, 
//                "VS_NoTessellation", "vs_5_0", 0, 0, null, &g_pNoTessellationVS);
//        CreateVertexShaderFromFile(device, wcPath, pDetailTessellationDefines, null, 
//                "VS","vs_5_0", 0, 0, null, &g_pDetailTessellationVS);
//        CreateHullShaderFromFile(device,   wcPath, pDetailTessellationDefines, null, 
//                "HS", "hs_5_0", 0, 0, null, &g_pDetailTessellationHS);
//        CreateDomainShaderFromFile(device, wcPath, pDetailTessellationDefines, null, 
//                "DS", "ds_5_0", 0, 0, null, &g_pDetailTessellationDS);
//        CreatePixelShaderFromFile(device,  wcPath, pDetailTessellationDefines, null, 
//                "BumpMapPS", "ps_5_0", 0, 0, null, &g_pBumpMapPS);
        
//        g_bDetailTessellationShadersNeedReloading = false;
//    }

//    // Projection Matrix
//    D3DXMatrix mProj = *g_Camera.GetProjMatrix();

//    // View Matrix
//    D3DXMatrix mView;
//    if ( g_bAnimatedCamera )
//    {
//        float fRPS = 0.1f;
//        float fAngleRAD;
//        if ( g_bFrameBasedAnimation )
//        {
//            static float s_fTick = 0.0f;
//            s_fTick += 1.0f/60.0f;
//            fTime = s_fTick;
//        }
//        fAngleRAD = (float)( fRPS * fTime * 2.0f * D3DX_PI );
//        float fRadius = (3.0f/4.0f) * GRID_SIZE;
//        vFrom.X = fRadius * cos( fAngleRAD );
//        vFrom.Y = 80.0f;
//        vFrom.Z = fRadius * sin( fAngleRAD );
//        const D3DXVECTOR3 vAt( 0, 0, 0 );
//        const D3DXVECTOR3 vUp(0, 1, 0);
//        D3DXMatrixLookAtLH( &mView, &vFrom, &vAt, &vUp );
//    }
//    else
//    {
//        vFrom = *g_Camera.GetEyePt();
//        mView = *g_Camera.GetViewMatrix();
//    }

//    // World Matrix: identity
//    D3DXMatrix mWorld;
//    D3DXMatrixIdentity( &mWorld );

//    // Update combined matrices
//    D3DXMatrix mWorldViewProjection = mWorld * mView * mProj;    
//    D3DXMatrix mViewProjection = mView * mProj;    
//    D3DXMatrix mInvView;
//    D3DXMatrixInverse( &mInvView, null, &mView );

//    // Transpose matrices before passing to shader stages
//    D3DXMatrixTranspose( &mProj, &mProj );
//    D3DXMatrixTranspose( &mView, &mView );
//    D3DXMatrixTranspose( &mWorld, &mWorld );                    
//    D3DXMatrixTranspose( &mWorldViewProjection, &mWorldViewProjection ); 
//    D3DXMatrixTranspose( &mViewProjection, &mViewProjection );
//    D3DXMatrixTranspose( &mInvView, &mInvView );

//    // Update light position from light direction control
//    g_LightPosition.X =  -(GRID_SIZE/2.0f) * g_LightControl.GetLightDirection().X;
//    g_LightPosition.Y =  -(GRID_SIZE/2.0f) * g_LightControl.GetLightDirection().Y;
//    g_LightPosition.Z =  -(GRID_SIZE/2.0f) * g_LightControl.GetLightDirection().Z;

//    // Update main constant buffer
//    Vector4 vWhite( 1.0f, 1.0f, 1.0f, 1.0f );
//    D3D11_MAPPED_SUBRESOURCE MappedSubResource;
//    hr = context.Map( g_pMainCB, 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedSubResource );
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).mWorld =               mWorld;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).mView =                mView;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).mProjection =          mProj;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).mWorldViewProjection = mWorldViewProjection;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).mViewProjection =      mViewProjection;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).mInvView =             mInvView;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).vMeshColor =           vWhite;
//    // Min tessellation factor is half the selected edge tessellation factor
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).vTessellationFactor =             
//        Vector4( g_fTessellationFactorEdges, g_fTessellationFactorInside, g_fTessellationFactorEdges / 2.0f, 0.0f );  
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).vDetailTessellationHeightScale =  
//        g_bDisplacementEnabled ? Vector4( DetailTessellationTextures[g_nCurrentTexture].fHeightScale, 0.0f, 0.0f, 0.0f ) : vWhite;
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).vGridSize =                       
//        Vector4( GRID_SIZE, GRID_SIZE, 1.0f / GRID_SIZE, 1.0f / GRID_SIZE );
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).vDebugColorMultiply=   Vector4( g_vDebugColorMultiply, 1.0f );
//    ((CONSTANT_BUFFER_STRUCT *)MappedSubResource.pData).vDebugColorAdd =       Vector4( g_vDebugColorAdd, 0.0f );
//    context.Unmap( g_pMainCB, 0 );

//    // Update material constant buffer
//    Vector4 g_colorMtrlAmbient( 0.35f, 0.35f, 0.35f, 0 );
//    Vector4 g_colorMtrlDiffuse( 1.0f, 1.0f, 1.0f, 1.0f );
//    Vector4 g_colorMtrlSpecular( 1.0f, 1.0f, 1.0f, 1.0f );
//    float       g_fSpecularExponent( 60.0f );
//    hr = context.Map( g_pMaterialCB, 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedSubResource );
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_materialAmbientColor =    g_colorMtrlAmbient;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_materialDiffuseColor =    g_colorMtrlDiffuse;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_materialSpecularColor =   g_colorMtrlSpecular;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_fSpecularExponent =       Vector4( g_fSpecularExponent, 0.0f, 0.0f, 0.0f );
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_LightPosition =           g_LightPosition;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_LightDiffuse =            vWhite;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_LightAmbient =            vWhite;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_vEye =                    Vector4( vFrom, 0.0f );
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_fBaseTextureRepeat =      
//        Vector4( DetailTessellationTextures[g_nCurrentTexture].fBaseTextureRepeat, 0.0f, 0.0f, 0.0f );
//    // POM height scale is in texcoord per world space unit thus: (heightscale * basetexturerepeat) / (texture size in world space units)
//    float fPomHeightScale = 
//        ( DetailTessellationTextures[g_nCurrentTexture].fHeightScale*DetailTessellationTextures[g_nCurrentTexture].fBaseTextureRepeat ) / GRID_SIZE;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_fPOMHeightMapScale =      Vector4(fPomHeightScale , 0.0f, 0.0f, 0.0f );
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_fShadowSoftening =        Vector4( 0.58f, 0.0f, 0.0f, 0.0f );
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_nMinSamples =             8;
//    ((MATERIAL_CB_STRUCT *)MappedSubResource.pData).g_nMaxSamples =             50;
//    context.Unmap(g_pMaterialCB, 0);

//    // Bind the constant buffers to the device for all stages
//    pBuffers[0] = g_pMainCB;
//    pBuffers[1] = g_pMaterialCB;
//    context.VertexShader.SetConstantBuffers( 0, 2, pBuffers );
//    context.GeometryShader . SetConstantBuffers( 0, 2, pBuffers );
//    context.HullShader.SetConstantBuffers( 0, 2, pBuffers );
//    context.DomainShader .SetConstantBuffers( 0, 2, pBuffers );
//    context.PixelShader .SetConstantBuffers( 0, 2, pBuffers );

//    // Set sampler states
//    pSS[0] = g_pSamplerStateLinear;
//    context.VSSetSamplers(0, 1, pSS);
//    context.HSSetSamplers(0, 1, pSS);
//    context.DSSetSamplers(0, 1, pSS);
//    context.PSSetSamplers(0, 1, pSS);

//    // Set states
//    context.OutputMerger.SetBlendState( g_pBlendStateNoBlend, 0, 0xffffffff );
//    context.Rasterizer.SetState( g_bEnableWireFrame ? g_pRasterizerStateWireframe : g_pRasterizerStateSolid );
//    context.OutputMerger.SetDepthStencilState( g_pDepthStencilState, 0 );

//    // Clear the render target and depth stencil
//    float ClearColor[4] = { 0.05f, 0.05f, 0.05f, 1.0f };
//    ID3D11RenderTargetView* pRTV = DXUTGetD3D11RenderTargetView();
//    context.ClearRenderTargetView( pRTV, ClearColor );
//    ID3D11DepthStencilView* pDSV = DXUTGetD3D11DepthStencilView();
//    context.ClearDepthStencilView( pDSV, D3D11_CLEAR_DEPTH, 1.0f, 0 );

//    // Set vertex buffer
//    UINT stride = sizeof(TANGENTSPACEVERTEX);
//    UINT offset = 0;
//    context.IASetVertexBuffers( 0, 1, &g_pGridTangentSpaceVB, &stride, &offset );

//    // Set index buffer
//    context.IASetIndexBuffer( g_pGridTangentSpaceIB, DXGI_FORMAT_R16_UINT, 0 );

//    // Set input layout
//    context.IASetInputLayout( g_pTangentSpaceVertexLayout );

//    // Set shaders and geometries
//    switch (g_nTessellationMethod)
//    {
//        case TESSELLATIONMETHOD_DISABLED:
//        {
//            // Render grid with simple bump mapping applied            
//            context.VSSetShader( g_pNoTessellationVS, null, 0 );
//            context.HSSetShader( null, null, 0);
//            context.DSSetShader( null, null, 0);
//            context.GSSetShader( null, null, 0 );
//            context.PSSetShader( g_pBumpMapPS, null, 0 ); 

//            // Set primitive topology
//            context.IASetPrimitiveTopology( D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST );

//            // Set shader resources
//            pSRV[0] = g_pDetailTessellationBaseTextureRV[g_nCurrentTexture];
//            pSRV[1] = g_pDetailTessellationHeightTextureRV[g_nCurrentTexture];
//            context.VSSetShaderResources( 0, 2, pSRV );
//            context.PSSetShaderResources( 0, 2, pSRV );
//        }
//        break;

//        case TESSELLATIONMETHOD_DISABLED_POM:
//        {
//            // Render grid with POM applied                
//            context.VSSetShader( g_pPOMVS, null, 0 );
//            context.HSSetShader( null, null, 0);
//            context.DSSetShader( null, null, 0);
//            context.GSSetShader( null, null, 0 );
//            context.PSSetShader( g_pPOMPS, null, 0 ); 

//            // Set primitive topology
//            context.IASetPrimitiveTopology( D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST );

//            // Set shader resources
//            pSRV[0] = g_pDetailTessellationBaseTextureRV[g_nCurrentTexture];
//            pSRV[1] = g_pDetailTessellationHeightTextureRV[g_nCurrentTexture];
//            context.PSSetShaderResources( 0, 2, pSRV );
//        }
//        break;

//        case TESSELLATIONMETHOD_DETAIL_TESSELLATION:
//        {
//            // Render grid with detail tessellation
//            context.VSSetShader( g_pDetailTessellationVS, null, 0 );
//            context.HSSetShader( g_pDetailTessellationHS, null, 0);
//            context.DSSetShader( g_pDetailTessellationDS, null, 0);
//            context.GSSetShader( null, null, 0 );
//            context.PSSetShader( g_pBumpMapPS, null, 0 ); 

//            // Set primitive topology
//            context.IASetPrimitiveTopology( D3D11_PRIMITIVE_TOPOLOGY_3_CONTROL_POINT_PATCHLIST );

//            // Set shader resources
//            pSRV[0] = g_pDetailTessellationBaseTextureRV[g_nCurrentTexture];
//            pSRV[1] = g_pDetailTessellationHeightTextureRV[g_nCurrentTexture];
//            pSRV[2] = g_pDetailTessellationDensityTextureRV[g_nCurrentTexture];
//            pSRV[3] = g_pGridDensityVBSRV[g_nCurrentTexture];
//            context.PSSetShaderResources( 0, 3, pSRV );
//            context.DSSetShaderResources( 0, 3, pSRV );
//            context.VSSetShaderResources( 0, 4, pSRV );

//            pSRV[0] = g_pGridDensityVBSRV[g_nCurrentTexture];
//            context.HSSetShaderResources( 0, 1, pSRV );
//        }
//        break;
//    }

//    // Draw grid
//    context.DrawIndexed( 3*2*g_dwGridTessellation*g_dwGridTessellation, 0, 0 );

//    // Draw light source if requested
//    if ( g_bDrawLightSource )
//    {
//        // Set shaders
//        context.VSSetShader( g_pParticleVS, null, 0 );
//        context.HSSetShader( null, null, 0);
//        context.DSSetShader( null, null, 0);
//        context.GSSetShader( g_pParticleGS, null, 0 );
//        context.PSSetShader( g_pParticlePS, null, 0 ); 

//        // Set primitive topology
//        context.IASetPrimitiveTopology( D3D11_PRIMITIVE_TOPOLOGY_POINTLIST );

//        // Set shader resources
//        pSRV[0] = g_pLightTextureRV;
//        context.PSSetShaderResources( 0, 1, pSRV );

//        // Store new light position into particle's VB
//        D3D11_MAPPED_SUBRESOURCE MappedSubresource;
//        context.Map( g_pParticleVB, 0, D3D11_MAP_WRITE_DISCARD, 0, &MappedSubresource );
//        *(D3DXVECTOR3*)MappedSubresource.pData = (D3DXVECTOR3)g_LightPosition;
//        context.Unmap( g_pParticleVB, 0 );

//        // Set vertex buffer
//        UINT stride = sizeof(D3DXVECTOR3);
//        UINT offset = 0;
//        context.IASetVertexBuffers( 0, 1, &g_pParticleVB, &stride, &offset );

//        // Set input layout
//        context.IASetInputLayout( g_pPositionOnlyVertexLayout );

//        // Additive blending
//        context.OMSetBlendState( g_pBlendStateAdditiveBlending, 0, 0xffffffff );

//        // Solid rendering (not affected by global wireframe toggle)
//        context.RSSetState( g_pRasterizerStateSolid );

//        // Draw light
//        context.Draw( 1, 0 );
//    }

//    // Render the HUD
//    if ( g_nRenderHUD > 0 )
//    {
//        DXUT_BeginPerfEvent( DXUT_PERFEVENTCOLOR, "HUD / Stats" );
//        if ( g_nRenderHUD > 1 )
//        {
//            g_HUD.OnRender( fElapsedTime );
//            g_SampleUI.OnRender( fElapsedTime );
//        }
//        RenderText();
//        DXUT_EndPerfEvent();
//    }

//    // Check if current frame needs to be dumped to disk
//    if ( s_dwFrameNumber == g_dwFrameNumberToDump )
//    {
//        // Retrieve resource for current render target
//        ID3D11Resource *pRTResource;
//        DXUTGetD3D11RenderTargetView().GetResource( &pRTResource );

//        // Retrieve a Texture2D interface from resource
//        ID3D11Texture2D* RTTexture;
//        hr = pRTResource.QueryInterface( __uuidof( ID3D11Texture2D ), ( LPVOID* )&RTTexture );

//        // Check if RT is multisampled or not
//        D3D11_TEXTURE2D_DESC TexDesc;
//        RTTexture.GetDesc( &TexDesc );
//        if ( TexDesc.SampleDesc.Count > 1 )
//        {
//            // RT is multisampled: need resolving before dumping to disk

//            // Create single-sample RT of the same type and dimensions
//            DXGI_SAMPLE_DESC SingleSample = { 1, 0 };
//            TexDesc.CPUAccessFlags =    D3D11_CPU_ACCESS_READ;
//            TexDesc.MipLevels =         1;
//            TexDesc.Usage =             D3D11_USAGE_DEFAULT;
//            TexDesc.CPUAccessFlags =    0;
//            TexDesc.BindFlags =         0;
//            TexDesc.SampleDesc =        SingleSample;

//            // Create single-sample texture
//            ID3D11Texture2D *pSingleSampleTexture;
//            hr = device.CreateTexture2D( &TexDesc, null, &pSingleSampleTexture );

//            // Resolve multisampled render target into single-sample texture
//            context.ResolveSubresource( pSingleSampleTexture, 0, RTTexture, 0, TexDesc.Format );

//            // Save texture to disk
//            hr = D3DX11SaveTextureToFile( context, pSingleSampleTexture, D3DX11_IFF_BMP, "RTOutput.bmp" );

//            // Release single-sample version of texture
//            pSingleSampleTexture.Dispose();
            
//        }
//        else
//        {
//            // Single sample case
//            hr = D3DX11SaveTextureToFile( context, RTTexture, D3DX11_IFF_BMP, "RTOutput.bmp" );
//        }

//        // Release texture and resource
//        RTTexture.Dispose();
//        pRTResource.Dispose();
//    }

//    // Increase frame number
//    s_dwFrameNumber++;
//}


////--------------------------------------------------------------------------------------
//// Release D3D11 resources created in OnD3D11ResizedSwapChain 
////--------------------------------------------------------------------------------------
//void CALLBACK OnD3D11ReleasingSwapChain( void* pUserContext )
//{
//    g_DialogResourceManager.OnD3D11ReleasingSwapChain();
//}


////--------------------------------------------------------------------------------------
//// Release D3D11 resources created in OnD3D11CreateDevice 
////--------------------------------------------------------------------------------------
//void CALLBACK OnD3D11DestroyDevice( void* pUserContext )
//{
//    g_DialogResourceManager.OnD3D11DestroyDevice();
//    g_D3DSettingsDlg.OnD3D11DestroyDevice();
//    g_LightControl.StaticOnD3D11DestroyDevice();
//    DXUTGetGlobalResourceCache().OnDestroyDevice();
//    SAFE_DELETE( g_pTxtHelper );

//    g_pLightTextureRV.Dispose();
//    for ( int i=0; i<(int)g_dwNumTextures; ++i )
//    {
//        g_pGridDensityVBSRV[i].Dispose();
//        g_pGridDensityVB[i].Dispose();
//        g_pDetailTessellationDensityTextureRV[i].Dispose();
//        g_pDetailTessellationHeightTextureRV[i].Dispose();
//        g_pDetailTessellationBaseTextureRV[i].Dispose();
//    }
//    g_pParticleVB.Dispose();
//    g_pGridTangentSpaceIB.Dispose();
//    g_pGridTangentSpaceVB.Dispose();
    
//    g_pPositionOnlyVertexLayout.Dispose();
//    g_pTangentSpaceVertexLayout.Dispose();

//    g_pParticlePS.Dispose();
//    g_pParticleGS.Dispose();
//    g_pParticleVS.Dispose();
//    g_pBumpMapPS.Dispose();
//    g_pDetailTessellationDS.Dispose();
//    g_pDetailTessellationHS.Dispose();
//    g_pDetailTessellationVS.Dispose();
//    g_pNoTessellationVS.Dispose();

//    g_pPOMPS.Dispose();
//    g_pPOMVS.Dispose();

//    g_pMaterialCB.Dispose();
//    g_pMainCB.Dispose();

//    g_pDepthStencilState.Dispose();
//    g_pBlendStateAdditiveBlending.Dispose();
//    g_pBlendStateNoBlend.Dispose();
//    g_pRasterizerStateSolid.Dispose();
//    g_pRasterizerStateWireframe.Dispose();
//    g_pSamplerStateLinear.Dispose();
//}









////--------------------------------------------------------------------------------------
//// Sample a 32-bit texture at the specified texture coordinate (point sampling only)
////--------------------------------------------------------------------------------------
//__inline RGBQUAD SampleTexture2D_32bit( DWORD *pTexture2D, DWORD dwWidth, DWORD dwHeight, D3DXVECTOR2 fTexcoord, DWORD dwStride )
//{
//#define FROUND(x)    ( (int)( (x) + 0.5 ) )

//    // Convert texture coordinates to texel coordinates using round-to-nearest
//    int nU = FROUND( fTexcoord.X * ( dwWidth-1 ) );
//    int nV = FROUND( fTexcoord.Y * ( dwHeight-1 ) );

//    // Clamp texcoord between 0 and [width-1, height-1]
//    nU = nU % dwWidth;
//    nV = nV % dwHeight;

//    // Return value at this texel coordinate
//    return *(RGBQUAD *)( ( (BYTE *)pTexture2D) + ( nV*dwStride ) + ( nU*sizeof(DWORD) ) );
//}


////--------------------------------------------------------------------------------------
//// Sample density map along specified edge and return maximum value
////--------------------------------------------------------------------------------------
//float GetMaximumDensityAlongEdge( DWORD *pTexture2D, DWORD dwStride, DWORD dwWidth, DWORD dwHeight, D3DXVECTOR2 fTexcoord0, D3DXVECTOR2 fTexcoord1 )
//{
//#define GETTEXEL(x, y)       ( *(RGBQUAD *)( ( (BYTE *)pTexture2D ) + ( (y)*dwStride ) + ( (x)*sizeof(DWORD) ) ) )
//#define LERP(x, y, a)        ( (x)*(1.0f-(a)) + (y)*(a) )

//    // Convert texture coordinates to texel coordinates using round-to-nearest
//    int nU0 = FROUND( fTexcoord0.X * ( dwWidth  - 1 )  );
//    int nV0 = FROUND( fTexcoord0.Y * ( dwHeight - 1 ) );
//    int nU1 = FROUND( fTexcoord1.X * ( dwWidth  - 1 )  );
//    int nV1 = FROUND( fTexcoord1.Y * ( dwHeight - 1 ) );

//    // Wrap texture coordinates
//    nU0 = nU0 % dwWidth;
//    nV0 = nV0 % dwHeight;
//    nU1 = nU1 % dwWidth;
//    nV1 = nV1 % dwHeight;

//    // Find how many texels on edge
//    int nNumTexelsOnEdge = max( abs( nU1 - nU0 ), abs( nV1 - nV0 ) ) + 1;

//    float fU, fV;
//    float fMaxDensity = 0.0f;
//    for ( int i=0; i<nNumTexelsOnEdge; ++i )
//    {
//        // Calculate current texel coordinates
//        fU = LERP( (float)nU0, (float)nU1, ( (float)i ) / ( nNumTexelsOnEdge - 1 ) );
//        fV = LERP( (float)nV0, (float)nV1, ( (float)i ) / ( nNumTexelsOnEdge - 1 ) );

//        // Round texel texture coordinates to nearest
//        int nCurrentU = FROUND( fU );
//        int nCurrentV = FROUND( fV );

//        // Update max density along edge
//        fMaxDensity = max( fMaxDensity, GETTEXEL( nCurrentU, nCurrentV ).rgbBlue / 255.0f );
//    }
        
//    return fMaxDensity;
//}


////--------------------------------------------------------------------------------------
//// Calculate the maximum density along a triangle edge and
//// store it in the resulting vertex stream.
////--------------------------------------------------------------------------------------
//void CreateEdgeDensityVertexStream( Device pd3dDevice, DeviceContext pDeviceContext, D3DXVECTOR2* pTexcoord, 
//                                    DWORD dwVertexStride, void *pIndex, DWORD dwIndexSize, DWORD dwNumIndices, 
//                                    ID3D11Texture2D* pDensityMap, Buffer* ppEdgeDensityVertexStream,
//                                    float fBaseTextureRepeat )
//{
//    HRESULT                hr;
//    D3D11_SUBRESOURCE_DATA InitData;
//    ID3D11Texture2D*       pDensityMapReadFrom;
//    DWORD                  dwNumTriangles = dwNumIndices / 3;

//    // Create host memory buffer
//    Vector4 *pEdgeDensityBuffer = new Vector4[dwNumTriangles];

//    // Retrieve density map description
//    D3D11_TEXTURE2D_DESC Tex2DDesc;
//    pDensityMap.GetDesc( &Tex2DDesc );

//    // Check if provided texture can be Mapped() for reading directly
//    BOOL bCanBeRead = Tex2DDesc.CPUAccessFlags & D3D11_CPU_ACCESS_READ;
//    if ( !bCanBeRead )
//    {
//        // Texture cannot be read directly, need to create STAGING texture and copy contents into it
//        Tex2DDesc.CPUAccessFlags =   D3D11_CPU_ACCESS_READ;
//        Tex2DDesc.Usage =            D3D11_USAGE_STAGING;
//        Tex2DDesc.BindFlags =        0;
//        pd3dDevice.CreateTexture2D( &Tex2DDesc, null, &pDensityMapReadFrom );

//        // Copy contents of height map into staging version
//        pDeviceContext.CopyResource( pDensityMapReadFrom, pDensityMap );
//    }
//    else
//    {
//        pDensityMapReadFrom = pDensityMap;
//    }

//    // Map density map for reading
//    D3D11_MAPPED_SUBRESOURCE MappedSubResource;
//    pDeviceContext.Map( pDensityMapReadFrom, 0, D3D11_MAP_READ, 0, &MappedSubResource );

//    // Loop through all triangles
//    for ( DWORD i=0; i<dwNumTriangles; ++i )
//    {
//        DWORD wIndex0, wIndex1, wIndex2;

//        // Retrieve indices of current triangle
//        if ( dwIndexSize == sizeof(WORD) )
//        {
//            wIndex0 = (DWORD)( (WORD *)pIndex )[3*i+0];
//            wIndex1 = (DWORD)( (WORD *)pIndex )[3*i+1];
//            wIndex2 = (DWORD)( (WORD *)pIndex )[3*i+2];
//        }
//        else
//        {
//            wIndex0 = ( (DWORD *)pIndex )[3*i+0];
//            wIndex1 = ( (DWORD *)pIndex )[3*i+1];
//            wIndex2 = ( (DWORD *)pIndex )[3*i+2];
//        }

//        // Retrieve texture coordinates of each vertex making up current triangle
//        D3DXVECTOR2    vTexcoord0 = *(D3DXVECTOR2 *)( (BYTE *)pTexcoord + wIndex0 * dwVertexStride );
//        D3DXVECTOR2    vTexcoord1 = *(D3DXVECTOR2 *)( (BYTE *)pTexcoord + wIndex1 * dwVertexStride );
//        D3DXVECTOR2    vTexcoord2 = *(D3DXVECTOR2 *)( (BYTE *)pTexcoord + wIndex2 * dwVertexStride );

//        // Adjust texture coordinates with texture repeat
//        vTexcoord0 *= fBaseTextureRepeat;
//        vTexcoord1 *= fBaseTextureRepeat;
//        vTexcoord2 *= fBaseTextureRepeat;

//        // Sample density map at vertex location
//        float fEdgeDensity0 = GetMaximumDensityAlongEdge( (DWORD *)MappedSubResource.pData, MappedSubResource.RowPitch, 
//                                                          Tex2DDesc.Width, Tex2DDesc.Height, vTexcoord0, vTexcoord1 );
//        float fEdgeDensity1 = GetMaximumDensityAlongEdge( (DWORD *)MappedSubResource.pData, MappedSubResource.RowPitch, 
//                                                          Tex2DDesc.Width, Tex2DDesc.Height, vTexcoord1, vTexcoord2 );
//        float fEdgeDensity2 = GetMaximumDensityAlongEdge( (DWORD *)MappedSubResource.pData, MappedSubResource.RowPitch, 
//                                                          Tex2DDesc.Width, Tex2DDesc.Height, vTexcoord2, vTexcoord0 );

//        // Store edge density in x,y,z and store maximum density in .w
//        pEdgeDensityBuffer[i] = Vector4( fEdgeDensity0, fEdgeDensity1, fEdgeDensity2, 
//                                             max( max( fEdgeDensity0, fEdgeDensity1 ), fEdgeDensity2 ) );
//    }

//    // Unmap density map
//    pDeviceContext.Unmap( pDensityMapReadFrom, 0 );

//    // Release staging density map if we had to create it
//    if ( !bCanBeRead )
//    {
//        pDensityMapReadFrom.Dispose();
//    }
    
//    // Set source buffer for initialization data
//    InitData.pSysMem = (void *)pEdgeDensityBuffer;

//    // Create density vertex stream buffer: 1 float per entry
//    D3D11_BUFFER_DESC bd;
//    bd.Usage =            D3D11_USAGE_DEFAULT;
//    bd.ByteWidth =        dwNumTriangles * sizeof( Vector4 );
//    bd.BindFlags =        D3D11_BIND_VERTEX_BUFFER | D3D11_BIND_SHADER_RESOURCE;
//    bd.CPUAccessFlags =   0;
//    bd.MiscFlags =        0;
//    hr = pd3dDevice.CreateBuffer( &bd, &InitData, ppEdgeDensityVertexStream );
//    if( FAILED( hr ) )
//    {
//        OutputDebugString( "Failed to create vertex buffer.\n" );
//    }

//    // Free host memory buffer
//    delete [] pEdgeDensityBuffer;
//}


////--------------------------------------------------------------------------------------
//// Helper function to create a staging buffer from a buffer resource
////--------------------------------------------------------------------------------------
//void CreateStagingBufferFromBuffer( Device pd3dDevice, DeviceContext pContext, 
//                                   Buffer pInputBuffer, ID3D11Buffer **ppStagingBuffer )
//{
//    D3D11_BUFFER_DESC BufferDesc;

//    // Get buffer description
//    pInputBuffer.GetDesc( &BufferDesc );

//    // Modify description to create STAGING buffer
//    BufferDesc.BindFlags =          0;
//    BufferDesc.CPUAccessFlags =     D3D11_CPU_ACCESS_WRITE | D3D11_CPU_ACCESS_READ;
//    BufferDesc.Usage =              D3D11_USAGE_STAGING;

//    // Create staging buffer
//    pd3dDevice.CreateBuffer( &BufferDesc, null, ppStagingBuffer );

//    // Copy buffer into STAGING buffer
//    pContext.CopyResource( *ppStagingBuffer, pInputBuffer );
//}


////--------------------------------------------------------------------------------------
//// Helper function to create a shader from the specified filename
//// This function is called by the shader-specific versions of this
//// function located after the body of this function.
////--------------------------------------------------------------------------------------
//HRESULT CreateShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                              Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                              ID3DX11ThreadPump* pPump, ID3D11DeviceChild** ppShader, ID3D10Blob** ppShaderBlob, 
//                              BOOL bDumpShader)
//{
//    HRESULT     hr = D3D_OK;
//    ID3D10Blob* pShaderBlob = null;
//    ID3D10Blob* pErrorBlob = null;

//    // Dump HLSL shader to disk if requested


//    WCHAR wcFullPath[256];
//    DXUTFindDXSDKMediaFileCch( wcFullPath, 256, pSrcFile );
//    // Compile shader into binary blob
//    hr = D3DX11CompileFromFile( wcFullPath, pDefines, pInclude, pFunctionName, pProfile, 
//                                Flags1, Flags2, pPump, &pShaderBlob, &pErrorBlob, null );
//    if( FAILED( hr ) )
//    {
//        OutputDebugStringA( (char*)pErrorBlob.GetBufferPointer() );
//        pErrorBlob.Dispose();
//        return hr;
//    }
    
//    // Create shader from binary blob
//    if ( ppShader )
//    {
//        hr = E_FAIL;
//        if ( strstr( pProfile, "vs" ) )
//        {
//            hr = pd3dDevice.CreateVertexShader( pShaderBlob.GetBufferPointer(), 
//                    pShaderBlob.GetBufferSize(), null, (VertexShader*)ppShader );
//        }
//        else if ( strstr( pProfile, "hs" ) )
//        {
//            hr = pd3dDevice.CreateHullShader( pShaderBlob.GetBufferPointer(), 
//                    pShaderBlob.GetBufferSize(), null, (HullShader*)ppShader ); 
//        }
//        else if ( strstr( pProfile, "ds" ) )
//        {
//            hr = pd3dDevice.CreateDomainShader( pShaderBlob.GetBufferPointer(), 
//                    pShaderBlob.GetBufferSize(), null, (DomainShader*)ppShader );
//        }
//        else if ( strstr( pProfile, "gs" ) )
//        {
//            hr = pd3dDevice.CreateGeometryShader( pShaderBlob.GetBufferPointer(), 
//                    pShaderBlob.GetBufferSize(), null, (GeometryShader*)ppShader ); 
//        }
//        else if ( strstr( pProfile, "ps" ) )
//        {
//            hr = pd3dDevice.CreatePixelShader( pShaderBlob.GetBufferPointer(), 
//                    pShaderBlob.GetBufferSize(), null, (PixelShader*)ppShader ); 
//        }
//        else if ( strstr( pProfile, "cs" ) )
//        {
//            hr = pd3dDevice.CreateComputeShader( pShaderBlob.GetBufferPointer(), 
//                    pShaderBlob.GetBufferSize(), null, (ID3D11ComputeShader**)ppShader );
//        }
//        if ( FAILED( hr ) )
//        {
//            OutputDebugString( "Shader creation failed\n" );
//            pErrorBlob.Dispose();
//            pShaderBlob.Dispose();
//            return hr;
//        }
//    }

//    // If blob was requested then pass it otherwise release it
//    if ( ppShaderBlob )
//    {
//        *ppShaderBlob = pShaderBlob;
//    }
//    else
//    {
//        pShaderBlob.Release();
//    }

//    // Return error code
//    return hr;
//}


////--------------------------------------------------------------------------------------
//// Create a vertex shader from the specified filename
////--------------------------------------------------------------------------------------
//HRESULT CreateVertexShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                                    Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                                    ID3DX11ThreadPump* pPump, VertexShader* ppShader, ID3D10Blob** ppShaderBlob, 
//                                    BOOL bDumpShader )
//{
//    return CreateShaderFromFile( pd3dDevice, pSrcFile, pDefines, pInclude, pFunctionName, pProfile, 
//                                 Flags1, Flags2, pPump, (ID3D11DeviceChild **)ppShader, ppShaderBlob, 
//                                 bDumpShader );
//}


////--------------------------------------------------------------------------------------
//// Create a hull shader from the specified filename
////--------------------------------------------------------------------------------------
//HRESULT CreateHullShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                                  Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                                  ID3DX11ThreadPump* pPump, HullShader* ppShader, ID3D10Blob** ppShaderBlob, 
//                                  BOOL bDumpShader )
//{
//    return CreateShaderFromFile( pd3dDevice, pSrcFile, pDefines, pInclude, pFunctionName, pProfile, 
//                                 Flags1, Flags2, pPump, (ID3D11DeviceChild **)ppShader, ppShaderBlob, 
//                                 bDumpShader );
//}
////--------------------------------------------------------------------------------------
//// Create a domain shader from the specified filename
////--------------------------------------------------------------------------------------
//HRESULT CreateDomainShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                                    Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                                    ID3DX11ThreadPump* pPump, DomainShader* ppShader, ID3D10Blob** ppShaderBlob, 
//                                    BOOL bDumpShader )
//{
//    return CreateShaderFromFile( pd3dDevice, pSrcFile, pDefines, pInclude, pFunctionName, pProfile, 
//                                 Flags1, Flags2, pPump, (ID3D11DeviceChild **)ppShader, ppShaderBlob, 
//                                 bDumpShader );
//}


////--------------------------------------------------------------------------------------
//// Create a geometry shader from the specified filename
////--------------------------------------------------------------------------------------
//HRESULT CreateGeometryShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                                      Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                                      ID3DX11ThreadPump* pPump, GeometryShader* ppShader, ID3D10Blob** ppShaderBlob, 
//                                      BOOL bDumpShader )
//{
//    return CreateShaderFromFile( pd3dDevice, pSrcFile, pDefines, pInclude, pFunctionName, pProfile, 
//                                 Flags1, Flags2, pPump, (ID3D11DeviceChild **)ppShader, ppShaderBlob, 
//                                 bDumpShader );
//}


////--------------------------------------------------------------------------------------
//// Create a pixel shader from the specified filename
////--------------------------------------------------------------------------------------
//HRESULT CreatePixelShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                                   Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                                   ID3DX11ThreadPump* pPump, PixelShader* ppShader, ID3D10Blob** ppShaderBlob, 
//                                   BOOL bDumpShader )
//{
//    return CreateShaderFromFile( pd3dDevice, pSrcFile, pDefines, pInclude, pFunctionName, pProfile, 
//                                 Flags1, Flags2, pPump, (ID3D11DeviceChild **)ppShader, ppShaderBlob, 
//                                 bDumpShader );
//}


////--------------------------------------------------------------------------------------
//// Create a compute shader from the specified filename
////--------------------------------------------------------------------------------------
//HRESULT CreateComputeShaderFromFile( Device pd3dDevice, string pSrcFile, CONST D3D10_SHADER_MACRO* pDefines, 
//                                     Include pInclude, string pFunctionName, string pProfile, UINT Flags1, UINT Flags2, 
//                                     ID3DX11ThreadPump* pPump, ID3D11ComputeShader** ppShader, ID3D10Blob** ppShaderBlob, 
//                                     BOOL bDumpShader)
//{
//    return CreateShaderFromFile( pd3dDevice, pSrcFile, pDefines, pInclude, pFunctionName, pProfile, 
//                                 Flags1, Flags2, pPump, (ID3D11DeviceChild **)ppShader, ppShaderBlob, 
//                                 bDumpShader );
//}





//}
//    }
