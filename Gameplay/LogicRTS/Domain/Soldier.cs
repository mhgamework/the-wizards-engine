using System.Drawing;
using System.Linq;
using System.Reactive;
using MHGameWork.TheWizards.LogicRTS.EngineGameObjects;
using MHGameWork.TheWizards.LogicRTS.Framework;
using MHGameWork.TheWizards.RTSTestCase1;
using SlimDX;

namespace MHGameWork.TheWizards.LogicRTS
{
    public class Soldier : BaseGameComponent
    {
        private readonly Scheduler scheduler;
        private readonly UnitSenses senses;
        private readonly Unit unit;

        public Soldier(Scheduler scheduler, UnitSenses senses, Unit unit, StaticMeshComponent meshC)
        {
            this.scheduler = scheduler;
            this.senses = senses;
            this.unit = unit;

            meshC.Mesh = UtilityMeshes.CreateBoxColored(Color.Red.dx(), new Vector3(1));
        }

        private float lastShot = float.MinValue;
        public float shotInterval = 1;
        public float gunDamage;
        public void Shoot(Unit target)
        {
            if (lastShot + shotInterval > scheduler.CurrentTime) return;
            lastShot = scheduler.CurrentTime;
            target.ApplyDamage(gunDamage);

        }

        public void Think()
        {
            var enemies = senses.Units.Where(u => u.Team != unit.Team);
            if (!enemies.Any()) return;
            Shoot(enemies.First());
        }

    }
}