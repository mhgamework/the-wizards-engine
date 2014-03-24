using System.Collections.Generic;
using MHGameWork.TheWizards.Scattered.Model;
using MHGameWork.TheWizards.Scattered.SceneGraphing;
using System.Linq;
using DirectX11;
using SlimDX;

namespace MHGameWork.TheWizards.Scattered.Core
{
    public class Tower : IIslandAddon
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

        private float timeWaiting = 0;
        private IEnumerator<float> behaviourEnumerable;
        public void PrepareForRendering()
        {
            // Move this to the update method?
            updateBehaviour();
        }

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
            // find enemy islands
            var enemyIslands = level.Islands.Where(i => i.Addons.OfType<Enemy>().Any() && Vector3.Distance(i.Node.Absolute.GetTranslation(), Node.Absolute.GetTranslation()) < 60);
            if (!enemyIslands.Any())
            {
                yield return 0.5f;
                yield break;
            }

            var target = enemyIslands.First();
            var enemy = target.Addons.OfType<Enemy>().First();

            var remainingShooting = 1f;

            yield return 0;
            while (remainingShooting > 0)
            {
                remainingShooting -= TW.Graphics.Elapsed;
                TW.Graphics.LineManager3D.AddLine(Node.Absolute.GetTranslation() + Vector3.UnitY * 10f, enemy.Node.Absolute.GetTranslation(), new Color4(1, remainingShooting, 0));
                if (enemy.Node.Children == null) // Cheat
                    break;
                yield return 0;
            }

            // destroy enemy!
            level.DestroyNode(enemy.Node);

            yield return 1;



        }
    }
}