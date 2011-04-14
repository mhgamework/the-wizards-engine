using System;
using System.Collections.Generic;
using System.Text;

namespace Nuclex {

  internal abstract class GameHost {

    // Events
    internal event EventHandler Activated;

    internal event EventHandler Deactivated;

    internal event EventHandler Exiting;

    internal event EventHandler Idle;

    internal event EventHandler Resume;

    internal event EventHandler Suspend;

    // Methods
    protected GameHost() {
    }

    internal abstract void Exit();

    protected void OnActivated() {
      if(this.Activated != null) {
        this.Activated(this, EventArgs.Empty);
      }
    }

    protected void OnDeactivated() {
      if(this.Deactivated != null) {
        this.Deactivated(this, EventArgs.Empty);
      }
    }

    protected void OnExiting() {
      if(this.Exiting != null) {
        this.Exiting(this, EventArgs.Empty);
      }
    }

    protected void OnIdle() {
      if(this.Idle != null) {
        this.Idle(this, EventArgs.Empty);
      }
    }

    protected void OnResume() {
      if(this.Resume != null) {
        this.Resume(this, EventArgs.Empty);
      }
    }

    protected void OnSuspend() {
      if(this.Suspend != null) {
        this.Suspend(this, EventArgs.Empty);
      }
    }

    internal abstract void PreRun();
    internal abstract void PostRun();

    // Properties
    internal abstract GameControl Control { get; }
  }

} // namespace Nuclex
