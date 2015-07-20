﻿using MHGameWork.TheWizards.DualContouring;
using MHGameWork.TheWizards.Engine.WorldRendering;
using MHGameWork.TheWizards.Gameplay;

namespace MHGameWork.TheWizards.VoxelEngine.Environments
{
    /// <summary>
    /// Base class meant to load basic testing/development utilities into the engine, with the purpose of doing visual testing.
    /// Enables:
    /// - rendering
    /// - recording
    /// - cameralight
    /// TODO: console
    /// </summary>
    public class VisualTestingEnvironment
    {
        public static void Load()
        {
            var engine = EngineFactory.CreateEngine();

            engine.AddSimulator(new WorldRenderingSimulator());
            engine.AddSimulator(new CameraLightSimulator());
            engine.AddSimulator(new RecordingSimulator());
        }
    }
}