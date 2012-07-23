using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    /// <summary>
    /// TODO: batch calls!
    /// extend this by using Texture2D instead of a resourceview, and cache resourceviews
    /// </summary>
    public class TextureRenderer
    {
        private readonly Device device;
        private EffectPass pass;
        private EffectVectorVariable pOffsetSS;
        private EffectVectorVariable pScale;
        private EffectResourceVariable pTxDiffuse;
        private InputLayout layout;
        private FullScreenQuad quad;
        private DeviceContext context;
        private Effect effect;

        public TextureRenderer(Device device)
        {
            this.device = device;
            context = device.ImmediateContext;
            var bytecode = ShaderBytecode.CompileFromFile(CompiledShaderCache.Current.RootShaderPath+ "TextureRenderer.fx", "fx_5_0",
                                                          ShaderFlags.None,
                                                          EffectFlags.None);

            effect = new Effect(device, bytecode);
            var technique = effect.GetTechniqueByName("TextureRenderer");
            pass = technique.GetPassByIndex(0);
            //TODO: use constant buffers
            pOffsetSS = effect.GetVariableByName("OffsetSS").AsVector();
            pScale = effect.GetVariableByName("Scale").AsVector();
            pTxDiffuse = effect.GetVariableByName("txDiffuse").AsResource();


            quad = new FullScreenQuad(device);

            layout = FullScreenQuad.CreateInputLayout(device, pass);



        }

        //TODO: maybe use tex.Resource.AsSurface().Description.Width, for auto-size rendering
        public void Draw(ShaderResourceView tex, Vector2 pos, Vector2 size)
        {
            
            setQuadTransform(size, pos);
            pTxDiffuse.SetResource(tex);

            pass.Apply(context);
            quad.Draw(layout);

            
            // Unset from GPU
            pTxDiffuse.SetResource(null);

            // This should update the constant buffers and unset the resource. Should be optimized in some way
            pass.Apply(context); 


        }
        public void DrawColor(Color4 color, Vector2 pos, Vector2 size)
        {

            setQuadTransform(size, pos);
            effect.GetVariableByName("Color").AsVector().Set(color);
            effect.GetTechniqueByName("ColorRenderer").GetPassByIndex(0).Apply(context);
            quad.Draw(layout);


  

            // This should update the constant buffers and unset the resource. Should be optimized in some way
            pass.Apply(context);


        }
        private void setQuadTransform(Vector2 size, Vector2 pos)
        {
            var viewport =  context.Rasterizer.GetViewports()[0];
            var scale = new Vector2(size.X/viewport.Width, size.Y/viewport.Height);
            
            // We need to correct for the y-inversion

            var offsetSS = new Vector2(pos.X/viewport.Width*2,  pos.Y/viewport.Height*2);
            

            pOffsetSS.Set(offsetSS);
            pScale.Set(scale);
        }
    }
}
