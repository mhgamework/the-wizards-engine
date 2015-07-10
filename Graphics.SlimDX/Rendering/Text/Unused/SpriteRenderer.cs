using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlimDX;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;
using Buffer = SlimDX.Direct3D11.Buffer;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Text.Unused
{
    /// <summary>
    /// Specifies, how coordinates are interpreted.
    /// </summary>
    /// <remarks>
    /// <para>Sprites (and with that text) can be drawn in several coordinate systems. The user can choose, which system
    /// fits his needs best. There are basically two types of coordinate system:</para>
    /// <para><b>Type 1 systems</b><br/>
    /// <img src="../Coordinate1.jpg" alt="Type 1 coordinate system"/><br/>
    /// The origin of T1 systems is located at the top left corner of the screen. The x-axis points to the right,
    /// the y-axis points downwards. All T1 systems differ in the axes' scaling. <see cref="CoordinateType.UNorm"/>
    /// uses unsigned normalized coordinates. <see cref="CoordinateType.Absolute"/> uses the screen's pixel coordinates.
    /// Therefore, the SpriteRenderer needs the D3DDevice's viewport. For performance reasons the viewport will not be
    /// queried repeatedly, but only once at the construction of the <see cref="SpriteRenderer"/> or on a call to 
    /// <see cref="SpriteRenderer.RefreshViewport"/>. <see cref="CoordinateType.Relative"/> uses a T1 coordinate 
    /// system of custom size.
    /// </para>
    /// <para><b>Type 2 systems</b><br/>
    /// <img src="../Coordinate2.jpg" alt="Type 2 coordinate system"/><br/>
    /// The origin of T2 systems is at the screen center. The x-axis points to the right, the y-axis points upwards.
    /// I.e. this coordinate system uses a flipped y-axis. Because the bottom coordinate is calculated with Top + Size,
    /// T2 coordinates usually have negative vertical sizes. <see cref="CoordinateType.SNorm"/> uses signed normalized
    /// coordinates.
    /// </para>
    /// 
    /// </remarks>
    public enum CoordinateType
    {
        /// <summary>
        /// Coordinates are in the range from 0 to 1. (0, 0) is the top left corner; (1, 1) is the bottom right corner.
        /// </summary>
        UNorm,
        /// <summary>
        /// Coordinates are in the range from -1 to 1. (-1, -1) is the bottom left corner; (1, 1) is the top right corner. This is the DirectX standard interpretation.
        /// </summary>
        SNorm,
        /// <summary>
        /// Coordinates are in the range of the relative screen size. (0, 0) is the top left corner; (ScreenSize.X, ScreenSize.Y) is the bottom right corner. A variable screen size is used. Use <see cref="SpriteRenderer.ScreenSize"/>.
        /// </summary>
        Relative,
        /// <summary>
        /// Coordinates are in the range of the actual screen size. (0, 0) is the top left corner; (Viewport.Width, Viewport.Height) is the bottom right corner. Use <see cref="SpriteRenderer.RefreshViewport"/> for updates to the used viewport.
        /// </summary>
        Absolute
    }

    /// <summary>
    /// This class is responsible for rendering 2D sprites. Typically, only one instance of this class is necessary.
    /// </summary>
    public class SpriteRenderer : IDisposable
    {
        private Device device;
        /// <summary>
        /// Returns the Direct3D device that this SpriteRenderer was created for.
        /// </summary>
        public Device Device { get { return device; } }
        private DeviceContext context;

        private Effect Fx;
        private EffectPass Pass;
        private InputLayout InputLayout;
        private EffectResourceVariable TextureVariable;
        private int BufferSize;
        private Viewport Viewport;

        DepthStencilState DSState;

        /// <summary>
        /// Gets or sets, if this SpriteRenderer handles DepthStencilState
        /// </summary>
        /// <remarks>
        /// <para>
        /// Sprites have to be drawn with depth test disabled. If HandleDepthStencilState is set to true, the
        /// SpriteRenderer sets the DepthStencilState to a predefined state before drawing and resets it to
        /// the previous state after that. Set this value to false, if you want to handle states yourself.
        /// </para>
        /// <para>
        /// The default value is true.
        /// </para>
        /// </remarks>
        public bool HandleDepthStencilState { get; set; }

        /// <summary>
        /// Set to true, if the order of draw calls can be rearranged for better performance.
        /// </summary>
        /// <remarks>
        /// Sprites are not drawn immediately, but only on a call to <see cref="SpriteRenderer.Flush"/>.
        /// Rendering performance can be improved, if the order of sprites can be changed, so that sprites
        /// with the same texture can be drawn with one draw call. However, this will not preserve the z-order.
        /// Use <see cref="SpriteRenderer.ClearReorderBuffer"/> to force a set of sprites to be drawn before another set.
        /// </remarks>
        /// <example>
        /// Consider the following pseudo code:
        /// <code>
        /// Draw left intense red circle
        /// Draw middle light red circle
        /// Draw right intense red circle
        /// </code>
        /// <para>With AllowReorder set to true, this will result in the following image:<br/>
        /// <img src="../Reorder1.jpg" alt=""/><br/>
        /// That is because the last circle is reordered to be drawn together with the first circle.
        /// </para>
        /// <para>With AllowReorder set to false, this will result in the following image:<br/>
        /// <img src="../Reorder2.jpg" alt=""/><br/>
        /// No optimization is applied. Performance may be slightly worse than with reordering.
        /// </para>
        /// </example>
        public bool AllowReorder { get; set; }

        /// <summary>
        /// When using relative coordinates, the screen size has to be set. Typically the screen size in pixels is used. However, other values are possible as well.
        /// </summary>
        public Vector2 ScreenSize { get; set; }

        /// <summary>
        /// A list of all sprites to draw. Sprites are drawn in the order in this list.
        /// </summary>
        private List<SpriteSegment> Sprites = new List<SpriteSegment>();
        /// <summary>
        /// Allows direct access to the according SpriteSegments based on the texture
        /// </summary>
        private Dictionary<ShaderResourceView, List<SpriteSegment>> TextureSprites = new Dictionary<ShaderResourceView,List<SpriteSegment>>();

        /// <summary>
        /// The number of currently buffered sprites
        /// </summary>
        private int SpriteCount = 0;

        private Buffer VB;

        /// <summary>
        /// Create a new SpriteRenderer instance.
        /// </summary>
        /// <param name="device">Direct3D device, which will be used for rendering</param>
        /// <param name="BufferSize">The number of elements that can be stored in the sprite buffer.</param>
        /// <remarks>
        /// Sprites are not drawn immediately, but buffered instead. The buffer size defines, how much sprites can be buffered.
        /// If the buffer is full, according draw calls will be issued on the GPU clearing the buffer. Its size should be as big as
        /// possible without wasting empty space.
        /// </remarks>
        public SpriteRenderer(Device device, int BufferSize = 128)
        {
            this.device = device;
            this.context = device.ImmediateContext;
            this.BufferSize = BufferSize;

            AllowReorder = true;

            Initialize();

            RefreshViewport();
        }

        private void Initialize()
        {

            using (var code = ShaderBytecode.Compile(File.ReadAllText( "..\\..\\NewModules\\Rendering\\Text\\SpriteShader.fx"), "fx_5_0"))
            {
                Fx = new Effect(device, code);
            }

            Pass = Fx.GetTechniqueByIndex(0).GetPassByIndex(0);
            InputLayout = new InputLayout(device, Pass.Description.Signature, SpriteVertexLayout.Description);
            InputLayout.DebugName = "Input Layout for Sprites";

            TextureVariable = Fx.GetVariableByName("Tex").AsResource();

            VB = new Buffer(device, BufferSize * SpriteVertexLayout.Struct.SizeInBytes, ResourceUsage.Dynamic, BindFlags.VertexBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, SpriteVertexLayout.Struct.SizeInBytes);
            VB.DebugName = "Sprites Vertexbuffer";

            var dssd = new DepthStencilStateDescription()
            {
                IsDepthEnabled = false,
                DepthWriteMask = DepthWriteMask.Zero
            };
            DSState = DepthStencilState.FromDescription(Device, dssd);
        }

        /// <summary>
        /// Updates the viewport used for absolute positioning. The first current viewport of the device's rasterizer will be used.
        /// </summary>
        public void RefreshViewport()
        {
            Viewport = device.ImmediateContext.Rasterizer.GetViewports()[0];
        }

        /// <summary>
        /// Closes a reorder session. Further draw calls will not be drawn together with previous draw calls.
        /// </summary>
        public void ClearReorderBuffer()
        {
            TextureSprites.Clear();
        }

        private Vector2 ConvertCoordinate(Vector2 Coordinate, CoordinateType CoordinateType)
        {
            switch (CoordinateType)
            {
                case CoordinateType.SNorm:
                    return Coordinate;
                case CoordinateType.UNorm:
			        Coordinate.X = (Coordinate.X - 0.5f) * 2;
                    Coordinate.Y = -(Coordinate.Y - 0.5f) * 2;
                    return Coordinate;
                case CoordinateType.Relative:
                    Coordinate.X = Coordinate.X / ScreenSize.X * 2 - 1;
                    Coordinate.Y = -(Coordinate.Y / ScreenSize.Y * 2 - 1);
                    return Coordinate;
                case CoordinateType.Absolute:                   
                    Coordinate.X = Coordinate.X / Viewport.Width * 2 - 1;
                    Coordinate.Y = -(Coordinate.Y / Viewport.Height * 2 - 1);
                    return Coordinate;
            }
            return Vector2.Zero;
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="Texture">The shader resource view of the texture to draw</param>
        /// <param name="Position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="Size">Size of the texture in the chosen coordinate system</param>
        /// <param name="CoordinateType">A custom coordinate system in which to draw the texture</param>
        public void Draw(ShaderResourceView Texture, Vector2 Position, Vector2 Size, CoordinateType CoordinateType)
        {
            Draw(Texture, Position, Size, new Color4(1, 1, 1, 1), CoordinateType);
        }

        /// <summary>
        /// Draws a complete texture on the screen.
        /// </summary>
        /// <param name="Texture">The shader resource view of the texture to draw</param>
        /// <param name="Position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="Size">Size of the texture in the chosen coordinate system</param>
        /// <param name="CoordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="Color">The color with which to multiply the texture</param>
        public void Draw(ShaderResourceView Texture, Vector2 Position, Vector2 Size, Color4 Color, CoordinateType CoordinateType)
        {
            Draw(Texture, Position, Size, Vector2.Zero, new Vector2(1, 1), Color, CoordinateType);
        }

        /// <summary>
        /// Draws a region of a texture on the screen.
        /// </summary>
        /// <param name="Texture">The shader resource view of the texture to draw</param>
        /// <param name="Position">Position of the top left corner of the texture in the chosen coordinate system</param>
        /// <param name="Size">Size of the texture in the chosen coordinate system</param>
        /// <param name="CoordinateType">A custom coordinate system in which to draw the texture</param>
        /// <param name="Color">The color with which to multiply the texture</param>
        /// <param name="TexCoords">Texture coordinates for the top left corner</param>
        /// <param name="TexCoordsSize">Size of the region in texture coordinates</param>
        public void Draw(ShaderResourceView Texture, Vector2 Position, Vector2 Size, Vector2 TexCoords, Vector2 TexCoordsSize, Color4 Color, CoordinateType CoordinateType)
        {
            if (Texture == null)
                return;
            var Data = new SpriteVertexLayout.Struct();
            Data.Position = ConvertCoordinate(Position, CoordinateType);
            Data.Size = ConvertCoordinate(Position + Size, CoordinateType) - Data.Position;
            Data.Size.X = Math.Abs(Data.Size.X);
            Data.Size.Y = Math.Abs(Data.Size.Y);
            Data.TexCoord = TexCoords;
            Data.TexCoordSize = TexCoordsSize;
            Data.Color = Color.ToArgb();

            if (AllowReorder)
            {
                //Is there already a sprite for this texture?
                if (TextureSprites.ContainsKey(Texture))
                {
                    //Add the sprite to the last segment for this texture
                    var Segment = TextureSprites[Texture].Last();
                    AddIn(Segment, Data);
                }
                else
                    //Add a new segment for this texture
                    AddNew(Texture, Data);
            }
            else
                //Add a new segment for this texture
                AddNew(Texture, Data);
        }

        private void AddNew(ShaderResourceView Texture, SpriteVertexLayout.Struct Data)
        {
            //Create new segment with initial values
            var NewSegment = new SpriteSegment();
            NewSegment.Texture = Texture;
            NewSegment.Sprites.Add(Data);
            Sprites.Add(NewSegment);

            //Create reference for segment in dictionary
            if (!TextureSprites.ContainsKey(Texture))
                TextureSprites.Add(Texture, new List<SpriteSegment>());

            TextureSprites[Texture].Add(NewSegment);
            CheckForFullBuffer();
        }

        /// <summary>
        /// If the buffer is full, then draw all sprites and clear it.
        /// </summary>
        private void CheckForFullBuffer()
        {
            SpriteCount++;
            if (SpriteCount >= BufferSize)
                Flush();
        }

        private void AddIn(SpriteSegment Segment, SpriteVertexLayout.Struct Data)
        {
            Segment.Sprites.Add(Data);
            CheckForFullBuffer();
        }

        /// <summary>
        /// This method causes the SpriteRenderer to immediately draw all buffered sprites.
        /// </summary>
        /// <remarks>
        /// This method should be called at the end of a frame in order to draw the last sprites that are in the buffer.
        /// </remarks>
        public void Flush()
        {
            if (SpriteCount == 0)
                return;

            System.Threading.Monitor.Enter(device);
            //Update DepthStencilState if necessary
            DepthStencilState oldDSState = null;
            if (HandleDepthStencilState)
            {
                oldDSState = Device.ImmediateContext.OutputMerger.DepthStencilState;
                Device.ImmediateContext.OutputMerger.DepthStencilState = DSState;
            }

            //Construct vertexbuffer
            var Data = context.MapSubresource(VB, MapMode.WriteDiscard, MapFlags.None);
            foreach (var Segment in Sprites)
            {
                var Vertices = Segment.Sprites.ToArray();
                Data.Data.WriteRange(Vertices);
            }
            context.UnmapSubresource(VB, 0);
            

            //Initialize render calls
 
            device.ImmediateContext.InputAssembler.InputLayout = InputLayout;
            device.ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.PointList;
            device.ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VB, SpriteVertexLayout.Struct.SizeInBytes, 0));

            //Draw
            int offset = 0;
            foreach (var Segment in Sprites)
            {
                int count = Segment.Sprites.Count;
                TextureVariable.SetResource(Segment.Texture);
                Pass.Apply(context);
                device.ImmediateContext.Draw(count, offset);
                offset += count;
            }
            
            if (HandleDepthStencilState)
            {
                Device.ImmediateContext.OutputMerger.DepthStencilState = oldDSState;
            }

            System.Threading.Monitor.Exit(device);

            //System.Diagnostics.Debug.Print(SpriteCount + " Sprites gezeichnet.");
            
            //Reset buffers
            SpriteCount = 0;
            Sprites.Clear();
            TextureSprites.Clear();
        }

        #region IDisposable Support
        private bool disposed = false;
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //There are no managed resources to dispose
                }

                Fx.Dispose();
                InputLayout.Dispose();
                DSState.Dispose();

                VB.Dispose();
            }
            this.disposed = true;
        }

        /// <summary>
        /// Disposes of the SpriteRenderer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
