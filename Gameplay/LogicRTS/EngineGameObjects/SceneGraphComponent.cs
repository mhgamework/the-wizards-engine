using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using MHGameWork.TheWizards.LogicRTS.Framework;
using SlimDX;

namespace MHGameWork.TheWizards.LogicRTS.EngineGameObjects
{
    public class SceneGraphComponent : BaseGameComponent
    {
        public SceneGraphComponent ParentObject
        {
            get { return parentObject; }
            set
            {
                if (value == parentObject) return;
                if (parentObject != null)
                    parentObject.children.Remove(this);

                parentObject = value;
                parentObject.children.Add(this);

                updateWorldTransform();
            }
        }

        public IEnumerable<SceneGraphComponent> ChildObjects { get { return children; } }
        private List<SceneGraphComponent> children;
        private Matrix localTransform;
        private SceneGraphComponent parentObject;
        private Matrix worldTransform;

        public Matrix WorldTransform
        {
            get { return worldTransform; }
            set
            {
                if (worldTransform == value) return;
                var parentInverse = parentObject == null ? Matrix.Identity : Matrix.Invert(parentObject.WorldTransform);
                LocalTransform = value * parentInverse;
            }
        }

        public Matrix LocalTransform
        {
            get { return localTransform; }
            set
            {
                if (localTransform == value) return;
                localTransform = value;
                updateWorldTransform();
            }
        }

        private void updateWorldTransform()
        {
            var parentTransform = parentObject == null ? Matrix.Identity : parentObject.WorldTransform;
            worldTransform = localTransform * parentTransform;
            ChildObjects.ForEach(c => c.updateWorldTransform());
            observableWorldTransform.OnNext(worldTransform);
        }

        private BehaviorSubject<Matrix> observableWorldTransform;
        public IObservable<Matrix> ObservableWorldTransform { get { return observableWorldTransform; } }

        public override void Destroy()
        {
            foreach (var gameObject in children.Select(c => c.GameObject))
            {
                gameObject.Destroy();
            }
            children = null;
            ParentObject = null;
            base.Destroy();
        }
    }
}