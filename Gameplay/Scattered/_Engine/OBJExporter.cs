using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using NSubstitute;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered._Engine
{
    /// <summary>
    /// TODO: WARNING: WILL probably cause problems when using numbers in the meshes larger than 1000 due to number seperators
    /// </summary>
    public class OBJExporter
    {
        public interface IMesh
        {
            IEnumerable<IMeshPart> Parts { get; }
        }

        public interface IMeshPart
        {
            FileInfo DiffuseTexture { get; }
            Vector3[] Positions { get; }
            Vector3[] Normals { get; }
            Vector2[] Texcoords { get; }
        }

        public IMesh ConvertFromTWMesh(TheWizards.Rendering.IMesh mesh)
        {
            var objMesh = Substitute.For<OBJExporter.IMesh>();
            objMesh.Parts.Returns(mesh.GetCoreData().Parts.Select(part =>
                {
                    var ret = Substitute.For<OBJExporter.IMeshPart>();
                    ret.DiffuseTexture.Returns(part.MeshMaterial.DiffuseMap == null ? null : new FileInfo(part.MeshMaterial.DiffuseMap.GetCoreData().DiskFilePath));
                    var transformation = part.ObjectMatrix.dx();
                    ret.Positions.Returns(
                        Vector3.TransformCoordinate(
                            part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Position).Select(
                                v => v.dx()).ToArray(),
                            ref transformation));
                    ret.Normals.Returns(
                        Vector3.TransformNormal(
                            part.MeshPart.GetGeometryData().GetSourceVector3(MeshPartGeometryData.Semantic.Normal).Select(
                                v => v.dx()).ToArray(),
                            ref transformation));
                    ret.Texcoords.Returns(
                        part.MeshPart.GetGeometryData().GetSourceVector2(MeshPartGeometryData.Semantic.Texcoord).Select(
                            v => v.ToSlimDX()).ToArray());
                    return ret;
                }));
            return objMesh;
        }

        public void SaveToFile(IMesh mesh, string filename)
        {

            var mathFileName = Path.GetFileNameWithoutExtension(filename) + ".mtl";
            var mapsPath = Path.GetDirectoryName(filename) + "\\maps";
            Directory.CreateDirectory(mapsPath);


            using (var wr = File.CreateText(filename))
            using (var matwr = File.CreateText(Path.ChangeExtension(filename, "mtl")))
                SaveToFile(mesh, wr, matwr, mathFileName, mapsPath, "maps");
        }

        public void SaveToFile(IMesh mesh, StreamWriter objWr, StreamWriter matWr, string relativeMatPath, string mapsAbsolutePath, string mapsRelativePath)
        {
            saveObj(mesh, objWr, relativeMatPath);
            saveMtl(mesh, matWr, mapsAbsolutePath, mapsRelativePath);
        }

        private void saveMtl(IMesh mesh, StreamWriter matWr, string mapsAbsolutePath, string mapsRelativePath)
        {
            var num = -1;
            foreach (var part in mesh.Parts)
            {
                num++;
                matWr.WriteLine("newmtl Material{0}", num);
                if (part.DiffuseTexture != null && part.DiffuseTexture.Exists)
                {
                    var target = mapsAbsolutePath + "\\" + Path.GetFileName(part.DiffuseTexture.FullName);
                    if (!File.Exists(target))
                        part.DiffuseTexture.CopyTo(target, false);
                    matWr.WriteLine("   map_Kd {1}/{0}", part.DiffuseTexture.Name, mapsRelativePath);
                }
            }
        }

        private static void saveObj(IMesh mesh, StreamWriter wr, string mtlRelativePath)
        {
            var cult = CultureInfo.GetCultureInfo("en-US").NumberFormat;

            wr.WriteLine("mtllib {0}", mtlRelativePath);

            mesh.Parts.SelectMany(p => p.Positions).ForEach(p => wr.WriteLine(string.Format(cult, "v {0:F4} {1:F4} {2:F4}", p.X, p.Y, p.Z)));
            mesh.Parts.SelectMany(p => p.Normals).ForEach(p => wr.WriteLine(string.Format(cult, "vn {0:F4} {1:F4} {2:F4}", p.X, p.Y, p.Z)));
            mesh.Parts.SelectMany(p => p.Texcoords).ForEach(p => wr.WriteLine(string.Format(cult, "vt {0:F4} {1:F4} {2:F4}", p.X, p.Y, 0)));

            var num = -1;
            var totalCount = 1;
            foreach (var part in mesh.Parts)
            {
                num++;
                wr.WriteLine("g Part{0}", num);
                wr.WriteLine("usemtl Material{0}", num);

                for (int i = totalCount; i < part.Positions.Length + totalCount; i += 3)
                {
                    wr.WriteLine("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", i, i + 1, i + 2);
                }
                totalCount += part.Positions.Length;
            }
        }

        private TextWriter textWriter;

        private int count = 0;
        private int totalVertexCount = 0;
        //public void ExportToObj(string fileName, List<TangentVertex> vertices)
        //{
        //    count = 0;
        //    textWriter = new StreamWriter(fileName);
        //    WriteObject(vertices);
        //    textWriter.Close();
        //}
        //public void ExportToObj(string fileName, TreeEngine.EngineTreeRenderData renderData)
        //{
        //    count = 0;
        //    totalVertexCount = 0;
        //    textWriter = new StreamWriter(fileName);
        //    WriteObject(renderData.TreeBody.Vertices1);
        //    count++;
        //    for (int i = 0; i < renderData.Leaves.Count; i++)
        //    {
        //        WriteObject(renderData.Leaves[i].Vertices1);
        //        count++;
        //    }
        //    textWriter.Close();
        //}
        private void WriteObject(List<TangentVertex> vertices)
        {
            int vertexCount = vertices.Count;
            textWriter.WriteLine('#');
            textWriter.WriteLine("#object Object" + count.ToString());
            textWriter.WriteLine('#');
            textWriter.WriteLine("");

            for (int i = 0; i < vertexCount; i++)
            {
                textWriter.WriteLine(("v  " + vertices[i].pos.X.ToString("F4") + " " + vertices[i].pos.Y.ToString("F4") + " " + vertices[i].pos.Z.ToString("F4")).Replace(',', '.'));
            }
            textWriter.WriteLine("# " + vertexCount.ToString() + " Vertices");
            textWriter.WriteLine("");

            for (int i = 0; i < vertexCount; i++)
            {
                textWriter.WriteLine(("vn " + (vertices[i].normal.X).ToString("F4") + " " + (vertices[i].normal.Y).ToString("F4") + " " + (vertices[i].normal.Z).ToString("F4")).Replace(',', '.'));
            }
            textWriter.WriteLine("# " + vertexCount.ToString() + " vertex normals");
            textWriter.WriteLine("");

            for (int i = 0; i < vertexCount; i++)
            {
                textWriter.WriteLine(("vt " + vertices[i].U.ToString("F4") + " " + vertices[i].V.ToString("F4") + " " + "0.0000").Replace(',', '.'));
            }

            textWriter.WriteLine("g Object" + count.ToString());

            for (int i = 0; i < vertexCount / 3; i++)
            {
                //for some reasen 3dsmax uses an other cullmode than I do
                textWriter.WriteLine("f " + (totalVertexCount + i * 3 + 1).ToString() + '/' + (totalVertexCount + i * 3 + 1).ToString() + '/' + (totalVertexCount + i * 3 + 1).ToString() + ' ' + (totalVertexCount + i * 3 + 3).ToString() + '/' + (totalVertexCount + i * 3 + 3).ToString() + '/' + (totalVertexCount + i * 3 + 3).ToString() + ' ' + (totalVertexCount + i * 3 + 2).ToString() + '/' + (totalVertexCount + i * 3 + 2).ToString() + '/' + (totalVertexCount + i * 3 + 2).ToString());
            }
            textWriter.WriteLine("# " + (vertexCount / 3).ToString() + " faces");

            totalVertexCount += vertexCount;
        }

    }
}