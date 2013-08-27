using MHGameWork.TheWizards.RTSTestCase1;

namespace MHGameWork.TheWizards.SkyMerchant.Prototype.Parts
{
    public class ItemPart
    {
        #region Injection
        public Physical Physical { get; set; }
        #endregion

        public ItemPart()
        {
        }


        public ItemType Type { get; set; }
        public IslandPart Island { get; set; }
        public bool InStorage { get; set; }

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
        }

        public void FixPosition()
        {
            if (InStorage) return;
            Physical.SetPosition(Island.Physical.GetPosition().ChangeY(Island.GetMaxY()));
        }
    }

    public class ItemType
    {
        public string Name;
    }
}