using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MHGameWork.TheWizards;
using MHGameWork.TheWizards.Assets;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.ServerClient;

namespace TreeGenerator
{
    /// <summary>
    /// Data klasse
    /// </summary>
    public class TreeTypeData
    {
        public List<TreeTypeLevel> Levels = new List<TreeTypeLevel>();

        public ITexture TextureBark;
        public ITexture BumpTexture;

        public float TextureHeight = 1;
        public float TextureWidth = 1;
        public TreeTypeData()
        {
        }

        public void WriteToXML(string filename)
        {
            TWXmlNode node = new TWXmlNode(TWXmlNode.CreateXmlDocument(), "TreeType");

            TWXmlNode textureNode = node.CreateChildNode("TextureValue");
            textureNode.AddChildNode("TextureName", TextureBark.Guid.ToString());
            textureNode.AddChildNode("TextureHeight", TextureHeight.ToString());
            textureNode.AddChildNode("TextureWidth", TextureWidth.ToString());

            for (int i = 0; i < Levels.Count; i++)
            {
                TWXmlNode levelNode = node.CreateChildNode("Level");
                TreeTypeLevel iLevel = Levels[i];

                Range.WriteToXML(levelNode.CreateChildNode("BranchCount"), iLevel.BranchCount);
                RangeSpreading.WriteToXML(iLevel.BranchPositionFactor, levelNode.CreateChildNode("BranchPositionFactor"));
                Range.WriteToXML(levelNode.CreateChildNode("BranchDropAngle"), iLevel.BranchDropAngle);
                Range.WriteToXML(levelNode.CreateChildNode("BranchWobbleDropAngle"), iLevel.BranchWobbleDropAngle);
                Range.WriteToXML(levelNode.CreateChildNode("BranchWobbleAxialSplit"), iLevel.BranchWobbleAxialSplit);
                Range.WriteToXML(levelNode.CreateChildNode("BranchLength"), iLevel.BranchLength);
                //levelNode.AddChildNode("BranchLengthDecrease", iLevel.BranchLengthDecrease.ToString());
                Range.WriteToXML(levelNode.CreateChildNode("BranchEndDiameterFactor"), iLevel.BranchEndDiameterFactor);
                levelNode.AddChildNode("BranchMaxSegmentLength", iLevel.BranchMaxSegmentLength.ToString());
                Range.WriteToXML(levelNode.CreateChildNode("BranchStartDiameterFactor"), iLevel.BranchStartDiameterFactor);
                RangeSpreading.WriteToXML(iLevel.BranchAxialSplit, levelNode.CreateChildNode("BranchAxialSplit"));
                levelNode.AddChildNode("BranchBendingStrenght", iLevel.BranchBendingStrenght.ToString());
                levelNode.AddChildNode("BranchBendingFlexibility", iLevel.BranchBendingFlexibility.ToString());

                //steps
                levelNode.AddChildNode("Steps", iLevel.Steps.ToString());
                levelNode.AddChildNode("StepsPerMeter", iLevel.StepsPerMeter.ToString());
                levelNode.AddChildNode("BranchDistributionPercentage", iLevel.BranchDistributionPercentage.ToString());
                Range.WriteToXML(levelNode.CreateChildNode("BranchStepSpreading"), iLevel.BranchStepSpreading);
                Range.WriteToXML(levelNode.CreateChildNode("BranchLenghtDegradation"), iLevel.BranchLenghtDegradation);

                for (int l = 0; l < Levels[i].LeafType.Count; l++)
                {
                    TWXmlNode leafNode = levelNode.CreateChildNode("leaftype");
                    TreeLeafType ileafLevel = iLevel.LeafType[l];

                    RangeSpreading.WriteToXML(ileafLevel.RelativePosition, leafNode.CreateChildNode("RelativePosition"));
                    RangeSpreading.WriteToXML(ileafLevel.AxialSplitPosition, leafNode.CreateChildNode("AxialSplit"));
                    RangeSpreading.WriteToXML(ileafLevel.DropAngle, leafNode.CreateChildNode("DropAngle"));
                    Range.WriteToXML(leafNode.CreateChildNode("DistanceFromTrunk"), ileafLevel.DistanceFromTrunk);
                    Range.WriteToXML(leafNode.CreateChildNode("LeafCount"), ileafLevel.LeafCount);
                    Range.WriteToXML(leafNode.CreateChildNode("Length"), ileafLevel.Length);
                    Range.WriteToXML(leafNode.CreateChildNode("width"), ileafLevel.width);
                    leafNode.AddChildNode("Texture", ileafLevel.Texture.Guid.ToString());
                    leafNode.AddChildNode("Bump", ileafLevel.BumpTexture.Guid.ToString());
                    leafNode.AddChildNode("BillBoardLeaf", ileafLevel.BillBoardLeaf.ToString());
                    leafNode.AddChildNode("VolumetricLeaves", ileafLevel.VolumetricLeaves.ToString());
                    Range.WriteToXML(leafNode.CreateChildNode("BendingLength"), ileafLevel.BendingLength);
                    Range.WriteToXML(leafNode.CreateChildNode("BendingWidth"), ileafLevel.BendingWidth);
                    leafNode.AddChildNode("FaceCountLength", ileafLevel.FaceCountLength.ToString());
                    leafNode.AddChildNode("FaceCountWidth", ileafLevel.FaceCountWidth.ToString());



                }

                //leaves and wind animation parameters must be added later
                //levelNode.
            }
            node.Document.Save(filename + ".XML");

        }

        public static TreeTypeData LoadFromXML(string filename, ITextureFactory textureFactory)
        {
            using (var fs = File.Open(filename + ".XML", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return LoadFromXML(fs, textureFactory);
            }
        }
        public static TreeTypeData LoadFromXML(Stream stream, ITextureFactory textureFactory)
        {
            TreeTypeData tree = new TreeTypeData();

            TWXmlNode node = TWXmlNode.GetRootNodeFromStream(stream);

            if (node.Name != "TreeType") throw new Exception("Rootnode is not a TreeType node! Invalid XML format!");


            foreach (TWXmlNode childNode in node.GetChildNodes())
            {
                if (childNode.Name == "TextureValue")
                {
                    tree.TextureBark = textureFactory.GetTexture(new Guid(childNode.ReadChildNodeValue("TextureName")));
                    tree.TextureHeight = float.Parse(childNode.ReadChildNodeValue("TextureHeight"));
                    tree.TextureWidth = float.Parse(childNode.ReadChildNodeValue("TextureWidth"));

                }
                //TWXmlNode node = node.GetChildNodes[i];
                if (childNode.Name != "Level") continue;

                TreeTypeLevel level = new TreeTypeLevel();
                level.BranchCount = Range.LoadFromXML(childNode.FindChildNode("BranchCount"));
                level.BranchPositionFactor = RangeSpreading.LoadFromXML(childNode.FindChildNode("BranchPositionFactor"));
                level.BranchDropAngle = Range.LoadFromXML(childNode.FindChildNode("BranchDropAngle"));
                level.BranchWobbleDropAngle = Range.LoadFromXML(childNode.FindChildNode("BranchWobbleDropAngle"));
                level.BranchWobbleAxialSplit = Range.LoadFromXML(childNode.FindChildNode("BranchWobbleAxialSplit"));
                level.BranchLength = Range.LoadFromXML(childNode.FindChildNode("BranchLength"));
                //level.BranchLengthDecrease = float.Parse(childNode.ReadChildNodeValue("BranchLengthDecrease"));
                level.BranchEndDiameterFactor = Range.LoadFromXML(childNode.FindChildNode("BranchEndDiameterFactor"));
                level.BranchMaxSegmentLength = float.Parse(childNode.ReadChildNodeValue("BranchMaxSegmentLength"));
                level.BranchStartDiameterFactor = Range.LoadFromXML(childNode.FindChildNode("BranchStartDiameterFactor"));
                level.BranchAxialSplit = RangeSpreading.LoadFromXML(childNode.FindChildNode("BranchAxialSplit"));
                level.BranchBendingStrenght = float.Parse(childNode.ReadChildNodeValue("BranchBendingStrenght"));
                level.BranchBendingFlexibility = float.Parse(childNode.ReadChildNodeValue("BranchBendingFlexibility"));

                //steps
                level.Steps = bool.Parse(childNode.ReadChildNodeValue("Steps"));
                level.StepsPerMeter = float.Parse(childNode.ReadChildNodeValue("StepsPerMeter"));
                level.BranchDistributionPercentage = float.Parse(childNode.ReadChildNodeValue("BranchDistributionPercentage"));
                level.BranchStepSpreading = Range.LoadFromXML(childNode.FindChildNode("BranchStepSpreading"));
                level.BranchLenghtDegradation = Range.LoadFromXML(childNode.FindChildNode("BranchLenghtDegradation"));



                foreach (TWXmlNode leafNode in childNode.GetChildNodes())
                {
                    if (leafNode.Name != "leaftype") continue;
                    TreeLeafType leaf = new TreeLeafType();
                    leaf.BumpTexture = textureFactory.GetTexture(new Guid(leafNode.ReadChildNodeValue("Bump")));
                    leaf.Texture = textureFactory.GetTexture(new Guid(leafNode.ReadChildNodeValue("Texture")));
                    leaf.AxialSplitPosition = RangeSpreading.LoadFromXML(leafNode.FindChildNode("AxialSplit"));
                    leaf.DistanceFromTrunk = Range.LoadFromXML(leafNode.FindChildNode("DistanceFromTrunk"));
                    leaf.DropAngle = RangeSpreading.LoadFromXML(leafNode.FindChildNode("DropAngle"));
                    leaf.LeafCount = Range.LoadFromXML(leafNode.FindChildNode("LeafCount"));
                    leaf.Length = Range.LoadFromXML(leafNode.FindChildNode("Length"));
                    leaf.RelativePosition = RangeSpreading.LoadFromXML(leafNode.FindChildNode("RelativePosition"));
                    leaf.width = Range.LoadFromXML(leafNode.FindChildNode("width"));
                    leaf.BillBoardLeaf = bool.Parse(leafNode.ReadChildNodeValue("BillBoardLeaf"));
                    leaf.VolumetricLeaves = bool.Parse(leafNode.ReadChildNodeValue("VolumetricLeaves"));
                    leaf.FaceCountWidth = int.Parse(leafNode.ReadChildNodeValue("FaceCountWidth"));
                    leaf.FaceCountLength = int.Parse(leafNode.ReadChildNodeValue("FaceCountLength"));
                    leaf.BendingWidth = Range.LoadFromXML(leafNode.FindChildNode("BendingWidth"));
                    leaf.BendingLength = Range.LoadFromXML(leafNode.FindChildNode("BendingLength"));

                    level.LeafType.Add(leaf);

                }
                tree.Levels.Add(level);


            }

            return tree;
        }

        [Obsolete("Use other overload")]
        public static TreeTypeData GetTestTreeType(XNAGame game)
        {
            return GetTestTreeType();
        }
        public static TreeTypeData GetTestTreeType()
        {
            Stream st = MHGameWork.TheWizards.Common.Core.EmbeddedFile.GetStream("TreeGenerator.Files.TestTree.XML", "TestTree.XML");
            TreeTypeData treeTypeData;
            var texFact = new SimpleTextureFactory();
            var tex = new RAMTexture();
            tex.GetCoreData().DiskFilePath = TWDir.GameData + "\\TreeGenerator\\DefaultBark.tga";
            texFact.AddTexture(new Guid("1B1B473E-1B26-4879-8BE7-0485048D75C3"), tex);
            tex.GetCoreData().DiskFilePath = TWDir.GameData + "\\TreeGenerator\\DefaultLeaves.tga";
            texFact.AddTexture(new Guid("A50338ED-2156-4A5F-B579-6B06A7394CAF"), tex);
            treeTypeData = LoadFromXML(st, texFact);
            return treeTypeData;
        }

        
    }
}
