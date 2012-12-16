using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MHGameWork.TheWizards.Data;
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
    /// </summary>

    public class TWEngine
    {
        public TWEngine()
        {
            GameplayDll = "../../Gameplay/bin/x86/Debug/Gameplay.dll";
        }



        private List<ISimulator> simulators;
        private GraphicsWrapper game;
        private PhysicsWrapper physX;
        public string GameplayDll { get; set; }

        public bool DontLoadPlugin { get; set; }


        private void setTWGlobals(DataWrapper container)
        {
            var context = new TW.Context();
            context.Graphics = game;
            context.Data = container;
            context.Physics = physX;
            context.Audio = new AudioWrapper();
            context.Assets = new AssetsWrapper();
            TW.SetContext(context);
        }

        public void AddSimulator(ISimulator sim)
        {
            simulators.Add(sim);
        }

        public void Run()
        {
            if (game == null)
                Initialize();

            game.Run();
        }
        public void Exit()
        {
            game.Exit();
        }


        [Obsolete]
        public void Start()
        {
            if (game == null)
                Initialize();

            game.Run();
        }

        public void Initialize()
        {
            simulators = new List<ISimulator>();

            game = new GraphicsWrapper();

            game.InitDirectX();


            typeSerializer = new TypeSerializer(this);

            var container = new DataWrapper();

            physX = new PhysicsWrapper();
            physX.Initialize();

            game.GameLoopEvent += delegate
                                  {
                                      TW.Debug.MainProfilingPoint.Begin();
                                      gameLoopStep(container);
                                      TW.Debug.MainProfilingPoint.End();
                                  };


            setTWGlobals(container);

            startFilesystemWatcher();

            updateActiveGameplayAssembly();
            createSimulators();

            var stringSerializer = StringSerializer.Create();
            stringSerializer.AddConditional(new FilebasedAssetSerializer());

            TW.Data.ModelSerializer = new ModelSerializer(stringSerializer, typeSerializer);
            TW.Data.TypeSerializer = typeSerializer;

        }

        private void gameLoopStep(DataWrapper container)
        {
            foreach (var sim in simulators)
            {
                //sim.Simulate();
                simulateSave(sim);
            }

            if (game.Keyboard.IsKeyReleased(Key.R))
                needsReload = true;
            checkReload();

            container.ClearDirty();
            updatePhysics();
        }

        private void simulateSave(ISimulator sim)
        {
            try
            {
                sim.Simulate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in simulator: {0}", sim.GetType().Name);
                Console.WriteLine(ex.ToString());

            }
        }

        private void checkReload()
        {
            if (!needsReload) return;
            needsReload = false;
            reloadGameplayDll();
        }

        private void updatePhysics()
        {
            physX.Update(game.Elapsed);
        }

        private void createSimulators()
        {
            simulators.Clear();

            // This is configurable code

            loadPlugin();


            // End configurable
        }

        private void loadPlugin()
        {
            if (DontLoadPlugin) return;
            try
            {
                var plugin = (IGameplayPlugin)activeGameplayAssembly.CreateInstance("MHGameWork.TheWizards.Plugin");
                plugin.Initialize(this);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void updateActiveGameplayAssembly()
        {
            try
            {
                var tempFile = Path.GetTempFileName();

                File.Copy(GameplayDll, tempFile, true);
                activeGameplayAssembly = Assembly.LoadFile(tempFile);


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void reloadGameplayDll()
        {
            var serializer = new ModelSerializer(StringSerializer.Create(), typeSerializer);

            var mem = new MemoryStream();
            var writer = new StreamWriter(mem);

            serializer.Serialize(TW.Data, writer);
            mem.Flush();

            TW.Data.Objects.Clear();

            updateActiveGameplayAssembly();

            serializer.Deserialize(new StreamReader(mem));


            TW.Graphics.AcquireRenderer().ClearAll();
            createSimulators();
        }

        private void startFilesystemWatcher()
        {
            var watcher = new FileSystemWatcher(new FileInfo(GameplayDll).Directory.FullName);
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);

            watcher.EnableRaisingEvents = true;
        }

        private volatile bool needsReload = false;
        private Assembly activeGameplayAssembly;
        private TypeSerializer typeSerializer;

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == new FileInfo(GameplayDll).FullName)
                needsReload = true;
        }

        public Assembly GetLoadedGameplayAssembly()
        {
            return activeGameplayAssembly;
        }
    }
}
