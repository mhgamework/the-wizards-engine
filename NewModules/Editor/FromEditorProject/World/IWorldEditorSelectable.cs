using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.TheWizards.Editor.World
{
    /// <summary>
    /// Represents a group of things that can be selected using the select button in the world editor.
    /// Probably this should implement the other transform buttons.
    /// This also can be used in general world picking, but this isn't entirely thought out (yet)
    /// 
    /// TODO!!!!
    /// 
    /// </summary>
    public interface IWorldEditorSelectable : Raycast.IRaycastable<IWorldEditorSelectable>
    {
        void SelectLastRaycasted();
        void Deselect();

        /// <summary>
        /// Return the center of the item currently selected
        /// </summary>
        /// <returns></returns>
        Vector3 GetSelectionCenter();

        //These 2 are somewhat fishy to be in this location

        /// <summary>
        /// Called when something of this selectable is selected
        /// </summary>
        void Render();

        /// <summary>
        /// Called when something of this selectable is selected
        /// </summary>
        void Update();



    }
}
