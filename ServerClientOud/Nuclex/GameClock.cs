using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclex {

  internal class GameClock {

    // Methods
    public GameClock() {
      this.Reset();
    }

    private static TimeSpan CounterToTimeSpan(long delta) {
      long num = 0x989680;
      long num2 = (delta * num) / Frequency;
      return TimeSpan.FromTicks(num2);
    }

    internal void Reset() {
      this.currentTime = TimeSpan.Zero;
      this.baseRealTime = Counter;
      this.lastRealTimeValid = false;
    }

    internal void Resume() {
      this.suspendCount--;
      if(this.suspendCount <= 0) {
        long counter = Counter;
        this.timeLostToSuspension += counter - this.suspendStartTime;
        this.suspendStartTime = 0;
      }
    }

    internal void Step() {
      long counter = Counter;
      if(!this.lastRealTimeValid) {
        this.lastRealTime = counter;
        this.lastRealTimeValid = true;
      }
      this.baseRealTime += this.timeLostToSuspension;
      this.lastRealTime += this.timeLostToSuspension;
      this.timeLostToSuspension = 0;
      this.currentTime = CounterToTimeSpan(counter - this.baseRealTime);
      this.elapsedTime = CounterToTimeSpan(counter - this.lastRealTime);
      this.lastRealTime = counter;
    }

    internal void Suspend() {
      this.suspendCount++;
      if(this.suspendCount == 1) {
        this.suspendStartTime = Counter;
      }
    }

    // Properties
    internal static long Counter {
      get {
        return Stopwatch.GetTimestamp();
      }
    }

    internal TimeSpan CurrentTime {
      get {
        return this.currentTime;
      }
    }

    internal TimeSpan ElapsedTime {
      get {
        return this.elapsedTime;
      }
    }

    internal static long Frequency {
      get {
        return Stopwatch.Frequency;
      }
    }

    // Fields
    private long baseRealTime;
    private TimeSpan currentTime;
    private TimeSpan elapsedTime;
    private long lastRealTime;
    private bool lastRealTimeValid;
    private int suspendCount;
    private long suspendStartTime;
    private long timeLostToSuspension;

  }

} // namespace Nuclex
