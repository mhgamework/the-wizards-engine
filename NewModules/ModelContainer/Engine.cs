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
    /// </summary>
    public class Engine
    {
        public Engine()
        {

        }

        public DirectoryInfo GameplayFolder { get; set; }

        private Assembly compileGameplay()
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.IncludeDebugInformation = true;
            cp.TreatWarningsAsErrors = false;
            //cp.CompilerOptions = "/optimize";

            //cp.ReferencedAssemblies.Add("System.Core.dll");
            //cp.ReferencedAssemblies.Add("System.Data.dll");
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!a.IsDynamic)
                    cp.ReferencedAssemblies.Add(a.Location);
            }
            //cp.ReferencedAssemblies.Add(typeof(IScript).Assembly.Location); // Gameplay
            //cp.ReferencedAssemblies.Add(typeof(Vector3).Assembly.Location); // Microsoft.Xna.Framework
            //cp.ReferencedAssemblies.Add(typeof(PlayerData).Assembly.Location); //NewModules
            //cp.ReferencedAssemblies.Add(typeof(XNAGame).Assembly.Location); //Common.core

            var files = Directory.EnumerateFiles(GameplayFolder.FullName, "*.cs", SearchOption.AllDirectories).ToArray();

            try
            {

                Assembly assembly = AssemblyBuilder.CompileExecutableFile(cp, files);
                return assembly;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }





        private List<ISimulator> simulators;
        private DX11Game game;
        private PhysicsEngine physX;
        private string gameplayDll = "Gameplay.dll";


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

                container.ClearDirty();
                physX.Update(game.Elapsed);

                if (game.Keyboard.IsKeyReleased(Key.R))
                {
                    reloadGameplayDll();
                }
                if (needsReload)
                {
                    needsReload = false;
                    reloadGameplayDll();
                }

            };



            setTWGlobals(container);


            startFilesystemWatcher();




            createSimulators();


            game.Run();

        }

        private void createSimulators()
        {
            simulators.Clear();
            var tempFile = Path.GetTempFileName();

            File.Copy(gameplayDll, tempFile, true);
            activeGameplayAssembly = Assembly.LoadFile(tempFile);


            // This is configurable code

            tryLoadSimulator("MHGameWork.TheWizards.Simulators.RenderingSimulator", activeGameplayAssembly);

            // End configurable

        }

        private void reloadGameplayDll()
        {
            var typelessModel = new TypelessModel();
            typelessModel.UpdateFromModel(TW.Model);
            TW.Model.Objects.Clear();
            typelessModel.AddToModel(TW.Model, activeGameplayAssembly);

            createSimulators();
        }

        private void startFilesystemWatcher()
        {
            var watcher = new FileSystemWatcher(new FileInfo(gameplayDll).Directory.FullName);
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
        }

        private volatile bool needsReload = false;
        private Assembly activeGameplayAssembly;

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.FullPath == new FileInfo(gameplayDll).FullName)
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
