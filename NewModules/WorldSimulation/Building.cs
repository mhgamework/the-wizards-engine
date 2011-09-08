using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.WorldSimulation.Actions;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation
{
   public class Building
   {
       public Vector3 Position;
       public float Size;
       public Creature Creature;
       public bool Home=false;

       public Building(Vector3 position,Creature creature,BuildingBluePrints print)
       {
           Position = position;
           Creature = creature;
           Size = print.Size;
       }
   }
}
