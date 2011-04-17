using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scene.Editor;

namespace MHGameWork.TheWizards.Scene
{
    public class SimpleEditorViewModel
    {
        private SceneEditor editor;
        public ICommand JumpInto { get; private set; }
        public ICommand PlaceEntity { get; private set; }
        public Dictionary<string, IMesh> PlaceMeshes { get; private set; }


        public string SelectedEntityMesh { get; private set; }


        public SimpleEditorViewModel()
        {
            SelectedEntityMesh = "Hello!";
            PlaceMeshes = new Dictionary<string, IMesh>();
            PlaceMeshes.Add("Mesh1", new RAMMesh());
            PlaceMeshes.Add("Mesh2", new RAMMesh());

        }

        public static SimpleEditorViewModel Create(IXNAGame game, SceneEditor editor)
        {
            var vm = new SimpleEditorViewModel();
            vm.editor = editor;

            vm.PlaceEntity = new DelegateCommand(o => game.InvokeUpdate(editor.EnablePlaceEntityMode));

            return vm;

        }
    }
}
