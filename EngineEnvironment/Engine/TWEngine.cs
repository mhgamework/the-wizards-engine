using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine.CodeLoading;
using MHGameWork.TheWizards.Engine.Diagnostics;
using MHGameWork.TheWizards.Engine.Diagnostics.Profiling;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.Engine.Services;
using MHGameWork.TheWizards.Graphics.SlimDX.DirectX11;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Profiling;
using MHGameWork.TheWizards.Serialization;
using PostSharp.Extensibility;
using SlimDX.DirectInput;



namespace MHGameWork.TheWizards.Engine
{


    /// <summary>
    /// This is a host for TW gameplay, it starts and manages the TW context resources, and hotloads the gameplay code
    /// 
    /// Hotloading is done by relinking: all simulators and all objects in the model are released (this is supposed to release all old code objects)
    ///                                  the objects are serialized to a format independent of the old gameplay dll
    ///                                  The new dll is loaded, and the old types are mapped onto the new types, new objects are created and the links are re-established
    /// 
    /// Note: their is a concept hotloading and a concept reloading. Hotloading was something changes that has been removed now, only reloading is used, see wiki for more info
    /// </summary>

    public class TWEngine
    {
        public TWEngine()
        {
            ReloadScheduled = false;
            GameplayDll = "../../Gameplay/bin/x86/Debug/Gameplay.dll";
            GameplayDll = "../BinariesGame/Gameplay.dll";
            codeLoader = new RestartCodeLoader(this); // Use the PersistentCodeLoader for the old style hotloading!
            TraceLogger = new EngineTraceLogger();
            EngineErrorLogger = new EngineErrorLogger();
            // WARNING: BUG: Note: this is somewhat fishy!!!
            DI.Set<IErrorLogger>(EngineErrorLogger);
        }

        public EngineErrorLogger EngineErrorLogger { get; private set; }


        public List<ISimulator> simulators = new List<ISimulator>();

        private AssemblyHotloader gameplayAssemblyHotloader;
        public EngineTWContext twcontext;

        private Assembly activeGameplayAssembly;
        private readonly ICodeLoader codeLoader;
        private EngineDebugTools debugTools;

        public string GameplayDll { get; set; }

        public bool DontLoadPlugin { get; set; }
        public bool HotloadingEnabled { get; set; }

        public EngineTraceLogger TraceLogger { get; private set; }


        public void AddSimulator(ISimulator sim)
        {
            AddSimulator(sim, sim.GetType().Name);
        }
        public void AddSimulator(Action action, string name)
        {
            AddSimulator(new BasicSimulator(action), name);
        }
        public void AddSimulator(ISimulator sim, string name)
        {
            //NOT a problem anymore:
            //if (sim.GetType().GetConstructor(new Type[] { }) == null)
            //    Console.WriteLine("Simulator found without empty constructor, hotloading will fail! " + sim.GetType().FullName);

            var oriSim = sim;

            sim = new ContextDecoratorSimulator(sim, EngineErrorLogger, name);
            sim = new ProfilingDecoratorSimulator(sim, name);
            sim = new TracingDecoratorSimulator(TraceLogger, sim, name);

            simulators.Add(sim);


        }

        public void Run()
        {
            if (twcontext == null)
                Initialize();

            if (TW.Graphics.Running) return;

            TW.Graphics.Run();
        }
        public void Exit()
        {
            TW.Graphics.Exit();
        }


        [Obsolete]
        public void Start()
        {
            if (twcontext == null)
                Initialize();

            TW.Graphics.Run();
        }

        public void Initialize()
        {
            if (twcontext != null) return;

            twcontext = new EngineTWContext(this);

            twcontext.Context.SetService<WPFApplicationService>(new WPFApplicationService());

            ScheduleReload();
            TW.Graphics.GameLoopEvent += gameLoopProfiled;
            gameplayAssemblyHotloader = new AssemblyHotloader(new FileInfo(GameplayDll));
            gameplayAssemblyHotloader.Changed += () => ScheduleReload();
            debugTools = new EngineDebugTools(TW.Graphics.Form.GameLoopProfilingPoint);

            attachAssemblyLoader();

        }

        #region Assembly resolver for gameplaydll

        /// <summary>
        /// Solves postsharp loader issues. When someone requests the gameplay dll (eg serialized postsharp aspects), it should resolve to the current gameplay dll!
        /// </summary>
        private void attachAssemblyLoader()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name == "Gameplay, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                return GetLoadedGameplayAssembly();
            }
            return null;
        }

        #endregion


        #region Code loading

        public bool ReloadScheduled { get; private set; }

        public void ScheduleReload()
        {
            ReloadScheduled = true;
        }

        private void checkReload()
        {
            if ( !HotloadingEnabled )
            {
                ReloadScheduled = false;
                return;
            }
            if (!ReloadScheduled) return;
            codeLoader.Reload();
            ReloadScheduled = false;
        }

        #endregion

        #region Game loop


        private void gameLoopProfiled(DX11Game dx11Game)
        {
            TW.Debug.MainProfilingPoint.Begin();
            gameLoopStep();
            TW.Debug.MainProfilingPoint.End();
        }


        private void gameLoopStep()
        {
            if ((TW.Graphics.Keyboard.IsKeyReleased(Key.R) && TW.Graphics.Keyboard.IsKeyDown(Key.RightControl))
                || TW.Graphics.Keyboard.IsKeyReleased(Key.F2)) ScheduleReload();
            if (TW.Graphics.Keyboard.IsKeyReleased(Key.P) && TW.Graphics.Keyboard.IsKeyDown(Key.RightControl)) debugTools.Show();
            if (TW.Graphics.Keyboard.IsKeyReleased(Key.M) && TW.Graphics.Keyboard.IsKeyDown(Key.RightControl)) debugTools.Profiler.TakeSnapshot();
            if (TW.Debug.NeedsReload) ScheduleReload();
            checkReload();

            TW.Debug.NeedsReload = false;

            foreach ( var sim in simulators )
            {
                TW.Internal.SetActiveSimulator( sim );
                sim.Simulate();
                TW.Internal.SetActiveSimulator(null);
            }

            TW.Data.ClearDirty();
            updatePhysics();
        }


        private void updatePhysics()
        {
            TW.Physics.Update(TW.Graphics.Elapsed);
        }

        #endregion

        public void CreateSimulators()
        {
            simulators.Clear();
            loadPlugin();
        }
        [CatchExceptions]
        private void loadPlugin()
        {
            if (DontLoadPlugin) return;
            try
            {
                var plugin = (IGameplayPlugin)activeGameplayAssembly.CreateInstance("MHGameWork.TheWizards.Plugin");
                plugin.Initialize(this);
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to load the gameplay plugin!!!!");
            }

        }

        [CatchExceptions]
        public void updateActiveGameplayAssembly()
        {
            activeGameplayAssembly = gameplayAssemblyHotloader.LoadCopied();
            TW.Data.GameplayAssembly = activeGameplayAssembly;
        }





        public Assembly GetLoadedGameplayAssembly()
        {
            return activeGameplayAssembly;
        }


        /// <summary>
        /// Resets all the persistent things about the engine, which are not from the gameplay dll
        /// </summary>
        public void ClearAllEngineState()
        {
            TW.Graphics.AcquireRenderer().ClearAll();
            TW.Physics.ClearAll();
            EngineErrorLogger.ClearLast();

            try { TW.Engine.OnClearEngineState(); } // Executes user code, so execute after logger clear so that error can occur in log
            catch (Exception ex) { EngineErrorLogger.Log(ex, "ClearStateCallback"); }

            TW.Engine.ClearCallbacks();
        }

        /// <summary>
        /// Resets all the gameplay dll's data
        /// </summary>
        public void ClearAllGameplayData()
        {
            TW.Data.Objects.Clear();
            TW.Data.PersistentModelObjects.Clear();
        }
    }
}