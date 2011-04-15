//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public struct NxRaycastHit
	{
		public	IntPtr		shapePtr;			//!< Touched shape (NX_RAYCAST_SHAPE)
		public	Vector3		worldImpact;		//!< Impact point in world space (NX_RAYCAST_IMPACT)
		public	Vector3		worldNormal;		//!< Impact normal in world space (NX_RAYCAST_NORMAL / NX_RAYCAST_FACE_NORMAL)
		public	uint		faceID;				//!< Index of touched face (NX_RAYCAST_FACE_INDEX)
		public  uint		internalFaceID;
		public	float		distance;			//!< Distance from ray start to impact point (NX_RAYCAST_DISTANCE)
		public	float		u,v;				//!< Impact barycentric coordinates (NX_RAYCAST_UV)
		public	ushort		materialIndex;		//!< Index of touched material (NX_RAYCAST_MATERIAL)
		public	uint		flags;				//!< Combination of ::NxRaycastBit, validating above members.
	
		public NxShape Shape
			{get{return NxShape.createFromPointer(shapePtr);}}
	};

}


/*
struct NxRaycastHit
{
	NxShape*		shape;
	NxVec3			worldImpact;
	NxVec3			worldNormal;
	NxU32			faceID;
	NxU32			internalFaceID;
	NxReal			distance;
	NxReal			u,v;
	NxMaterialIndex	materialIndex;
	NxU32			flags;											
};
*/