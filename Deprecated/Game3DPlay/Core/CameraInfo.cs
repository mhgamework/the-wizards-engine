using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace MHGameWork.Game3DPlay.Core
{
	public class CameraInfo
	{
		private Matrix view;
		private Matrix proj;
		private Matrix world;

		private Matrix viewProj;
		private Matrix inverseView;

		public CameraInfo()
		{
			viewProj = Matrix.Identity;
			proj = Matrix.Identity;
			world = Matrix.Identity;
			viewProj = Matrix.Identity;
		}

		public Matrix ViewMatrix
		{
			get { return view; }
			set
			{
				view = value;
				viewProj = view * proj;
				inverseView = Matrix.Invert( view );
			}
		}

		public Matrix ProjectionMatrix
		{
			get { return proj; }
			set
			{
				proj = value;
				viewProj = view * proj;
			}
		}

		public Matrix WorldMatrix
		{
			get { return world; }
			set
			{
				world = value;
				//viewProj = view * proj;
			}
		}

		public Matrix ViewProjectionMatrix { get { return viewProj; } }
		public Matrix InverseViewMatrix { get { return inverseView; } }
		public BoundingFrustum Frustum;
	}
}
