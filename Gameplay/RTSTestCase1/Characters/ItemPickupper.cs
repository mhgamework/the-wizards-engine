using System;
using MHGameWork.TheWizards.RTSTestCase1.Items;

namespace MHGameWork.TheWizards.RTSTestCase1.Characters
{
    public class ItemPickupper
    {
        public void Pickup(IRTSCharacter player, DroppedThing found)
        {
            if (player.Holding != null) return;

            player.Holding = found.Thing;
            TW.Data.Objects.Remove(found);

        }
        public void Drop(IRTSCharacter character)
        {
            if (character.Holding == null) return;


        }
         
    }
}