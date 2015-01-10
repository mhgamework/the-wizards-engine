using System;
using System.Linq;
using System.Reflection;
using MHGameWork.TheWizards.GodGame.Internal.Model;

namespace MHGameWork.TheWizards.GodGame.Types.Transportation.Generic
{
    public class GenericVoxelType<T> : GameVoxelType where T : class
    {
        private readonly Func<IVoxel, T> create;

        public GenericVoxelType(Func<IVoxel, T> create)
            : base(typeof(T).Name)
        {
            this.create = create;
        }

        public override void OnCreated(IVoxel handle)
        {
            handle.SetPart(create(handle));
            base.OnCreated(handle);
        }

        public override void OnDestroyed(IVoxel handle)
        {
            handle.SetPart<T>(null);
            base.OnDestroyed(handle);
        }

        public override void Tick(IVoxelHandle handle)
        {
            if (typeof(T).GetInterfaces().Contains(typeof(ITickable)))
                ((ITickable)((IVoxel)handle).GetPart<T>()).Tick();
        }

        protected override string getDebugDescription(IVoxelHandle handle)
        {
            var ret = base.getDebugDescription(handle);
            var part = ((IVoxel)handle).GetPart<T>();
            foreach (var fi in part.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                ret += fi.Name + ": " + fi.GetValue(part).ToString() + "\n";
            }
            return ret;
        }

    }
}