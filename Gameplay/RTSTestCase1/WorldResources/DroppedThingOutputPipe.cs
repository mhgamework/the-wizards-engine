using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.RTS;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldResources
{
    /// <summary>
    /// Control PhysX stuff to safely create a new DroppedThing (without causing jumping items)
    /// The New item is spawned in a location which is surrounded by a PhysX invisible box, which makes this area free of collisions
    /// </summary>
    public class DroppedThingOutputPipe
    {
        private readonly Vector3 spawnPosition;
        private readonly Vector3 moveDirection;

        public DroppedThingOutputPipe(Vector3 spawnPosition, Vector3 moveDirection)
        {
            this.spawnPosition = spawnPosition;
            this.moveDirection = moveDirection;

        }

        public void Update()
        {
            
        }
        public void SpawnItem(Thing thing)
        {
            
        }
    }
}
