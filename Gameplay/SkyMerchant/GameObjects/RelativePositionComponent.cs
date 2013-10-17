using System;
using MHGameWork.TheWizards.SkyMerchant._GameplayInterfacing.GameObjects;
using SlimDX;

namespace MHGameWork.TheWizards.SkyMerchant.GameObjects
{
    /// <summary>
    /// Implements relative positioning
    /// This is implemented as a decorator.
    /// 
    /// TODO: probably invert this decorater! Make this component a normal IPositionComponent, and make another serivce responsible for updating the 
    /// underlying position component (which is in fact only used to update the renderer!!!)
    /// </summary>
    public class RelativePositionComponent : IRelativePositionComponent
    {
        private readonly IPositionComponent decorated;
        private static NullRelativePositionComponent nullComponent = new NullRelativePositionComponent();
        private IRelativePositionComponent parent;

        public RelativePositionComponent(IPositionComponent decorated)
        {
            this.decorated = decorated;
            RelativeRotation = Quaternion.Identity;
            RelativePosition = new Vector3();
            parent = nullComponent;
        }

        public Vector3 Position
        {
            get { return parent.Position + Vector3.TransformCoordinate(RelativePosition, Matrix.RotationQuaternion(parent.Rotation)); }
            set
            {
                var val = value - parent.Position;
                RelativePosition = Vector3.TransformCoordinate(val, Matrix.RotationQuaternion(Quaternion.Invert(parent.Rotation)));
            }
        }

        public Quaternion Rotation
        {
            get { return parent.Rotation * RelativeRotation; }
            set { RelativeRotation = Quaternion.Invert(parent.Rotation)*value; }
        }

        public BoundingBox LocalBoundingBox
        {
            get { return decorated.LocalBoundingBox; }
        }

        public IRelativePositionComponent Parent
        {
            get { return parent == nullComponent ? null : parent; }
            set
            {
                if (value != null)
                {
                    // Detect loop
                    var current = value;
                    while (current != null)
                    {
                        if (current == this) throw new InvalidOperationException("Circular relative positioning parent detected!!!");
                        current = current.Parent;
                    }
                }
                parent = value ?? nullComponent;
            }
        }

        public Vector3 RelativePosition { get; set; }
        public Quaternion RelativeRotation { get; set; }

        /// <summary>
        /// Updates the state of the underlying component
        /// TODO: replace this with an observer to changes in RelativePositionComponents!! 
        /// Add change logging to RelativePositionComponent, attach an observer that creates a list
        /// of all changed components, and then apply this algorithm on those components.
        /// Note that this components responsibility is still required when using this optimization!
        /// NOTE: inverting the decorated is probably even better, as described in <see cref="RelativePositionComponent"/>
        /// </summary>
        public void UpdateDecoratedComponent()
        {
            decorated.Position = Position;
            decorated.Rotation = Rotation;
        }

        private class NullRelativePositionComponent : IRelativePositionComponent
        {
            public Vector3 Position
            {
                get { return Vector3.Zero; }
                set { throw new System.NotImplementedException(); }
            }

            public Quaternion Rotation
            {
                get { return Quaternion.Identity; }
                set { throw new System.NotImplementedException(); }
            }

            public BoundingBox LocalBoundingBox
            {
                get { throw new System.NotImplementedException(); }
            }

            public IRelativePositionComponent Parent
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }

            public Vector3 RelativePosition
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }

            public Quaternion RelativeRotation
            {
                get { throw new System.NotImplementedException(); }
                set { throw new System.NotImplementedException(); }
            }
        }
    }
}