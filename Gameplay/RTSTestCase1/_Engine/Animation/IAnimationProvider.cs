namespace MHGameWork.TheWizards.RTSTestCase1.Animation
{
    /// <summary>
    /// Factory for animators.
    /// </summary>
    public interface IAnimationProvider
    {
        IAnimator CreateAnimator();
    }
}