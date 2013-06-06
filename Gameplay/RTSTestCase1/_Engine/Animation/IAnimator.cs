namespace MHGameWork.TheWizards.RTSTestCase1.Animation
{
    /// <summary>
    /// Responsible for running a number of animations
    /// </summary>
    public interface IAnimator
    {
        AnimationDescription CreateDescription();
        void Run(AnimationDescription desc);
        void Update(float elapsed);
    }
}