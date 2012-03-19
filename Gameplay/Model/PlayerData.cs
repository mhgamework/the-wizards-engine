using MHGameWork.TheWizards.ModelContainer;
using SlimDX;

namespace MHGameWork.TheWizards.Model
{
    /// <summary>
    /// Functionality
    /// 
    /// The Look dir for angle (0,0) is (0,0,-1) = forward. The horizontal angle is around the Y-axis and the vertical around the right axis.
    /// </summary>
    [ModelObjectChanged]
    public class PlayerData : IModelObject
    {
        private Vector3 position;
        public Vector3 Position
        {
            get { return position; }
            set
            {
                position = value;
                updateEntity();
            }
        }

        private Model.Entity entity;
        public Model.Entity Entity
        {
            get { return entity; }
            set { entity = value;
                updateEntity();
            }
        }

        public float Health;

        public float LookAngleHorizontal;
        public float LookAngleVertical;



        public string Name;


        private void updateEntity()
        {
            if (entity == null) return;
            entity.WorldMatrix = Matrix.Translation(position);
        }



        private ModelContainer.ModelContainer container;

        public ModelContainer.ModelContainer Container
        {
            get { return container; }
        }

        public void Initialize(ModelContainer.ModelContainer container)
        {
            this.container = container;
        }
    }
}
