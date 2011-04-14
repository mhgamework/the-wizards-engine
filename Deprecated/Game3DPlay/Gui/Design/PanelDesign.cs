using System;
using System.Collections.Generic;
using System.Text;
using Microsoft;
using Microsoft.Xna.Framework;

namespace MHGameWork.Game3DPlay.Gui.Design
{
	public static class PanelDesign
	{
		public static void FillPanel(Panel nPanel, System.Windows.Forms.Control nControl)
		{
			PanelDesign.SetBasicInfo(nPanel, nControl);
			nPanel.BackgroundColor = PanelDesign.ConvertColor( nControl.BackColor);

			object obj;
			IDesignPanelPart idpp;
			foreach (System.Windows.Forms.Control c in nControl.Controls)
			{
				obj = c;
				if (PanelDesign.ImplementsInterface(obj, typeof(IDesignPanelPart)))
				{
					idpp = (IDesignPanelPart)obj;
					idpp.AddToPanel(nPanel);
				}
			}
		}

		public static bool ImplementsInterface(object obj, Type interf)
		{
			foreach (Type I in obj.GetType().GetInterfaces())
			{
				if (I.Equals(interf))
				{
					return true;
				}
			}
			return false;
		}


		public static Vector2 ConvertVector2(System.Drawing.Point nP)
		{
			return new Vector2(nP.X, nP.Y);
		}

		public static Vector2 ConvertVector2(System.Drawing.Size nS)
		{
			return new Vector2(nS.Width, nS.Height);
		}
		public static Microsoft.Xna.Framework.Graphics.Color ConvertColor(System.Drawing.Color nC)
		{
			return new Microsoft.Xna.Framework.Graphics.Color(nC.R,nC.G,nC.B,nC.A);
		}

		public static PanelPart.AnchorType ConvertAnchor(System.Windows.Forms.AnchorStyles nA)
		{
			PanelPart.AnchorType ret = PanelPart.AnchorType.None;
			if ((int)(nA & System.Windows.Forms.AnchorStyles.Top) > 0)
			{
				ret = ret | PanelPart.AnchorType.Top;
			}
			if ((int)(nA & System.Windows.Forms.AnchorStyles.Right) > 0)
			{
				ret = ret | PanelPart.AnchorType.Right;
			}
			if ((int)(nA & System.Windows.Forms.AnchorStyles.Bottom) > 0)
			{
				ret = ret | PanelPart.AnchorType.Bottom;
			}
			if ((int)(nA & System.Windows.Forms.AnchorStyles.Left) > 0)
			{
				ret = ret | PanelPart.AnchorType.Left;
			}

			return ret;
		}
		/*
			public void ConvertDrawTextFormat(ByVal nA As System.Drawing.ContentAlignment) As Direct3D.DrawTextFormat
			{    Select Case nA
					Case ContentAlignment.TopLeft
						Return Direct3D.DrawTextFormat.Top && Direct3D.DrawTextFormat.Left
					Case ContentAlignment.TopCenter
						Return Direct3D.DrawTextFormat.Top && Direct3D.DrawTextFormat.Center
					Case ContentAlignment.TopRight
						Return Direct3D.DrawTextFormat.Top && Direct3D.DrawTextFormat.Right

					Case ContentAlignment.MiddleLeft
						Return Direct3D.DrawTextFormat.VerticalCenter && Direct3D.DrawTextFormat.Left
					Case ContentAlignment.MiddleCenter
						Return Direct3D.DrawTextFormat.VerticalCenter && Direct3D.DrawTextFormat.Center
					Case ContentAlignment.MiddleRight
						Return Direct3D.DrawTextFormat.VerticalCenter && Direct3D.DrawTextFormat.Right

					Case ContentAlignment.BottomLeft
						Return Direct3D.DrawTextFormat.Bottom && Direct3D.DrawTextFormat.Left
					Case ContentAlignment.BottomCenter
						Return Direct3D.DrawTextFormat.Bottom && Direct3D.DrawTextFormat.Center
					Case ContentAlignment.BottomRight
						Return Direct3D.DrawTextFormat.Bottom && Direct3D.DrawTextFormat.Right

					Case Else
						Return Direct3D.DrawTextFormat.None
				End Select
				}
		*/
		public static void SetBasicInfo(MHGameWork.Game3DPlay.Gui.PanelPart nPart, System.Windows.Forms.Control nControl)
		{
			nPart.Positie = PanelDesign.ConvertVector2(nControl.Location);
			nPart.Size = PanelDesign.ConvertVector2(nControl.Size);
			nPart.Anchor = PanelDesign.ConvertAnchor(nControl.Anchor);
		}

	}
}
