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
    /// this is more of a helper class and can lose its use later in the engine
    /// </summary>
    public class PhysicsEngine : IDisposable, IXNAObject
    {
        private Core _core;
        private StillDesign.PhysX.Scene _scene;

        public Core Core
        {
            get { return _core; }
            set { _core = value; }
        }

        public StillDesign.PhysX.Scene Scene
        {
            get { return _scene; }
            set { _scene = value; }
        }


        public PhysicsEngine()
        {

        }

        /// <summary>
        /// This constructor is specifically implemented to allow multiple runs of a engine, for server-client testing
        /// </summary>
        public PhysicsEngine(Core core)
        {
            _core = core;
        }

        ~PhysicsEngine()
        {
            // Without this the application can hang because of the physx not being disposed. (never)
            if (_core != null)
                _core.Dispose();
            _core = null;

        }

        public void Initialize()
        {
            //





            if (_core == null)
            {
                if (StillDesign.PhysX.Core.IsCoreCreated)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                if (StillDesign.PhysX.Core.IsCoreCreated)
                    throw new InvalidOperationException("Core is already created and cannot be released");

                CoreDescription coreDesc = new CoreDescription();
                UserOutput output = new UserOutput();

                _core = new Core(coreDesc, output);

                Core core = this.Core;
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



                //TODO: this was at the bottom of the function

                // Connect to the remote debugger if its there
                core.Foundation.RemoteDebugger.Connect("localhost");
            }



            SceneDescription sceneDesc = new SceneDescription();
            //SimulationType = SimulationType.Hardware,
            sceneDesc.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
            sceneDesc.GroundPlaneEnabled = true;
            sceneDesc.UserContactReport = new CustomContactReport(this);

            this.Scene = _core.CreateScene(sceneDesc);




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
            if (_core != null)
            {
                _core.Dispose();

            }
            _core = null;
        }

        #endregion

        /// <summary>
        /// This should be removed
        /// </summary>
        /// <param name="_game"></param>
        #region IXNAObject Members

        public void Initialize(IXNAGame _game)
        {
            if (Scene == null)
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
    }
}
