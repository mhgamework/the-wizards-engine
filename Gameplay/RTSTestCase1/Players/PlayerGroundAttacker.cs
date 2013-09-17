using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MHGameWork.TheWizards.DirectX11.Graphics;
using MHGameWork.TheWizards.Engine.Worlding;
using MHGameWork.TheWizards.RTSTestCase1.Animation;

using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Players
{
    /// <summary>
    /// Implements a ground attack. At the ground below a targeted point a rock emerges.
    /// 
    /// This should do:
    /// find out where to create a rock and what kind of rock
    /// show animation of rock coming out of surface (for user clearability)
    /// do damage
    /// show before attack hint
    /// </summary>
    public class PlayerGroundAttacker
    {
        private IUserTargeter targeter;
        private IRockCreator rockCreator;
        private IAnimator animator;

        public IDamageApplier DamageApplier { get; set; }
        public IWorldLocator WorldLocator { get; set; }

        public PlayerGroundAttacker(IUserTargeter targeter, IRockCreator rockCreator, IAnimationProvider animationProvider)
        {
            this.targeter = targeter;
            this.rockCreator = rockCreator;
            animator = animationProvider.CreateAnimator();
        }

        public void Attack(float strength)
        {
            if (targeter.Targeted == null) return;

            var point = targeter.TargetPoint;
            point.Y = 0;

            emergeRockAt(point,strength);

        }

        private void emergeRockAt(Vector3 point, float strength)
        {
            var rock = rockCreator.CreateRock();

            var desc = animator.CreateDescription();
            var prop = desc.CreateProperty(() => rock.Position, v => rock.Position = v);
            prop.AddKey(0, point);
            prop.AddKey(0.1f, point + Vector3.UnitY * strength);
            prop.AddKey(0.2f, point + Vector3.UnitY * strength);
            prop.AddKey(0.25f, point + Vector3.UnitY * strength);
            prop.AddKey(0.35f, point - Vector3.UnitY * 1.001f);

            
            desc.AddAction(0.1f, delegate
                {
                    foreach (var o in getObjectsHitByRock(point).ToArray<object>())
                    {
                        DamageApplier.ApplyDamage(o);
                    }
                });

            desc.AddAction(2, () => rockCreator.DestroyRock(rock));

            animator.Run(desc);
        }

        private IEnumerable<object> getObjectsHitByRock(Vector3 point)
        {
            //TODO: specify by exact rock collision.
            return WorldLocator.AtPosition(point, 1f);
        }

        public void Update(float elapsed)
        {
            animator.Update(elapsed);
        }

        public void Render(LineManager3D l)
        {
            if (targeter.Targeted == null) return;

            var point = targeter.TargetPoint;

            l.AddCenteredBox(point, 0.2f, new Color4(1, 1, 0));
        }
    }

    /// <summary>
    /// Use RTS scope interface IWorldObject?
    /// </summary>
    public interface IDamageApplier
    {
        void ApplyDamage(object o);
    }


    /// <summary>
    /// Factory for IRock
    /// </summary>
    public interface IRockCreator
    {
        IRock CreateRock();
        void DestroyRock(IRock rock);
    }

    /// <summary>
    /// A rock that is created as a result of a ground attack
    /// </summary>
    public interface IRock
    {
        Vector3 Position { get; set; }
    }
}