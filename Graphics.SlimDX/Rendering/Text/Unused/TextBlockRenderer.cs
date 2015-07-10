using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using SlimDX;
using SlimDX.Direct2D;
using SlimDX.Direct3D10;
using SlimDX.DirectWrite;
using SlimDX.DXGI;

namespace MHGameWork.TheWizards.Graphics.SlimDX.Rendering.Text.Unused
{
    /// <summary>
    /// Defines how a text is aligned in a rectangle. Use OR-combinations of vertical and horizontal alignment.
    /// </summary>
    /// <example>
    /// This example aligns the textblock on the top edge of the rectangle horizontally centered:
    /// <code lang="cs">var textAlignment = TextAlignment.Top | TextAlignment.HorizontalCenter</code>
    /// <code lang="vb">Dim textAlignment = TextAlignment.Top Or TextAlignment.HorizontalCenter</code>
    /// </example>
    [Flags]
    public enum TextAlignment
    {
        /// <summary>
        /// The top edge of the text is aligned at the top edge of the rectangle.
        /// </summary>
        Top = 1,
        /// <summary>
        /// The vertical center of the text is aligned at the vertical center of the rectangle.
        /// </summary>
        VerticalCenter = 2,
        /// <summary>
        /// The bottom edge of the text is aligned at the bottom edge of the rectangle.
        /// </summary>
        Bottom = 4,

        /// <summary>
        /// The left edge of the text is aligned at the left edge of the rectangle.
        /// </summary>
        Left = 8,
        /// <summary>
        /// The horizontal center of the text is aligned at the horizontal center of the rectangle. Each line is aligned independently.
        /// </summary>
        HorizontalCenter = 16,
        /// <summary>
        /// The right edge of the text is aligned at the right edge of the rectangle. Each line is aligned independently.
        /// </summary>
        Right = 32
    }

    /// <summary>
    /// This class is responsible for rendering arbitrary text. Every TextRenderer is specialized for a specific font and relies on
    /// a SpriteRenderer for rendering the text.
    /// </summary>
    public class TextBlockRenderer : IDisposable
    {
        private static int ReferenceCount;
        private static global::SlimDX.Direct3D10_1.Device1 D3DDevice10 = null;
        private global::SlimDX.Direct3D11.Device D3DDevice11 = null;
        private SpriteRenderer Sprite;

        private TextFormat Font;
        private static global::SlimDX.DirectWrite.Factory WriteFactory;
        private static global::SlimDX.Direct2D.Factory D2DFactory;
        private RenderTargetProperties rtp;

        private float _FontSize;

        /// <summary>
        /// Returns the font size that this TextRenderer was created for.
        /// </summary>
        public float FontSize { get { return _FontSize; } }

        /// <summary>
        /// Gets or sets whether this TextRenderer should behave PIX compatibly.
        /// </summary>
        /// <remarks>
        /// PIX compatibility means that no shared resource is used.
        /// However, this will result in no visible text being drawn. 
        /// The geometry itself will be visible in PIX.
        /// </remarks>
        public static bool PixCompatible { get; set; }

        static TextBlockRenderer()
        {
            PixCompatible = false;
        }

        /// <summary>
        /// Contains information about every char table that has been created.
        /// </summary>
        private Dictionary<byte, CharTableDescription> CharTables = new Dictionary<byte, CharTableDescription>();

        /// <summary>
        /// Creates a new text renderer for a specific font.
        /// </summary>
        /// <param name="Sprite">The sprite renderer that is used for rendering</param>
        /// <param name="FontName">Name of font. The font has to be installed on the system. 
        /// If no font can be found, a default one is used.</param>
        /// <param name="FontSize">Size in which to prerender the text. FontSize should be equal to render size for best results.</param>
        /// <param name="FontStretch">Font stretch parameter</param>
        /// <param name="FontStyle">Font style parameter</param>
        /// <param name="FontWeight">Font weight parameter</param>
        public TextBlockRenderer(SpriteRenderer Sprite, String FontName, global::SlimDX.DirectWrite.FontWeight FontWeight, global::SlimDX.DirectWrite.FontStyle FontStyle, FontStretch FontStretch, float FontSize)
        {
            AssertDevice();
            ReferenceCount++;
            this.Sprite = Sprite;
            this._FontSize = FontSize;
            D3DDevice11 = Sprite.Device;
            System.Threading.Monitor.Enter(D3DDevice11);
            rtp = new RenderTargetProperties()
            {
                HorizontalDpi = D2DFactory.DesktopDpi.Width,
                VerticalDpi = D2DFactory.DesktopDpi.Height,
                Type = RenderTargetType.Default,
                PixelFormat = new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied),
                MinimumFeatureLevel = FeatureLevel.Direct3D10
            };
            
            Font = WriteFactory.CreateTextFormat(FontName, FontWeight, FontStyle, FontStretch, FontSize, CultureInfo.CurrentCulture.Name);
            System.Threading.Monitor.Exit(D3DDevice11);
            CreateCharTable(0);
        }

        /// <summary>
        /// Creates the texture and necessary structures for 256 chars whose unicode number starts with the given byte.
        /// The table containing ASCII has a prefix of 0 (0x00/00 - 0x00/FF).
        /// </summary>
        /// <param name="BytePrefix">The byte prefix of characters.</param>
        private void CreateCharTable(byte BytePrefix)
        {
            var TableDesc = new CharTableDescription();

            //Get appropriate texture size
            int SizeX = (int)(Font.FontSize * 12);
            SizeX = (int)Math.Pow(2, Math.Ceiling(Math.Log(SizeX, 2)));
            //Try how many lines are needed:
            var TL = new TextLayout[256];
            float Line = 0, XPos = 0, YPos = 0;
            for (int i = 0; i < 256; ++i)
            {
                TL[i] = new TextLayout(WriteFactory, Convert.ToChar(i + (BytePrefix << 8)).ToString(), Font);
                float CharWidth = 2 + (float)Math.Ceiling(TL[i].Metrics.LayoutWidth + Math.Max(0, TL[i].OverhangMetrics.Left) + Math.Max(0, TL[i].OverhangMetrics.Right));
                float CharHeight = 2 + (float)Math.Ceiling(TL[i].Metrics.LayoutHeight + Math.Max(0, TL[i].OverhangMetrics.Top) + Math.Max(0, TL[i].OverhangMetrics.Bottom));
                Line = Math.Max(Line, CharHeight);
                if (XPos + CharWidth >= SizeX)
                {
                    XPos = 0;
                    YPos += Line;
                    Line = 0;
                }
                XPos += CharWidth;
            }
            int SizeY = (int)(Line + YPos);
            SizeY = (int)Math.Pow(2, Math.Ceiling(Math.Log(SizeY, 2)));

            //Create Texture
            var TexDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource | BindFlags.RenderTarget,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                Height = SizeY,
                Width = SizeX,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.KeyedMutex,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default
            };
            var Texture = new Texture2D(D3DDevice10, TexDesc);
            var rtv = new RenderTargetView(D3DDevice10, Texture);
            D3DDevice10.ClearRenderTargetView(rtv, new global::SlimDX.Color4(0, 1, 1, 1));
            //D3DDevice10.ClearRenderTargetView(rtv, new SlimDX.Color4(1, 0, 0, 0));
            Surface Surface = Texture.AsSurface();
            Surface1 s1;

            var Target = RenderTarget.FromDXGI(D2DFactory, Surface, rtp);
            var Color = new SolidColorBrush(Target, new global::SlimDX.Color4(1, 1, 1, 1));

            Target.BeginDraw();
            Line = 0; XPos = 0; YPos = 0;
            for (int i = 0; i < 256; ++i)
            {
                float CharWidth = 2 + (float)Math.Ceiling(TL[i].Metrics.LayoutWidth + Math.Max(0, TL[i].OverhangMetrics.Left) + Math.Max(0, TL[i].OverhangMetrics.Right));
                float CharHeight = 2 + (float)Math.Ceiling(TL[i].Metrics.LayoutHeight + Math.Max(0, TL[i].OverhangMetrics.Top) + Math.Max(0, TL[i].OverhangMetrics.Bottom));
                Line = Math.Max(Line, CharHeight);
                if (XPos + CharWidth >= SizeX)
                {
                    XPos = 0;
                    YPos += Line;
                    Line = 0;
                }
                var CD = new CharDescription();

                CD.CharSize = new Vector2(TL[i].Metrics.WidthIncludingTrailingWhitespace, TL[i].Metrics.Height);
                CD.OverhangLeft = TL[i].OverhangMetrics.Left;
                CD.OverhangTop = TL[i].OverhangMetrics.Top;
                CD.OverhangRight = TL[i].Metrics.LayoutWidth + TL[i].OverhangMetrics.Right - TL[i].Metrics.WidthIncludingTrailingWhitespace;
                CD.OverhangBottom = TL[i].Metrics.LayoutHeight + TL[i].OverhangMetrics.Bottom - TL[i].Metrics.Height;
                //Safety space around chars that are not unvisible
                if (CD.CharSize.X + CD.OverhangLeft + CD.OverhangRight > 0 && CD.CharSize.Y + CD.OverhangTop + CD.OverhangBottom > 0)
                {
                    CD.OverhangBottom += 1;
                    CD.OverhangTop += 1;
                    CD.OverhangRight += 1;
                    CD.OverhangLeft += 1;
                }

                //Correct overhangs, so size in pixels will be integer numbers
                //this avoids incorrect filtering results
                CD.OverhangRight += (float)Math.Ceiling(CD.OverhangLeft + CD.CharSize.X + CD.OverhangRight) - (CD.OverhangLeft + CD.CharSize.X + CD.OverhangRight);
                CD.OverhangBottom += (float)Math.Ceiling(CD.OverhangTop + CD.CharSize.Y + CD.OverhangBottom) - (CD.OverhangTop + CD.CharSize.Y + CD.OverhangBottom);

                CD.TexCoordsStart = new Vector2((XPos / SizeX), (YPos / SizeY));
                CD.TexCoordsSize = new Vector2((CD.OverhangLeft + CD.CharSize.X + CD.OverhangRight) / SizeX, (CD.OverhangTop + CD.CharSize.Y + CD.OverhangBottom) / SizeY);

                CD.TableDescription = TableDesc;

                TableDesc.Chars[i] = CD;

                Target.DrawTextLayout(new PointF(XPos + CD.OverhangLeft, YPos + CD.OverhangTop), TL[i], Color);
                XPos += CharWidth;
                TL[i].Dispose();
            }
            Target.EndDraw();
            Color.Dispose();

            System.Threading.Monitor.Enter(D3DDevice11);
            var DXGIResource = new global::SlimDX.DXGI.Resource(Texture);
            global::SlimDX.Direct3D11.Texture2D Texture11;
            if (PixCompatible)
            {
                Texture11 = new global::SlimDX.Direct3D11.Texture2D(D3DDevice11, new global::SlimDX.Direct3D11.Texture2DDescription()
                    {
                        ArraySize = 1,
                        BindFlags = global::SlimDX.Direct3D11.BindFlags.ShaderResource | global::SlimDX.Direct3D11.BindFlags.RenderTarget,
                        CpuAccessFlags = global::SlimDX.Direct3D11.CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        Height = SizeY,
                        Width = SizeX,
                        MipLevels = 1,
                        OptionFlags = global::SlimDX.Direct3D11.ResourceOptionFlags.Shared,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = global::SlimDX.Direct3D11.ResourceUsage.Default
                    });
            }
            else
            {
                Texture11 = D3DDevice11.OpenSharedResource <global::SlimDX.Direct3D11.Texture2D>(DXGIResource.SharedHandle);
            }
            var SRV = new global::SlimDX.Direct3D11.ShaderResourceView(D3DDevice11, Texture11);
            TableDesc.Texture = Texture11;
            TableDesc.SRV = SRV;
            rtv.Dispose();
            System.Threading.Monitor.Exit(D3DDevice11);

            System.Diagnostics.Debug.WriteLine("Created Char Table " + BytePrefix + " in " + SizeX + " x " + SizeY);
            
            //System.Threading.Monitor.Enter(D3DDevice11);
            //SlimDX.Direct3D11.Texture2D.SaveTextureToFile(Sprite.Device.ImmediateContext, Texture11, SlimDX.Direct3D11.ImageFileFormat.Png, Font.FontFamilyName + "Table" + BytePrefix + ".png");
            //System.Threading.Monitor.Exit(D3DDevice11);
            
            CharTables.Add(BytePrefix, TableDesc);

            DXGIResource.Dispose();
            Target.Dispose();
            Surface.Dispose();
            Texture.Dispose();
        }
        
        /// <summary>
        /// Draws the string in the specified coordinate system.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="Position">A position in the chosen coordinate system where the top left corner of the first character will be</param>
        /// <param name="RealFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="Color">The color in which to draw the text</param>
        /// <param name="CoordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, Vector2 Position, float RealFontSize, Color4 Color, CoordinateType CoordinateType)
        {
            StringMetrics sm;
            IterateStringEm(text, Position, true, RealFontSize, Color, CoordinateType, out sm);
            return sm;
        }

        /// <summary>
        /// Draws the string untransformed in absolute coordinate system.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="Position">A position in absolute coordinates where the top left corner of the first character will be</param>
        /// <param name="Color">The color in which to draw the text</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, Vector2 Position, Color4 Color)
        {
            return DrawString(text, Position, FontSize, Color, CoordinateType.Absolute);
        }

        /// <summary>
        /// Measures the untransformed string in absolute coordinate system.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <returns>The StringMetrics for the text</returns>
        public StringMetrics MeasureString(string text)
        {
            StringMetrics sm;
            IterateString(text, Vector2.Zero, false, 1, new Color4(), CoordinateType.Absolute, out sm);
            return sm;
        }

        /// <summary>
        /// Measures the string in the specified coordinate system.
        /// </summary>
        /// <param name="text">The text to measure</param>
        /// <param name="RealFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="CoordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the text</returns>
        public StringMetrics MeasureString(string text, float RealFontSize, CoordinateType CoordinateType)
        {
            StringMetrics sm;
            IterateStringEm(text, Vector2.Zero, false, RealFontSize, new Color4(), CoordinateType, out sm);
            return sm;
        }

        /// <summary>
        /// Draws the string in the specified coordinate system aligned in the given rectangle. The text is not clipped or wrapped.
        /// </summary>
        /// <param name="text">The text to draw</param>
        /// <param name="Rect">The rectangle in which to align the text</param>
        /// <param name="Align">Alignment of text in rectangle</param>
        /// <param name="RealFontSize">The real font size in the chosen coordinate system</param>
        /// <param name="Color">The color in which to draw the text</param>
        /// <param name="CoordinateType">The chosen coordinate system</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, RectangleF Rect, TextAlignment Align, float RealFontSize, Color4 Color, CoordinateType CoordinateType)
        {
            //If text is aligned top and left, no adjustment has to be made
            if (Align.HasFlag(TextAlignment.Top) && Align.HasFlag(TextAlignment.Left))
            {
                return DrawString(text, new Vector2(Rect.X, Rect.Y), RealFontSize, Color, CoordinateType);
            }

            text = text.Replace("\r", "");
            var RawTextMetrics = MeasureString(text, RealFontSize, CoordinateType);
            var mMetrics = MeasureString("m", RealFontSize, CoordinateType);
            float startY;
            if (Align.HasFlag(TextAlignment.Top))
                startY = Rect.Top;
            else if (Align.HasFlag(TextAlignment.VerticalCenter))
                startY = Rect.Top + Rect.Height / 2 - RawTextMetrics.Size.Y / 2;
            else //Bottom
                startY = Rect.Bottom - RawTextMetrics.Size.Y;

            var TotalMetrics = new StringMetrics();

            //break text into lines
            var lines = text.Split('\n');

            foreach (var line in lines)
            {
                float startX;
                if (Align.HasFlag(TextAlignment.Left))
                    startX = Rect.X;
                else
                {
                    var lineMetrics = MeasureString(line, RealFontSize, CoordinateType);
                    if (Align.HasFlag(TextAlignment.HorizontalCenter))
                        startX = Rect.X + Rect.Width / 2 - lineMetrics.Size.X / 2;
                    else //Right
                        startX = Rect.Right - lineMetrics.Size.X;
                }

                var lineMetrics2 = DrawString(line, new Vector2(startX, startY), RealFontSize, Color, CoordinateType);
                float lineHeight;
                if (mMetrics.Size.Y < 0)
                    lineHeight = Math.Min(lineMetrics2.Size.Y, mMetrics.Size.Y);
                else
                    lineHeight = Math.Max(lineMetrics2.Size.Y, mMetrics.Size.Y);
                startY += lineHeight;
                TotalMetrics.Merge(lineMetrics2);
            }

            return TotalMetrics;
        }

        /// <summary>
        /// Draws the string unscaled in absolute coordinate system aligned in the given rectangle. The text is not clipped or wrapped.
        /// </summary>
        /// <param name="text">Text to draw</param>
        /// <param name="Rect">A position in absolute coordinates where the top left corner of the first character will be</param>
        /// <param name="Align">Alignment in rectangle</param>
        /// <param name="Color">Color in which to draw the text</param>
        /// <returns>The StringMetrics for the rendered text</returns>
        public StringMetrics DrawString(string text, RectangleF Rect, TextAlignment Align, Color4 Color)
        {
            return DrawString(text, Rect, Align, FontSize, Color, CoordinateType.Absolute);
        }

        private void IterateStringEm(string text, Vector2 Position, bool Draw, float RealFontSize, Color4 Color, CoordinateType CoordinateType, out StringMetrics Metrics)
        {
            float scale = RealFontSize / _FontSize;
            IterateString(text, Position, Draw, scale, Color, CoordinateType, out Metrics);
        }

        private void IterateString(string text, Vector2 Position, bool Draw, float scale, Color4 Color, CoordinateType CoordinateType, out StringMetrics Metrics)
        {
            Metrics = new StringMetrics();
            Vector2 StartPosition = Position;
            float scalY = CoordinateType == CoordinateType.SNorm ? -1 : 1;
            foreach (char c in text)
            {
                var CD = GetCharDescription(c);
                var CharMetrics = CD.ToStringMetrics(Position, scale, scale * scalY);
                if (Draw)
                {
                    if (CharMetrics.FullRectSize.X != 0 && CharMetrics.FullRectSize.Y != 0)
                    {
                        float posY = Position.Y - scalY * CharMetrics.OverhangTop;
                        float posX = Position.X - CharMetrics.OverhangLeft;
                        Sprite.Draw(CD.TableDescription.SRV, new Vector2(posX, posY), CharMetrics.FullRectSize, CD.TexCoordsStart, CD.TexCoordsSize, Color, CoordinateType);
                    }
                }

                Metrics.Merge(CharMetrics);

                Position.X += CharMetrics.Size.X;

                //Break newlines
                if (c == '\r')
                    Position.X = Metrics.TopLeft.X;

                if (c == '\n')
                    Position.Y = Metrics.BottomRight.Y - CharMetrics.Size.Y / 2;
            }
        }

        private CharDescription GetCharDescription(char c)
        {
            int Unicode = (int)c;
            byte Byte = (byte)(c & 0x000000FF);
            byte BytePrefix = (byte)((c & 0x0000FF00) >> 8);
            if (!CharTables.ContainsKey(BytePrefix))
                CreateCharTable(BytePrefix);
            return CharTables[BytePrefix].Chars[Byte];
        }

        static void AssertDevice()
        {
            if (D3DDevice10 != null)
                return;
            D3DDevice10 = new global::SlimDX.Direct3D10_1.Device1(DeviceCreationFlags.BgraSupport | DeviceCreationFlags.Debug, global::SlimDX.Direct3D10_1.FeatureLevel.Level_10_0);
            WriteFactory = new global::SlimDX.DirectWrite.Factory(global::SlimDX.DirectWrite.FactoryType.Shared);
            D2DFactory = new global::SlimDX.Direct2D.Factory(global::SlimDX.Direct2D.FactoryType.SingleThreaded);
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

                Font.Dispose();
                ReferenceCount--;
                foreach (var Table in CharTables)
                {
                    Table.Value.SRV.Dispose();
                    Table.Value.Texture.Dispose();
                }
                if (ReferenceCount <= 0)
                {
                    D3DDevice10.Dispose();
                    WriteFactory.Dispose();
                    D2DFactory.Dispose();
                }
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
