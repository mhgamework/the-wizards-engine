using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Graphics.Xna.Graphics;
using Microsoft.Xna.Framework;

namespace TreeGenerator.LodEngine
{
    public class TreeLodEngine
    {
        private List<TreeLodEntity> treeLodEntities = new List<TreeLodEntity>();
        private List<ITreeLodLayer> treeLodLayers = new List<ITreeLodLayer>();

        public TreeLodEntity CreateTreeLodEntity(TreeStructure treeStructure)
        {
            TreeLodEntity entity = new TreeLodEntity(treeStructure);
            treeLodEntities.Add(entity);
            return entity;
        }
        public void AddITreeLodLayer(ITreeLodLayer lodLayer, float minDistance)
        {

            lodLayer.MinDistance = minDistance;
            treeLodLayers.Add(lodLayer);
            if (treeLodLayers.Count > 1)
            {
                treeLodLayers.Sort(delegate(ITreeLodLayer x, ITreeLodLayer y)
                                       {
                                           if (x.MinDistance == y.MinDistance) return 0;
                                           if (x.MinDistance > y.MinDistance) return 1;
                                           return -1;
                                       });
            }
        }
        public void Update(IXNAGame game)
        {
            updateNewLodLevels(game);
            updateLod();
        }

        private void updateLod()
        {
            for (int i = 0; i < treeLodEntities.Count; i++)
            {
                bool succeeded = false;
                ITreeLodLayer oldlayer;
                TreeLodEntity ent = treeLodEntities[i];

                if (ent.LodLayer == ent.NextLodLayer) continue;

                if (!ent.NextLodLayer.AddEntity(ent))
                    continue;

                if (ent.LodLayer != null) ent.LodLayer.RemoveEntity(ent);

                ent.LodLayer = ent.NextLodLayer;

            }
        }

        private void updateNewLodLevels(IXNAGame game)
        {
            bool nextlodEntity = false;
            //This is trick
            if (treeLodLayers.Count == 1)
            {
                for (int i = 0; i < treeLodEntities.Count; i++)
                {
                    treeLodEntities[i].NextLodLayer = treeLodLayers[0];
                }
                return;
            }

            for (int i = 0; i < treeLodEntities.Count; i++)
            {
                nextlodEntity = false;
                float distance = Vector3.Distance(game.Camera.ViewInverse.Translation, treeLodEntities[i].WorldMatrix.Translation);
                for (int j = 0; j < treeLodLayers.Count; j++)
                {
                    if (distance < treeLodLayers[j].MinDistance)
                    {
                        if (treeLodLayers[j].IsFull())
                            continue;

                        /*for (int k = j - 1; k < treeLodLayers.Count; k++)
                        {
                            if (!treeLodLayers[k].IsFull())
                            {*/
                        treeLodEntities[i].NextLodLayer = treeLodLayers[j];
                        nextlodEntity = true;
                        break;
                        /*}
                    }*/
                    }
                    /*if (nextlodEntity)
                        break;*/
                }
            }

        }
    }
}
