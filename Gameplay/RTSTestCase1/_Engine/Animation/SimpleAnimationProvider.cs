namespace MHGameWork.TheWizards.RTSTestCase1.Animation
{
    /// <summary>
    /// Simple factory implementation
    /// </summary>
    public class SimpleAnimationProvider : IAnimationProvider
    {
        public IAnimator CreateAnimator()
        {
            return new Animator();
        }
    }
}