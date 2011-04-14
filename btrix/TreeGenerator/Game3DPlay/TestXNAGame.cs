using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient
{
    public class TestXNAGame : XNAGame
    {

        /// <summary>
        /// Render delegate for rendering methods, also used for many other
        /// methods.
        /// </summary>
        public delegate void RenderDelegate();

        /// <summary>
        /// Init code
        /// </summary>
        public RenderDelegate initCode,updateCode, renderCode;


        public static TestXNAGame Instance;

        public TestXNAGame( string nWindowsTitle )
        {
            TestXNAGame.Instance = this;
            Window.Title = nWindowsTitle;
        }


        /// <summary>
        /// Create test game
        /// </summary>
        /// <param name="setWindowsTitle">Set windows title</param>
        /// <param name="windowWidth">Window width</param>
        /// <param name="windowHeight">Window height</param>
        /// <param name="setInitCode">Set init code</param>
        /// <param name="setRenderCode">Set render code</param>
        public TestXNAGame( string setWindowsTitle, RenderDelegate setInitCode, RenderDelegate setRenderCode )
        {
            Instance = this;

            Instance.Window.Title = setWindowsTitle;

            //#if DEBUG
            //            // Force window on top
            //            WindowsHelper.ForceForegroundWindow( this.Window.Handle.ToInt32() );
            //#endif
            initCode = setInitCode;
            renderCode = setRenderCode;
        }

        /// <summary>
        /// Initialize
        /// </summary>
        protected override void Initialize()
        {
            // Initialize game
            base.Initialize();

            // Call our custom initCode
            if ( initCode != null )
                initCode();
        } // Initialize()



        protected override void Draw()
        {
            base.Draw();
            if ( renderCode != null )
                renderCode();
        }

        protected override void Update()
        {
            base.Update();
            if ( updateCode != null )
                updateCode();
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
            using ( Instance = new TestXNAGame( testName, initCode, renderCode ) )
            {
                Instance.Run();
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
            Start( testName, null, renderCode );
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
            TestXNAGame.Start( null );
        }

#endif
        #endregion
    }
}
