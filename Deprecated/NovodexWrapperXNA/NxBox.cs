//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxBox
	{
		public Vector3 center;
		public Vector3 extents;
		public NxMat33 rot;

		public NxBox()
		{
			center=new Vector3(0,0,0);
			extents=new Vector3(0,0,0);
			rot=NxMat33.Identity;
		}

		public NxBox(Vector3 center,Vector3 extents,NxMat33 rot)
		{
			this.center=center;
			this.extents=extents;
			this.rot=rot;
		}
		
		public void setEmpty()
		{
			center=new Vector3(0,0,0);
			extents=new Vector3(float.MinValue,float.MinValue,float.MinValue);
			rot=NxMat33.Identity;
		}

		public bool containsPoint(Vector3 p)
			{return NxExportedUtils.boxContainsPoint(this,p);}

		public void create(NxBounds3 aabb,NxMat34 mat)
			{NxExportedUtils.createBox(this,aabb,mat);}

		public bool isValid()
		{
			// Consistency condition for (Center, Extents) boxes: Extents >= 0.0f
			if(extents.X < 0.0f){return false;}
			if(extents.Y < 0.0f){return false;}
			if(extents.Z < 0.0f){return false;}
			return true;
		}

 		public bool computePlanes(NxPlane[] planeArray)
			{return NxExportedUtils.computeBoxPlanes(this,planeArray);}

		public bool computePoints(Vector3[] pointArray)
			{return NxExportedUtils.computeBoxPoints(this,pointArray);}

		public bool computeVertexNormals(Vector3[] pointArray)
			{return NxExportedUtils.computeBoxVertexNormals(this,pointArray);}


		public void computeCapsule(NxCapsule capsule)
			{NxExportedUtils.computeCapsuleAroundBox(this,capsule);}

		public bool isInside(NxBox box)
			{return NxExportedUtils.isBoxAInsideBoxB(this,box);}






		public NxPlane[] getPlanes()
		{
			NxPlane[] planeArray=new NxPlane[6];
			for(int i=0;i<planeArray.Length;i++)
			{planeArray[i]=new NxPlane();}
			computePlanes(planeArray);
			return planeArray;
		}

		public Vector3[] getPoints()
		{
			Vector3[] pointArray=new Vector3[8];
			for(int i=0;i<pointArray.Length;i++)
				{pointArray[i]=new Vector3();}
			computePoints(pointArray);
			return pointArray;
		}

		public Vector3[] getVertexNormals()
		{
			Vector3[] vertexNormalsArray=new Vector3[8];
			for(int i=0;i<vertexNormalsArray.Length;i++)
				{vertexNormalsArray[i]=new Vector3();}
			computeVertexNormals(vertexNormalsArray);
			return vertexNormalsArray;
		}
	}
}


#region Incomplete Code

/*
		public void rotate(NxMat34 mat,ref NxBox obb)
		{
			obb.extents = extents;
			mat.M.multiplyByTranspose(center, obb.center);
			obb.center += mat.t;
			obb.rot.multiply(rot, mat.M);	// ### check order
		}

		public const NxU32* getEdges() const
			{return NxGetBoxEdges();}

		public const NxI32* getEdgesAxes() const
			{return NxGetBoxEdgesAxes();}

		public const NxU32* getTriangles() const
			{return NxGetBoxTriangles();}
			
		public const NxVec3* getLocalEdgeNormals() const
			{return NxGetBoxLocalEdgeNormals();}

		public void computeWorldEdgeNormal(NxU32 edge_index, NxVec3& world_normal) const
			{NxComputeBoxWorldEdgeNormal(*this, edge_index, world_normal);}
*/

#endregion

