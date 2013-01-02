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
        public bool HotloadingEnabled { get; set; }


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

            if (game.Running) return;

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
            if (game != null) return;

            simulators = new List<ISimulator>();

            game = new GraphicsWrapper();

            game.InitDirectX();


            typeSerializer = new TypeSerializer(this);

            var container = new DataWrapper();

            physX = new PhysicsWrapper();
            physX.Initialize();

            needsReload = true;
            game.GameLoopEvent += delegate
                                  {
                                      TW.Debug.MainProfilingPoint.Begin();
                                      gameLoopStep(container);
                                      TW.Debug.MainProfilingPoint.End();
                                  };


            setTWGlobals(container);

            startFilesystemWatcher();

            //updateActiveGameplayAssembly();
            //createSimulators();

            var stringSerializer = StringSerializer.Create();
            stringSerializer.AddConditional(new FilebasedAssetSerializer());

            TW.Data.ModelSerializer = new ModelSerializer(stringSerializer, typeSerializer);
            TW.Data.TypeSerializer = typeSerializer;

        }

        private void gameLoopStep(DataWrapper container)
        {
            if (game.Keyboard.IsKeyReleased(Key.R))
                needsReload = true;
            if (TW.Debug.NeedsReload)
                needsReload = true;
            checkReload();

            TW.Debug.NeedsReload = false;

            foreach (var sim in simulators)
            {
                //sim.Simulate();
                simulateSave(sim);
            }

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
            if (!HotloadingEnabled) return;
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

                File.Copy(GameplayDll, tempFile + ".dll", true);
                File.Copy(Path.ChangeExtension(GameplayDll, "pdb"), tempFile + ".pdb", true);
                //File.Delete(GameplayDll);
                //File.Delete(Path.ChangeExtension(GameplayDll,"pdb"));
                //File.Delete(Path.ChangeExtension(GameplayDll, "pssym"));
                //File.Delete(Path.ChangeExtension(GameplayDll, "dll.config"));
                activeGameplayAssembly = Assembly.LoadFile(tempFile + ".dll");
                TW.Data.GameplayAssembly = activeGameplayAssembly;


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void reloadGameplayDll()
        {
            var mem = serializeData();
            File.WriteAllBytes("temp.txt", mem.ToArray());
            TW.Data.Objects.Clear();

            updateActiveGameplayAssembly();

            var objects = deserializeData(mem);
            // The deserialized objects are added to TW.Data automatically
            //foreach (var obj in objects)
            //    TW.Data.AddObject(obj);

            TW.Graphics.AcquireRenderer().ClearAll();
            createSimulators();
        }
        private MemoryStream serializeData()
        {

            var mem = new MemoryStream(1024 * 1024 * 64);
            var writer = new StreamWriter(mem);
            foreach (IModelObject obj in TW.Data.Objects)
            {
                if (obj == null) continue; //TODO: fix this this should be impossible
                TW.Data.ModelSerializer.QueueForSerialization(obj);
            }
            TW.Data.ModelSerializer.Serialize(writer);
            writer.Flush();
            mem.Flush();
            mem.Position = 0;
            return mem;
        }
        private List<IModelObject> deserializeData(MemoryStream memoryStream)
        {
            return TW.Data.ModelSerializer.Deserialize(new StreamReader(memoryStream));
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
