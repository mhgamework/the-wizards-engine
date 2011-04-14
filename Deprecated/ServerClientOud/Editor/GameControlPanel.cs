using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MHGameWork.TheWizards.ServerClient.Editor
{
    /// <summary>
    /// Taken from http://www.wilcob.com/Wilco/Pastecode/6132/showpaste.aspx
    /// </summary>
    public partial class GameControlPanel : UserControl
    {
        Thread gameThread;
        AddInGame game;

        private delegate IntPtr GetHandleCallBack();

        public GameControlPanel()
        {
            InitializeComponent();

            gameThread = new Thread( new ThreadStart( startGame ) );
        }


        public void Start()
        {
            gameThread.Start();
        }

        private void startGame()
        {
            Debug.WriteLine( "Creating Game..." );
            game = new AddInGame( this );
            Debug.WriteLine( "Game Created..." + Environment.NewLine + "Running Game..." );
            game.Run();
        }

        public void CloseGame()
        {
            if ( gameThread.IsAlive )
            {
                game.Exit();
            }
        }

        protected override void Dispose( bool disposing )
        {
            base.Dispose( disposing );
            if ( game != null ) game.Dispose();

        }


        public IntPtr GetSafeHandle()
        {
            if ( this.InvokeRequired )
            {
                GetHandleCallBack getHandleCallBack = new GetHandleCallBack( GetSafeHandle );
                return (IntPtr)this.Invoke( getHandleCallBack );
            }
            else
            {
                return this.Handle;
            }

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GameControlPanel
            // 
            this.Name = "GameControlPanel";
            this.ResumeLayout( false );

        }



        private static System.Threading.AutoResetEvent autoResetEvent;
        private static Form form = new Form();
        private static GameControlPanel panel1;
        private static GameControlPanel panel2;
        public static void TestCreateDispose()
        {
            autoResetEvent = new AutoResetEvent( false );
            form.KeyPreview = true;
            form.KeyDown += new KeyEventHandler( form_KeyDown );
            Application.Run( form );



        }



        static void form_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyCode == Keys.A )
            {
                panel1 = new GameControlPanel();
                form.Controls.Add( panel1 );
                panel1.BackColor = System.Drawing.Color.Red;
                panel1.Dock = DockStyle.Fill;
                panel1.Start();
            }
            if ( e.KeyCode == Keys.B )
            {


                panel1.CloseGame();
                panel1.Dispose();

            }
            if ( e.KeyCode == Keys.C )
            {
                panel2 = new GameControlPanel();
                form.Controls.Add( panel2 );
                panel1.BackColor = System.Drawing.Color.Green;
                panel2.Dock = DockStyle.Fill;
                panel2.Start();
            }
            if ( e.KeyCode == Keys.D )
            {
                panel2.CloseGame();
            }

        }
    }
}
