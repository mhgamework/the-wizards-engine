namespace MHGameWork.TheWizards.World.Static
{
    public interface IStaticWorldObjectFactory
    {
        void ApplyUpdatePacket(IStaticWorldObject obj, StaticWorldObjectUpdatePacket p);

        IStaticWorldObject CreateNew();
        void Delete(IStaticWorldObject obj);
    }
}
