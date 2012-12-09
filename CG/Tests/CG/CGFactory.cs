using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.CG;
using MHGameWork.TheWizards.CG.Cameras;
using MHGameWork.TheWizards.CG.Math;
using MHGameWork.TheWizards.CG.OBJParser;
using MHGameWork.TheWizards.CG.Raytracing;
using MHGameWork.TheWizards.CG.Raytracing.Surfaces;
using MHGameWork.TheWizards.CG.Shading;
using MHGameWork.TheWizards.CG.Spatial;
using MHGameWork.TheWizards.CG.UI;

namespace MHGameWork.TheWizards.Tests.CG
{
    public class CGFactory
    {
        private GraphicalRayTracer ui;
        private GenericTraceableScene scene;
        private IRenderedImage image;
        private ICamera cam;

        public GraphicalRayTracer CreateUI()
        {
            ui = new GraphicalRayTracer();
            return ui;
        }

        public PhongShader CreatePhong()
        {
            return new PhongShader(GetScene(), GetCamera());
        }
        public RefractionShader CreateRefraction()
        {
            return new RefractionShader(GetScene());
        }

        public PerspectiveCamera CreatePerspectiveCamera(Vector3 position, Vector3 lookAt)
        {
            var ret = new PerspectiveCamera();

            if (cam != null)
            {
                if (cam is PerspectiveCamera)
                {
                    ret = (PerspectiveCamera)cam;
                }
                else
                    throw new InvalidOperationException();
            }
            ret.Position = position;
            ret.Direction = Vector3.Normalize(lookAt - position);
            cam = ret;
            return ret;
        }


        public ICamera GetCamera()
        {
            if (cam == null) CreatePerspectiveCamera(new Vector3(0, 5, 20), new Vector3());
            return cam;
        }


        public void Run(int numThreads = 4)
        {
            if (ui == null) CreateUI();
            ui.Run(GetRenderedImage(), numThreads);

        }

        public IRenderedImage GetRenderedImage()
        {
            if (image == null)
                image = new TracedSceneImage(GetScene(), GetCamera());
            return image;
        }

        public GenericTraceableScene GetScene()
        {
            if (scene == null) CreateGenericTraceableScene();

            return scene;
        }

        public GenericTraceableScene CreateGenericTraceableScene()
        {
            var ret = new GenericTraceableScene();
            scene = ret;

            return ret;
        }


        /// <summary>
        /// Loads a mesh from an obj file, and the similarly name mat file.
        /// Textures are loaded in the texturefactory
        /// </summary>
        public RAMMesh CreateMesh(FileInfo objFile)
        {
            var converter = new OBJToRAMMeshConverter();
            var importer = new ObjImporter(); //TODO: Garbage Collector fancyness

            var materialFilePath = objFile.FullName.Substring(0, objFile.FullName.Length - objFile.Extension.Length) + ".mtl";
            var materialFileName = objFile.Name.Substring(0, objFile.Name.Length - objFile.Extension.Length) + ".mtl";
            FileStream materialFileStream = null;
            try
            {
                if (File.Exists(materialFilePath))
                {
                    materialFileStream = File.Open(materialFilePath, FileMode.Open, FileAccess.Read);
                    importer.AddMaterialFileStream(materialFileName, materialFileStream);
                }
                importer.ImportObjFile(objFile.FullName);
                return converter.CreateMesh(importer);
            }
            finally
            {
                if (materialFileStream != null) materialFileStream.Close();
                materialFileStream = null;
            }



        }

        public void CreateMeshWithGridSurface(string barrelObj)
        {
            var mesh = CreateMesh(new FileInfo(barrelObj));

            var shader = CreatePhong();


            var converter = new MeshToTriangleConverter();

            List<TriangleGeometricSurface> triangles = converter.GetTrianglesWithPhong(mesh, CreatePhong);

            var grid = new CompactGrid();
            grid.buildGrid(triangles.Select(o => (ISceneObject)new GeometrySceneObject { Shader = shader, GeometricSurface = o }).ToList());
            GetScene().AddSceneObject(new CompactGridGeometricSurface(grid));

        }
    }
}
