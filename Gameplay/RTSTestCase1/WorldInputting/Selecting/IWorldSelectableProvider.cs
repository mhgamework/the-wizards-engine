using System.Collections.Generic;

namespace MHGameWork.TheWizards.RTSTestCase1.WorldInputting.Selecting
{
    /// <summary>
    /// Provides selectables to a WorldSelector
    /// </summary>
    public interface IWorldSelectableProvider
    {
        IEnumerable<Selectable> GetSelectables();
        void SetTargeted(Selectable selectable);
        void Select(Selectable selectable);
        void Render();
    }
}