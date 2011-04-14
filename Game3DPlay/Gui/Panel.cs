using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay.Core;
using MHGameWork.Game3DPlay.Core.Elements;

namespace MHGameWork.Game3DPlay.Gui
{
	public class Panel : PanelPart
	{

		public Panel(SpelObject nParent)
			: base(nParent)
		{
			SetStartValues();


			//AddHandler HoofdObj.SizeChanged, AddressOf OnParentSizeChanged
			//setClickedElement(new ClickedEventElement(Me))
		}


		protected Renderer2DContainer renderer2DContainer;

		public Panel(Panel nParent)
			: base(nParent)
		{
			SetStartValues();
		}

		private void SetStartValues()
		{
			Parts = new List<PanelPart>();

			renderer2DContainer = new Renderer2DContainer(this);
			base.FocusLost += new FocusLostEventHandler(Panel_FocusLost);
			base.PositieChanged += new EventHandler<PositieChangedEventArgs>(Panel_PositieChanged);
			base.SizeChanged += new EventHandler<SizeChangedEventArgs>(Panel_SizeChanged);
		}



		/*private WithEvents mClickedElement As ClickedEventElement
		public ReadOnly Property ClickedElement() As ClickedEventElement
			<System.Diagnostics.DebuggerStepThrough()> Get
				Return mClickedElement
			End Get
		End Property
		private void setClickedElement(ByVal value As ClickedEventElement)
			mClickedElement = value
		End void*/



		private Microsoft.Xna.Framework.Graphics.Color _backgroundColor;

		public Microsoft.Xna.Framework.Graphics.Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				if (renderer2DContainer != null) renderer2DContainer.BackgroundColor = value;
			}
		}



		public override void Focus()
		{
			if (Panel == null)
			{
				//Renderer2DContainer.SendToFront();
			}
			else
			{
				base.Focus();
			}

		}

		private PanelPart _focusedPart;
		/// <summary>
		/// Nothing: geen enkel element heeft focus in deze panel
		/// Part: deze panel heeft focus en de part heeft focus
		/// </summary>
		public PanelPart FocusedPart
		{
			get { return _focusedPart; }
			private set { _focusedPart = value; }
		}





		public void FocusPart(PanelPart nPart)
		{
			if (nPart == FocusedPart) return;
			if (nPart == null) throw new Exception();

			if (FocusedPart != null)
			{
				FocusedPart.HasFocus = false;
			}
			FocusedPart = nPart;
			//if nPart IsNot Nothing Then
			nPart.HasFocus = true;
			//End if


			if (Panel == null)
			{
				Focus();
			}

		}

		protected void UnFocusPart()
		{
			if (FocusedPart != null)
			{
				FocusedPart.HasFocus = false;
				FocusedPart = null;
			}
		}

		private List<PanelPart> _parts;

		public List<PanelPart> Parts
		{
			get { return _parts; }
			private set { _parts = value; }
		}



		public override void AddChild(SpelObject nSpO)
		{
			base.AddChild(nSpO);
			if (nSpO is PanelPart) Parts.Add((PanelPart)nSpO);
		}
		public override void RemoveChild(SpelObject nSpO)
		{
			base.RemoveChild(nSpO);
			if (nSpO is PanelPart) Parts.Remove((PanelPart)nSpO);

		}
		void Panel_FocusLost(object sender, EventArgs e)
		{
			UnFocusPart();
		}


		void Panel_PositieChanged(object sender, PanelPart.PositieChangedEventArgs e)
		{
			renderer2DContainer.Positie = Positie;
		}



		void Panel_SizeChanged(object sender, PanelPart.SizeChangedEventArgs e)
		{
			renderer2DContainer.Size = Size;
		}



		/*private void mClickedElement_Clicked(ByVal sender As Object, ByVal e As ClickedElement.ClickedEventArgs) Handles mClickedElement.Clicked
			if CollMath.PointInBox(Positie, Size, e.PointClicked) Then
				OnClicked(sender, e)
			End if
		End void

		public Overrides void OnClicked(ByVal sender As Object, ByVal e As ClickedElement.ClickedEventArgs)
			Dim newE As new ClickedElement.ClickedEventArgs(e.Cursor, e.PointClicked - Positie, e.Button, e.State)

			MyBase.OnClicked(sender, newE)
			Focus()

			Dim FocusChanged As Boolean = False


			For Each P As PanelPart In Parts
				if e.Handled Then Exit For
				if P.Enabled Then
					if CollMath.PointInBox(P.Positie, P.Size, newE.PointClicked) Then
						if FocusChanged = False Then
							P.Focus()
							FocusChanged = True
						End if
						P.OnClicked(sender, newE)
					End if
				End if
			Next



			e.Handled = newE.Handled
			if FocusChanged = False Then UnFocusPart()

		End void*/

	}
}
