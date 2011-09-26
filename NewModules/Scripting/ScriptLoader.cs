using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Windows;
using AssemblyBuilder = MHGameWork.TheWizards.Networking.AssemblyBuilder;

namespace MHGameWork.TheWizards.Scripting
{
    /// <summary>
    /// This class is a singleton class (NOT a helper class).
    /// </summary>

    public class ScriptLoader
    {
        public static ScriptLoader Current { get; set; }


        public ScriptLoader()
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ScriptLoaderWrappers"), System.Reflection.Emit.AssemblyBuilderAccess.Run);
            module = assembly.DefineDynamicModule("Wrappers");

            initFileSystemWatcher();

            startRecompileJob();
        }

        private void startRecompileJob()
        {
            var t = new Thread(recompileJob);
            t.Name = "SceneScriptLoaderCompiler";
            t.IsBackground = true;

            t.Start();
        }

        private void initFileSystemWatcher()
        {
            watcher = new FileSystemWatcher();

            watcher.NotifyFilter = NotifyFilters.LastWrite;

            //TODO: THIS SHOULD BE FIXED
            watcher.Path = TWDir.RootDirectory.Parent.FullName;
            watcher.IncludeSubdirectories = true;


            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;
        }


        public T Load<T>(FileInfo file) where T : class
        {
            if (!file.Exists) throw new InvalidOperationException();


            var s = findOrCreateScript(file);
            var instance = createScriptInstance(s);


            var wrapper = CreateWrapper(typeof(T));
            wrapper.Script = instance;

            s.Handles.Add(wrapper);


            object cheat = wrapper;
            //NOTE: This is because the compiler seems to think that (T)wrapper is impossible. (probably because that Type is generated runtime)
            return (T)cheat;
        }

        public ScriptWrapper CreateWrapper(Type interfaceType)
        {
            var name = getWrapperName(interfaceType);

            var types = module.FindTypes((t, filterCriteria) => t.Name == name, null);
            Type type;

            type = types.Length == 0 ? DefineWrapperType(module, interfaceType) : types[0];


            return (ScriptWrapper)Activator.CreateInstance(type);






        }

        private static string getWrapperName(Type interfaceType)
        {
            return interfaceType.Name + "Wrapper";
        }

        public static Type DefineWrapperType(ModuleBuilder module, Type interfaceType)
        {
            var type = module.DefineType(getWrapperName(interfaceType), TypeAttributes.Public, typeof(ScriptWrapper));
            type.AddInterfaceImplementation(interfaceType);
            var scriptField = type.DefineField("typedScript", interfaceType, FieldAttributes.Private);
            var test = typeof(ScriptWrapper).GetMethods(BindingFlags.NonPublic);

            var method = type.DefineMethod("setScript",
                                           MethodAttributes.HideBySig | MethodAttributes.Family |
                                           MethodAttributes.Virtual | MethodAttributes.ReuseSlot, null,
                                           new[] { typeof(object) });
            var gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0); /* this */
            gen.Emit(OpCodes.Ldarg_1); /* value */
            gen.Emit(OpCodes.Castclass, interfaceType);
            gen.Emit(OpCodes.Stfld, scriptField); // sets the field value
            gen.Emit(OpCodes.Ret);

            method = type.DefineMethod("getScript",
                                       MethodAttributes.HideBySig | MethodAttributes.Family |
                                       MethodAttributes.Virtual | MethodAttributes.ReuseSlot, typeof(object),
                                       Type.EmptyTypes);
            gen = method.GetILGenerator();
            gen.Emit(OpCodes.Ldarg_0); /* this */
            //gen.Emit(OpCodes.Ldarg_1); /* value */
            //gen.Emit(OpCodes.Castclass, interfaceType);
            gen.Emit(OpCodes.Ldfld, scriptField); // sets the field value
            gen.Emit(OpCodes.Ret);


            foreach (var interfaceMethod in interfaceType.GetMethods())
            {

                var attributes = interfaceMethod.Attributes;
                attributes &= ~MethodAttributes.Abstract;

                method = type.DefineMethod(interfaceMethod.Name,
                                               attributes, interfaceMethod.ReturnType,
                                               interfaceMethod.GetParameters().Select(o => o.ParameterType).ToArray());
                gen = method.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0); /* this */
                gen.Emit(OpCodes.Ldfld, scriptField); // Load field onto stack
                gen.Emit(OpCodes.Callvirt, interfaceMethod); // instance void MHGameWork.TheWizards.Tests.Scripting.ScriptingTest/ITestScriptInterface::Execute()
                gen.Emit(OpCodes.Ret);
            }



            return type.CreateType();
        }






        public void AttempReload()
        {
            Script s;
            while (reloadQueue.TryDequeue(out s))
                reloadScriptCode(s);

        }





        private FileSystemWatcher watcher;
        private List<Script> scripts = new List<Script>();
        /// <summary>
        /// Contains all Scripts that have their assembly changed and need to be reloaded into the engine.
        /// </summary>
        private ConcurrentQueue<Script> reloadQueue = new ConcurrentQueue<Script>();
        private Queue<string> recompileQueue = new Queue<string>();
        private ModuleBuilder module;


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

        private object createScriptInstance(Script s)
        {
            Assembly assembly = s.CachedAssembly;

            var type = assembly.GetTypes()[0];
            return assembly.CreateInstance(type.FullName);
        }

        private Assembly CompileAssemblyForScript(FileInfo fi)
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

            public List<ScriptWrapper> Handles = new List<ScriptWrapper>();

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
                //scene.ExecuteInScriptScope(handle, handle.Script.Destroy);
                //handle.Entity.DestroyEntityHandle(handle);


                // Load new script
                var instance = createScriptInstance(s);

                handle.Script = instance;

                //var newhandle = handle.Entity.CreateEntityHandle(instance);
                //scene.ExecuteInScriptScope(newhandle, () => instance.Init(newhandle));
                //s.Handles[i] = newhandle;

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
