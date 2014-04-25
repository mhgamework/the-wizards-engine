using System.Collections.Generic;
using MHGameWork.TheWizards.Scattered.Core.Bindings;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Tower : IIslandAddon, IDebugAddon
    {
        private readonly Level level;

        public Tower(Level level, SceneGraphNode node)
        {
            this.level = level;
            Node = node;
            node.AssociatedObject = this;

            var ent = level.CreateEntityNode(node);
            ent.Entity.Mesh = TW.Assets.LoadMesh("Scattered\\Models\\Tower");
        }

        public SceneGraphNode Node { get; private set; }

        public bool IsOperational { get { return BuildPercentage >= 1; } }
        //TODO: bound 0 to 1
        public float BuildPercentage { get; private set; }


        public void PrepareForRendering()
        {
            // Move this to the update method?
            updateBehaviour();
        }

        private int state = 0;


        private float timeWaiting = 0;
        private IEnumerator<float> behaviourEnumerable;
        private void updateBehaviour()
        {
            if (timeWaiting > 0.001)
            {
                timeWaiting -= TW.Graphics.Elapsed;
                return;
            }

            if (behaviourEnumerable != null && !behaviourEnumerable.MoveNext())
                behaviourEnumerable = null;

            // In this implementation i try to have movenext and current next to eachother since i do not know which one of the 2 executes the enumerable
            if (behaviourEnumerable == null)
            {
                behaviourEnumerable = stepBehaviour().GetEnumerator();
                if (!behaviourEnumerable.MoveNext()) return; // empty enumerator, throw exception?
            }
            timeWaiting = behaviourEnumerable.Current;
        }

        public IEnumerable<float> stepBehaviour()
        {
            //if (IsOperational)
            if (IsOperational)
                foreach (var f in stepOperationalBehaviour()) yield return f;
            else
                foreach (var f in stepBuildingBehaviour()) yield return f;
        }

        private IEnumerable<float> stepBuildingBehaviour()
        {
            var closest = level.FindClosest<Resource>(Node, 10, r => r.Type == level.CoalType);
            if (closest == null)
            {
                yield return 0.1f; // Wait for local change
                yield break;
            }

            closest.TakeAmount(1);

            var numberSecondsToFullBuild = 10;

            var growth = 0.1f;
            while (growth > 0 && BuildPercentage < 1)
            {
                var player = level.FindClosest<ScatteredPlayer>(Node, 10, _ => true);
                if (player == null)
                {
                    yield return 0.1f; // Wait when not player near
                    continue;
                }

                var diff = TW.Graphics.Elapsed * (1f / numberSecondsToFullBuild);
                BuildPercentage += diff;
                growth -= diff;

                if (BuildPercentage > 1) BuildPercentage = 1;
                yield return 0;

            }




        }

        private IEnumerable<float> stepOperationalBehaviour()
        {
            // find enemy islands
            var enemy = level.FindClosest<Enemy>(Node, 60, _ => true);
            if (enemy == null)
            {
                yield return 0.1f;
                yield break;
            }


            var remainingShooting = 0.3f;

            while (remainingShooting > 0)
            {
                remainingShooting -= TW.Graphics.Elapsed;
                TW.Graphics.LineManager3D.AddLine(Node.Absolute.GetTranslation() + Vector3.UnitY * 10f,
                                                  enemy.Node.Absolute.GetTranslation(), new Color4(1, remainingShooting, 0));
                if (enemy.Node.Children == null) // Cheat
                    break;
                yield return 0;
            }

            // destroy enemy!
            enemy.Kill();

            yield return 0.3f;
        }

        public string GetDebugText()
        {
            return "Build: " + BuildPercentage;
        }
    }
}