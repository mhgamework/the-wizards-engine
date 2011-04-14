using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class TestServerClientMain : ServerClientMainOud
    {

        /// <summary>
        /// Render delegate for rendering methods, also used for many other
        /// methods.
        /// </summary>
        public delegate void RenderDelegate();

        /// <summary>
        /// Init code
        /// </summary>
        protected RenderDelegate initCode, renderCode, tickCode;


        public static TestServerClientMain Instance;


        /// <summary>
        /// Create test game
        /// </summary>
        /// <param name="setWindowsTitle">Set windows title</param>
        /// <param name="windowWidth">Window width</param>
        /// <param name="windowHeight">Window height</param>
        /// <param name="setInitCode">Set init code</param>
        /// <param name="setRenderCode">Set render code</param>
        protected TestServerClientMain( string setWindowsTitle, RenderDelegate setInitCode, RenderDelegate setRenderCode, RenderDelegate setTickCode )
        {
            Instance = this;

            this.XNAGame.Window.Title = setWindowsTitle;

            //#if DEBUG
            //            // Force window on top
            //            WindowsHelper.ForceForegroundWindow( this.Window.Handle.ToInt32() );
            //#endif
            initCode = setInitCode;
            renderCode = setRenderCode;
            tickCode = setTickCode;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        public override void Initialize()
        {
            // Initialize game
            base.Initialize();

            // Call our custom initCode
            if ( initCode != null )
                initCode();
        } // Initialize()





        public override void OnRender( object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e )
        {
            base.OnRender( sender, e );


            if ( renderCode != null )
                renderCode();
        }

        public override void OnTick( object sender, MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e )
        {
            base.OnTick( sender, e );
            if ( tickCode != null )
                tickCode();
        }





        public static TestServerClientMain game;


        /// <summary>
        /// Start
        /// </summary>
        /// <param name="testName">Test name</param>
        /// <param name="initCode">Init code</param>
        /// <param name="renderCode">Render code</param>
        public static void Start( string testName,
            RenderDelegate initCode, RenderDelegate renderCode, RenderDelegate tickCode )
        {
            using ( game = new TestServerClientMain( testName, initCode, renderCode, tickCode ) )
            {
                game.Run();
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="testName">Test name</param>
        /// <param name="initCode">Init code</param>
        /// <param name="renderCode">Render code</param>
        public static void Start( string testName,
            RenderDelegate initCode, RenderDelegate renderCode )
        {
            using ( game = new TestServerClientMain( testName, initCode, renderCode, null ) )
            {
                game.Run();
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="testName">Test name</param>
        /// <param name="renderCode">Render code</param>
        public static void Start( string testName,
            RenderDelegate renderCode )
        {
            Start( testName, null, renderCode, null );
        }

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="renderCode">Render code</param>
        public static void Start( RenderDelegate renderCode )
        {
            Start( "Unit Test", null, renderCode );
        }


        #region Unit Testing
#if DEBUG

        /// <summary>
        /// Test empty game
        /// </summary>
        public static void TestEmptyGame()
        {
            TestServerClientMain.Start( null );
        }

#endif
        #endregion
    }
}
