using System;
using System.Collections.Generic;
using Castle.Core.Internal;
using MHGameWork.TheWizards.Audio;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using MHGameWork.TheWizards.Scattered._Engine;
using SlimDX;
using System.Linq;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Storage : IIslandAddon
    {
        private readonly Level level;
        public Inventory Inventory = new Inventory();

        private List<EntityNode> itemNodes = new List<EntityNode>();

        private SoundEmitter emitter = new SoundEmitter();

        public Storage(Level level, SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            node.AssociatedObject = this;
            var warehouseMesh = TW.Assets.LoadMesh("Scattered\\Models\\WarehouseTile");

            int sizeX = 5;
            int sizeY = 3;

            var renderNode = node.CreateChild();
            renderNode.Relative = Matrix.Translation(-sizeX * 0.5f, 0, -sizeY * 0.5f);//sizeX * 0.5f, 0, sizeY * 0.5f);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    var n = renderNode.CreateChild();
                    itemNodes.Add(level.CreateEntityNode(n.CreateChild()
                        .Alter(a => a.Relative = Matrix.Scaling(2, 2, 2) * Matrix.Translation(0, 0.5f, 0)))
                        .Alter(a => a.CreateInteractable(() => onInteractItem(a))));
                    level.CreateEntityNode(n).Alter(c => c.Entity.Mesh = warehouseMesh)
                        .Alter(c => c.Node.Relative = Matrix.Translation(x, 0, y))
                        .Alter(c => c.CreateInteractable(onInteractPad));
                }
            }

            //emitter.Sound = SoundFactory.Load("Scattered\\Sound\\Ploep1.wav");
            //emitter.Stop();

        }

        private void onInteractPad()
        {
            if (Inventory.ItemCount >= itemNodes.Count) return;
            if (level.LocalPlayer.Inventory.ItemCount == 0) return;

            level.LocalPlayer.Inventory.TransferItemsTo(Inventory, level.LocalPlayer.Inventory.Items.First(), 1);
            playPloep();
        }

        private Random r = new Random();
        private void playPloep()
        {
            return;
            if (emitter.Playing) return;
            var sounds = new[]
                {
                    SoundFactory.Load("Scattered\\Sound\\Ploep1.wav"),
                    SoundFactory.Load("Scattered\\Sound\\Ploep2.wav"),
                    SoundFactory.Load("Scattered\\Sound\\Ploep3.wav")
                };
            emitter.Sound = sounds[r.Next(0, 2)];
            emitter.Start();
        }

        private void onInteractItem(EntityNode entityNode)
        {
            var type = Inventory.Items.ElementAt(itemNodes.IndexOf(entityNode));
            if (type == null) throw new InvalidOperationException();
            Inventory.TransferItemsTo(level.LocalPlayer.Inventory, type, 1);
            playPloep();
        }

        public void PrepareForRendering()
        {
            itemNodes.ForEach(n => n.Entity.Mesh = null);
            itemNodes.Zip(Inventory.Items, (e, i) => new { E = e, I = i }).ForEach(n => n.E.Entity.Mesh = n.I.Mesh);
        }

        public SceneGraphNode Node { get; private set; }
    }
}