using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using System.Linq;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins.Components
{
    [ModelObjectChanged]
    public class ItemStoragePart : EngineModelObject,IObjectPart
    {
        public ItemStoragePart()
        {
            EnvDTE.Project f = null;

            foreach (var c in f.CodeModel.CodeElements)
            {
                
            }

            var mine = f.CodeModel.CodeElements.OfType<EnvDTE.CodeNamespace>()
             .First(g => g.FullName == "MHGameWork.TheWizards.RTSTestCase1.Goblins.Components");
            foreach (var source in mine.Members.OfType<EnvDTE.CodeClass>())
            {
                var i = source.Bases;
            }

        }
    }
}