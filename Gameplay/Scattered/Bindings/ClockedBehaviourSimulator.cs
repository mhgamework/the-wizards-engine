using System.Collections.Generic;

namespace MHGameWork.TheWizards.Scattered.Bindings
{
    /// <summary>
    /// The update function steps the behaviour. This behaviour is executed repeatedly, and the return value of the behaviour enumerable
    /// is used as a waiting period until the next frame.
    /// </summary>
    public class ClockedBehaviourSimulator
    {
        private float timeWaiting = 0;
        private IEnumerator<float> behaviourEnumerable;
        private IEnumerable<float> stepBehaviour;

        /// <summary>
        /// TODO: not sure whether this should be a Func of IEnumerator or just an enumerator
        /// </summary>
        /// <param name="stepBehaviour"></param>
        public ClockedBehaviourSimulator(IEnumerable<float> stepBehaviour)
        {
            this.stepBehaviour = stepBehaviour;
        }

        public void Update()
        {
            if (timeWaiting > 0.001)
            {
                timeWaiting -= TW.Graphics.Elapsed;
                return;
            }

            if (behaviourEnumerable != null && !behaviourEnumerable.MoveNext())
                behaviourEnumerable = null;

            // In this implementation i try to have movenext and current next to eachother since i do not know which one of the 2 executes the enumerable
            if (behaviourEnumerable == null)
            {
                behaviourEnumerable = stepBehaviour.GetEnumerator();
                if (!behaviourEnumerable.MoveNext()) return; // empty enumerator, throw exception?
            }
            timeWaiting = behaviourEnumerable.Current;
        } 
    }
}