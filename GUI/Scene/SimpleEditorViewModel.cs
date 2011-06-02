using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Input;
using MHGameWork.TheWizards.Entity;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Rendering;
using MHGameWork.TheWizards.Scene.Editor;

namespace MHGameWork.TheWizards.Scene
{
    public class SimpleEditorViewModel
    {
        private SimpleEditorForm form;
        private SceneEditor editor;
        public ICommand JumpInto { get; private set; }
        public ICommand Select { get; private set; }
        public ICommand PlaceEntity { get; private set; }
        public List<MeshItem> PlaceMeshes { get; private set; }
        public List<ScriptItem> Scripts { get; private set; }
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
        public ScriptItem SelectedScript { get; set; }

        public string SelectedEntityMesh { get; private set; }
        public bool SelectedEntitySolid { get; set; }
        public bool SelectedEntityStatic { get; set; }


        private Entity selectedEntity;

        public SimpleEditorViewModel()
        {
            SelectedEntityMesh = "Hello!";
            PlaceMeshes = new List<MeshItem>();
            Scripts = new List<ScriptItem>();

        }

        public static SimpleEditorViewModel Create(IXNAGame game, SceneEditor editor, SimpleEditorForm form)
        {
            var vm = new SimpleEditorViewModel();
            vm.editor = editor;
            vm.form = form;
            vm.Select = new DelegateCommand(o => game.InvokeUpdate(editor.EnableSelectMod));
            vm.PlaceEntity = new DelegateCommand(o => game.InvokeUpdate(editor.EnablePlaceEntityMode));
            vm.form.cAssignScript.Click += delegate { game.InvokeUpdate(() => editor.AssignScriptToSelected(vm.SelectedScript.FileInfo)); };

            var t = new Thread(vm.pollJob);
            t.IsBackground = true;
            t.Name = "SimpleEditorViewModelPoll";
            t.Start();

            return vm;

        }

        private void updateSelectedEntity()
        {
            if (editor.SelectedEntity == selectedEntity) return;

            selectedEntity = editor.SelectedEntity;
            form.cEntityProperties.DataContext = selectedEntity;
        }

        private void pollJob()
        {
            // This is actually a timer but im lazy
            for (; ; )
            {
                form.Dispatcher.Invoke(new Action(pollChanges));

                Thread.Sleep(100);
            }
        }

        private void pollChanges()
        {
            updateSelectedEntity();
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
        public class ScriptItem
        {
            public string Name;
            public FileInfo FileInfo;

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
