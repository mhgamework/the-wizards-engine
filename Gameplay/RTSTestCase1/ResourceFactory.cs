using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using MHGameWork.TheWizards.RTSTestCase1.Items;

namespace MHGameWork.TheWizards.RTSTestCase1
{
    [ModelObjectChanged]
    public class ResourceFactory : EngineModelObject
    {
        public ResourceFactory()
        {
            Stone = new ResourceType(){Texture = TW.Assets.LoadTexture("RTS//bark.jpg")};
            Wood = new ResourceType() { Texture = TW.Assets.LoadTexture("RTS//bark.jpg") };
            Barrel = new ResourceType() { Texture = TW.Assets.LoadTexture("RTS//bark.jpg") };
            Crystal = new ResourceType() { Texture = TW.Assets.LoadTexture("RTS//bark.jpg") };
            Cannonball = new ResourceType() { Texture = TW.Assets.LoadTexture("RTS//bark.jpg") };
        }
        public ResourceType Stone { get; private set; }
        public ResourceType Wood { get; private set; }
        public ResourceType Barrel { get; private set; }
        public ResourceType Crystal { get; private set; }
        public ResourceType Cannonball { get; private set; }
    }
}