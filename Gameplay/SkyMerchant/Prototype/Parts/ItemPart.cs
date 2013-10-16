using System;
using DirectX11;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    [ModelObjectChanged]
    public class ItemPart : EngineModelObject,IGameObjectComponent 
    {
        #region Injection
        public IPositionComponent Physical { get; set; }
        public IMeshRenderComponent RenderComponent { get; set; }
        public Random Random { get; set; }
        #endregion

        public ItemPart(IPositionComponent physical, IMeshRenderComponent renderComponent, Random random)
        {
            Physical = physical;
            RenderComponent = renderComponent;
            Random = random;
            InStorage = true;
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
            RenderComponent.Visible = false;
        }

        public void PlaceOnIsland(IslandPart island)
        {
            InStorage = false;
            Island = island;
            RenderComponent.Visible = true;
            RandomOffset = new Vector3((float)Random.NextDouble(), 0, (float)Random.NextDouble());
            RandomOffset = RandomOffset - MathHelper.One*0.5f;
            RandomOffset *= 4;
            RandomOffset = RandomOffset.ChangeY(0.2f);
        }

        public void FixPosition()
        {
            if (InStorage) return;
            Physical.Position = Island.Physical.Position.ChangeY(Island.GetMaxY()) + RandomOffset;
        }
    }
}