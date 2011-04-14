//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxClothMeshDesc : NxSimpleTriangleMesh
	{
		public NxClothMeshTarget target;

		public static NxClothMeshDesc Default
			{get{return new NxClothMeshDesc();}}

		public NxClothMeshDesc()
			{setToDefault();}

		public override void setToDefault()
		{
			base.setToDefault();
			target = NxClothMeshTarget.NX_CLOTH_MESH_SOFTWARE;
		}
	}
}
