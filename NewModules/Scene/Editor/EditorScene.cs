using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.TheWizards.Entities.Editor;
using MHGameWork.TheWizards.Entity.Editor;

namespace MHGameWork.TheWizards.Editor.Scene
{
    public class EditorScene
    {
        public SceneCoreData CoreData = new SceneCoreData();

        public EditorEntity CreateEntity(EditorObject eObj)
        {
            // TODO
            EditorEntity ent = new EditorEntity();
            ent.CoreData.ObjectType = eObj;

            CoreData.Entities.Add(ent);

            return ent;
        }
    }
}
