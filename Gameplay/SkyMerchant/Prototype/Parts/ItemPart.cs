using System;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class ItemPart : EngineModelObject ,IPhysical
    {
        #region Injection
        public Physical Physical { get; set; }
       

        public Random Random { get; set; }
        #endregion

        public ItemPart()
        {
        }

        public void UpdatePhysical()
        {
        }
        public ItemType Type { get; set; }
        public IslandPart Island { get; set; }
        public bool InStorage { get; set; }

        public Vector3 RandomOffset { get; set; }

        public void RemoveFromIsland()
        {
            InStorage = true;
            Island = null;
            Physical.Visible = false;
        }

        public void PlaceOnIsland(IslandPart island)
        {
            InStorage = false;
            Island = island;
            Physical.Visible = true;
            RandomOffset = new Vector3((float)Random.NextDouble(), 0, (float)Random.NextDouble());
            RandomOffset = RandomOffset - MathHelper.One*0.5f;
            RandomOffset *= 4;
            RandomOffset = RandomOffset.ChangeY(0.2f);
        }

        public void FixPosition()
        {
            if (InStorage) return;
            Physical.SetPosition(Island.Physical.GetPosition().ChangeY(Island.GetMaxY()) + RandomOffset);
        }
    }

    public class ItemType
    {
        public string Name;
    }
}