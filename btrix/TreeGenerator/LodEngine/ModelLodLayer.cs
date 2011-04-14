using System;
using System.Collections.Generic;
using System.Text;
using TreeGenerator.TreeEngine;

namespace TreeGenerator.LodEngine
{
    public class ModelLodLayer : ITreeLodLayer
    {
        private TWRenderer renderer;
        private EngineTreeRenderDataGenerater treeRenderGenerater;

        private Dictionary<TreeLodEntity, TWRenderElement> elementsDict = new Dictionary<TreeLodEntity, TWRenderElement>();

        public int MaxNumberOfEntities = 0;
        public ModelLodLayer(int lodLevel, TWRenderer renderer, EngineTreeRenderDataGenerater treeRenderGenerater, int maxNumberOfEntities)
        {
            LodLevel = lodLevel;
            this.renderer = renderer;
            this.treeRenderGenerater = treeRenderGenerater;
            MaxNumberOfEntities = maxNumberOfEntities;
        }

        public int LodLevel { get; private set; }


        #region ITreeLodLayer Members

        public bool AddEntity(TreeLodEntity ent)
        {
            TWRenderElement element;

            EngineTreeRenderData renderData = treeRenderGenerater.GetRenderData(ent.TreeStructure, renderer.Game, LodLevel);
            renderData.Initialize();
            element = renderer.CreateElement(renderData);
            element.WorldMatrix = ent.WorldMatrix;

            elementsDict[ent] = element;
            return true;
        }

        public void RemoveEntity(TreeLodEntity ent)
        {
            TWRenderElement element;
            element = elementsDict[ent];
            renderer.ReleaseElement(element);
        }

        #endregion

        #region ITreeLodLayer Members

        private float minDistance;
        public float MinDistance
        {
            get { return minDistance; }
            set { minDistance = value; }
        }

        #endregion

        #region ITreeLodLayer Members


        public bool IsFull()
        {
            if (elementsDict.Count >= MaxNumberOfEntities)
                return true;
            return false;
          
        }

        #endregion
    }
}
