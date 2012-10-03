using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Animation;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;

namespace MHGameWork.TheWizards.Simulators
{
    public class AnimationSimulator : ISimulator
    {
        private List<AnimationController> animationControllers;

        public AnimationSimulator()
        {
            animationControllers = new List<AnimationController>();
        }

        public void Simulate()
        {
            foreach (AnimationController a in animationControllers)
            {
                a.Update();
            }
        }

        public void AddAnimationController(AnimationController a)
        {
            animationControllers.Add(a);
        }

        public void RemoveAnimationController(AnimationController a)
        {
            animationControllers.Remove(a);
        }

       
    }
}
