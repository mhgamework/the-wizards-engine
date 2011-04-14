using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// This class simply calculates the average frame length from framelenghts passed to it. It then holds a property that contains the FPS
    /// </summary>
    public class AverageFPSCalculater
    {
        private int frameCount;
        private float timeCount;

        private float refreshInterval;
        private float averageFPS;

        public float AverageFps
        {
            [DebuggerStepThrough]
            get { return averageFPS; }
        }

        public float RefreshInterval
        {
            [DebuggerStepThrough]
            get { return refreshInterval; }
            [DebuggerStepThrough]
            set { refreshInterval = value; }
        }

        public AverageFPSCalculater()
        {
            refreshInterval = 1;
        }

        public void AddFrame(float elapsed)
        {
            timeCount += elapsed;
            frameCount++;

            if (timeCount >= refreshInterval)
            {
                averageFPS = frameCount/timeCount;
                frameCount = 0;
                timeCount = 0;
            }
        }
    }
}
