using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using MHGameWork.Game3DPlay;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.Game3DPlay.Core.Elements;
using MHGameWork.Game3DPlay.SpelObjecten;

namespace MHGameWork.Game3DPlay.SpelObjecten
{
    public class Ruimteschip : MHGameWork.Game3DPlay.SpelObjecten.XNBModel, MHGameWork.Game3DPlay.Core.Elements.IProcessable
    {
        public Ruimteschip(SpelObject nParent)
            : base(nParent)
        {
            RelativePath = "Content\\Models\\p1_wedge";

            //processElement = new ProcessElement(this);
            _snelheid = Vector3.Zero;

        }

        private Vector3 _snelheid;
                public Vector3 Snelheid
        {
            get { return _snelheid; }
            set { _snelheid = value; }
        }
	


        //ProcessElement processElement;

        #region IProcessable Members

        public void OnProcess(object sender, MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
        {
            //if (e.Keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.R))
            //{
            Positie += Snelheid * e.Elapsed;
            //}
        }

        #endregion
    }
}

