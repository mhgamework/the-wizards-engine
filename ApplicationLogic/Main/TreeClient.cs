using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Networking.Client;
using TreeGenerator.EngineSynchronisation;
using TreeGenerator.LodEngine;
using TreeGenerator.TreeEngine;

namespace MHGameWork.TheWizards.Main
{
    public class TreeClient
    {
        private TreeLodEngine treeLodEngine;
        private ClientTreeSyncer clientTreeSyncer;
        private ModelLodLayer lodLayer;
        private EngineTreeRenderDataGenerater treeRenderGenerater;
        private TWRenderer renderer;

        public void StartJob(IClientPacketManager packetManager)
        {
            throw new NotImplementedException();
            //clientTreeSyncer = new ClientTreeSyncer(packetManager, treeLodEngine);
        }

        public void RequestInitialState()
        {
            clientTreeSyncer.RequestAllTrees();
        }

        public void Render()
        {
            renderer.Render();

        }

        public void Update()
        {
            if (clientTreeSyncer != null)
                clientTreeSyncer.Update();
            treeLodEngine.Update(renderer.Game);
        }

        public void Initialize(IXNAGame xnaGame)
        {
            treeLodEngine = new TreeLodEngine();
            renderer = new TWRenderer(xnaGame);
            treeRenderGenerater = new EngineTreeRenderDataGenerater(10);
            lodLayer = new ModelLodLayer(0, renderer, treeRenderGenerater, 100);
            treeLodEngine.AddITreeLodLayer(lodLayer, 0);
        }
    }
}
