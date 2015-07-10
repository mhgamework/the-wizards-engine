using System;
using DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.DXGI;
using Device = SlimDX.Direct3D11.Device;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.SSAO
{
    /// <summary>
    /// From NVIDIA D3D10 SDK
    /// </summary>
    public class HorizonSSAORenderer
    {
        private readonly DX11Game game;
        private DeviceContext context;
        private BasicShader shader;


        private FullScreenQuad quad;
        private InputLayout layout;
        private RenderTargetView averageLumRTV;
        private ShaderResourceView averageLumRV;


        // Buffer sizes
        // ReSharper disable InconsistentNaming

        int m_BBWidth;
        int m_BBHeight;

        // D3D Device  
        Device m_D3DDevice;
        Viewport m_Viewport;

        // The effects and rendering techniques
        Effect m_Effect;
        EffectTechnique m_Technique_HBAO;
        EffectTechnique[] m_Technique_HBAO_LD = new EffectTechnique[3];
        EffectTechnique[] m_Technique_HBAO_NLD = new EffectTechnique[3];

        // Effect variable pointers
        EffectResourceVariable m_DepthTexVar;
        EffectResourceVariable m_NormalTexVar;
        EffectResourceVariable m_RandTexVar;

        EffectScalarVariable m_pNumSteps;
        EffectScalarVariable m_pNumDirs;
        EffectScalarVariable m_pRadius;
        EffectScalarVariable m_pAngleBias;
        EffectScalarVariable m_pTanAngleBias;
        EffectScalarVariable m_pInvRadius;
        EffectScalarVariable m_pSqrRadius;
        EffectScalarVariable m_pAttenuation;
        EffectScalarVariable m_pContrast;
        EffectScalarVariable m_pAspectRatio;
        EffectScalarVariable m_pInvAspectRatio;
        EffectVectorVariable m_pInvFocalLen;
        EffectVectorVariable m_pFocalLen;
        EffectVectorVariable m_pInvResolution;
        EffectVectorVariable m_pResolution;

        int m_NumSteps;
        int m_NumDirs;
        float m_RadiusMultiplier;
        float m_AngleBias;
        float m_Attenuation;
        float m_Contrast;
        float m_AspectRatio;
        float m_InvAspectRatio;
        private Vector2 m_InvFocalLen;
        private Vector2 m_FocalLen;
        private Vector2 m_InvResolution;
        private Vector2 m_Resolution;
        byte m_QualityMode;

        public int MNumSteps
        {
            get { return m_NumSteps; }
            set { m_NumSteps = value; updateShaderState(); }
        }

        public int MNumDirs
        {
            get { return m_NumDirs; }
            set { m_NumDirs = value; updateShaderState(); }
        }

        public float MRadiusMultiplier
        {
            get { return m_RadiusMultiplier; }
            set { m_RadiusMultiplier = value; updateShaderState(); }
        }

        public float MAngleBias
        {
            get { return m_AngleBias; }
            set { m_AngleBias = value; updateShaderState(); }
        }

        public float MAttenuation
        {
            get { return m_Attenuation; }
            set { m_Attenuation = value; updateShaderState(); }
        }

        public float MContrast
        {
            get { return m_Contrast; }
            set { m_Contrast = value; updateShaderState(); }
        }

        public byte MQualityMode
        {
            get { return m_QualityMode; }
            set { m_QualityMode = value; updateShaderState(); }
        }

        ShaderResourceView m_DepthBuffer;

        Texture2D m_pRndTexture;
        private ShaderResourceView m_pRndTexSRV;
        private float m_AORadius;
        private Random rng;


        public HorizonSSAORenderer(DX11Game game, int width, int height)
        {
            this.game = game;
            var device = game.Device;
            m_D3DDevice = device;
            context = device.ImmediateContext;

            initialize();
            float fovy = MathHelper.PiOver4; //TODO: this is fishy
            OnResizedSwapChain(width, height, fovy);
        }

        public SimpleRT MSsaoBuffer { get; set; }


        void initialize()
        {
            m_BBWidth = 0;
            m_BBHeight = 0;

            m_Effect = null;
            MSsaoBuffer = null;
            m_pRndTexture = null;
            m_pRndTexSRV = null;

            m_RadiusMultiplier = 1.0f;
            m_AngleBias = 30;
            m_NumDirs = 16;
            m_NumSteps = 3;
            m_Contrast = 1.25f;
            m_Attenuation = 1.0f;
            m_QualityMode = 1;
            m_AORadius = 0.001f;
        }

        void OnResizedSwapChain(int width, int height, float fovy)
        {

            // Recalculate buffer sizes
            m_BBWidth = width / 2;
            m_BBHeight = height / 2;


            RecompileShader();
            UpdateDirs();

            // Create the RT
            Texture2DDescription pDesc = new Texture2DDescription();
            pDesc.Width = m_BBWidth;
            pDesc.Height = m_BBHeight;
            pDesc.MipLevels = 1;
            pDesc.ArraySize = 1;
            pDesc.Format = Format.R8G8B8A8_UNorm;
            //pDesc.Format = Format.R8_UNorm;
            pDesc.SampleDescription = new SampleDescription(1, 0);
            pDesc.Usage = ResourceUsage.Default;
            pDesc.BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget;
            pDesc.CpuAccessFlags = CpuAccessFlags.None;
            pDesc.OptionFlags = ResourceOptionFlags.None;

            //SAFE_DELETE(m_SSAOBuffer);
            MSsaoBuffer = new SimpleRT(m_D3DDevice, pDesc);

            // tanf ?? using normal tan (probl tangent float
            m_FocalLen[0] = 1.0f / (float)Math.Tan(fovy * 0.5f) * (float)m_BBHeight / (float)m_BBWidth;
            m_FocalLen[1] = 1.0f / (float)Math.Tan(fovy * 0.5f);
            m_InvFocalLen[0] = 1.0f / m_FocalLen[0];
            m_InvFocalLen[1] = 1.0f / m_FocalLen[1];
            m_InvResolution[0] = 1.0f / m_BBWidth;
            m_InvResolution[1] = 1.0f / m_BBHeight;
            m_Resolution[0] = (float)m_BBWidth;
            m_Resolution[1] = (float)m_BBHeight;

            m_pFocalLen.Set(m_FocalLen);
            m_pInvFocalLen.Set(m_InvFocalLen);
            m_pInvResolution.Set(m_InvResolution);
            m_pResolution.Set(m_Resolution);

            m_Viewport.X = 0;
            m_Viewport.Y = 0;
            m_Viewport.MinZ = 0;
            m_Viewport.MaxZ = 1;
            m_Viewport.Width = m_BBWidth;
            m_Viewport.Height = m_BBHeight;

        }

        public class SimpleRT
        {
            public Texture2D Texture { get; private set; }
            public SimpleRT(Device device, Texture2DDescription desc)
            {
                Texture = new Texture2D(device, desc);
                pRTV = new RenderTargetView(device, Texture);
                pSRV = new ShaderResourceView(device, Texture);
            }

            public ShaderResourceView pSRV { get; private set; }

            public RenderTargetView pRTV { get; private set; }
        }

        void OnReleasingSwapChain()
        {
            m_BBWidth = 0;
            m_BBHeight = 0;

            //SAFE_DELETE(m_SSAOBuffer);
        }

        void OnDestroyDevice()
        {
            //SAFE_RELEASE(m_Effect);
            //SAFE_RELEASE(m_pRndTexSRV);
            //SAFE_RELEASE(m_pRndTexture);
            //SAFE_RELEASE(m_Effect);

        }

        void UpdateRadius()
        {
            float R = m_RadiusMultiplier * m_AORadius;
            m_pRadius.Set(R);
            m_pInvRadius.Set(1.0f / R);
            m_pSqrRadius.Set(R * R);
        }

        void UpdateAngleBias()
        {
            float angle = m_AngleBias * MathHelper.Pi / 180;
            m_pAngleBias.Set(angle);
            // Was: tan
            m_pTanAngleBias.Set((float)Math.Tan(angle));
            UpdateContrast();
        }

        void UpdateContrast()
        {
            float contrast = m_Contrast / (1.0f - (float)Math.Sin(m_AngleBias * MathHelper.Pi / 180));
            m_pContrast.Set(contrast);
        }

        void OnFrameRender(ShaderResourceView Depth)
        {
            OnFrameRender(Depth, null);

        }
        public void OnFrameRender(ShaderResourceView Depth, ShaderResourceView Normal)
        {

            UpdateRadius();

            if (Normal != null)
                m_Technique_HBAO = m_Technique_HBAO_NLD[m_QualityMode];
            else
                m_Technique_HBAO = m_Technique_HBAO_LD[m_QualityMode];

            m_D3DDevice.ImmediateContext.Rasterizer.SetViewports(m_Viewport);
            m_D3DDevice.ImmediateContext.OutputMerger.SetTargets(MSsaoBuffer.pRTV);

            //V( m_DepthTexVar.SetResource  ( Depth ) );
            //V( m_NormalTexVar.SetResource ( Normal ) );
            //V( m_RandTexVar.SetResource   ( m_pRndTexSRV ) );

            m_DepthTexVar.SetResource(Depth);
            m_NormalTexVar.SetResource(Normal);
            m_RandTexVar.SetResource(m_pRndTexSRV);
            m_Effect.GetVariableByName("InvertProjection").AsMatrix().SetMatrix(Matrix.Invert(game.Camera.Projection));

            m_Technique_HBAO.GetPassByIndex(0).Apply(m_D3DDevice.ImmediateContext);

            //TODO: What the?? where is the vertex buffer???

            m_D3DDevice.ImmediateContext.Draw(3, 0);

            m_DepthTexVar.SetResource(null);
            m_NormalTexVar.SetResource(null);
            m_RandTexVar.SetResource(null);

            m_Technique_HBAO.GetPassByIndex(0).Apply(m_D3DDevice.ImmediateContext);

        }


        private int SCALE = (1 << 15);

        void UpdateDirs()
        {
            rng = new Random(0);

            m_pNumDirs.Set((float)m_NumDirs);

            if (m_pRndTexSRV != null)
                m_pRndTexSRV.Dispose();
            if (m_pRndTexture != null)
                m_pRndTexture.Dispose();

            short[] f = new short[64 * 64 * 4];
            for (int i = 0; i < 64 * 64 * 4; i += 4)
            {
                // Was rng.randExc()
                float angle = 2.0f * MathHelper.Pi * (float)rng.NextDouble() / (float)m_NumDirs;
                f[i] = (short)(SCALE * Math.Cos(angle));
                f[i + 1] = (short)(SCALE * Math.Sin(angle));
                f[i + 2] = (short)(SCALE * (float)rng.NextDouble());
                f[i + 3] = 0;
            }

            Texture2DDescription tex_desc = new Texture2DDescription();
            tex_desc.Width = 64;
            tex_desc.Height = 64;
            tex_desc.MipLevels = 1;
            tex_desc.ArraySize = 1;
            tex_desc.Format = Format.R16G16B16A16_SNorm;
            tex_desc.SampleDescription = new SampleDescription(1, 0);
            tex_desc.Usage = ResourceUsage.Immutable;
            tex_desc.BindFlags = BindFlags.ShaderResource;
            tex_desc.CpuAccessFlags = CpuAccessFlags.None;
            tex_desc.OptionFlags = ResourceOptionFlags.None;

            //D3D10_SUBRESOURCE_DATA sr_desc;
            //sr_desc.pSysMem = f;
            //sr_desc.SysMemPitch = 64 * 4 * sizeof(short);
            //sr_desc.SysMemSlicePitch = 0;

            ShaderResourceViewDescription srv_desc = new ShaderResourceViewDescription();
            srv_desc.Format = tex_desc.Format;
            srv_desc.Dimension = ShaderResourceViewDimension.Texture2D;
            srv_desc.MostDetailedMip = 0;
            srv_desc.MipLevels = 1;

            using (var strm = new DataStream(f, true, false))
                m_pRndTexture = new Texture2D(m_D3DDevice, tex_desc, new DataRectangle(64 * 4 * sizeof(short), strm));
            m_pRndTexSRV = new ShaderResourceView(m_D3DDevice, m_pRndTexture, srv_desc);
        }

        void Dispose()
        {
            //SAFE_RELEASE(m_pRndTexture);
            //SAFE_RELEASE(m_pRndTexSRV);
        }

        void RecompileShader()
        {


            var filename = CompiledShaderCache.Current.RootShaderPath + "SSAO\\HorizonSSAO.fx";


            shader = BasicShader.LoadAutoreload(game, new System.IO.FileInfo(filename), loadShaderVariables);


        }

        private void loadShaderVariables(BasicShader shader)
        {
            m_Effect = shader.Effect;


            // Obtain the technique
            m_Technique_HBAO_NLD[0] = m_Effect.GetTechniqueByName("HORIZON_BASED_AO_NLD_LOWQUALITY_Pass");
            m_Technique_HBAO_LD[0] = m_Effect.GetTechniqueByName("HORIZON_BASED_AO_LD_LOWQUALITY_Pass");
            m_Technique_HBAO_NLD[1] = m_Effect.GetTechniqueByName("HORIZON_BASED_AO_NLD_Pass");
            m_Technique_HBAO_LD[1] = m_Effect.GetTechniqueByName("HORIZON_BASED_AO_LD_Pass");
            m_Technique_HBAO_NLD[2] = m_Effect.GetTechniqueByName("HORIZON_BASED_AO_NLD_QUALITY_Pass");
            m_Technique_HBAO_LD[2] = m_Effect.GetTechniqueByName("HORIZON_BASED_AO_LD_QUALITY_Pass");

            // Obtain pointers to the shader variales
            m_DepthTexVar = m_Effect.GetVariableByName("tLinDepth").AsResource();
            m_NormalTexVar = m_Effect.GetVariableByName("tNormal").AsResource();
            m_RandTexVar = m_Effect.GetVariableByName("tRandom").AsResource();

            m_pNumSteps = m_Effect.GetVariableByName("g_NumSteps").AsScalar();
            m_pNumDirs = m_Effect.GetVariableByName("g_NumDir").AsScalar();
            m_pRadius = m_Effect.GetVariableByName("g_R").AsScalar();
            m_pInvRadius = m_Effect.GetVariableByName("g_inv_R").AsScalar();
            m_pSqrRadius = m_Effect.GetVariableByName("g_sqr_R").AsScalar();
            m_pAngleBias = m_Effect.GetVariableByName("g_AngleBias").AsScalar();
            m_pTanAngleBias = m_Effect.GetVariableByName("g_TanAngleBias").AsScalar();
            m_pAttenuation = m_Effect.GetVariableByName("g_Attenuation").AsScalar();
            m_pContrast = m_Effect.GetVariableByName("g_Contrast").AsScalar();
            m_pFocalLen = m_Effect.GetVariableByName("g_FocalLen").AsVector();
            m_pInvFocalLen = m_Effect.GetVariableByName("g_InvFocalLen").AsVector();
            m_pInvResolution = m_Effect.GetVariableByName("g_InvResolution").AsVector();
            m_pResolution = m_Effect.GetVariableByName("g_Resolution").AsVector();

            // Set Defaults
            updateShaderState();
        }

        private void updateShaderState()
        {
            UpdateDirs();
            UpdateRadius();
            UpdateAngleBias();

            m_pNumSteps.Set((float)m_NumSteps);
            m_pAttenuation.Set(m_Attenuation);

            m_pFocalLen.Set(m_FocalLen);
            m_pInvFocalLen.Set(m_InvFocalLen);
            m_pInvResolution.Set(m_InvResolution);
            m_pResolution.Set(m_Resolution);
        }
    }
}
