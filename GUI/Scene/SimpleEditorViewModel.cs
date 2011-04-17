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
        public List<MeshItem> PlaceMeshes { get; private set; }
        private MeshItem selectedPlaceMesh;
        public MeshItem SelectedPlaceMesh
        {
            get { return selectedPlaceMesh; }
            set
            {
                selectedPlaceMesh = value; 
                if (editor != null)
                {
                    if (value == null)
                        editor.PlaceModeMesh = null;
                    else
                        editor.PlaceModeMesh = selectedPlaceMesh.Mesh;
                }
            }
        }

        public string SelectedEntityMesh { get; private set; }


        public SimpleEditorViewModel()
        {
            SelectedEntityMesh = "Hello!";
            PlaceMeshes = new List<MeshItem>();
            PlaceMeshes.Add(new MeshItem { Name = "Mesh1", Mesh = null });
            PlaceMeshes.Add(new MeshItem { Name = "Mesh2", Mesh = null });

            SelectedPlaceMesh = PlaceMeshes[1];

        }

        public static SimpleEditorViewModel Create(IXNAGame game, SceneEditor editor)
        {
            var vm = new SimpleEditorViewModel();
            vm.editor = editor;

            vm.PlaceEntity = new DelegateCommand(o => game.InvokeUpdate(editor.EnablePlaceEntityMode));

            return vm;

        }

        public class MeshItem
        {
            public string Name;
            public IMesh Mesh;

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
