using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Serialization;
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


        private void setTWGlobals(DataWrapper container)
        {
            var context = new TW.Context();
            context.Graphics = game;
            context.Data = container;
            context.Physics = physX;
            context.Audio = new AudioWrapper();
            TW.SetContext(context);
        }

        public void AddSimulator(ISimulator sim)
        {
            simulators.Add(sim);
        }

        public void Run()
        {
            game.Run();
        }
        public void Exit()
        {
            game.Exit();
        }



        public void Start()
        {
            simulators = new List<ISimulator>();

            game = new GraphicsWrapper();

            game.InitDirectX();

            var container = new DataWrapper();

            physX = new PhysicsWrapper();
            physX.Initialize();

            game.GameLoopEvent += delegate
            {
                foreach (var sim in simulators)
                {
                    try
                    {
                        sim.Simulate();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Error in simulator: {0}",sim.GetType().Name);
                        Console.WriteLine(ex.ToString());
                    }
                }

                if (game.Keyboard.IsKeyReleased(Key.R))
                {
                    reloadGameplayDll();
                }
                if (needsReload)
                {
                    needsReload = false;
                    reloadGameplayDll();
                }

                container.ClearDirty();
                physX.Update(game.Elapsed);



            };



            setTWGlobals(container);


            startFilesystemWatcher();



            updateActiveGameplayAssembly();
            createSimulators();


            game.Run();

        }

        private void createSimulators()
        {
            simulators.Clear();

            // This is configurable code

            try
            {
                var plugin = (IGameplayPlugin)activeGameplayAssembly.CreateInstance("MHGameWork.TheWizards.Plugin");
                plugin.Initialize(this);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //tryLoadSimulator("MHGameWork.TheWizards.Simulators.RenderingSimulator", activeGameplayAssembly);


            // End configurable

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
            var serializer = new ModelSerializer(StringSerializer.Create());

            var mem = new MemoryStream();
            var writer = new StreamWriter(mem);

            serializer.Serialize(TW.Data, writer);
            mem.Flush();

            TW.Data.Objects.Clear();

            updateActiveGameplayAssembly();

            serializer.Deserialize(TW.Data, new StreamReader(mem));


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

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == new FileInfo(GameplayDll).FullName)
                needsReload = true;
        }

        private void tryLoadSimulator(string renderingsimulator, Assembly gameplay)
        {
            ISimulator sim;
            try
            {
                sim = (ISimulator)gameplay.CreateInstance(renderingsimulator);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            AddSimulator(sim);
        }
    }
}
