using System.Collections.Generic;
using SlimDX;

namespace MHGameWork.TheWizards.WorldSimulation
{
    /// <summary>
    /// Responsible for holding a creatures state
    /// </summary>
    public class Creature
    {
        public Creature(int seed)
        {
            Seeder = new Seeder(seed);
        }
        public ICreatureBehavior Behaviour;
        public IAction CurrentAction;
        public IPriority CurrentPriority;
        public Vector3 Position;
        public Seeder Seeder;
        
        public bool Alive=true;
        private List<Building> buildings = new List<Building>();//note: not sure if this is the correct place to put this
        private Dictionary<ResourceTypes, float> resources = new Dictionary<ResourceTypes, float>();

        public List<Building> Buildings
        {
            get { return buildings; }
        }

        public float GetResource(ResourceTypes type)
        {
            if (!resources.ContainsKey(type)) return 0;
            return resources[type];
        }
        public void SetResource(ResourceTypes type, float value)
        {

            resources[type] = value;
        }
        
        private Dictionary<object, object> propertyMap = new Dictionary<object, object>();
        
        public T GetProperty<T>(CreatureProperty<T> p)
        {
            return (T)propertyMap[p];
        }
        public void SetProperty<T>(CreatureProperty<T> p , T value)
        {
            propertyMap[p] = value;
        }

    }
}