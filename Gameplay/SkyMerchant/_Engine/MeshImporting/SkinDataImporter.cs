using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MHGameWork.TheWizards.SkyMerchant._Engine.MeshImporting
{
    /// <summary>
    /// Responsible for parsing .twskin files
    /// </summary>
    public class SkinDataImporter
    {
        private List<VertexSkinData> skinData;

        public void LoadSkinData(String path, out List<VertexSkinData> vertexSkinData)
        {
            reset();

            using (var reader = File.OpenText(path))
            {
                var line = reader.ReadLine();
                while (line != null)
                {
                    String[] linePieces = line.Split('/');
                    parseLine(linePieces);
                    line = reader.ReadLine();
                }

                vertexSkinData = skinData.ToList();
            }
        }

        private void reset()
        {
            skinData = new List<VertexSkinData>();
        }

        private void parseLine(String[] linePieces)
        {
            var pieces = linePieces.Select(e => e.Replace('.', ',')).ToArray();

            var sData = new VertexSkinData();

            var vertID = int.Parse(pieces[0]);
            var temp = skinData.Where(e => e.VertexID == vertID).ToArray();
            if (temp != null && temp.Length > 0) //if already a SkinData entry for this vertexID
                sData = temp[0];
            else
            {
                skinData.Add(sData);
            }
            
            sData.VertexID = vertID;

            var bWeight = new BoneWeight();
            bWeight.BoneName = pieces[1];
            bWeight.Weight = float.Parse(pieces[2]);

            sData.Weights.Add(bWeight);
        }

    }
}
