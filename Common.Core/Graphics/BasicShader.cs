using System;
using System.IO;
using System.Reflection;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.Graphics
{
    /// <summary>
    /// This class supports automatic reloading
    /// TODO: store all SetParameter calls in RAM, so that a reload auto-sets the parameters
    /// Auto-reload is only partially implemented is will be buggy
    /// </summary>
    public class BasicShader : IDisposable
    {
        private IXNAGame game;

        public IXNAGame Game
        {
            get { return game; }
        }

        public Effect effect;
        public Effect Effect
        { get { return effect; } }
        private Matrix world;
        private Matrix viewProjection;
        private Matrix viewInverse;
        private Matrix worldViewProjection;

        private EffectParameter worldParam;
        private EffectParameter viewProjectionParam;
        private EffectParameter viewInverseParam;
        private EffectParameter worldViewProjectionParam;

        private string currentTechnique;

        private string reloadFilePath;
        private EffectPool reloadPool;

        private bool reloadScheduled;

        protected BasicShader(IXNAGame nGame)
        {
            game = nGame;
            game.AddBasicShader(this);
        }

        public BasicShader Clone()
        {
            BasicShader clone = new BasicShader(game);
            clone.effect = effect.Clone(game.GraphicsDevice);
            clone.LoadParameters();

            return clone;

            //TODO: maybe also set the world, viewprojection, etc params to be identical, but probably not necessary
        }


        public static BasicShader LoadFromFXFile(IXNAGame game, System.IO.Stream strm, EffectPool pool)
        {
            BasicShader s = new BasicShader(game);
            s.LoadFromFXFile(strm, pool);

            return s;
        }
        public static BasicShader LoadFromFXFile(IXNAGame game, IGameFile file)
        {
            return LoadFromFXFile(game, file, null);
        }

        /// <summary>
        /// Load an effect from an hlsl .fx file
        /// </summary>
        public static BasicShader LoadFromFXFile(IXNAGame game, IGameFile file, EffectPool pool)
        {
            BasicShader s = new BasicShader(game);
            s.LoadFromFXFile(file, pool);

            return s;
        }

        protected void LoadFromFXFile(IGameFile file)
        {
            LoadFromFXFile(file, null);
        }

        /// <summary>
        /// Load an effect from an hlsl .fx file
        /// </summary>
        protected void LoadFromFXFile(IGameFile file, EffectPool pool)
        {
            //BasicShader shader = new BasicShader( game );

            //obs?
            // Dispose old shader
            if (effect != null)
            {
                effect.Dispose();
                effect = null;
            }
            CompiledEffect compiledEffect;
            try
            {
                compiledEffect = Effect.CompileEffectFromFile(file.GetFullFilename(), null, null, CompilerOptions.None, TargetPlatform.Windows);


                if (compiledEffect.Success == false)
                {
                    Console.WriteLine("Shader compile error in BasicShader. Error message is as follows:\n" +
                                      compiledEffect.ErrorsAndWarnings);
                    throw new Exception("Failed to compile shader. Error message is as follows:\n" +
                                        compiledEffect.ErrorsAndWarnings);
                }
                effect = new Effect(game.GraphicsDevice, compiledEffect.GetEffectCode(), CompilerOptions.None, pool);
            } // try
            catch (Exception ex)
            {
                /*Log.Write( "Failed to load shader " + shaderContentName + ". " +
                    "Error: " + ex.ToString() );*/
                // Rethrow error, app can't continue!
                throw ex;
            }




            LoadParameters();

            //return shader;
        } // Load()

        /// <summary>
        /// Load an effect from an hlsl .fx file
        /// </summary>
        protected void LoadFromFXFile(System.IO.Stream strm, EffectPool pool)
        {


            CompiledEffect compiledEffect;

            using (var reader = new System.IO.StreamReader(strm))
            {
                string source = reader.ReadToEnd();
                compiledEffect = Effect.CompileEffectFromSource(source, null, null, CompilerOptions.Debug, TargetPlatform.Windows);



            }
            if (compiledEffect.Success == false)
            {
                Console.WriteLine("Shader compile error in BasicShader. Error message is as follows:\n" +
                                  compiledEffect.ErrorsAndWarnings);
                if (effect == null)
                    throw new Exception("Failed to compile shader. Error message is as follows:\n" +
                                        compiledEffect.ErrorsAndWarnings);

                return;
            }

            // Dispose old shader
            if (effect != null)
            {
                effect.Dispose();
                effect = null;
            }
            effect = new Effect(game.GraphicsDevice, compiledEffect.GetEffectCode(), CompilerOptions.None, pool);

            LoadParameters();

            //return shader;
        } // Load()


        public static BasicShader LoadFromEmbeddedFile(IXNAGame game, Assembly assembly, string manifestResource, string sourceFileRelativePath, EffectPool pool)
        {
            var afi = new FileInfo(assembly.Location);
            FileInfo fi = new FileInfo(afi.DirectoryName + "\\" + sourceFileRelativePath);
            if (fi.Exists)
            {
                Console.WriteLine("Loading debug shader: " + sourceFileRelativePath);
                var shader = new BasicShader(game);
                shader.loadFromFXFileAutoReload(fi.FullName, pool);
                return shader;
            }
            using (var strm = assembly.GetManifestResourceStream(manifestResource))
            {
                if (strm == null)
                    throw new InvalidOperationException("Embedded resource not found!");
                return LoadFromFXFile(game, strm, pool);

            }
        }

        private void loadFromFXFileAutoReload(string path, EffectPool pool)
        {
            reloadFilePath = path;
            reloadPool = pool;
            autoReload();
            var fi = new FileInfo(path);
            var watcher = new FileSystemWatcher(fi.DirectoryName, fi.Name);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += delegate
                                   {
                                       lock (this)
                                       {
                                           reloadScheduled = true;
                                       }
                                   };
            watcher.EnableRaisingEvents = true;
        }

        private void autoReload()
        {
            using (var fs = new FileStream(reloadFilePath, FileMode.Open))
            {
                LoadFromFXFile(fs, reloadPool);
            }
            if (currentTechnique != null)
            {
                var tech = GetTechnique(currentTechnique);
                if (tech != null)
                    effect.CurrentTechnique = tech;
            }

        }

        public delegate void RenderDelegate();

        /// <summary>
        /// Render using this shader.
        /// </summary>
        /// <param name="setMat">Set matrix</param>
        /// <param name="techniqueName">Technique name</param>
        /// <param name="renderDelegate">Render delegate</param>
        public void RenderMultipass(RenderDelegate renderDelegate)
        {
            //SetCameraParameters( game.Camera );
            // Start shader
            //effect.CurrentTechnique = effect.Techniques[ techniqueName ];
            effect.Begin(SaveStateMode.None);

            // Render all passes (usually just one)
            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            for (int num = 0; num < effect.CurrentTechnique.Passes.Count; num++)
            {
                EffectPass pass = effect.CurrentTechnique.Passes[num];

                pass.Begin();
                renderDelegate();
                pass.End();
            } // foreach (pass)

            // End shader
            effect.End();
        } // Render(passName, renderDelegate)

        public void SetTechnique(string techniqueName)
        {
            if (currentTechnique == techniqueName) return;
            EffectTechnique technique = GetTechnique(techniqueName);
            if (technique == null) throw new InvalidOperationException("Given technique does not exists!");
            effect.CurrentTechnique = technique;
            currentTechnique = techniqueName;
        }


        /// <summary>
        /// Returns null when technique doesn't exist.
        /// </summary>
        /// <param name="techniqueName"></param>
        public EffectTechnique GetTechnique(string techniqueName)
        {
            EffectTechnique technique = effect.Techniques[techniqueName];
            return technique;
        }

        public void SetParameter(string parameterName, Matrix value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value);
        }
        public void SetParameter(string parameterName, Vector2 value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value);
        }
        public void SetParameter(string parameterName, Vector3 value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value);
        }
        public void SetParameter(string parameterName, TWTexture value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value.XnaTexture);
        }
        public void SetParameter(string parameterName, Texture2D value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value);
        }
        public void SetParameter(string parameterName, float value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value);
        }
        public void SetParameter(string parameterName, Color value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value.ToVector4());
        }
        public void SetParameter(string parameterName, Vector4 value)
        {
            EffectParameter parameter = GetParameter(parameterName);
            if (parameter == null) throw new InvalidOperationException("Given parameter does not exist!");
            parameter.SetValue(value);
        }




        public void SaveToXML(TWXmlNode node)
        {


        }

        public void LoadFromXML(TWXmlNode node)
        {
        }










        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public EffectParameter GetParameter(string parameterName)
        {
            EffectParameter parameter = effect.Parameters[parameterName];

            return parameter;
        }


        //private void SetCameraParameters( ICamera cam )
        //{
        //    ViewProjection = cam.ViewProjection;
        //    ViewInverse = cam.ViewInverse;


        //}

        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="setMatrix">Set matrix</param>
        protected void SetValue(EffectParameter param,
            ref Matrix lastUsedMatrix, Matrix newMatrix)
        {
            if (param == null) return;
            /*obs, always update, matrices change every frame anyway!
             * matrix compare takes too long, it eats up almost 50% of this method.
            if (param != null &&
                lastUsedMatrix != newMatrix)
             */
            {
                lastUsedMatrix = newMatrix;
                param.SetValue(newMatrix);
            } // if (param)
        } // SetValue(param, setMatrix)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedVector">Last used vector</param>
        /// <param name="newVector">New vector</param>
        protected void SetValue(EffectParameter param,
            ref Vector3 lastUsedVector, Vector3 newVector)
        {
            if (param != null &&
                lastUsedVector != newVector)
            {
                lastUsedVector = newVector;
                param.SetValue(newVector);
            } // if (param)
        } // SetValue(param, lastUsedVector, newVector)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedColor">Last used color</param>
        /// <param name="newColor">New color</param>
        protected void SetValue(EffectParameter param,
            ref Color lastUsedColor, Color newColor)
        {
            // Note: This check eats few % of the performance, but the color
            // often stays the change (around 50%).
            if (param != null &&
                //slower: lastUsedColor != newColor)
                lastUsedColor.PackedValue != newColor.PackedValue)
            {
                lastUsedColor = newColor;
                //obs: param.SetValue(ColorHelper.ConvertColorToVector4(newColor));
                param.SetValue(newColor.ToVector4());
            } // if (param)
        } // SetValue(param, lastUsedColor, newColor)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedValue">Last used value</param>
        /// <param name="newValue">New value</param>
        protected void SetValue(EffectParameter param,
            ref float lastUsedValue, float newValue)
        {
            if (param != null &&
                lastUsedValue != newValue)
            {
                lastUsedValue = newValue;
                param.SetValue(newValue);
            } // if (param)
        } // SetValue(param, lastUsedValue, newValue)
        /// <summary>
        /// Set value helper to set an effect parameter.
        /// </summary>
        /// <param name="param">Param</param>
        /// <param name="lastUsedValue">Last used value</param>
        /// <param name="newValue">New value</param>
        protected void SetValue(EffectParameter param,
            ref TWTexture lastUsedValue, TWTexture newValue)
        {
            if (param != null &&
                lastUsedValue != newValue)
            {
                lastUsedValue = newValue;
                param.SetValue(newValue.XnaTexture);
            } // if (param)
        } // SetValue(param, lastUsedValue, newValue)


        protected virtual void LoadParameters()
        {
            if (effect == null) throw new InvalidOperationException("Impossible! No effect has been loaded.");

            worldParam = effect.Parameters["world"];
            viewProjectionParam = effect.Parameters["viewProjection"];
            viewInverseParam = effect.Parameters["viewInverse"];
            worldViewProjectionParam = effect.Parameters["worldViewProjection"];


        }


        public void Update()
        {
            lock (this)
            {
                if (!reloadScheduled) return;
                Console.WriteLine("Auto reload shader: " + reloadFilePath);
                autoReload();
                reloadScheduled = false;
            }
        }


        public Matrix World
        {
            get
            { return world; }
            set
            { SetValue(worldParam, ref world, value); }
        }

        public Matrix ViewProjection
        {
            get
            { return viewProjection; }
            set
            { SetValue(viewProjectionParam, ref viewProjection, value); }
        }

        public Matrix ViewInverse
        {
            get
            { return viewInverse; }
            set
            { SetValue(viewInverseParam, ref viewInverse, value); }
        }

        public Matrix WorldViewProjection
        {
            get
            { return worldViewProjection; }
            set
            { SetValue(worldViewProjectionParam, ref worldViewProjection, value); }
        }


        #region IDisposable Members

        public void Dispose()
        {
            effect.Dispose();
            effect = null;
        }

        #endregion
    }
}

