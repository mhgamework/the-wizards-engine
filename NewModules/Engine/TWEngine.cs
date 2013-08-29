using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Debugging;
using MHGameWork.TheWizards.DirectX11;
using MHGameWork.TheWizards.Engine.Diagnostics;
using MHGameWork.TheWizards.Engine.Diagnostics.Profiling;
using MHGameWork.TheWizards.Engine.Diagnostics.Tracing;
using MHGameWork.TheWizards.Engine.Services;
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
            GameplayDll = "../../Gameplay/bin/x86/Debug/Gameplay.dll";
            GameplayDll = "../BinariesGame/Gameplay.dll";
            codeLoader = new CodeLoader(this);
            TraceLogger = new EngineTraceLogger();
            EngineErrorLogger = new EngineErrorLogger();
            // WARNING: BUG: Note: this is somewhat fishy!!!
            DI.Set<IErrorLogger>(EngineErrorLogger);
        }

        public EngineErrorLogger EngineErrorLogger { get; private set; }


        private List<ISimulator> simulators = new List<ISimulator>();

        private AssemblyHotloader gameplayAssemblyHotloader;
        public EngineTWContext twcontext;

        private Assembly activeGameplayAssembly;
        private readonly CodeLoader codeLoader;
        private EngineDebugTools debugTools;

        public string GameplayDll { get; set; }

        public bool DontLoadPlugin { get; set; }
        public bool HotloadingEnabled { get; set; }

        public EngineTraceLogger TraceLogger { get; private set; }


        public void AddSimulator(ISimulator sim)
        {
            //NOT a problem anymore:
            //if (sim.GetType().GetConstructor(new Type[] { }) == null)
            //    Console.WriteLine("Simulator found without empty constructor, hotloading will fail! " + sim.GetType().FullName);

            sim = new ContextDecoratorSimulator(sim, EngineErrorLogger);
            sim = new ProfilingDecoratorSimulator(sim);
            sim = new TracingDecoratorSimulator(TraceLogger, sim);

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

            codeLoader.setNeedsReload();
            TW.Graphics.GameLoopEvent += gameLoopProfiled;
            gameplayAssemblyHotloader = new AssemblyHotloader(new FileInfo(GameplayDll));
            gameplayAssemblyHotloader.Changed += () => codeLoader.setNeedsReload();
            debugTools = new EngineDebugTools(TW.Graphics.Form.GameLoopProfilingPoint);

        }

        private void gameLoopProfiled(DX11Game dx11Game)
        {
            TW.Debug.MainProfilingPoint.Begin();
            gameLoopStep();
            TW.Debug.MainProfilingPoint.End();
        }


        private void gameLoopStep()
        {
            if (TW.Graphics.Keyboard.IsKeyReleased(Key.R)) codeLoader.setNeedsReload();
            if (TW.Graphics.Keyboard.IsKeyReleased(Key.H)) codeLoader.setNeedsHotload();
            if (TW.Graphics.Keyboard.IsKeyReleased(Key.P)) debugTools.Show();
            if (TW.Graphics.Keyboard.IsKeyReleased(Key.M)) debugTools.Profiler.TakeSnapshot();
            if (TW.Debug.NeedsReload) codeLoader.setNeedsReload();
            codeLoader.checkReload();
            codeLoader.checkHotload();

            TW.Debug.NeedsReload = false;

            foreach (var sim in simulators)
                sim.Simulate();

            TW.Data.ClearDirty();
            updatePhysics();
        }


        private void updatePhysics()
        {
            TW.Physics.Update(TW.Graphics.Elapsed);
        }

        private void createSimulators()
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
        private void updateActiveGameplayAssembly()
        {
            activeGameplayAssembly = gameplayAssemblyHotloader.LoadCopied();
            TW.Data.GameplayAssembly = activeGameplayAssembly;
        }





        public Assembly GetLoadedGameplayAssembly()
        {
            return activeGameplayAssembly;
        }









        public class CodeLoader
        {
            private TWEngine twEngine;
            private volatile bool needsReload;
            private bool needsHotload = true;

            public CodeLoader(TWEngine twEngine)
            {
                this.twEngine = twEngine;
            }

            public void checkReload()
            {
                if (!needsReload) return;
                needsReload = false;
                reload();
            }

            private void reload()
            {
                reloadGameplayDll();
                twEngine.createSimulators(); // reinitialize!
            }

            public void checkHotload()
            {
                if (!twEngine.HotloadingEnabled) return;
                if (!needsHotload) return;
                needsHotload = false;

                hotload();


            }

            private void hotload()
            {
                //TODO: hotloading currently disabled!
                return;
                try
                {
                    var simList = serializeSimulatorList();
                    reloadGameplayDll();
                    deserializeSimulatorList(simList);
                }
                catch (Exception party)
                {
                    Console.WriteLine("Hotload failed");
                    Console.Write(party);
                }
            }

            private void deserializeSimulatorList(string[] simList)
            {
                twEngine.simulators.Clear();
                foreach (var name in simList)
                {
                    var type = TW.Data.TypeSerializer.Deserialize(name);
                    if (type == null)
                    {
                        Console.Write("Unable to hotload simulator: " + name);
                        continue;
                    }

                    var sim = (ISimulator)Activator.CreateInstance(type);
                    twEngine.simulators.Add(sim);
                }
            }

            private string[] serializeSimulatorList()
            {
                return twEngine.simulators.Select(s => s.GetType()).Select(TW.Data.TypeSerializer.Serialize).ToArray();
            }

            private void reloadGameplayDll()
            {
                //TODO: broke the persistance scope here, not sure whether it should be readded
                var persistentModels = TW.Data.Objects;
                //var persistentModels = TW.Data.Objects.Where(o => TW.Data.PersistentModelObjects.Contains(o));
                MemoryStream mem = null;
                try
                {
                    mem = TW.Data.ModelSerializer.SerializeToStream(persistentModels);
                    File.WriteAllBytes("temp.txt", mem.ToArray());
                }
                catch (Exception ex)
                {
                    DI.Get<IErrorLogger>().Log(ex,"Serialize engine data");
                }


                TW.Data.Objects.Clear();
                TW.Data.PersistentModelObjects.Clear();

                twEngine.updateActiveGameplayAssembly();

                try
                {
                    if (mem != null)
                    {
                        var objects = deserializeData(mem);
                        // The deserialized objects are added to TW.Data automatically
                        //foreach (var obj in objects)
                        //    TW.Data.AddObject(obj);
                    }
                }
                catch (Exception ex)
                {
                    DI.Get<IErrorLogger>().Log(ex, "Deserialize engine data");
                    return;
                }
        

                TW.Graphics.AcquireRenderer().ClearAll();
                TW.Physics.ClearAll();
                twEngine.EngineErrorLogger.ClearLast();

            }

            [PersistanceScope]
            private List<IModelObject> deserializeData(MemoryStream memoryStream)
            {
                TW.Data.InPersistenceScope = true;

                var ret = TW.Data.ModelSerializer.Deserialize(new StreamReader(memoryStream));

                TW.Data.InPersistenceScope = false;

                return ret;
            }

            public void setNeedsReload()
            {
                needsReload = true;
            }

            public void setNeedsHotload()
            {
                needsHotload = true;
            }
        }

    }
}
