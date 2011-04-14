using System;
using System.Collections.Generic;
using System.Text;
using MHGameWork.Game3DPlay;
using MHGameWork.Game3DPlay.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MHGameWork.Game3DPlay.Core
{
	public class Camera : SpelObject
	{
		public Camera(SpelObject nParent)
			: base( nParent )
		{
			_cameraInfo = new CameraInfo();
			CameraInfo.ViewMatrix = Matrix.Identity;//Matrix.CreateLookAt(new Vector3(0, 0, -4000), Vector3.Zero, Vector3.Up);
			CameraInfo.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView( MathHelper.ToRadians( 45.0f ),
					4 / 3, 1.23456f, 10000.0f );

			CameraUp = Vector3.Up;
			CameraDirection = Vector3.Backward;//???
			CameraPosition = new Vector3( 0, 0, -4000 );



			UpdateCameraInfo();
		}

		private CameraInfo _cameraInfo;
		public CameraInfo CameraInfo
		{
			get { return _cameraInfo; }
			set { _cameraInfo = value; }
		}


		public float angleX;
		public float angleY;
		public float angleZ;
		private Vector3 vLookAt;
		private Vector3 vLookDir;
		private Vector3 vLookEye;
		private Vector3 vLookUp;

		private bool mChanged;


		public void UpdateCameraInfo()
		{
			if ( mChanged )
			{
				mChanged = false;
				_cameraInfo.ViewMatrix = Matrix.CreateLookAt( this.vLookEye, this.vLookAt, this.vLookUp );
				_cameraInfo.Frustum = new BoundingFrustum( _cameraInfo.ViewMatrix * _cameraInfo.ProjectionMatrix );

			}
		}

		public Vector3 CameraPosition
		{
			get
			{
				return this.vLookEye;
			}
			set
			{
				this.vLookEye = value;
				this.mChanged = true;
				/*if (this.curStyle == CameraStyle.PositionBased)
				{*/
				this.vLookAt = Vector3.Add( this.vLookEye, this.vLookDir );
				//}
			}
		}
		public Vector3 CameraDirection
		{
			get
			{
				return this.vLookDir;
			}
			set
			{
				this.vLookDir = value;
				this.mChanged = true;
				/*if (this.curStyle == CameraStyle.PositionBased)
				{*/
				this.vLookAt = Vector3.Add( this.vLookEye, this.vLookDir );
				//}
			}
		}
		public Vector3 CameraUp
		{
			get
			{
				return this.vLookUp;
			}
			set
			{
				this.vLookUp = value;
				this.mChanged = true;
				/*if (this.curStyle == CameraStyle.PositionBased)
				{*/
				this.mChanged = true;
				//}
			}
		}







		public float AngleVertical
		{
			get
			{
				return this.angleX;
			}
			set
			{
				if ( value != this.angleX )
				{
					this.angleX = value;
					this.mChanged = true;
					/*if (this.curStyle == CameraStyle.PositionBased)
					{*/
					Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll( -this.angleY, -this.angleX, -this.angleZ );
					Vector3 source = new Vector3( 0f, 0f, 1f );
					this.CameraDirection = Vector3.TransformNormal( source, sourceMatrix );
					source = new Vector3( 0f, 1f, 0f );
					this.CameraUp = Vector3.TransformNormal( source, sourceMatrix );
					//}
				}
			}
		}
		public float AngleRoll
		{
			get
			{
				return this.angleZ;
			}
			set
			{
				this.angleZ = value;
				this.mChanged = true;
				/*if (this.curStyle == CameraStyle.PositionBased)
				{*/
				Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll( -this.angleY, -this.angleX, -this.angleZ );
				Vector3 source = new Vector3( 0f, 0f, 1f );
				this.CameraDirection = Vector3.TransformNormal( source, sourceMatrix );
				source = new Vector3( 0f, 1f, 0f );
				this.CameraUp = Vector3.TransformNormal( source, sourceMatrix );
				//}
			}
		}
		public float AngleHorizontal
		{
			get
			{
				return this.angleY;
			}
			set
			{
				if ( value != this.angleY )
				{
					this.angleY = value;
					this.mChanged = true;
					/*if (this.curStyle == CameraStyle.PositionBased)
					{*/
					Matrix sourceMatrix = Matrix.CreateFromYawPitchRoll( -this.angleY, -this.angleX, -this.angleZ );
					Vector3 source = new Vector3( 0f, 0f, 1f );
					this.CameraDirection = Vector3.TransformNormal( source, sourceMatrix );
					source = new Vector3( 0f, 1f, 0f );
					this.CameraUp = Vector3.TransformNormal( source, sourceMatrix );
					//}
				}
			}
		}

	}
}
