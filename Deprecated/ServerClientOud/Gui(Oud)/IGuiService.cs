using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.Gui
{
    public interface IGuiService
    {
        /// <summary>
        /// ONLY TEMPORARELY USED IN RENDER TO TEXTURE
        /// </summary>
        XNAGame Game
        {
            get;
            set;
        }

        void Process();
        void Render();
        void Tick( MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e );
        void StartDrag( GuiControl control );
    }
}
