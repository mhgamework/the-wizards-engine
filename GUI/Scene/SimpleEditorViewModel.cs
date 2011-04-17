﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MHGameWork.TheWizards.Graphics;
using MHGameWork.TheWizards.Scene.Editor;

namespace MHGameWork.TheWizards.Scene
{
    public class SimpleEditorViewModel
    {
        private readonly SceneEditor editor;
        public ICommand JumpInto { get; private set; }
        public ICommand PlaceEntity { get; private set; }

        public SimpleEditorViewModel()
        {

        }
        public SimpleEditorViewModel(IXNAGame game, SceneEditor editor)
        {
            this.editor = editor;

            PlaceEntity = new DelegateCommand(o => game.InvokeUpdate(editor.EnablePlaceEntityMode));

        }
    }
}
