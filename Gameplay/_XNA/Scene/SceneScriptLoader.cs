using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using MHGameWork.TheWizards._XNA.Scripting.API;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking;
using MHGameWork.TheWizards.Player;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards._XNA.Scene
{
    public class SceneScriptLoader : IXNAObject
    {
        public SceneScriptLoader(Scene scene)
        {
            this.scene = scene;
            watcher = new FileSystemWatcher();

            watcher.NotifyFilter = NotifyFilters.LastWrite;

            //TODO: THIS SHOULD BE FIXED
            watcher.Path = TWDir.RootDirectory.Parent.FullName;
            watcher.IncludeSubdirectories = true;


            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;

            var t = new Thread(recompileJob);
            t.Name = "SceneScriptLoaderCompiler";
            t.IsBackground = true;

            t.Start();
        }

        public void Initialize(IXNAGame _game)
        {
        }
        public void Render(IXNAGame _game)
        {
        }
        public void Update(IXNAGame _game)
        {
            Script s;
            while (reloadQueue.TryDequeue(out s))
                reloadScriptCode(s);

        }

        public void LoadScript(Entity entity, FileInfo scriptFile)
        {
            if (!scriptFile.Exists) throw new InvalidOperationException();
            if (entity.Scene != scene) throw new InvalidOperationException();

            var s = findOrCreateScript(scriptFile);
            var instance = CreateScriptInstance(s);


            var handle = entity.CreateEntityHandle(instance);
            instance.Init(handle);

            s.Handles.Add(handle);
        }




        private FileSystemWatcher watcher;
        private readonly Scene scene;
        private List<Script> scripts = new List<Script>();
        private ConcurrentQueue<Script> reloadQueue = new ConcurrentQueue<Script>();
        private Queue<string> recompileQueue = new Queue<string>();

       


        /// <summary>
        /// Thread Safe
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        private Script findOrCreateScript(FileInfo fi)
        {

            Script s = findScript(fi);
            if (s != null) return s;

            s = new Script();
            s.File = fi;
            s.CachedAssembly = CompileAssemblyForScript(fi);

            lock (this)
            {
                scripts.Add(s);
            }

            return s;

        }
        /// <summary>
        /// Thread Safe
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        private Script findScript(FileInfo fi)
        {
            lock (this)
            {
                return scripts.Find(o => o.File.FullName == fi.FullName);
            }
        }

        private IScript CreateScriptInstance(Script s)
        {
            Assembly assembly = s.CachedAssembly;

            var type = assembly.GetTypes().Single(o => typeof(IScript).IsAssignableFrom(o));
            return (IScript)assembly.CreateInstance(type.FullName);
        }

        private Assembly CompileAssemblyForScript(FileInfo fi)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.IncludeDebugInformation = true;
            cp.TreatWarningsAsErrors = false;
            //cp.CompilerOptions = "/optimize";

            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add(typeof(IScript).Assembly.Location); // Gameplay
            cp.ReferencedAssemblies.Add(typeof(Vector3).Assembly.Location); // Microsoft.Xna.Framework
            cp.ReferencedAssemblies.Add(typeof(PlayerData).Assembly.Location); //NewModules
            cp.ReferencedAssemblies.Add(typeof(XNAGame).Assembly.Location); //Common.core

            try
            {
                Assembly assembly = AssemblyBuilder.CompileExecutableFile(cp, new[] { fi.FullName });
                return assembly;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return null;
        }

        private class Script
        {
            public FileInfo File;
            public Assembly CachedAssembly;

            public List<EntityScriptHandle> Handles = new List<EntityScriptHandle>();

        }

        void watcher_Changed(object sender, FileSystemEventArgs e)
        {

            lock (recompileQueue)
            {
                recompileQueue.Enqueue(e.FullPath);
                Monitor.Pulse(recompileQueue);
            }

        }

        private void reloadScriptCode(Script s)
        {


            for (int i = 0; i < s.Handles.Count; i++)
            {
                var handle = s.Handles[i];

                // Destory old script
                scene.ExecuteInScriptScope(handle, handle.Script.Destroy);
                handle.Entity.DestroyEntityHandle(handle);


                // Load new script
                var instance = CreateScriptInstance(s);


                var newhandle = handle.Entity.CreateEntityHandle(instance);
                scene.ExecuteInScriptScope(newhandle, () => instance.Init(newhandle));
                s.Handles[i] = newhandle;

            }




        }


        void recompileJob()
        {
            for (; ; )
            {
                lock (recompileQueue)
                {
                    while (recompileQueue.Count == 0)
                        Monitor.Wait(recompileQueue);

                    var s = findScript(new FileInfo(recompileQueue.Dequeue()));
                    if (s == null) continue;

                    var assembly = CompileAssemblyForScript(s.File);
                    if (assembly == null) continue;

                    //TODO: Not thread-safe!!
                    s.CachedAssembly = assembly;

                    reloadQueue.Enqueue(s);
                }
            }
        }


    }
}
