//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;




namespace NovodexWrapper
{
	public class NxExportedUtils
	{
		static float[] boxPlaneBuffer=new float[6*4];	//6 planes with 4 floats each



		static public bool boxContainsPoint(NxBox box,Vector3 p)
			{return NxBoxContainsPoint(box,ref p)!=0;}

		static public void createBox(NxBox box,NxBounds3 aabb,NxMat34 mat)
			{NxCreateBox(box,aabb,ref mat);}

		static public bool computeBoxPlanes(NxBox box,NxPlane[] planeArray)
		{
			byte result=NxComputeBoxPlanes(box,boxPlaneBuffer);
			for(int i=0;i<6;i++)
			{
				planeArray[i].normal.X=boxPlaneBuffer[(i*4)+0];
				planeArray[i].normal.Y=boxPlaneBuffer[(i*4)+1];
				planeArray[i].normal.Z=boxPlaneBuffer[(i*4)+2];
				planeArray[i].d=boxPlaneBuffer[(i*4)+3];
			}
			return result!=0;
		}
		
		static public bool computeBoxPoints(NxBox box,Vector3[] boxPoints)
			{return NxComputeBoxPoints(box,ref boxPoints[0])!=0;}

		static public bool computeBoxVertexNormals(NxBox box,Vector3[] boxVertexNormals)
			{return NxComputeBoxVertexNormals(box,ref boxVertexNormals[0])!=0;}

		static public void computeBoxAroundCapsule(NxCapsule capsule,NxBox box)
			{NxComputeBoxAroundCapsule(capsule,box);}

		static public float computeDistanceSquared(NxRay ray,Vector3 point,ref float t)
			{return NxComputeDistanceSquared(ray,ref point,ref t);}

		static public float computeSquareDistance(NxSegment segment,Vector3 point,ref float t)
			{return NxComputeSquareDistance(segment,ref point,ref t);}

		static public NxBSphereMethod computeSphere(NxSphere sphere,Vector3[] verts)
			{return NxComputeSphere(sphere,(uint)verts.Length,ref verts[0]);}

		static public bool fastComputeSphere(NxSphere sphere,Vector3[] verts)
			{return NxFastComputeSphere(sphere,(uint)verts.Length,ref verts[0])!=0;}

		static public void mergeSpheres(NxSphere mergedSphere,NxSphere sphere0,NxSphere sphere1)
			{NxMergeSpheres(mergedSphere,sphere0,sphere1);}
			
		static public void normalToTangents(ref Vector3 n,ref Vector3 t1,ref Vector3 t2)
			{NxNormalToTangents(ref n,ref t1,ref t2);}

		static public void diagonalizeInertiaTensor(ref NxMat33 denseInertia,ref Vector3 diagonalInertia,ref NxMat33 rotation)
			{NxDiagonalizeInertiaTensor(ref denseInertia,ref diagonalInertia,ref rotation);}

		static public void findRotationMatrix(ref Vector3 x,ref Vector3 b,ref NxMat33 M)
			{NxFindRotationMatrix(ref x,ref b,ref M);}

		static public void computeBounds(ref Vector3 min,ref Vector3 max,Vector3[] verts)
			{NxComputeBounds(ref min,ref max,(uint)verts.Length,ref verts[0]);}

		static public uint crc32(IntPtr buffer,uint numBytes)
			{return NxCrc32(buffer,numBytes);}
			
		static public void computeCapsuleAroundBox(NxBox box,NxCapsule capsule)
			{NxComputeCapsuleAroundBox(box,capsule);}
		
		static public bool isBoxAInsideBoxB(NxBox a,NxBox b)
			{return NxIsBoxAInsideBoxB(a,b)!=0;}



		[DllImport("NxPhysics.dll")]
		private static extern byte NxBoxContainsPoint(NxBox box,ref Vector3 p);

		[DllImport("NxPhysics.dll")]
		private static extern void NxCreateBox(NxBox box,NxBounds3 aabb,ref NxMat34 mat);

		[DllImport("NxPhysics.dll")]
		private static extern byte NxComputeBoxPlanes(NxBox box,float[] boxPlaneBuffer);

		[DllImport("NxPhysics.dll")]
		private static extern byte NxComputeBoxPoints(NxBox box,ref Vector3 boxPoints);

		[DllImport("NxPhysics.dll")]
		private static extern byte NxComputeBoxVertexNormals(NxBox box,ref Vector3 boxVertexNormals);

		[DllImport("NxPhysics.dll")]
		private static extern void NxComputeBoxAroundCapsule(NxCapsule capsule,NxBox box);

		[DllImport("NxPhysics.dll")]
		private static extern float NxComputeDistanceSquared(NxRay ray,ref Vector3 point,ref float t);

		[DllImport("NxPhysics.dll")]
		private static extern float NxComputeSquareDistance(NxSegment segment,ref Vector3 point,ref float t);

		[DllImport("NxPhysics.dll")]
		private static extern NxBSphereMethod NxComputeSphere(NxSphere sphere,uint numVerts,ref Vector3 verts);
		
		[DllImport("NxPhysics.dll")]
		private static extern byte NxFastComputeSphere(NxSphere sphere,uint numVerts,ref Vector3 verts);

		[DllImport("NxPhysics.dll")]
		private static extern void NxMergeSpheres(NxSphere mergedSphere,NxSphere sphere0,NxSphere sphere1);

		[DllImport("NxPhysics.dll")]
		private static extern void NxNormalToTangents(ref Vector3 n,ref Vector3 t1,ref Vector3 t2);

		[DllImport("NxPhysics.dll")]
		private static extern void NxDiagonalizeInertiaTensor(ref NxMat33 denseInertia,ref Vector3 diagonalInertia,ref NxMat33 rotation);

		[DllImport("NxPhysics.dll")]
		private static extern void NxFindRotationMatrix(ref Vector3 x,ref Vector3 b,ref NxMat33 M);

		[DllImport("NxPhysics.dll")]
		private static extern void NxComputeBounds(ref Vector3 min,ref Vector3 max,uint numVerts,ref Vector3 verts);

		[DllImport("NxPhysics.dll")]
		private static extern uint NxCrc32(IntPtr buffer,uint numBytes);

		[DllImport("NxPhysics.dll")]
		private static extern void NxComputeCapsuleAroundBox(NxBox box,NxCapsule capsule);

		[DllImport("NxPhysics.dll")]
		private static extern byte NxIsBoxAInsideBoxB(NxBox a,NxBox b);
	}
}



/*
X	bool NxBoxContainsPoint(NxBox& box, NxVec3& p);
X	void NxCreateBox(NxBox& box, NxBounds3& aabb, NxMat34& mat);
X	bool NxComputeBoxPlanes(NxBox& box, NxPlane* planes);
X	bool NxComputeBoxPoints(NxBox& box, NxVec3* pts);
X	bool NxComputeBoxVertexNormals(NxBox& box, NxVec3* pts);
	NxU32* NxGetBoxEdges();
	NxI32* NxGetBoxEdgesAxes();
	NxU32* NxGetBoxTriangles();
	NxVec3* NxGetBoxLocalEdgeNormals();
	void NxComputeBoxWorldEdgeNormal(NxBox& box, NxU32 edge_index, NxVec3& world_normal);
-	void NxComputeCapsuleAroundBox(NxBox& box, NxCapsule& capsule);
-	bool NxIsBoxAInsideBoxB(NxBox& a, NxBox& b);
	NxU32* NxGetBoxQuads();
	NxU32* NxBoxVertexToQuad(NxU32 vertexIndex);
-	void NxComputeBoxAroundCapsule(NxCapsule& capsule, NxBox& box);
	void NxSetFPUPrecision24();
	void NxSetFPUPrecision53();
	void NxSetFPUPrecision64();
	void NxSetFPURoundingChop();
	void NxSetFPURoundingUp();
	void NxSetFPURoundingDown();
	void NxSetFPURoundingNear();
	void NxSetFPUExceptions(bool);
	int NxIntChop(NxF32& f);
	int NxIntFloor(NxF32& f);
	int NxIntCeil(NxF32& f);
-	NxF32 NxComputeDistanceSquared(NxRay& ray, NxVec3& point, NxF32* t);
-	NxF32 NxComputeSquareDistance(NxSegment& seg, NxVec3& point, NxF32* t);
-	NxBSphereMethod NxComputeSphere(NxSphere& sphere, unsigned nb_verts, NxVec3* verts);
-	bool NxFastComputeSphere(NxSphere& sphere, unsigned nb_verts, NxVec3* verts);
-	void NxMergeSpheres(NxSphere& merged, NxSphere& sphere0, NxSphere& sphere1);
-	void NxNormalToTangents(NxVec3 & n, NxVec3 & t1, NxVec3 & t2);
-	bool NxDiagonalizeInertiaTensor(NxMat33 & denseInertia, NxVec3 & diagonalInertia, NxMat33 & rotation);
-	void NxFindRotationMatrix(NxVec3 & x, NxVec3 & b, NxMat33 & M);
-	void NxComputeBounds(NxVec3& min, NxVec3& max, NxU32 nbVerts, NxVec3* verts);
-	NX_INLINE void NxComputeBounds(NxBounds3& bounds, NxU32 nbVerts, NxVec3* verts)
-	NxU32 NxCrc32(void* buffer, NxU32 nbBytes);
*/
