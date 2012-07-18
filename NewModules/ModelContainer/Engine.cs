using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DirectX11;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Physics;
using MHGameWork.TheWizards.Simulators;
using SlimDX.DirectInput;

namespace MHGameWork.TheWizards.ModelContainer
{
    /// <summary>
    /// This is a host for TW gameplay, it starts and manages the TW context resources, and hotloads the gameplay code
    /// 
    /// Hotloading is done by relinking: all simulators and all objects in the model are released (this is supposed to release all old code objects)
    ///                                  the objects are serialized to a format independent of the old gameplay dll
    ///                                  The new dll is loaded, and the old types are mapped onto the new types, new objects are created and the links are re-established
    /// 
    /// </summary>
    public class Engine
    {
        public Engine()
        {
            GameplayDll = "../../Gameplay/bin/x86/Debug/Gameplay.dll";
        }

       

        private List<ISimulator> simulators;
        private DX11Game game;
        private PhysicsEngine physX;
        public string GameplayDll { get; set; }


        private void setTWGlobals(ModelContainer container)
        {
            var context = new TW.Context();
            context.Game = game;
            context.Model = container;
            context.PhysX = physX;
            context.Scene = physX.Scene;
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

            game = new DX11Game();

            game.InitDirectX();

            var container = new ModelContainer();

            physX = new PhysicsEngine();
            physX.Initialize();

            game.GameLoopEvent += delegate
            {
                foreach (var sim in simulators)
                {
                    sim.Simulate();
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
            var typelessModel = new TypelessModel();
            typelessModel.UpdateFromModel(TW.Model);
            TW.Model.Objects.Clear();

            updateActiveGameplayAssembly();
            typelessModel.AddToModel(TW.Model, activeGameplayAssembly);


            TW.AcquireRenderer().ClearAll();
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
