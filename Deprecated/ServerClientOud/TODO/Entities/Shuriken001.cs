using System;
using System.Collections.Generic;
using System.Text;
using NovodexWrapper;
using Microsoft.Xna.Framework;
using MHGameWork.TheWizards.Server;
using Wereld = MHGameWork.TheWizards.Server.Wereld;


namespace MHGameWork.TheWizards.ServerClient.Entities
{
	public class Shuriken001 : Wereld.IClientEntity
	{
		private Vector3 positie;
		private Vector3 scale;
		private Engine.Model model;


		public Shuriken001()
		{
			positie = new Vector3( 0 );
			scale = new Vector3( 4 );

            model = new Engine.Model( ProgramOud.SvClMain, "Content\\Shuriken001" );






		}


		public void Render()
		{
			model.TempRender( Matrix.CreateScale( scale ) * Matrix.CreateTranslation( positie ) );
		}



		public void Process(MHGameWork.Game3DPlay.Core.Elements.ProcessEventArgs e)
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public void Tick(MHGameWork.Game3DPlay.Core.Elements.TickEventArgs e)
		{
			throw new Exception( "The method or operation is not implemented." );
		}

		public Microsoft.Xna.Framework.BoundingSphere BoundingSphere
		{
			get
			{
				//TODO: scale.Length() gebruikt veel tijd
				return new BoundingSphere( positie, scale.Length() );
			}
		}

		public Vector3 Positie
		{ get { return positie; } set { positie = value; } }

		public Vector3 Scale
		{ get { return scale; } set { scale = value; } }









	}
}
