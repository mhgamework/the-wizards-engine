using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.WorldRendering
{
    /// <summary>
    /// Responsible for processing changes to a textarea
    /// </summary>
    public class TextareaUpdater
    {
        private List<Textarea> areas = new List<Textarea>();

        public void Update()
        {
            foreach (var change in TW.Model.GetChangesOfType<Textarea>())
            {
                if (change.Change == ModelContainer.ModelChange.Added)
                    areas.Add(change.ModelObject as Textarea);
                else if (change.Change == ModelContainer.ModelChange.Removed)
                    areas.Remove(change.ModelObject as Textarea);
            }
        }
    }
}
