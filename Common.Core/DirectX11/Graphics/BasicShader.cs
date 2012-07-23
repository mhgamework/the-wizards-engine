using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SlimDX.D3DCompiler;
using SlimDX.Direct3D11;

namespace MHGameWork.TheWizards.DirectX11.Graphics
{
    /// <summary>
    /// This class supports automatic reloading
    /// TODO: store all SetParameter calls in RAM, so that a reload auto-sets the parameters
    /// Auto-reload is only partially implemented is will be buggy
    /// </summary>
    public class BasicShader : IDisposable
    {
        private readonly Device device;
        public Effect Effect { get; private set; }

        //TODO: replace with manualresetevent
        private bool reloadScheduled;
        private FileInfo reloadFile;

        private BasicShader(Device device)
        {
            this.device = device;
            context = device.ImmediateContext;
        }

        private List<BasicShader> clones = new List<BasicShader>();
        private ShaderMacro[] shaderMacros;
        private DeviceContext context;

        // filenames of all the fx files this shader used to compile itself
        private string[] shaderDependencies;

        public BasicShader ParentShader { get; private set; }

        public bool IsClone
        {
            get { return ParentShader != null; }
        }

        public BasicShader Clone()
        {
            if (IsClone) throw new InvalidOperationException("Can't clone a cloned shader!");

            var clone = new BasicShader(device);
            clone.loadCloned(this);

            //Track clones
            clones.Add(clone);


            return clone;
        }


        public static BasicShader LoadAutoreload(IGraphicsManager graphics, FileInfo fi)
        {
            return LoadAutoreload(graphics, fi, null);
        }
        public static BasicShader LoadAutoreload(IGraphicsManager graphics, FileInfo fi, Action<BasicShader> loadedDelegate)
        {
            return LoadAutoreload(graphics, fi, loadedDelegate, null);
        }
        public static BasicShader LoadAutoreload(IGraphicsManager graphics, FileInfo fi, Action<BasicShader> loadedDelegate, ShaderMacro[] macros)
        {
            var s = new BasicShader(graphics.Device);
            if (loadedDelegate != null) s.loadedEvent += loadedDelegate;
            s.shaderMacros = macros;
            s.loadAutoreload(fi);
            graphics.AddBasicShader(s);
            return s;
        }
        private void loadAutoreload(FileInfo fi)
        {
            reloadFile = fi;
            autoReload();
            var watcher = new FileSystemWatcher(fi.DirectoryName, fi.Name);

            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += delegate(object sender, FileSystemEventArgs e)
                                   {
                                       lock (this)
                                       {
                                           reloadScheduled = true;
                                       }
                                   };
            watcher.EnableRaisingEvents = true;

            watcher = new FileSystemWatcher(CompiledShaderCache.Current.RootShaderPath, "*");
            watcher.IncludeSubdirectories = true;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += delegate(object sender, FileSystemEventArgs e)
            {
                lock (this)
                {
                    if (shaderDependencies.Contains(e.Name.Replace("\\", "/")))
                        reloadScheduled = true;
                }
            };
            watcher.EnableRaisingEvents = true;
        }

        private void loadCloned(BasicShader baseShader)
        {
            ParentShader = baseShader;
            Effect = ParentShader.Effect.Clone(false);
            if (ParentShader.CurrentTechnique != null)
                SetTechnique(ParentShader.CurrentTechnique.Description.Name);


        }

      

        private void loadFromFXFile(string filename, ShaderMacro[] shaderMacros)
        {
            
            ShaderBytecode bytecode = null;

            for (; ; )
            {
                DateTime lastWrite = File.GetLastWriteTime(filename);

                try
                {
                    bytecode = CompiledShaderCache.Current.CompileFromFile(filename, "fx_5_0", shaderMacros);
                    shaderDependencies = CompiledShaderCache.Current.LastCompiledFiles;
                    break;
                }
                catch (Exception ex)
                {
                    if (Effect != null)
                    {
                        Console.WriteLine("Autoreload failed!");
                        Console.WriteLine(ex.Message);
                        return;
                    }
                    Console.WriteLine("Shader cannot be loaded. Attempt to reload in 3 sec...");
                    Console.WriteLine(ex.Message);

                    //while (lastWrite == File.GetLastWriteTime(filename))
                    {
                        System.Threading.Thread.Sleep(3000);


                    }
                }
            }


            /*if (compiledEffect.Success == false)
            {
                Console.WriteLine("Shader compile error in BasicShader. Error message is as follows:\n" +
                                  compiledEffect.ErrorsAndWarnings);
                if (Effect == null)
                    throw new Exception("Failed to compile shader. Error message is as follows:\n" +
                                        compiledEffect.ErrorsAndWarnings);

                return;
            }*/



            // Dispose old shader
            disposeEffect();
            Effect = new Effect(device, bytecode);

            LoadParameters();

            //return shader;
        }

        private void disposeEffect()
        {
            if (Effect != null)
            {
                Effect.Dispose();
                Effect = null;
            }
            CurrentTechnique = null;
        }

        // Load()



        private void autoReload()
        {
            string techniqueName = null;
            if (CurrentTechnique != null) techniqueName = CurrentTechnique.Description.Name;
            loadFromFXFile(reloadFile.FullName, shaderMacros);
            if (techniqueName != null)
                SetTechnique(techniqueName);

            onLoaded();

            //Reload cloned shaders
            foreach (var clone in clones)
            {
                clone.loadCloned(this);
            }


        }

        private event Action<BasicShader> loadedEvent;

        private void onLoaded()
        {
            if (loadedEvent != null) loadedEvent(this);
        }


        /// <summary>
        /// This method applies the first past of the current technique to the device's immediatecontext
        /// </summary>
        public void Apply()
        {
            // Render all passes (usually just one)
            var pass = CurrentTechnique.GetPassByIndex(0);
            pass.Apply(context);
        }

        public EffectPass GetCurrentPass(int index)
        {
            return CurrentTechnique.GetPassByIndex(index);
        }

        protected EffectTechnique CurrentTechnique { get; private set; }

        // Render(passName, renderDelegate)

        public void SetTechnique(string techniqueName)
        {
            if (CurrentTechnique != null && CurrentTechnique.Description.Name == techniqueName) return;
            EffectTechnique technique = GetTechnique(techniqueName);
            if (technique == null) throw new InvalidOperationException("Given technique does not exists!");
            CurrentTechnique = technique;
            if (CurrentTechnique.Description.PassCount != 1)
                throw new InvalidOperationException("Only techniques with 1 pass are allowed");
        }


        /// <summary>
        /// Returns null when technique doesn't exist.
        /// </summary>
        /// <param name="techniqueName"></param>
        public EffectTechnique GetTechnique(string techniqueName)
        {

            var ret = Effect.GetTechniqueByName(techniqueName);
            if (!ret.IsValid) return null;
            return ret;
        }








        //TODO: encapsulate this to allow full auto-reload
        /*public EffectParameter GetParameter(string parameterName)
        {
            EffectParameter parameter = effect.Parameters[parameterName];

            return parameter;
        }*/


        //private void SetCameraParameters( ICamera cam )
        //{
        //    ViewProjection = cam.ViewProjection;
        //    ViewInverse = cam.ViewInverse;


        //}



        protected virtual void LoadParameters()
        {
            if (Effect == null) throw new InvalidOperationException("Impossible! No effect has been loaded.");


        }

        //TODO: this must be called
        public void Update()
        {
            lock (this)
            {
                if (!reloadScheduled) return;
                Console.WriteLine("Auto reload shader: " + reloadFile.FullName);
                autoReload();
                reloadScheduled = false;
            }
        }




        #region IDisposable Members

        public void Dispose()
        {
            Effect.Dispose();
            Effect = null;
        }

        #endregion
    }
}

