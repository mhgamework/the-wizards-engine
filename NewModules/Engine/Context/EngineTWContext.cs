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
        public TW.Context Context { get; private set; }


        public EngineTWContext(TWEngine engine)
        {
            Context = new TW.Context();

            game = new GraphicsWrapper();

            game.InitDirectX();


            typeSerializer = new TypeSerializer(engine);

            var container = new DataWrapper(engine.TraceLogger);


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
            Context.Graphics = game;
            Context.Data = container;
            Context.Physics = physX;
            Context.Audio = new AudioWrapper();
            Context.Assets = new AssetsWrapper();
            TW.SetContext(Context);
        }

    }
}
