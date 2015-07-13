using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    public partial class TextureChooser : UserControl
    {
        public TextureChooser()
        {
            InitializeComponent();

            textures = new List<TextureItem>();
            hoverColor = System.Drawing.Color.FromArgb( 200, 225, 255 );
            selectedColor = System.Drawing.Color.FromArgb( 150, 200, 255 );


            selectedIndex = -1;
            selectedItem = null;

            if ( LicenseManager.UsageMode == LicenseUsageMode.Designtime )
            {
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );

                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );

                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );

                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grind001.dds" );
                AddTexture( @"E:\MHGameWork\Projecten\The Wizards CVS\ServerClient\bin\x86\Debug\Content\Grass001.dds" );

                // For setting colors in design mode
                textures[ 0 ].Selected = true;
                textures[ 1 ].Hovered = true;
            }


        }

        private List<TextureItem> textures;

        private int selectedIndex;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if ( value == -1 )
                {
                    SelectedItem = null;
                }
                else
                {
                    SelectedItem = textures[ value ];
                }
            }
        }

        private TextureItem selectedItem;

        public TextureItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if ( value == SelectedItem ) return;
                if ( selectedItem != null )
                {
                    selectedItem.Selected = false;
                }
                selectedItem = value;


                if ( selectedItem != null )
                {
                    selectedItem.Selected = true;
                    selectedIndex = textures.IndexOf( selectedItem );
                }
                else
                {
                    selectedIndex = -1;
                }

                if ( SelectedItemChanged != null ) SelectedItemChanged( this, null );


            }
        }

        private System.Drawing.Color hoverColor;

        public System.Drawing.Color HoverColor
        {
            get { return hoverColor; }
            set
            {
                hoverColor = value;
                for ( int i = 0; i < textures.Count; i++ )
                {
                    TextureItem texture = textures[ i ];
                    texture.HoverColor = hoverColor;
                    texture.Invalidate();
                }
            }
        }

        private System.Drawing.Color selectedColor;

        public System.Drawing.Color SelectedColor
        {
            get { return selectedColor; }
            set
            {
                selectedColor = value;
                for ( int i = 0; i < textures.Count; i++ )
                {
                    TextureItem texture = textures[ i ];
                    texture.SelectedColor = selectedColor;
                    texture.Invalidate();
                }
            }
        }

        public event EventHandler SelectedItemChanged;



        public void AddTexture( string path )
        {
            TextureItem item = new TextureItem();
            item.Path = path;

            Image img = null;
            try
            {
                img = XNAGDIImageLoader.LoadImage( path, Handle );

                Bitmap b = new Bitmap( item.ImageSize, item.ImageSize );
                System.Drawing.Graphics g = System.Drawing.Graphics.FromImage( b );
                g.DrawImage( img, new Rectangle( 0, 0, b.Width, b.Height ) );
                g.Dispose();

                item.BitmapImage = b;
            }
            catch ( Exception e )
            {
                MessageBox.Show( e.ToString() );
            }
            finally
            {
                if ( img != null ) img.Dispose();
                img = null;
            }

            System.IO.FileInfo f = new System.IO.FileInfo( path );
            item.ControlText = f.Name;

            textures.Add( item );

            item.HoverColor = hoverColor;
            item.SelectedColor = selectedColor;

            Controls.Add( item );
            item.Location = new Point( 5, item.Height * ( textures.Count - 1 ) + 5 );
            item.Width = Width - 2 * 5 - 16; // 16: scrollbar size

            item.Click += new EventHandler( item_Click );

        }

        void item_Click( object sender, EventArgs e )
        {
            if ( !Focused ) Focus();

            TextureItem item = sender as TextureItem;

            SelectedItem = item;

        }

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );
            if ( textures == null ) return;
            for ( int i = 0; i < textures.Count; i++ )
            {
                TextureItem texture = textures[ i ];
                texture.Width = Width - 2 * 5 - 16;
            }
        }






        /*protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Zie WinUser.h voor deze regel:
                //#define WS_EX_TRANSPARENT       0x00000020L
                cp.ExStyle |= 0x00000020;


                return cp;
            }
        }



        protected override void OnPaintBackground( PaintEventArgs e )
        {
            // dont code anything here. Leave blank
        }*/



        protected void InvalidateEx()
        {
            if ( Parent == null )
                return;
            Rectangle rc = new Rectangle( this.Location, this.Size );
            Parent.Invalidate( rc, true );
        }


        /*private void CreateItemControls( TextureItem item )
        {
            RemoveItemControls( item );

            int index = textures.IndexOf( item );
            int imageSize = 64;
            Label l;

            l = new Label();
            Controls.Add( l );
            item.BackLabel = l;
            l.Size = new Size( Width - 2 * 10, imageSize + 2 * 5 );
            l.Location = new Point( 5, 5 + l.Size.Height * index );
            l.BackColor = System.Drawing.Color.FromArgb( 128, 150, 200, 255 );

            item.PictureBox = new PictureBox();
            Controls.Add( item.PictureBox );
            item.PictureBox.Size = new Size( imageSize, imageSize );
            item.PictureBox.Location = new Point( item.BackLabel.Left + 5, item.BackLabel.Top + 5 );
            item.PictureBox.Image = item.BitmapImage;

            l = new Label();
            Controls.Add( l );
            item.TextLabel = l;

            l.Size = new Size( item.BackLabel.Width - item.PictureBox.Right - 2 * 5, item.PictureBox.Height );
            l.Location = new Point( item.PictureBox.Right + 5, item.PictureBox.Top );
            l.TextAlign = ContentAlignment.MiddleLeft;
            l.Text = item.Text;
            l.BackColor = System.Drawing.Color.Transparent;


            item.BackLabel.SendToBack();



        }

        private void RemoveItemControls( TextureItem item )
        {
            if ( item.PictureBox != null )
            {
                Controls.Remove( item.PictureBox );
                item.PictureBox.Dispose();
                item.PictureBox = null;
            }
            if ( item.TextLabel != null )
            {
                Controls.Remove( item.TextLabel );
                item.TextLabel.Dispose();
                item.TextLabel = null;
            }
        }*/







        public class TextureItem : Control
        {
            public string Path;
            public Bitmap BitmapImage;
            public string ControlText;


            public System.Drawing.Color HoverColor;
            public System.Drawing.Color SelectedColor;
            public int ImageSize;

            private bool selected;

            public bool Selected
            {
                get { return selected; }
                set
                {
                    bool flag = false;
                    if ( selected != value ) flag = true;
                    selected = value;
                    if ( flag ) Invalidate();
                }
            }

            private bool hovered;

            public bool Hovered
            {
                get { return hovered; }
                set
                {
                    bool flag = false;
                    if ( hovered != value ) flag = true;
                    hovered = value;
                    if ( flag ) Invalidate();
                }
            }


            public TextureItem()
            {
                SetStyle( ControlStyles.Selectable, false );
                HoverColor = System.Drawing.Color.FromArgb( 200, 225, 255 );
                SelectedColor = System.Drawing.Color.FromArgb( 150, 200, 255 );
                ImageSize = 64;
                Width = 200;
                Height = ImageSize + 2 * 5;
            }

            protected override void OnPaint( PaintEventArgs e )
            {
                base.OnPaint( e );

                e.Graphics.Clear( System.Drawing.Color.FromArgb( 200, 200, 200 ) );

                if ( selected ) e.Graphics.FillRectangle( new SolidBrush( SelectedColor ), 0, 0, Width, Height );

                if ( BitmapImage != null ) e.Graphics.DrawImage( BitmapImage, 5, 5, ImageSize, ImageSize );

                StringFormat f = StringFormat.GenericDefault;
                f.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString( ControlText, Parent.Font, Brushes.Black, new RectangleF( ImageSize + 5 + 5, 5, Width - ImageSize - 5 - 5 - 5, Height - 5 - 5 ), f );

                if ( hovered ) e.Graphics.FillRectangle( new SolidBrush( HoverColor ), 0, 0, Width, Height );



            }

            protected override void OnMouseEnter( EventArgs e )
            {
                base.OnMouseEnter( e );
                Hovered = true;
            }

            protected override void OnMouseLeave( EventArgs e )
            {
                base.OnMouseLeave( e );
                Hovered = false;
            }

            /*protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    // Zie WinUser.h voor deze regel:
                    //#define WS_EX_TRANSPARENT       0x00000020L
                    cp.ExStyle |= 0x00000020;


                    return cp;
                }
            }



            protected override void OnPaintBackground( PaintEventArgs e )
            {
                // dont code anything here. Leave blank
            }*/



            protected void InvalidateEx()
            {
                if ( Parent == null )
                    return;
                Rectangle rc = new Rectangle( this.Location, this.Size );
                Parent.Invalidate( rc, true );
            }


            protected override void Dispose( bool disposing )
            {
                base.Dispose( disposing );
                if ( BitmapImage != null ) BitmapImage.Dispose();
                BitmapImage = null;
            }
        }
    }

}
