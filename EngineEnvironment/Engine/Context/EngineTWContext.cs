using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MHGameWork.TheWizards.Persistence;
using MHGameWork.TheWizards.Serialization;

namespace MHGameWork.TheWizards.Engine
{
    public class EngineTWContext
    {
        private readonly TWEngine engine;
        private GraphicsWrapper game;
        private PhysicsWrapper physX;
        private TypeSerializer typeSerializer;
        public TW.Context Context { get; private set; }


        public EngineTWContext(TWEngine engine)
        {
            this.engine = engine;
            Context = new TW.Context();

            game = new GraphicsWrapper();

            // TODO: make better setting resolution
            game.Form.FormSize = new Size(1280, 720);


            game.InitDirectX();

            // TODO: more resolution sjit
            game.SpectaterCamera.AspectRatio = (float)game.Form.Form.ClientSize.Width / game.Form.Form.ClientSize.Height;


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
            //Context.Audio = new AudioWrapper();
            Context.Assets = new AssetsWrapper();
            Context.Debug = new DebugWrapper(engine);
            Context.Engine = new EngineWrapper();
            TW.SetContext(Context);
        }

    }
}