using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    /// <summary>
    /// Responsible for parsing .twobj files
    /// </summary>
    public class MeshParser
    {

        //Per material, list of pos, norms and texcoords
        private Dictionary<int, List<int>> positionIndices = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> normalIndices = new Dictionary<int, List<int>>();
        private Dictionary<int, List<int>> texCoordIndices = new Dictionary<int, List<int>>();

        //Lists to use for faces that have no material (default material)
        private List<int> defaultPosInd;
        private List<int> defaultNormInd;
        private List<int> defaultTexcoInd; 

        //Data streams
        private List<Material> materials;
        private Vector3[] positions;
        private Vector3[] normals;
        private Vector3[] texCoords;

        private int currentPositionID = 0;
        private int currentNormalID = 0;
        private int currentTexCoordID = 0;

        private void reset()
        {
            positionIndices = new Dictionary<int, List<int>>();
            normalIndices = new Dictionary<int, List<int>>();
            texCoordIndices = new Dictionary<int, List<int>>();
            materials = null;
            positions = null;
            normals = null;
            texCoords = null;
            currentPositionID = 0;
            currentNormalID = 0;
            currentTexCoordID = 0;
        }

        public void LoadMesh(String path, out Material[] matStream, out Vector3[] posStream, out Vector3[] normStream, out Vector3[] texcoStream, out Dictionary<int, List<int>> posInd, out Dictionary<int, List<int>> normInd, out Dictionary<int, List<int>> texcoInd)
        {
            reset();

            using (var reader = File.OpenText(path))
            {
                //reader.ReadLine().Split().Select()
                var line = reader.ReadLine();
                while (line != null)
                {
                    String[] linePieces = line.Split(';');
                    int id;
                    int nb;
                    Material m;
                    String p;
                    Vector3 v;
                    switch (linePieces[0])
                    {
                        case "materials":
                            nb = int.Parse(linePieces[1]);
                            materials = new List<Material>();
                            for (int j = 0; j < nb; j++)
                            {
                                materials.Add(new Material());
                                positionIndices.Add(j + 1, new List<int>());
                                normalIndices.Add(j + 1, new List<int>());
                                texCoordIndices.Add(j + 1, new List<int>());
                            }
                            break;
                        case "mdiff":
                            id = int.Parse(linePieces[1]);
                            p = linePieces[2];
                            m = materials[id - 1];
                            if (p != "undefined")
                                m.Diff = p;
                            
                            break;
                        case "mspec":
                            id = int.Parse(linePieces[1]);
                            p = linePieces[2];
                            m = materials[id];
                            m.Spec = p;
                            break;
                        case "vertices":
                            nb = int.Parse(linePieces[1]);
                            positions = new Vector3[nb];
                            break;
                        case "v":
                            v = parseVector3(linePieces[1]);
                            positions[currentPositionID] = v;
                            currentPositionID++;
                            break;
                        case "vnormals":
                            nb = int.Parse(linePieces[1]);
                            normals = new Vector3[nb];
                            break;
                        case "vn":
                            v = parseVector3(linePieces[1]);
                            normals[currentNormalID] = v;
                            currentNormalID++;
                            break;
                        case "vtexcoords":
                            nb = int.Parse(linePieces[1]);
                            texCoords = new Vector3[nb];
                            break;
                        case "vt":
                            v = parseVector3(linePieces[1]);
                            texCoords[currentTexCoordID] = v;
                            currentTexCoordID++;
                            break;
                        case "faces":
                            //Do Nothing
                            break;
                        case "f":
                            var faceData = linePieces[1].Split('/');
                            Point3 cverts = parsePoint3(faceData[0]);
                            Point3 cnorms = parsePoint3(faceData[1]);
                            Point3 ctexcoords = parsePoint3(faceData[2]);
                            int matID = int.Parse(faceData[3]);
                            var cposIDList = getPosIndexList(matID);
                            var cnormsIDList = getNormIndexList(matID);
                            var ctexcoordIDList = getTexCoordIndexList(matID);
                            for (int i = 0; i < 3; i++)
                            {
                                cposIDList.Add(cverts[i] - 1);
                                cnormsIDList.Add(cnorms[i] - 1);
                                ctexcoordIDList.Add(ctexcoords[i] - 1);
                            }
                            break;
                        default:
                            break;

                    }
                    line = reader.ReadLine();
                }

                tryAppendDefaultLists();

                matStream = materials.ToArray();
                posStream = positions;
                normStream = normals;
                texcoStream = texCoords;
                posInd = positionIndices;
                normInd = normalIndices;
                texcoInd = texCoordIndices;
            }

        }

        private void tryAppendDefaultLists()
        {
            if (defaultPosInd == null)//equivalent tests are null-tests for defaultNormInd and defaultTexcoInd
                return;

            var defaultMat = new Material();
            var defaultMatID = materials.Count + 1;
            materials.Add(defaultMat);

            positionIndices.Add(defaultMatID, defaultPosInd);
            normalIndices.Add(defaultMatID, defaultNormInd);
            texCoordIndices.Add(defaultMatID, defaultTexcoInd);
        }

        private List<int> getPosIndexList(int matID)
        {
            List<int> ret;
            positionIndices.TryGetValue(matID, out ret);
            if (ret == null)
            {
                if(defaultPosInd == null)
                    defaultPosInd = new List<int>();
                ret = defaultPosInd;
            }
            return ret;
        }

        private List<int> getNormIndexList(int matID)
        {
            List<int> ret;
            normalIndices.TryGetValue(matID, out ret);
            if (ret == null)
            {
                if (defaultNormInd == null)
                    defaultNormInd = new List<int>();
                ret = defaultNormInd;
            }
            return ret;
        }

        private List<int> getTexCoordIndexList(int matID)
        {
            List<int> ret;
            texCoordIndices.TryGetValue(matID, out ret);
            if (ret == null)
            {
                if (defaultTexcoInd == null)
                    defaultTexcoInd = new List<int>();
                ret = defaultTexcoInd;
            }
            return ret;
        }

        private Vector3 parseVector3(String s)
        {
            var pieces = s.Split(',');
            pieces = pieces.Select(e => e.Replace('.', ',')).ToArray();
            return new Vector3(float.Parse(pieces[0]), float.Parse(pieces[1]), float.Parse(pieces[2]));
        }

        private Point3 parsePoint3(String s)
        {
            var pieces = s.Split(',');
            pieces = pieces.Select(e => e.Replace('.', ',')).ToArray();
            return new Point3((int)float.Parse(pieces[0]), (int)float.Parse(pieces[1]), (int)float.Parse(pieces[2]));
        }

    }
}
