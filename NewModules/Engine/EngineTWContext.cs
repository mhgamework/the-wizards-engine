using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Engine
{
    public class EngineTWContext
    {
        private GraphicsWrapper game;
        private PhysicsWrapper physX;
        private TypeSerializer typeSerializer;


        public EngineTWContext(TWEngine engine)
        {
            game = new GraphicsWrapper();

            game.InitDirectX();


            typeSerializer = new TypeSerializer(engine);

            var container = new DataWrapper();


            physX = new PhysicsWrapper();
            physX.Initialize();




            setTWGlobals(container);


            //updateActiveGameplayAssembly();
            //createSimulators();

            var stringSerializer = StringSerializer.Create();
            stringSerializer.AddConditional(new FilebasedAssetSerializer());

            TW.Data.ModelSerializer = new ModelSerializer(stringSerializer, typeSerializer);
            TW.Data.TypeSerializer = typeSerializer;
        }

        private void setTWGlobals(DataWrapper container)
        {
            var context = new TW.Context();
            context.Graphics = game;
            context.Data = container;
            context.Physics = physX;
            context.Audio = new AudioWrapper();
            context.Assets = new AssetsWrapper();
            TW.SetContext(context);
        }

    }
}
