using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics;
using MHGameWork.TheWizards.Rendering.Particles;
using MHGameWork.TheWizards.Simulation.ActionScheduling;
using SlimDX;

namespace MHGameWork.TheWizards.Testing
{
    /// <summary>
    /// Interface + implementation of this interface using the TWEngine system
    /// This is a colleciton interface, meant to be convenient for testing
    /// TODO: add debug printing
    /// </summary>
    public class IRenderingTester
    {
        private readonly TWEngine engine;
        private readonly IActionScheduler scheduler;
        private readonly ParticlesBoxRenderer particleBoxRenderer;
        private List<ParticleEffect> particleEffects = new List<ParticleEffect>();
        private List<Action<float>> updateObservers = new List<Action<float>>();

        public IRenderingTester(TWEngine engine, IActionScheduler scheduler, ParticlesBoxRenderer particleBoxRenderer)
        {
            this.engine = engine;
            this.scheduler = scheduler;
            this.particleBoxRenderer = particleBoxRenderer;

            scheduler.SetCurrentTime(TW.Graphics.TotalRunTime);

            engine.AddSimulator(new BasicSimulator(update));
            engine.AddSimulator(new WorldRenderingSimulator());
        }

        public LineManager3D LineManager3D { get { return TW.Graphics.LineManager3D; } }

        private void update()
        {
            particleEffects.ForEach(e => e.Update(TW.Graphics.Elapsed));
            updateObservers.ForEach(a => a(TW.Graphics.Elapsed));
            scheduler.ExecuteActions(TW.Graphics.TotalRunTime);

            particleBoxRenderer.RenderEffects(particleEffects);
        }

        public void SetRepeat(float interval, Action action)
        {
            Action loopedAction = null;

            loopedAction = () =>
                {
                    action();
                    SetTimeout(interval, loopedAction);
                };

            SetTimeout(0, loopedAction);
        }
        public void SetTimeout(float interval, Action action)
        {
            scheduler.SetTimeout(interval, action);
        }

        public void SetCameraPosition(Vector3 position, Vector3 lookTarget)
        {
            TW.Graphics.SpectaterCamera.CameraPosition = position;
            TW.Graphics.SpectaterCamera.CameraDirection = Vector3.Normalize(lookTarget - position);
        }

        public ParticleEffect CreateParticleEffect()
        {
            var ret = new ParticleEffect();

            particleEffects.Add(ret);
            return ret;
        }

        public void ObserveUpdate(Action onUpdate)
        {
            ObserveUpdate(_ => onUpdate());
        }

        public void ObserveUpdate(Action<float> onUpdate)
        {
            updateObservers.Add(onUpdate);
        }
    }
}