using System.Collections;
using MHGameWork.TheWizards.DirectX11.Graphics;
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

        public PlayerGroundAttacker(IUserTargeter targeter, IRockCreator rockCreator, IAnimationProvider animationProvider)
        {
            this.targeter = targeter;
            this.rockCreator = rockCreator;
            animator = animationProvider.CreateAnimator();
        }

        public void Attack()
        {
            if (targeter.Targeted == null) return;

            var point = targeter.TargetPoint;
            point.Y = 0;

            emergeRockAt(point);

        }

        private void emergeRockAt(Vector3 point)
        {
            var rock = rockCreator.CreateRock();

            var desc = animator.CreateDescription();
            var prop = desc.CreateProperty(() => rock.Position, v => rock.Position = v);
            prop.AddKey(0, point);
            prop.AddKey(0.1f, point + Vector3.UnitY);

            desc.AddAction(2, () => rockCreator.DestroyRock(rock));

            animator.Run(desc);
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