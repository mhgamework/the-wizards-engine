using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SlimDX;
using SlimDX.D3DCompiler;

namespace MHGameWork.TheWizards.Graphics.SlimDX.DirectX11.Graphics
{
    /// <summary>
    /// This class provides compiled shader code. It uses shader filename, fx effect, and macros to parameterize a shader. These parameters are hashed and used
    /// to identify the compiled code.
    /// 
    /// Note: Shaders
    /// </summary>
    public class CompiledShaderCache
    {
        private static string GlobalShaderPath;

        static CompiledShaderCache()
        {
            GlobalShaderPath = new DirectoryInfo("../../Graphics.SlimDX/Shaders/").FullName;
            Current = new CompiledShaderCache();
            Current.disableCache = true; //TODO: test this with autoreloading...

        }
        public static CompiledShaderCache Current { get; private set; }

        private CompiledShaderCache()
        {
        }

        private bool disableCache = false;

        /// <summary>
        /// This returns the root directory which contains all shader.fx files (used for autoreloading)
        /// </summary>
        public string RootShaderPath
        {
            get { return GlobalShaderPath; }
        }

        /// <summary>
        /// Returns a list of the files used for the last compile job. 
        /// This should be used when caching is disabled, and is used for autoreloading purposes
        /// </summary>
        public string[] LastCompiledFiles { get; private set; }

        public ShaderBytecode CompileFromFile(string filename, string fx, ShaderMacro[] shaderMacros)
        {
            //WARNING: using ShaderFlags.SkipOptimization simply causes compiler to go berserk when using functions, they get skipped or smth
            //var bytecodeOri = ShaderBytecode.CompileFromFile(filename, "fx_5_0",
            //                               ShaderFlags.WarningsAreErrors | ShaderFlags.SkipOptimization |
            //                               ShaderFlags.Debug, EffectFlags.None, null, includeHandler);
            var includeHandler = new IncludeHandler();
            var compileParams = new CompileParams(filename, "fx_5_0", ShaderFlags.OptimizationLevel3 | ShaderFlags.SkipValidation, EffectFlags.None, shaderMacros, includeHandler);
            //var compileParams = new CompileParams(filename, "fx_5_0", ShaderFlags.WarningsAreErrors | ShaderFlags.EnableStrictness | ShaderFlags.Debug, EffectFlags.None, shaderMacros, includeHandler);
            var ret = CompileFromFile(compileParams);

            LastCompiledFiles = includeHandler.IncludedFiles.ToArray();

            return ret;
        }


        public ShaderBytecode CompileFromFile(CompileParams compileParams)
        {
            //TODO: check if filename is in a subfolder of RootShaderPath!!
            if (!FileHelper.IsFileInDirectory(new FileInfo(compileParams.Filename), new DirectoryInfo(RootShaderPath)))
                throw new InvalidOperationException("Shader should be under the RootShaderPath!!");

            if (!isShaderUpToDate(compileParams))
            {
                var code = ShaderBytecode.CompileFromFile(compileParams.Filename, compileParams.FX, compileParams.ShaderFlags, compileParams.EffectFlags, compileParams.ShaderMacros, compileParams.IncludeHandler);
                saveBytecodeToFile(code, compileParams.CacheFilename);
            }

            return loadBytecodeFromFile(compileParams.CacheFilename);
        }

        private void saveBytecodeToFile(ShaderBytecode code, string filename)
        {
            var data = new byte[code.Data.Length];

            code.Data.Read(data, 0, (int)code.Data.Length);

            File.WriteAllBytes(filename, data);
        }

        private ShaderBytecode loadBytecodeFromFile(string filename)
        {
            var data = File.ReadAllBytes(filename);

            var strm = new DataStream(data.Length, true, true);
            strm.Write(data, 0, data.Length);
            strm.Position = 0;

            return new ShaderBytecode(strm);
        }

        private bool isShaderUpToDate(CompileParams p)
        {
            if (disableCache) return false;
            return File.Exists(p.CacheFilename);
        }





        public class IncludeHandler : Include
        {
            public List<string> IncludedFiles = new List<string>();


            public void Open(IncludeType type, string fileName, Stream parentStream, out Stream stream)
            {
                if (type != IncludeType.System)
                    throw new NotImplementedException();
                IncludedFiles.Add(fileName);
                var fs = File.OpenRead(GlobalShaderPath + fileName);
                stream = fs;
            }
            public void Close(Stream stream)
            {
                stream.Close();
            }
        }
        public struct CompileParams
        {
            private string filename;
            private string fx;
            private ShaderFlags shaderFlags;
            private EffectFlags effectFlags;
            private ShaderMacro[] shaderMacros;
            private IncludeHandler includeHandler;
            private string cacheFilename;

            public CompileParams(string filename, string fx, ShaderFlags shaderFlags, EffectFlags effectFlags, ShaderMacro[] shaderMacros, IncludeHandler includeHandler)
            {
                this.filename = filename;
                this.fx = fx;
                this.shaderFlags = shaderFlags;
                this.effectFlags = effectFlags;
                this.shaderMacros = shaderMacros;
                this.includeHandler = includeHandler;
                this.cacheFilename = "";

                cacheFilename = getBytecodeFilename();
            }

            private string getBytecodeFilename()
            {

                //TODO: if macro's are in different order, the hash will be falsely different
                var hashData = "";
                hashData += fx;
                if (ShaderMacros != null)
                    hashData = ShaderMacros.Aggregate("", (current, macro) => current + (macro.Name ?? "[NULL]" + macro.Value ?? "[NULL]"));


                var hasher = SHA1.Create();

                using (var strm = new MemoryStream(Encoding.ASCII.GetBytes(hashData)))
                    hasher.ComputeHash(strm);

                return string.Format("{0}\\{1}-{2}.bin",
                                     TWDir.Cache.CreateSubdirectory("Shaders"),
                                     FileHelper.ExtractFilename(filename, true),
                                     hasher.Hash.Aggregate("", (current, b) => current + b.ToString("X2"))
                                     );
            }

            public string CacheFilename
            {
                get { return cacheFilename; }
            }

            public string Filename
            {
                get { return filename; }
            }

            public string FX
            {
                get { return fx; }
            }

            public ShaderFlags ShaderFlags
            {
                get { return shaderFlags; }
            }

            public EffectFlags EffectFlags
            {
                get { return effectFlags; }
            }

            public ShaderMacro[] ShaderMacros
            {
                get { return shaderMacros; }
            }

            public IncludeHandler IncludeHandler
            {
                get { return includeHandler; }
            }
        }

    }
}
