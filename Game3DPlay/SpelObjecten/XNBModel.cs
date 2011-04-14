using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Build.BuildEngine;

namespace MHGameWork.Game3DPlay.SpelObjecten
{
    public class XNBModel : Core.SpelObject, Game3DPlay.Core.Elements.IRenderable, Game3DPlay.Core.Elements.ILoadable
    {
        public XNBModel(ISpelObject nParent)
            : base(nParent)
        {
            //_renderElement = new MHGameWork.Game3DPlay.Core.Elements.RenderElement(this);
            _relativePath = "";
        }
        //private Game3DPlay.Core.Elements.RenderElement _renderElement;

        private string _relativePath;
        public string RelativePath
        {
            get { return _relativePath; }
            set
            {
                if (_relativePath != value)
                {
                    _relativePath = value;
                    if (HoofdObj.InRun) LoadModel();
				}
            }
        }

        private Vector3 _positie;

        public Vector3 Positie
        {
            get { return _positie; }
            set { _positie = value; BuildRootMatrix(); }
        }

        private Matrix _rootMatrix;

        protected Matrix RootMatrix
        {
            get { return _rootMatrix; }
            //set { _rootMatrix = value; }
        }




        Model _xnaModel;

        /// <summary>
        /// Not Finished.
        /// </summary>
        /// <param name="nFilename"></param>
        public void BuildXModel(string nFilename)
        {

            string tempfileName = System.IO.Path.GetTempFileName();

            using (System.IO.StreamWriter w = new System.IO.StreamWriter(tempfileName, false))
            {
                w.WriteLine("<Project xmlns='http://schemas.microsoft.com/developer/msbuild/2003'>");
                w.WriteLine("<UsingTask TaskName='BuildContent' AssemblyName='Microsoft.Xna.Framework.Content.Pipeline, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6d5c3888ef60e27d' />");
                w.WriteLine("<PropertyGroup>");
                w.WriteLine("<XnaInstall>C:\\Program Files\\Microsoft XNA\\XNA Game Studio Express\\v1.0\\References\\Windows\\x86</XnaInstall>");
                w.WriteLine("</PropertyGroup>");
                w.WriteLine("<ItemGroup>");
                w.WriteLine("<PipelineAssembly Include='$(XnaInstall)\\Microsoft.Xna.Framework.Content.Pipeline.TextureImporter.dll' />");
                w.WriteLine("</ItemGroup>");
                w.WriteLine("<ItemGroup>");
                w.WriteLine("<Content Include='MyTexture.tga'>");
                w.WriteLine("<Importer>TextureImporter</Importer>");
                w.WriteLine("<Processor>SpriteTextureProcessor</Processor>");
                w.WriteLine("</Content>");
                w.WriteLine("</ItemGroup>");
                w.WriteLine("<Target Name='Build'>");
                w.WriteLine("<BuildContent SourceAssets='@(Content)' PipelineAssemblies='@(PipelineAssembly)' TargetPlatform='Windows' />");
                w.WriteLine("</Target>");
                w.WriteLine("</Project>");
            }





            // Instantiate a new Engine object
            Engine engine = new Engine();

            // Point to the path that contains the .NET Framework 2.0 CLR and tools
            //engine.BinPath = @"c:\windows\microsoft.net\framework\v2.0.xxxxx";


            // Instantiate a new FileLogger to generate build log
            FileLogger logger = new FileLogger();

            // Set the logfile parameter to indicate the log destination
            logger.Parameters = @"logfile=C:\temp\build.log";

            // Register the logger with the engine
            engine.RegisterLogger(logger);

            // Build a project file
            //bool success = engine.BuildProjectFile(@"c:\temp\validate.proj");
            bool success = engine.BuildProjectFile(@tempfileName);

            //Unregister all loggers to close the log file
            engine.UnregisterAllLoggers();

            if (success)
                Console.WriteLine("Build succeeded.");
            else
                Console.WriteLine(@"Build failed. View C:\temp\build.log for details");

        }


        public void BuildRootMatrix()
        {
            _rootMatrix = Matrix.CreateTranslation(Positie);
        }


        public void LoadModel()
        {


            if (System.IO.File.Exists(System.IO.Path.Combine(HoofdObj.XNAGame._content.RootDirectory, @RelativePath + ".xnb")) == false) return;

            //TODO: moet ik deze model nie disposen.
            //if (_xnaModel != null) { _xnaModel
            //if (HoofdObj.InRun == false) return;
            _xnaModel = HoofdObj.XNAGame._content.Load<Model>(RelativePath);
        }


        #region IRenderable Members

        public virtual void OnBeforeRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
        {

        }

        public virtual void OnRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
        {
            if (_xnaModel == null) return;
            //Copy any parent transforms
            Matrix[] transforms = new Matrix[_xnaModel.Bones.Count];
            _xnaModel.CopyAbsoluteBoneTransformsTo(transforms);

            //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in _xnaModel.Meshes)
            {
                //This is where the mesh orientation is set, as well as our camera and projection
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = transforms[mesh.ParentBone.Index] * RootMatrix; // Matrix.CreateRotationY(0)//modelRotation)
                        //* Matrix.CreateTranslation(new Vector3(0));//modelPosition);

                    effect.View = e.CameraInfo.ViewMatrix;
                    effect.Projection = e.CameraInfo.ProjectionMatrix;

                }
                //Draw the mesh, will use the effects set above.
                mesh.Draw();
            }
        }

        public virtual void OnAfterRender(object sender, MHGameWork.Game3DPlay.Core.Elements.RenderEventArgs e)
        {

        }

        #endregion

		#region ILoadable Members

		public void OnLoad(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			if (e.AllContent)
			{
				LoadModel();
			}
		}

		public void OnUnload(object sender, MHGameWork.Game3DPlay.Core.Elements.LoadEventArgs e)
		{
			
		}

		#endregion
	}
}
