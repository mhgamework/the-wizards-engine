//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxClothDesc
	{
		private IntPtr				clothMeshPtr;
		public NxMat34				globalPose;
		public float				thickness;
		public float				density;
		public float				bendingStiffness;
		public float				stretchingStiffness;
		public float				dampingCoefficient;
		public float				friction;
		public float				pressure;
		public float				tearFactor;
		public float				collisionResponseCoefficient;
		public float				attachmentResponseCoefficient;
		public uint					solverIterations;
		public Vector3				externalAcceleration;
		public NxMeshData			meshData;
		public ushort				collisionGroup;
		public NxGroupsMask			groupsMask;
		public uint					flags;
		public IntPtr				userDataPtr;
		public IntPtr				internalNamePtr;
		private NxClothMesh			internalClothMesh=null;
		public string				name=null;

		public static NxClothDesc Default
			{get{return new NxClothDesc();}}

		public NxClothDesc()
			{setToDefault();}

		public void setToDefault()
		{
			clothMeshPtr					= IntPtr.Zero;
			globalPose						= NxMat34.Identity;
			thickness						= 0.01f;
			density							= 1.0f;
			bendingStiffness				= 1.0f;
			stretchingStiffness				= 1.0f;
			dampingCoefficient				= 0.5f;
			friction						= 0.5f;
			pressure						= 1.0f;
			tearFactor						= 1.5f;
			attachmentResponseCoefficient	= 0.2f;
			collisionResponseCoefficient	= 0.2f;
			flags							= (uint)NxClothFlag.NX_CLF_GRAVITY;
			solverIterations				= 5;
			collisionGroup					= 0;
			externalAcceleration			= new Vector3(0,0,0);
			groupsMask.bits0				= 0;
			groupsMask.bits1				= 0;
			groupsMask.bits2				= 0;
			groupsMask.bits3				= 0;
			meshData						= NxMeshData.Default;
			userDataPtr						= IntPtr.Zero;
			internalNamePtr					= IntPtr.Zero;
			name							= null;
  		}

		public NxClothMesh ClothMesh
		{
			get
			{
				if(internalClothMesh==null)
				{internalClothMesh=new NxClothMesh(clothMeshPtr);}
				return internalClothMesh;
			}
			set
			{
				if(value==null)
				{
					clothMeshPtr=IntPtr.Zero;
					internalClothMesh=null;
				}
				else
				{
					clothMeshPtr=value.NxClothMeshPtr;
					internalClothMesh=new NxClothMesh(clothMeshPtr);
				}
			}
		}

		public bool FlagPressure
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_PRESSURE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_PRESSURE,value);}
		}

		public bool FlagStatic
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_STATIC);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_STATIC,value);}
		}

		public bool FlagDisableCollision
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_DISABLE_COLLISION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_DISABLE_COLLISION,value);}
		}

		public bool FlagSelfCollision
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_SELFCOLLISION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_SELFCOLLISION,value);}
		}

		public bool FlagVisualization
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_VISUALIZATION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_VISUALIZATION,value);}
		}

		public bool FlagGravity
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_GRAVITY);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_GRAVITY,value);}
		}

		public bool FlagBending
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_BENDING);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_BENDING,value);}
		}

		public bool FlagBendingOrtho
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_BENDING_ORTHO);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_BENDING_ORTHO,value);}
		}

		public bool FlagDamping
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_DAMPING);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_DAMPING,value);}
		}

		public bool FlagCollisionTwoWay
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_COLLISION_TWOWAY);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_COLLISION_TWOWAY,value);}
		}

		public bool FlagTriangleCollision
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_TRIANGLE_COLLISION);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_TRIANGLE_COLLISION,value);}
		}

		public bool FlagTearable
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_TEARABLE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_TEARABLE,value);}
		}

		public bool FlagHardware
		{
			get{return NovodexUtil.areBitsSet(flags,(uint)NxClothFlag.NX_CLF_HARDWARE);}
			set{flags=NovodexUtil.setBits(flags,(uint)NxClothFlag.NX_CLF_HARDWARE,value);}
		}

	}
}



