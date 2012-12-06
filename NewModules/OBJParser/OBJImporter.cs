using System;
using System.Collections.Generic;
using System.Globalization;
using MHGameWork.TheWizards.Entity;
using System.IO;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace MHGameWork.TheWizards.OBJParser
{
    public class ObjImporter
    {
        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector3> TexCoords { get; set; }
        private RAMMesh mesh;
        private RAMMeshPart meshPart;

        public List<OBJGroup> Groups { get; set; }
        public List<OBJMaterial> Materials { get; set; }

        private OBJGroup activeGroup;
        private OBJMaterial activeMaterial;
        private OBJGroup.SubObject activeSubObject;

        private Dictionary<string, Stream> materialFileStreams = new Dictionary<string, Stream>();
        CultureInfo culture = new CultureInfo("en-US");

        private string importingFile;

        /// <summary>
        /// Given stream will be used by the next ImportObjFile call to access the material file.
        /// After an ImportObjFile call these streams are erased, so they need to be added before every ImportObjFile call
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="stream"></param>
        public void AddMaterialFileStream(string filename, Stream stream)
        {
            materialFileStreams.Add(filename, stream);
        }

        public void ImportObjFile(string FileName)
        {
            importingFile = FileName;
            using (FileStream fileStream = new FileStream(FileName, FileMode.Open))
            {
                ImportObjFile(fileStream);
            }
            importingFile = null;

        }
        public void ImportObjFile(Stream fileStream)
        {
            StreamReader textReader = new StreamReader(fileStream);

            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TexCoords = new List<Vector3>();
            mesh = new RAMMesh();

            meshPart = new RAMMeshPart();
            Groups = new List<OBJGroup>();

            Materials = new List<OBJMaterial>();

            activeGroup = null;



            Vertices.Add(Vector3.Zero); // Indices following are 1 indexed, so add a dummy element as 0-th index
            Normals.Add(Vector3.Zero);
            TexCoords.Add(Vector3.Zero);


            /*List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> textureCoords = new List<Vector2>();*/


            string line = null;
            while (textReader.Peek() != -1)
            {
                line = textReader.ReadLine();
                //line = line.Replace('.', ',');
                if (line.StartsWith("#"))
                    continue;
                string[] pieces = line.Split(' ');

                string cmd = pieces[0];


                int dataStart = 1;
                while (pieces.Length > dataStart && pieces[dataStart] == "")
                {
                    dataStart++;
                }
                switch (cmd)
                {
                    case "v":
                        Vector3 v;
                        v.X = float.Parse(pieces[dataStart + 0], culture.NumberFormat);
                        v.Y = float.Parse(pieces[dataStart + 1], culture.NumberFormat);
                        v.Z = float.Parse(pieces[dataStart + 2], culture.NumberFormat);
                        Vertices.Add(v);
                        break;
                    case "vn":
                        Vector3 vn;
                        vn.X = float.Parse(pieces[dataStart + 0], culture.NumberFormat);
                        vn.Y = float.Parse(pieces[dataStart + 1], culture.NumberFormat);
                        vn.Z = float.Parse(pieces[dataStart + 2], culture.NumberFormat);
                        Normals.Add(vn);
                        break;
                    case "vt":
                        Vector3 vt;
                        vt.X = float.Parse(pieces[dataStart + 0], culture.NumberFormat);
                        vt.Y = float.Parse(pieces[dataStart + 1], culture.NumberFormat);
                        vt.Z = float.Parse(pieces[dataStart + 2], culture.NumberFormat);
                        TexCoords.Add(vt);
                        break;
                    case "g":

                        activeGroup = getOrCreateGroup(pieces[dataStart]);
                        updateActiveSubObject();
                        /*if (meshPart.AdditionalData.Name != null || meshPart.AdditionalData.Name == "")
                        {
                            meshPart = new EditorMeshPart();
                            meshPart.AdditionalData.Name = pieces[1];
                            createMeshPart();
                        }
                        meshPart = new EditorMeshPart();
                        meshPart.AdditionalData.Name = pieces[1];*/
                        break;
                    case "usemtl":
                        activeMaterial = GetOrCreateMaterial(pieces[dataStart]);
                        updateActiveSubObject();
                        break;
                    case "f":
                        Face face = new Face();

                        //TODO: check first index
                        face.V1 = ParseFaceVertex(pieces[dataStart + 0]);
                        face.V2 = ParseFaceVertex(pieces[dataStart + 1]);
                        face.V3 = ParseFaceVertex(pieces[dataStart + 2]);

                        if (activeSubObject == null)
                            throw new InvalidOperationException(
                                "A face was found that does not seem to belong to any object??");

                        activeSubObject.Faces.Add(face);


                        break;
                    case "mtllib":
                        Stream matStream;
                        if (materialFileStreams.TryGetValue(pieces[dataStart], out matStream))
                            importMaterialFile(matStream);
                        else
                            Console.WriteLine("OBJ Material Library not found: " + pieces[dataStart]);

                        break;
                }

            }

            //createMeshPart();


            //return mesh;


            materialFileStreams = new Dictionary<string, Stream>();
        }

        private void importMaterialFile(Stream fileStream)
        {
            StreamReader textReader = new StreamReader(fileStream);

            OBJMaterial readingMaterial = null;


            string line = null;
            while (textReader.Peek() != -1)
            {
                //TODO: remove tabs!!!
                //TODO: make a seperate reader for this kind of file
                line = textReader.ReadLine().Trim(new[] { ' ', '\t' });

                //line = line.Replace('.', ',');
                if (line.StartsWith("#"))
                    continue;
                string[] pieces = line.Split(' ');

                string cmd = pieces[0];


                int dataStart = 1;
                while (pieces.Length > dataStart && pieces[dataStart] == "")
                {
                    dataStart++;
                }
                switch (cmd)
                {
                    case "newmtl":
                        readingMaterial = GetOrCreateMaterial(pieces[dataStart]);
                        break;
                    case "Ns":
                        if (readingMaterial == null) throw new InvalidOperationException("File format invalid!");
                        readingMaterial.SpecularExponent = float.Parse(pieces[dataStart], culture);
                        break;
                    case "Ka":
                        if (readingMaterial == null) throw new InvalidOperationException("File format invalid!");
                        readingMaterial.AmbientColor = readColor(pieces, dataStart);
                        break;
                    case "Kd":
                        if (readingMaterial == null) throw new InvalidOperationException("File format invalid!");
                        readingMaterial.DiffuseColor = readColor(pieces, dataStart);
                        break;
                    case "Ks":
                        if (readingMaterial == null) throw new InvalidOperationException("File format invalid!");
                        readingMaterial.SpecularColor = readColor(pieces, dataStart);
                        break;
                    case "map_Ka":
                        if (readingMaterial == null) throw new InvalidOperationException("File format invalid!");
                        string pathKa = line.Substring(line.IndexOf("map_Ka") + ("map_Ka").Length).Trim();
                        readingMaterial.AmbientMap = fixFileUrl(pathKa);
                        break;
                    case "map_Kd":
                        if (readingMaterial == null) throw new InvalidOperationException("File format invalid!");
                        string pathKd = line.Substring(line.IndexOf("map_Kd") + ("map_Kd").Length).Trim();
                        readingMaterial.DiffuseMap = fixFileUrl(pathKd);
                        break;
                }

            }

            //createMeshPart();


            //return mesh;


            materialFileStreams = new Dictionary<string, Stream>();
        }

        /// <summary>
        /// This is temporary
        /// </summary>
        /// <returns></returns>
        private string fixFileUrl(string path)
        {
            if (System.IO.File.Exists(path))
                return path;
            if (importingFile == null)
            {
                Console.WriteLine("Unable to fix file path (" + path + ") in OBJImporter");
                return path;
            }

            string ret;

            var importingFileInfo = new FileInfo(importingFile);
            var fi = new FileInfo(path);

            //Fix relative path
            ret = importingFileInfo.Directory.FullName + "\\" + path;
            if (File.Exists(ret)) return ret;


            /*ret = importingFileInfo.Directory.FullName + "\\" + fi.Name;
            if (File.Exists(ret)) return ret;*/


            Console.WriteLine("Unable to fix file path (" + path + ") in OBJImporter");
            return path;
        }

        private Color readColor(string[] rgbPieces, int offset)
        {
            Vector3 Ka = new Vector3();
            Ka.X = float.Parse(rgbPieces[offset + 0], culture);
            Ka.Y = float.Parse(rgbPieces[offset + 1], culture);
            Ka.Z = float.Parse(rgbPieces[offset + 2], culture);
            return new Color(Ka);

        }

        private void updateActiveSubObject()
        {
            if (activeGroup == null)
            {
                activeSubObject = null;
                return;
            }
            activeSubObject = activeGroup.GetOrCreateSubObject(activeMaterial);
        }

        public OBJMaterial GetOrCreateMaterial(string name)
        {
            for (int i = 0; i < Materials.Count; i++)
            {
                if (Materials[i].Name == name)
                    return Materials[i];
            }

            var mat = new OBJMaterial { Name = name };
            Materials.Add(mat);
            return mat;

        }


        private FaceVertex ParseFaceVertex(string vertexText)
        {
            string[] facePiece = vertexText.Split('/');
            FaceVertex fv = new FaceVertex();
            fv.Position = int.Parse(facePiece[0]);
            if (facePiece.Length == 1) return fv;
            if (facePiece[1] != "")
            {
                fv.TextureCoordinate = int.Parse(facePiece[1]);
                fv.Normal = int.Parse(facePiece[2]);
            }
            else
            {
                //fv.Normal = int.Parse(facePiece[1]);  Inconsistency between formats...
                fv.Normal = int.Parse(facePiece[2]);//TODO: This might need to bee texcoords
            }
                
            return fv;
        }

        private OBJGroup getOrCreateGroup(string name)
        {
            for (int i = 0; i < Groups.Count; i++)
            {
                if (Groups[i].Name == name) return Groups[i];
            }
            var group = new OBJGroup(name);
            Groups.Add(group);
            return group;
        }

        /*private void createMeshPart()
        {

            MeshPartGeometryData.Source s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Position;
            s.DataVector3 = Vertices.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            s = new MeshPartGeometryData.Source();
            s.Semantic = MeshPartGeometryData.Semantic.Normal;
            s.DataVector3 = Normals.ToArray();
            meshPart.GeometryData.Sources.Add(s);
            if (TexCoords.Count > 0)
            {
                s = new MeshPartGeometryData.Source();
                s.Semantic = MeshPartGeometryData.Semantic.Texcoord;
                s.DataVector2 = TexCoords.ToArray();
                meshPart.GeometryData.Sources.Add(s);
            }



            mesh.AddPart(meshPart);

            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TexCoords = new List<Vector2>();
        }*/


        /*public static void TestEditorMeshPartRenderDataSimple()
        {
            EditorMesh eMesh = new EditorMesh();
            ObjImporter objImporter = new ObjImporter();

            eMesh = objImporter.ImportObjFile(@"C:\Users\Bart\Documents\3dsmax\export\boat.obj");



            XNAGame game = new XNAGame();
            ColladaShader shader = null;
            EditorMeshPartRenderData renderData = null;



            game.InitializeEvent +=
                delegate
                {
                    renderData = new EditorMeshPartRenderData(eMesh.CoreData.Parts[0].MeshPart as EditorMeshPart);
                    renderData.Initialize(game);

                    shader = new ColladaShader(game, null);

                    shader.DiffuseColor = Color.Red.ToVector4();
                    shader.SpecularColor = Color.Green.ToVector4();
                    shader.LightDirection = new Vector3(0, 0, -1);
                    shader.Shininess = 1;
                };

            game.DrawEvent +=
                delegate
                {
                    game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
                    shader.World = Matrix.Identity;
                    shader.ViewInverse = game.Camera.ViewInverse;
                    shader.ViewProjection = game.Camera.ViewProjection;

                    shader.RenderPrimitiveSinglePass(renderData, Microsoft.Xna.Framework.Graphics.SaveStateMode.None);

                };

            game.Run();

        }
        public static void TestEditorMeshPartRenderDataComplex()
        {
            EditorMesh eMesh = new EditorMesh();
            ObjImporter objImporter = new ObjImporter();

            eMesh = objImporter.ImportObjFile(@"C:\Users\Bart\Documents\3dsmax\export\buildings.obj");



            XNAGame game = new XNAGame();
            game.SpectaterCamera.FarClip = 5000;
            ColladaShader shader = null;
            List<EditorMeshPartRenderData> renderData = new List<EditorMeshPartRenderData>();

            game.InitializeEvent +=
                delegate
                {
                    for (int i = 0; i < eMesh.CoreData.Parts.Count; i++)
                    {
                        renderData.Add(new EditorMeshPartRenderData(eMesh.CoreData.Parts[i].MeshPart as EditorMeshPart));
                        renderData[i].Initialize(game);

                    }


                    shader = new ColladaShader(game, null);

                    shader.DiffuseColor = Color.Red.ToVector4();
                    shader.SpecularColor = Color.Green.ToVector4();
                    shader.LightDirection = new Vector3(0, 0, -1);
                    shader.Shininess = 1;
                };

            game.DrawEvent +=
                delegate
                {
                    game.GraphicsDevice.RenderState.CullMode = Microsoft.Xna.Framework.Graphics.CullMode.None;
                    shader.World = Matrix.Identity;
                    shader.ViewInverse = game.Camera.ViewInverse;
                    shader.ViewProjection = game.Camera.ViewProjection;
                    for (int i = 0; i < renderData.Count; i++)
                    {
                        shader.RenderPrimitiveSinglePass(renderData[i], Microsoft.Xna.Framework.Graphics.SaveStateMode.None);

                    }
                };

            game.Run();

        }*/



    }
}
