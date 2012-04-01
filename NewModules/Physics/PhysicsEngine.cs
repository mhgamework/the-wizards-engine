using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.ServerClient;
using Microsoft.Xna.Framework;
using StillDesign;
using StillDesign.PhysX;
using IDisposable = System.IDisposable;

namespace MHGameWork.TheWizards.Physics
{
    /// <summary>
    /// This is more of a helper class and can lose its use later in the engine
    /// EDIT: the PhysX Core has become 'static' to solve problems with the underlying unmanaged code
    /// </summary>
    public class PhysicsEngine : IDisposable, IXNAObject
    {
        private StillDesign.PhysX.Scene _scene;

        private static Core core;
        private static Core Core
        {
            get
            {
                if (core == null)
                {

                    CoreDescription coreDesc = new CoreDescription();
                    UserOutput output = new UserOutput();

                    core = new Core(coreDesc, output);

                    core.SetParameter(PhysicsParameter.VisualizationScale, 2.0f);
                    core.SetParameter(PhysicsParameter.VisualizeCollisionShapes, true);
                    core.SetParameter(PhysicsParameter.VisualizeActorAxes, true);
                    core.SetParameter(PhysicsParameter.VisualizeClothMesh, true);
                    core.SetParameter(PhysicsParameter.VisualizeJointLocalAxes, true);
                    core.SetParameter(PhysicsParameter.VisualizeJointLimits, true);
                    core.SetParameter(PhysicsParameter.VisualizeFluidPosition, true);
                    core.SetParameter(PhysicsParameter.VisualizeFluidEmitters, false); // Slows down rendering a bit to much
                    core.SetParameter(PhysicsParameter.VisualizeForceFields, true);
                    core.SetParameter(PhysicsParameter.VisualizeSoftBodyMesh, true);



                    // Connect to the remote debugger if its there
                    core.Foundation.RemoteDebugger.Connect("localhost");
                }
                return core;
            }
        }

        public StillDesign.PhysX.Scene Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }


        public PhysicsEngine()
        {

        }


        ~PhysicsEngine()
        {
            //// Without this the application can hang because of the physx not being disposed. (never)
            //if (_core != null)
            //    _core.Dispose();
            //_core = null;

        }

        public void Initialize()
        {
            //
            if (Scene != null) return;





          



            SceneDescription sceneDesc = new SceneDescription();
            //SimulationType = SimulationType.Hardware,
            sceneDesc.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
            sceneDesc.GroundPlaneEnabled = true;
            sceneDesc.UserContactReport = new CustomContactReport(this);

            this.Scene = Core.CreateScene(sceneDesc);




            HardwareVersion ver = Core.HardwareVersion;
            SimulationType simType = this.Scene.SimulationType;


        }


        public void Update(float elapsed)
        {
            // Update Physics
            UpdateScene(elapsed, this.Scene);
        }

        public void UpdateScene(float elapsed, StillDesign.PhysX.Scene scene)
        {
            scene.Simulate(elapsed);
            //_scene.Simulate( 1.0f / 60.0f );
            scene.FlushStream();
            scene.FetchResults(SimulationStatus.RigidBodyFinished, true);
        }

        

        #region IDisposable Members

        public void Dispose()
        {

            if (Scene != null)
            {
                Scene.Dispose();
                Scene = null;
            }
            //if (_core != null)
            //{
            //    _core.Dispose();

            //}
            //_core = null;
        }

        #endregion

        // This should be removed
        #region IXNAObject Members

        public void Initialize(IXNAGame _game)
        {
            Initialize();
        }

        public void Render(IXNAGame _game)
        {

        }

        public void Update(IXNAGame _game)
        {
            Update(_game.Elapsed);
        }

        #endregion

        public delegate void ContactNotifyDelegate(ContactPair contactInformation, ContactPairFlag events);

        private event ContactNotifyDelegate contactNofityEvent;

        /// <summary>
        /// WARNING: I am quite sure this runs during simulation!!!!!! dont execute code in this delegate
        /// </summary>
        /// <param name="delg"></param>
        public void AddContactNotification(ContactNotifyDelegate delg)
        {
            contactNofityEvent += delg;
        }

        public void OnContactNotify(ContactPair contactInformation, ContactPairFlag events)
        {
            if (contactNofityEvent != null) contactNofityEvent(contactInformation, events);
        }

        public Scene CreateScene(Vector3 gravity, bool b)
        {
            return Core.CreateScene(gravity, b);
        }
    }
}
