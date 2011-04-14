using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Xna.Framework.Input;

namespace Nuclex
{

    internal class WindowsGameHost : GameHost
    {
        // Fields
        private bool doneRun;
        //private bool exitRequested;
        private GameControl game;
        private GameControl gameControl;

        // Methods
        public WindowsGameHost( GameControl game )
        {
            this.game = game;
            this.LockThreadToProcessor();
            //this.gameWindow = new WindowsGameWindow();
            this.gameControl = game;
            // TODO: This line actually creates the control, running code too early
            //Mouse.WindowHandle = this.gameControl.Handle;
            this.gameControl.IsMouseVisible = game.IsMouseVisible;
            this.game.GotFocus += new EventHandler( this.GameWindowActivated );
            this.game.LostFocus += new EventHandler( this.GameWindowDeactivated );
            // TODO: Add Suspend event to GameControl
            //this.gameControl.Suspend += new EventHandler(this.GameWindowSuspend);
            // TODO: Add Resume event to GameControl
            //this.gameControl.Resume += new EventHandler(this.GameWindowResume);
        }

        private void ApplicationIdle( object sender, EventArgs e )
        {
            base.OnIdle();

            // When this code runs and you have multiple GameControls on a form, the
            // first GameControl will keep sitting in this loop and prevent the second
            // GameControl from redrawing itself, even if it's active
            //
            //NativeMethods.Message msg;
            //while(!NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0)) {
            //  base.OnIdle();
            //}

            // Instead, we invalidate the game control to force a redraw, which breaks the
            // application's idle state only to enter it again when the frame has been
            // processed. This does three things:
            //
            // (1) The GameControl keeps redrawing itself
            // (2) Any number of GameControls can be active at the same time and will
            //     continuously update their content
            // (3) It doesn't wreak havoc on the application's other idle functionality
            //     by hogging the idle message with a ceaseless loop
            this.gameControl.Invalidate();
        }

        internal override void Exit()
        {
            //this.exitRequested = true;
        }

        private void GameWindowActivated( object sender, EventArgs e )
        {
            base.OnActivated();
        }

        private void GameWindowDeactivated( object sender, EventArgs e )
        {
            base.OnDeactivated();
        }

        private void GameWindowResume( object sender, EventArgs e )
        {
            base.OnResume();
        }

        private void GameWindowSuspend( object sender, EventArgs e )
        {
            base.OnSuspend();
        }

        [DllImport( "kernel32.dll" )]
        private static extern IntPtr GetCurrentProcess();
        [DllImport( "kernel32.dll" )]
        private static extern IntPtr GetCurrentThread();
        [DllImport( "kernel32.dll", SetLastError = true )]
        private static extern bool GetProcessAffinityMask( IntPtr hProcess, out UIntPtr lpProcessAffinityMask, out UIntPtr lpSystemAffinityMask );
        private void LockThreadToProcessor()
        {
            UIntPtr lpProcessAffinityMask = UIntPtr.Zero;
            UIntPtr lpSystemAffinityMask = UIntPtr.Zero;
            if ( GetProcessAffinityMask( GetCurrentProcess(), out lpProcessAffinityMask, out lpSystemAffinityMask ) && ( lpProcessAffinityMask != UIntPtr.Zero ) )
            {
                UIntPtr dwThreadAffinityMask = (UIntPtr)( lpProcessAffinityMask.ToUInt64() & ( ~lpProcessAffinityMask.ToUInt64() + 1 ) );
                SetThreadAffinityMask( GetCurrentThread(), dwThreadAffinityMask );
            }
        }

        internal override void PreRun()
        {
            if ( this.doneRun )
            {
                //MHGWEdit: this causes unknown error (probably because of the mdi form)
                //EDIT: this no longer causes an error, probably originally because of the xnacontrol not being disposed or smth(by the end user)
                //throw new InvalidOperationException( "Resources.NoMulitpleRuns" );
            }
            try
            {
                Application.Idle += new EventHandler( this.ApplicationIdle );
            }
            catch ( Exception )
            {
                PostRun();
                throw;
            }
        }

        internal override void PostRun()
        {
            Application.Idle -= new EventHandler( this.ApplicationIdle );
            this.doneRun = true;
            base.OnExiting();
        }

        [DllImport( "kernel32.dll" )]
        private static extern UIntPtr SetThreadAffinityMask( IntPtr hThread, UIntPtr dwThreadAffinityMask );

        // Properties
        internal override GameControl Control
        {
            get
            {
                return this.gameControl;
            }
        }
    }

} // namespace Nuclex
