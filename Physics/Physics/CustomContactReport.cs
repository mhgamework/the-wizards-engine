using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StillDesign.PhysX;

namespace MHGameWork.TheWizards.Physics
{
    public class CustomContactReport  : UserContactReport
    {
        public PhysicsEngine Engine { get; private set; }

        public CustomContactReport(PhysicsEngine engine)
        {
            Engine = engine;
        }

        public override void OnContactNotify(ContactPair contactInformation, ContactPairFlag events)
        {
            Engine.OnContactNotify(contactInformation, events);
        }
    }
}
