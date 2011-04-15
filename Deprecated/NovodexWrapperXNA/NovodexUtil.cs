//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Xna.Framework;




namespace NovodexWrapper
{
	public class NovodexUtil
	{
		static public readonly float DEG_TO_RAD=(float)(Math.PI/180);
		static public readonly float RAD_TO_DEG=(float)(180/Math.PI);
		static public readonly float PI=(float)(Math.PI);
		static public readonly float HALF_PI=(float)(Math.PI/2);
		
		static public readonly float SIN_0_DEG=0.0f;
		static public readonly float SIN_15_DEG=0.258819045f;
		static public readonly float SIN_30_DEG=0.5f;
		static public readonly float SIN_45_DEG=0.707106781f;
		static public readonly float SIN_60_DEG=0.866025403f;
		static public readonly float SIN_75_DEG=0.965925826f;
		static public readonly float SIN_90_DEG=1.0f;
		
		
		static private Random random=new Random(12345);
		


		static public void seedRandom(int seed)
			{random=new Random(seed);}

		static public float randomFloat(float min,float max)
		{
			float r=(float)random.NextDouble();
			return (min+(r*(max-min)));
		}

		//Between 0.0f and 1.0f
		static public float randomFloat()
			{return (float)random.NextDouble();}

		static public bool randomBool()
			{return (random.NextDouble()>0.5f);}

		static public int randomInt(int min,int max)
			{return random.Next(min,max);}

		static public int randomInt()
			{return random.Next();}

		static public int clampInt(int num,int min,int max)
		{
			if(num<min){num=min;}
			if(num>max){num=max;}			
			return num;
		}

		static public float clampFloat(float num,float min,float max)
		{
			if(num<min){num=min;}
			if(num>max){num=max;}			
			return num;
		}




		static public NxMat34 convertMatrixToNxMat34(Matrix matrix)
		{
			NxMat34 mat=NxMat34.Identity;

			mat.M.M11=matrix.M11;	mat.M.M12=matrix.M12;	mat.M.M13=matrix.M13;
			mat.M.M21=matrix.M21;	mat.M.M22=matrix.M22;	mat.M.M23=matrix.M23;
			mat.M.M31=matrix.M31;	mat.M.M32=matrix.M32;	mat.M.M33=matrix.M33;
			mat.t.X=matrix.M41;		mat.t.Y=matrix.M42;		mat.t.Z=matrix.M43;

			return mat;
		}

		static public Matrix convertNxMat34ToMatrix(NxMat34 mat)
		{
			Matrix matrix=Matrix.Identity;

			matrix.M11=mat.M.M11;	matrix.M12=mat.M.M12;	matrix.M13=mat.M.M13;
			matrix.M21=mat.M.M21;	matrix.M22=mat.M.M22;	matrix.M23=mat.M.M23;
			matrix.M31=mat.M.M31;	matrix.M32=mat.M.M32;	matrix.M33=mat.M.M33;
			matrix.M41=mat.t.X;		matrix.M42=mat.t.Y;		matrix.M43=mat.t.Z;

			return matrix;
		}

		//Matrix's position will be lost during the conversion.
		static public NxMat33 convertMatrixToNxMat33(Matrix matrix)
		{
			NxMat33 mat=NxMat33.Identity;

			mat.M11=matrix.M11;	mat.M12=matrix.M12;	mat.M13=matrix.M13;
			mat.M21=matrix.M21;	mat.M22=matrix.M22;	mat.M23=matrix.M23;
			mat.M31=matrix.M31;	mat.M32=matrix.M32;	mat.M33=matrix.M33;

			return mat;
		}

		static public Matrix convertNxMat33ToMatrix(NxMat33 mat)
		{
			Matrix matrix=Matrix.Identity;

			matrix.M11=mat.M11;	matrix.M12=mat.M12;	matrix.M13=mat.M13;
			matrix.M21=mat.M21;	matrix.M22=mat.M22;	matrix.M23=mat.M23;
			matrix.M31=mat.M31;	matrix.M32=mat.M32;	matrix.M33=mat.M33;

			return matrix;
		}

		static public Vector3 convertNxVec3ToVector3(NxVec3 v)
			{return new Vector3(v.x,v.y,v.z);}

		static public NxVec3 convertVector3ToNxVec3(Vector3 v)
			{return new NxVec3(v.X,v.Y,v.Z);}

		static public Vector3 transformWorldPointIntoLocalSpace(Vector3 worldPoint,ref Matrix matrix)
		{
			Matrix tm=matrix;
			tm = Matrix.Invert(tm);
			return Vector3.Transform(worldPoint,tm);
		}

		static public Vector3 transformWorldNormalIntoLocalSpace(Vector3 worldNormal,ref Matrix matrix)
		{
			Matrix tm=matrix;
			tm = Matrix.Invert(tm);
			return Vector3.TransformNormal(worldNormal,tm);
		}

		static public Vector3 transformLocalPointIntoWorldSpace(Vector3 localPoint,ref Matrix matrix)
			{return Vector3.Transform(localPoint,matrix);}

		static public Vector3 transformLocalNormalIntoWorldSpace(Vector3 localNormal,ref Matrix matrix)
			{return Vector3.TransformNormal(localNormal,matrix);}
		
		static public Vector3 getMatrixPos(ref Matrix mat)
			{return new Vector3(mat.M41,mat.M42,mat.M43);}

		static public Vector3 getMatrixXaxis(ref Matrix mat)
			{return new Vector3(mat.M11,mat.M12,mat.M13);}
		
		static public Vector3 getMatrixYaxis(ref Matrix mat)
			{return new Vector3(mat.M21,mat.M22,mat.M23);}
		
		static public Vector3 getMatrixZaxis(ref Matrix mat)
			{return new Vector3(mat.M31,mat.M32,mat.M33);}

		static public void setMatrixPos(ref Matrix mat,Vector3 pos)
		{
			mat.M41=pos.X;
			mat.M42=pos.Y;
			mat.M43=pos.Z;
		}

		static public void setMatrixAxis(ref Matrix matrix,MatrixAxis axis,Vector3 v)
		{
			if(axis==MatrixAxis.X)
				{matrix.M11=v.X; matrix.M12=v.Y; matrix.M13=v.Z;}
			else if(axis==MatrixAxis.Y)
				{matrix.M21=v.X; matrix.M22=v.Y; matrix.M23=v.Z;}
			else if(axis==MatrixAxis.Z)
				{matrix.M31=v.X; matrix.M32=v.Y; matrix.M33=v.Z;}
			else if(axis==MatrixAxis.Pos)
				{matrix.M41=v.X; matrix.M42=v.Y; matrix.M43=v.Z;}
			else
				{throw new IndexOutOfRangeException("Util.setMatrixAxis(matrix,"+axis+",v)");}
		}


		
		
		static public Matrix createMatrix(Vector3 xAxis,Vector3 yAxis,Vector3 zAxis,Vector3 pos)
		{
			Matrix matrix=Matrix.Identity;
			
			matrix.M11=xAxis.X;
			matrix.M12=xAxis.Y;
			matrix.M13=xAxis.Z;
			matrix.M21=yAxis.X;
			matrix.M22=yAxis.Y;
			matrix.M23=yAxis.Z;
			matrix.M31=zAxis.X;
			matrix.M32=zAxis.Y;
			matrix.M33=zAxis.Z;
			matrix.M41=pos.X;
			matrix.M42=pos.Y;
			matrix.M43=pos.Z;
			
			return matrix;		
		}

		static public Matrix createMatrixFromRay(Vector3 pos,Vector3 dir,MatrixAxis MatrixAxis)
			{return createMatrixFromRay(new NxRay(pos,dir),MatrixAxis);}

		static public Matrix createMatrixFromRay(NxRay ray,MatrixAxis MatrixAxis)
		{
			Matrix matrix=Matrix.Identity;
			Vector3 u,v,w;
			
			if(MatrixAxis==MatrixAxis.X)
			{
				if(Math.Abs(ray.dir.X)<0.99f)
					{u=new Vector3(1,0,0);}
				else
					{u=new Vector3(0,1,0);}
			}
			else if(MatrixAxis==MatrixAxis.Y)
			{
				if(Math.Abs(ray.dir.Y)<0.99f)
					{u=new Vector3(0,1,0);}
				else
					{u=new Vector3(1,0,0);}
			}
			else if(MatrixAxis==MatrixAxis.Z)
			{
				if(Math.Abs(ray.dir.Z)<0.99f)
					{u=new Vector3(0,0,1);}
				else
					{u=new Vector3(1,0,0);}
			}
			else
				{return matrix;}

			v=Vector3.Cross(u,ray.dir);
			v.Normalize();
			w=Vector3.Cross(ray.dir,v);
			setMatrixAxis(ref matrix,MatrixAxis,ray.dir);
			setMatrixAxis(ref matrix,(MatrixAxis)(((int)MatrixAxis+1)%3),v);
			setMatrixAxis(ref matrix,(MatrixAxis)(((int)MatrixAxis+2)%3),w);
			setMatrixPos(ref matrix,ray.orig);

			return matrix;
		}
		
		static public Vector3 getPerpendicularVector(Vector3 v)
		{
			Vector3 vv=new Vector3(v.X*v.X,v.Y*v.Y,v.Z*v.Z);	//this is done instead of finding the absolute values

			Vector3 u=new Vector3(0,0,1);
			if(vv.X<=vv.Y && vv.X<=vv.Z)
				{u=new Vector3(1,0,0);}
			else if(vv.Y<=vv.X && vv.Y<=vv.Z)
				{u=new Vector3(0,1,0);}
				
			Vector3 perp=Vector3.Cross(v,u);
			perp.Normalize();
			return perp;
		}
		
		static public uint clearBits(uint flags,uint bits)
			{return flags&(~bits);}
			
		static public uint setBits(uint flags,uint bits,bool state)
		{
			if(state)
				{return flags|bits;}
			else
				{return flags&(~bits);}
		}
			
		static public bool areBitsSet(uint flags,uint bits)
			{return (flags&bits)==bits;}
			

		//Passing in the Identity Matrix will result in a ground plane
		static public NxActor createPlaneActor(NxScene scene,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxPlaneShapeDesc planeDesc=NxPlaneShapeDesc.Default;	//The default plane is the ground plane so no parameters need to be changed
			NxActorDesc planeActorDesc=new NxActorDesc(planeDesc,bodyDesc,density,globalPose);
			return scene.createActor(planeActorDesc);
		}
		
		//This takes true dimensions of the box, whereas Novodex uses half dimensions
		static public NxActor createBoxActor(NxScene scene,Vector3 size,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxBoxShapeDesc boxDesc=new NxBoxShapeDesc(size.X/2,size.Y/2,size.Z/2);	//Novodex uses "radii" for boxes
			NxActorDesc boxActorDesc=new NxActorDesc(boxDesc,bodyDesc,density,globalPose);
			return scene.createActor(boxActorDesc);
		}
		
		static public NxActor createSphereActor(NxScene scene,float radius,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxSphereShapeDesc sphereDesc=new NxSphereShapeDesc(radius);
			NxActorDesc sphereActorDesc=new NxActorDesc(sphereDesc,bodyDesc,density,globalPose);
			return scene.createActor(sphereActorDesc);
		}

		static public NxActor createCapsuleActor(NxScene scene,float radius,float totalHeight,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxCapsuleShapeDesc capsuleDesc=new NxCapsuleShapeDesc(radius,totalHeight-(radius*2),false);
			NxActorDesc capsuleActorDesc=new NxActorDesc(capsuleDesc,bodyDesc,density,globalPose);
			return scene.createActor(capsuleActorDesc);
		}

		static public NxActor createCapsuleActor(NxScene scene,float radius,Vector3 start,Vector3 end,float density)
		{
			Vector3 pos=(start+end)*0.5f;
			Vector3 dir=start-end;
			float totalHeight=dir.Length()+radius*2;
			dir.Normalize();
			
			return createCapsuleActor(scene,radius,totalHeight,createMatrixFromRay(pos,dir,MatrixAxis.Y),density);
		}

		static public NxActor createSweptCapsuleActor(NxScene scene,float radius,float totalHeight,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxCapsuleShapeDesc capsuleDesc=new NxCapsuleShapeDesc(radius,totalHeight-(radius*2),true);
			NxActorDesc capsuleActorDesc=new NxActorDesc(capsuleDesc,bodyDesc,density,globalPose);
			return scene.createActor(capsuleActorDesc);
		}

		static public NxActor createSweptCapsuleActor(NxScene scene,float radius,Vector3 start,Vector3 end,float density)
		{
			Vector3 pos=(start+end)*0.5f;
			Vector3 dir=start-end;
			float totalHeight=((Vector3)(dir)).Length()+radius*2;
			dir.Normalize();
			
			return createSweptCapsuleActor(scene,radius,totalHeight,createMatrixFromRay(pos,dir,MatrixAxis.Y),density);
		}

		static public NxActor createTriangleMeshActor(NxScene scene,NxTriangleMesh triangleMesh,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxTriangleMeshShapeDesc meshShapeDesc=NxTriangleMeshShapeDesc.Default;
			meshShapeDesc.MeshData=triangleMesh;
			NxActorDesc meshActorDesc=new NxActorDesc(meshShapeDesc,bodyDesc,density,globalPose);
			return scene.createActor(meshActorDesc);
		}

		static public NxActor createConvexActor(NxScene scene,NxConvexMesh convexMesh,Matrix globalPose,float density)
		{
			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxConvexShapeDesc convexShapeDesc=NxConvexShapeDesc.Default;
			convexShapeDesc.MeshData=convexMesh;
			NxActorDesc convexActorDesc=new NxActorDesc(convexShapeDesc,bodyDesc,density,globalPose);
			return scene.createActor(convexActorDesc);
		}

		static public NxActor createCylinderActor(NxScene scene,float radius,float height,int numSides,bool useCapsuleIfPossible,Matrix globalPose,float density)
		{
			if(numSides<3)
				{numSides=3;}

			Vector3[] pointArray=new Vector3[numSides*2];
			for(int i=0;i<numSides;i++)
			{
				pointArray[i].X=((float)Math.Sin((i*Math.PI*2)/numSides))*radius;
				pointArray[i].Y=height/2;
				pointArray[i].Z=((float)Math.Cos((i*Math.PI*2)/numSides))*radius;

				pointArray[i+numSides]=pointArray[i];
				pointArray[i].Y=-height/2;
			}
			
			NxConvexMeshDesc convexMeshDesc=NxConvexMeshDesc.Default;			
			convexMeshDesc.setPoints(pointArray,true);
			convexMeshDesc.FlagComputeConvex=true;

			NxConvexShapeDesc convexShapeDesc=NxConvexShapeDesc.Default;
			convexShapeDesc.MeshData=NovodexUtil.createConvexMesh(convexMeshDesc);

			if(convexShapeDesc.MeshData==null)
				{return null;}

			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxActorDesc cylinderActorDesc=new NxActorDesc(convexShapeDesc,bodyDesc,density,globalPose);
			if(useCapsuleIfPossible && height>radius*2)
				{cylinderActorDesc.addShapeDesc(new NxCapsuleShapeDesc(radius,height-(radius*2),false));}
			return scene.createActor(cylinderActorDesc);
		}

		static public NxActor createCylinderActor(NxScene scene,float radius,int numSides,bool useCapsuleIfPossible,Vector3 start,Vector3 end,float density)
		{
			Vector3 pos=(start+end)*0.5f;
			Vector3 dir=start-end;
			float height=((Vector3)(dir)).Length()+radius*2;
			dir.Normalize();

			return createCylinderActor(scene,radius,height,numSides,useCapsuleIfPossible,createMatrixFromRay(pos,dir,MatrixAxis.Y),density);
		}






		static public NxConvexMesh createConvexMesh(NxConvexMeshDesc convexMeshDesc)
		{
			NxConvexMesh convexMesh=null;
			NxCooking.InitCooking();
			NovodexMemoryStream memStream=new NovodexMemoryStream();
			bool result=NxCooking.CookConvexMesh(convexMeshDesc,memStream);
			if(NxCooking.CookConvexMesh(convexMeshDesc,memStream))
				{convexMesh=NxPhysicsSDK.StaticSDK.createConvexMesh(memStream);}
			NxCooking.CloseCooking();
			return convexMesh;
		}

		static public NxTriangleMesh createTriangleMesh(NxTriangleMeshDesc triangleMeshDesc)
		{
			NxTriangleMesh triangleMesh=null;
			NxCooking.InitCooking();
			NovodexMemoryStream memStream=new NovodexMemoryStream();
			if(NxCooking.CookTriangleMesh(triangleMeshDesc,memStream))
				{triangleMesh=NxPhysicsSDK.StaticSDK.createTriangleMesh(memStream);}
			NxCooking.CloseCooking();
			return triangleMesh;
		}

		static public NxClothMesh createClothMesh(NxClothMeshDesc clothMeshDesc)
		{
			NxClothMesh clothMesh=null;
			NxCooking.InitCooking();
			NovodexMemoryStream memStream=new NovodexMemoryStream();
			if(NxCooking.CookClothMesh(clothMeshDesc,memStream))
				{clothMesh=NxPhysicsSDK.StaticSDK.createClothMesh(memStream);}
			NxCooking.CloseCooking();
			return clothMesh;
		}





		static public NxCloth createClothGrid(NxScene scene,float width,float height,int numXsubdivisions,int numYsubdivisions,float extraVerticesScalar,float density,Matrix globalPose)
		{
			//This is used to allow extra vertices so the cloth can tear.
			extraVerticesScalar=NovodexUtil.clampFloat(extraVerticesScalar,1,6); //6 is the absolute worst case of the cloth being broken into individual triangles

			int numX=numXsubdivisions;
			int numY=numYsubdivisions;			
			int numVerts=(numX+1)*(numY+1);
			int numTriangles=numX*numY*2;

			NxClothMeshDesc clothMeshDesc=NxClothMeshDesc.Default;
			Vector3[] pointArray=new Vector3[numVerts];
			int[] tI=new int[numTriangles*3];
			for(int y=0;y<=numY;y++)
			{			  
				for(int x=0;x<=numX;x++)
					{pointArray[(y*(numX+1))+x]=new Vector3(-(width/2)+(width*x)/numX,0,-(height/2)+(height*y)/numY);}
			}

			int triIndex=0;
			for(int y=0;y<numY;y++)
			{			  
				for(int x=0;x<numX;x++)
				{
					int i0=(y*(numX+1)) + x;
					int i1=i0+1;
					int i2=((y+1)*(numX+1)) + x;
					int i3=i2+1;

					//This makes a diamond grid pattern which folds better than a regular grid pattern
					if((x+y)%2==1)
					{
						tI[triIndex++]=i0; tI[triIndex++]=i2; tI[triIndex++]=i1;
						tI[triIndex++]=i1; tI[triIndex++]=i2; tI[triIndex++]=i3;
					}
					else
					{
						tI[triIndex++]=i0; tI[triIndex++]=i2; tI[triIndex++]=i3;
						tI[triIndex++]=i0; tI[triIndex++]=i3; tI[triIndex++]=i1;
					}
				}
			}
			clothMeshDesc.setPoints(pointArray,true);
			clothMeshDesc.setTriangleIndices(tI,true);

			NxClothDesc clothDesc=NxClothDesc.Default;
			clothDesc.density=density;
			clothDesc.globalPose=NovodexUtil.convertMatrixToNxMat34(globalPose);
			clothDesc.ClothMesh=NovodexUtil.createClothMesh(clothMeshDesc);
			if(clothDesc.ClothMesh==null)
				{return null;}

			NxCloth cloth=scene.createCloth(clothDesc);
			if(cloth!=null)
			{
				//I wait until the cloth is successfully created before allocating the meshData.
				//The meshData isn't managed memory but it is freed when you release the cloth.
				int maxNumVerts=(int)(numVerts*extraVerticesScalar);
				NxMeshData clothMeshData=NxMeshData.Default;
				clothMeshData.allocateData(maxNumVerts,numTriangles*3,maxNumVerts);
				cloth.setMeshData(clothMeshData);
			}
			return cloth;
		}




		static public NxActor createConvexActorFromVertexCloud(NxScene scene,Vector3[] localSpaceVertArray,Matrix globalPose,float density)
		{
			NxConvexShapeDesc convexShapeDesc=createConvexShapeDescFromVertexCloud(localSpaceVertArray);
			if(convexShapeDesc==null)
				{return null;}

			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxActorDesc actorDesc=new NxActorDesc(convexShapeDesc,bodyDesc,density,globalPose);
			return scene.createActor(actorDesc);
		}

		//Only 32 and 64 are valid pmap densities. Any other value will skip creating a pmap.
		//Pass in null for materialIndiceArray to use the default shape material
		static public NxActor createTriangleMeshActor(NxScene scene,Vector3[] localSpaceVertArray,int[] triangleIndiceArray,ushort[] materialIndiceArray,uint pmapDensity,Matrix globalPose,float density)
		{
			NxTriangleMeshShapeDesc triangleMeshShapeDesc=createTriangleMeshShapeDesc(localSpaceVertArray,triangleIndiceArray,materialIndiceArray,pmapDensity);
			if(triangleMeshShapeDesc==null)
				{return null;}

			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxActorDesc actorDesc=new NxActorDesc(triangleMeshShapeDesc,bodyDesc,density,globalPose);
			return scene.createActor(actorDesc);
		}

		static public NxConvexShapeDesc createConvexShapeDescFromVertexCloud(Vector3[] localSpaceVertArray)
		{
			NxConvexMeshDesc convexMeshDesc=NxConvexMeshDesc.Default;			
			convexMeshDesc.setPoints(localSpaceVertArray,true);
			convexMeshDesc.FlagComputeConvex=true;

			NxConvexShapeDesc convexShapeDesc=NxConvexShapeDesc.Default;
			convexShapeDesc.MeshData=NovodexUtil.createConvexMesh(convexMeshDesc);

			if(convexShapeDesc.MeshData==null)
				{return null;}
			return convexShapeDesc;
		}
		
		//Only 32 and 64 are valid pmap densities. Any other value will skip creating a pmap.
		//Pass in null for materialIndiceArray to use the default shape material
		static public NxTriangleMeshShapeDesc createTriangleMeshShapeDesc(Vector3[] localSpaceVertArray,int[] triangleIndiceArray,ushort[] materialIndiceArray,uint pmapDensity)
		{
			NxTriangleMeshDesc triangleMeshDesc=NxTriangleMeshDesc.Default;			
			triangleMeshDesc.setPoints(localSpaceVertArray,true);
			triangleMeshDesc.setTriangleIndices(triangleIndiceArray,true);
			triangleMeshDesc.setMaterialIndices(materialIndiceArray,true);

			NxTriangleMeshShapeDesc triangleMeshShapeDesc=NxTriangleMeshShapeDesc.Default;
			NxTriangleMesh triangleMesh=NovodexUtil.createTriangleMesh(triangleMeshDesc);
			triangleMeshShapeDesc.MeshData=triangleMesh;

			if(triangleMeshShapeDesc.MeshData==null)
				{return null;}

			if(pmapDensity==32 || pmapDensity==64)
			{
				NxPMap pmap=NxPMap.Default;
				if(NxPMap.createPMap(pmap,triangleMesh,pmapDensity))
				{
					triangleMesh.loadPMap(pmap);
					NxPMap.releasePMap(pmap);
				}
			}

			return triangleMeshShapeDesc;
		}












		static public NxActor createTriangleHeightFieldActorFromGridHeights(NxScene scene,float[] localSpaceHeightArray,int numXverts,int numZverts,float gridWidth,float gridDepth,float verticalExtent,ushort materialIndex,Matrix globalPose,float density,bool smoothSphereCollisionsFlag)
		{
			NxTriangleMeshShapeDesc triangleMeshShapeDesc=createTriangleHeightFieldMeshShapeDescFromGridHeights(localSpaceHeightArray,numXverts,numZverts,gridWidth,gridDepth,verticalExtent,materialIndex,smoothSphereCollisionsFlag);
			if(triangleMeshShapeDesc==null)
				{return null;}

			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxActorDesc actorDesc=new NxActorDesc(triangleMeshShapeDesc,bodyDesc,density,globalPose);
			return scene.createActor(actorDesc);
		}

		static public NxActor createTriangleHeightFieldActorFromMesh(NxScene scene,Vector3[] localSpaceVertArray,int[] triangleIndiceArray,ushort[] materialIndiceArray,float verticalExtent,Matrix globalPose,float density,bool smoothSphereCollisionsFlag)
		{
			NxTriangleMeshShapeDesc triangleMeshShapeDesc=createTriangleHeightFieldMeshShapeDescFromMesh(localSpaceVertArray,triangleIndiceArray,materialIndiceArray,verticalExtent,smoothSphereCollisionsFlag);
			if(triangleMeshShapeDesc==null)
				{return null;}

			NxBodyDesc bodyDesc=NxBodyDesc.Default;
			if(density<=0){bodyDesc=null; density=1;}	//Make actor static if density isn't positive
			NxActorDesc actorDesc=new NxActorDesc(triangleMeshShapeDesc,bodyDesc,density,globalPose);
			return scene.createActor(actorDesc);
		}

		static public NxTriangleMeshShapeDesc createTriangleHeightFieldMeshShapeDescFromGridHeights(float[] localSpaceHeightArray,int numXverts,int numZverts,float gridWidth,float gridDepth,float verticalExtent,ushort materialIndex,bool smoothSphereCollisionsFlag)
		{
			if(numXverts<=1 || numZverts<=1 || (numXverts*numZverts) != localSpaceHeightArray.Length)
				{return null;}
			
			int numTris=(numXverts-1)*(numZverts-1)*2;

			int[] triangleIndiceArray=new int[numTris*3];
			ushort[] materialIndiceArray=new ushort[numTris];
			Vector3[] localSpaceVertArray=new Vector3[localSpaceHeightArray.Length];

			for(int z=0;z<numZverts;z++)
			{
				float zPos=((gridDepth*z)/numZverts)-(gridDepth/2);
				for(int x=0;x<numXverts;x++)
				{
					int index=(x+(z*numXverts));
					float xPos=((gridWidth*x)/numXverts)-(gridWidth/2);
					float yPos=localSpaceHeightArray[index];
					localSpaceVertArray[index]=new Vector3(xPos,yPos,zPos);
				}
			}

			int triIndex=0;
			for(int z=0;z<numZverts-1;z++)
			{
				for(int x=0;x<numXverts-1;x++)
				{
					materialIndiceArray[triIndex]=materialIndex;
					triangleIndiceArray[(triIndex*3)+0]=(x+0)+((z+0)*numXverts);
					triangleIndiceArray[(triIndex*3)+2]=(x+1)+((z+0)*numXverts);
					triangleIndiceArray[(triIndex*3)+1]=(x+0)+((z+1)*numXverts);
					triIndex++;

					materialIndiceArray[triIndex]=materialIndex;
					triangleIndiceArray[(triIndex*3)+0]=(x+1)+((z+0)*numXverts);
					triangleIndiceArray[(triIndex*3)+2]=(x+1)+((z+1)*numXverts);
					triangleIndiceArray[(triIndex*3)+1]=(x+0)+((z+1)*numXverts);
					triIndex++;
				}
			}

			return createTriangleHeightFieldMeshShapeDescFromMesh(localSpaceVertArray,triangleIndiceArray,materialIndiceArray,verticalExtent,smoothSphereCollisionsFlag);
		}
		
		static public NxTriangleMeshShapeDesc createTriangleHeightFieldMeshShapeDescFromMesh(Vector3[] localSpaceVertArray,int[] triangleIndiceArray,ushort[] materialIndiceArray,float verticalExtent,bool smoothSphereCollisionsFlag)
		{
			NxTriangleMeshDesc triangleMeshDesc=NxTriangleMeshDesc.Default;			
			triangleMeshDesc.setPoints(localSpaceVertArray,true);
			triangleMeshDesc.setTriangleIndices(triangleIndiceArray,true);
			triangleMeshDesc.setMaterialIndices(materialIndiceArray,true);
			triangleMeshDesc.heightFieldVerticalExtent=verticalExtent;
			triangleMeshDesc.heightFieldVerticalAxis=NxHeightFieldAxis.NX_Y;

			NxTriangleMeshShapeDesc triangleMeshShapeDesc=NxTriangleMeshShapeDesc.Default;
			NxTriangleMesh triangleMesh=NovodexUtil.createTriangleMesh(triangleMeshDesc);
			triangleMeshShapeDesc.MeshData=triangleMesh;
			triangleMeshShapeDesc.FlagMeshSmoothSphereCollisions=smoothSphereCollisionsFlag;

			if(triangleMeshShapeDesc.MeshData==null)
				{return null;}

			return triangleMeshShapeDesc;
		}


















		static public NxJoint createJoint(NxScene scene,NxJointType jointType,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor,Vector3 worldSpaceAxis,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.create(): Either scene, actor0, or actor1 is null.");}

			NxJointDesc jointDesc;
			
			if(jointType==NxJointType.NX_JOINT_CYLINDRICAL)
				{jointDesc=NxCylindricalJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_D6)
				{jointDesc=NxD6JointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_DISTANCE)
				{jointDesc=NxDistanceJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_FIXED)
				{jointDesc=NxFixedJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_POINT_IN_PLANE)
				{jointDesc=NxPointInPlaneJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_POINT_ON_LINE)
				{jointDesc=NxPointOnLineJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_PRISMATIC)
				{jointDesc=NxPrismaticJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_PULLEY)
				{jointDesc=NxPulleyJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_REVOLUTE)
				{jointDesc=NxRevoluteJointDesc.Default;}
			else if(jointType==NxJointType.NX_JOINT_SPHERICAL)
				{jointDesc=NxSphericalJointDesc.Default;}
			else
				{return null;}
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(worldSpaceAxis);
			jointDesc.FlagCollisionEnabled=collisionEnabled;

			return scene.createJoint(jointDesc);
		}

		static public NxPulleyJoint createPulleyJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor0,Vector3 worldSpaceAnchor1,Vector3 worldSpaceAxis0,Vector3 worldSpaceAxis1,Vector3 worldPulley0,Vector3 worldPulley1,float pulleyDistance,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createPulleyJoint(): Either scene, actor0, or actor1 is null.");}
			
			NxPulleyJointDesc jointDesc=NxPulleyJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(0,worldSpaceAnchor0);
			jointDesc.setGlobalAnchor(1,worldSpaceAnchor1);
			jointDesc.setGlobalAxis(0,worldSpaceAxis0);
			jointDesc.setGlobalAxis(1,worldSpaceAxis1);
			jointDesc.pulley_0=worldPulley0;
			jointDesc.pulley_1=worldPulley1;
			jointDesc.distance=pulleyDistance;
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxPulleyJoint)scene.createJoint(jointDesc);
		}

		static public NxFixedJoint createFixedJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createFixedJoint(): Either scene, actor0, or actor1 is null.");}

			NxFixedJointDesc jointDesc=NxFixedJointDesc.Default;
		
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(new Vector3(0,0,1));
			
			return (NxFixedJoint)scene.createJoint(jointDesc);
		}

		//pass in negative values for minDistance or maxDistance to disable using them.
		static public NxDistanceJoint createDistanceJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor0,Vector3 worldSpaceAnchor1,float minDistance,float maxDistance,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createDistanceJoint(): Either scene, actor0, or actor1 is null.");}

			NxDistanceJointDesc jointDesc=NxDistanceJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(0,worldSpaceAnchor0);
			jointDesc.setGlobalAnchor(1,worldSpaceAnchor1);
			jointDesc.setGlobalAxis(new Vector3(0,0,1));
			jointDesc.minDistance=minDistance;
			jointDesc.maxDistance=maxDistance;
			jointDesc.FlagMinDistanceEnabled=minDistance>=0;
			jointDesc.FlagMaxDistanceEnabled=maxDistance>=0;
			
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxDistanceJoint)scene.createJoint(jointDesc);
		}

		static public NxPrismaticJoint createPrismaticJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor,Vector3 worldSpaceAxis,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createPrismaticJoint(): Either scene, actor0, or actor1 is null.");}

			NxPrismaticJointDesc jointDesc=NxPrismaticJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(worldSpaceAxis);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxPrismaticJoint)scene.createJoint(jointDesc);
		}

		static public NxCylindricalJoint createCylindricalJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor,Vector3 worldSpaceAxis,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createCylindricalJoint(): Either scene, actor0, or actor1 is null.");}

			NxCylindricalJointDesc jointDesc=NxCylindricalJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(worldSpaceAxis);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxCylindricalJoint)scene.createJoint(jointDesc);
		}

		static public NxPointInPlaneJoint createPointInPlaneJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpacePoint,Vector3 worldSpacePlaneNormal,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createPointInPlaneJoint(): Either scene, actor0, or actor1 is null.");}

			NxPointInPlaneJointDesc jointDesc=NxPointInPlaneJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpacePoint);
			jointDesc.setGlobalAxis(worldSpacePlaneNormal);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxPointInPlaneJoint)scene.createJoint(jointDesc);
		}

		static public NxPointOnLineJoint createPointOnLineJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpacePoint,Vector3 worldSpaceLineDirection,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createPointOnLineJoint(): Either scene, actor0, or actor1 is null.");}

			NxPointOnLineJointDesc jointDesc=NxPointOnLineJointDesc.Default;

			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpacePoint);
			jointDesc.setGlobalAxis(worldSpaceLineDirection);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxPointOnLineJoint)scene.createJoint(jointDesc);
		}

		static public NxRevoluteJoint createRevoluteJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor,Vector3 worldSpaceAxis,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createRevoluteJoint(): Either scene, actor0, or actor1 is null.");}

			NxRevoluteJointDesc jointDesc=NxRevoluteJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(worldSpaceAxis);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxRevoluteJoint)scene.createJoint(jointDesc);
		}

		static public NxSphericalJoint createSphericalJoint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor,Vector3 worldSpaceAxis,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createSphericalJoint(): Either scene, actor0, or actor1 is null.");}
			
			NxSphericalJointDesc jointDesc=NxSphericalJointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(worldSpaceAxis);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxSphericalJoint)scene.createJoint(jointDesc);
		}

		static public NxD6Joint createD6Joint(NxScene scene,NxActor actor0,NxActor actor1,Vector3 worldSpaceAnchor,Vector3 worldSpaceAxis,bool collisionEnabled)
		{
			if(scene==null || actor0==null || actor1==null)
				{throw new NullReferenceException("NxJoint.createD6Joint(): Either scene, actor0, or actor1 is null.");}

			NxD6JointDesc jointDesc=NxD6JointDesc.Default;
			
			jointDesc.actor[0]=actor0;
			jointDesc.actor[1]=actor1;
			jointDesc.setGlobalAnchor(worldSpaceAnchor);
			jointDesc.setGlobalAxis(worldSpaceAxis);
			jointDesc.FlagCollisionEnabled=collisionEnabled;
			
			return (NxD6Joint)scene.createJoint(jointDesc);
		}








		static public void releaseAllMaterialsFromScene(NxScene scene)
		{
			NxMaterial[] materialArray=scene.getMaterialArray();
			for(int i=materialArray.Length-1;i>=0;i--)
				{scene.releaseMaterial(materialArray[i]);}
		}

		static public void releaseAllActorsFromScene(NxScene scene)
		{
			NxActor[] actorArray=scene.getActors();
			for(int i=actorArray.Length-1;i>=0;i--)
				{scene.releaseActor(actorArray[i]);}
		}
				
		static public void releaseAllJointsFromScene(NxScene scene)
		{
			NxJoint[] jointArray=scene.getJoints();
			for(int i=jointArray.Length-1;i>=0;i--)
				{scene.releaseJoint(jointArray[i]);}
		}
		
		static public void releaseAllClothsFromScene(NxScene scene)
		{
			NxCloth[] clothArray=scene.getCloths();
			for(int i=clothArray.Length-1;i>=0;i--)
				{scene.releaseCloth(clothArray[i]);}
		}




		static public NxTriangleMesh[] getAllTriangleMeshesAssociatedWithScene(NxScene scene)
		{
			ArrayList meshList=getAllMeshesAssociatedWithScene(scene,true,false,false);
			NxTriangleMesh[] meshArray=new NxTriangleMesh[meshList.Count];
			for(int i=0;i<meshList.Count;i++)
				{meshArray[i]=(NxTriangleMesh)meshList[i];}
			return meshArray;
		}

		static public NxConvexMesh[] getAllConvexMeshesAssociatedWithScene(NxScene scene)
		{
			ArrayList meshList=getAllMeshesAssociatedWithScene(scene,false,true,false);
			NxConvexMesh[] meshArray=new NxConvexMesh[meshList.Count];
			for(int i=0;i<meshList.Count;i++)
				{meshArray[i]=(NxConvexMesh)meshList[i];}
			return meshArray;
		}

		static public NxHeightField[] getAllHeightFieldsAssociatedWithScene(NxScene scene)
		{
			ArrayList meshList=getAllMeshesAssociatedWithScene(scene,false,false,true);
			NxHeightField[] heightFieldArray=new NxHeightField[meshList.Count];
			for(int i=0;i<meshList.Count;i++)
				{heightFieldArray[i]=(NxHeightField)meshList[i];}
			return heightFieldArray;
		}

		static public ArrayList getAllMeshesAssociatedWithScene(NxScene scene,bool getTriMeshesFlag,bool getConvexMeshesFlag,bool getHeightFieldsFlag)
		{
			Hashtable meshTable=new Hashtable();
			
			NxActor[] actorArray=scene.getActors();
			foreach(NxActor actor in actorArray)
			{
				NxShape[] shapeArray=actor.getShapes();
				foreach(NxShape shape in shapeArray)
				{
					if(getTriMeshesFlag && shape is NxTriangleMeshShape)
					{
						NxTriangleMesh triangleMesh=((NxTriangleMeshShape)shape).getTriangleMesh();
						if(meshTable[triangleMesh.NxTriangleMeshPtr]==null)
							{meshTable.Add(triangleMesh.NxTriangleMeshPtr,triangleMesh);}
					}
					if(getConvexMeshesFlag && shape is NxConvexShape)
					{
						NxConvexMesh convexMesh=((NxConvexShape)shape).getConvexMesh();
						if(meshTable[convexMesh.NxConvexMeshPtr]==null)
							{meshTable.Add(convexMesh.NxConvexMeshPtr,convexMesh);}
					}
					if(getHeightFieldsFlag && shape is NxHeightFieldShape)
					{
						NxHeightField heightField=((NxHeightFieldShape)shape).getHeightField();
						if(meshTable[heightField.NxHeightFieldPtr]==null)
							{meshTable.Add(heightField.NxHeightFieldPtr,heightField);}
					}
				}
			}

			ArrayList meshList=new ArrayList();

			IDictionaryEnumerator myEnumerator = meshTable.GetEnumerator();
			while(myEnumerator.MoveNext())
				{meshList.Add(myEnumerator.Value);}
			return meshList;
		}

		static public void releaseMeshes(NxPhysicsSDK physicsSDK,ArrayList meshList)
		{
			for(int i=0;i<meshList.Count;i++)
			{
				if(meshList[i] is NxTriangleMesh)
					{physicsSDK.releaseTriangleMesh((NxTriangleMesh)meshList[i]);}
				else if(meshList[i] is NxConvexMesh)
					{physicsSDK.releaseConvexMesh((NxConvexMesh)meshList[i]);}
				else if(meshList[i] is NxHeightField)
					{physicsSDK.releaseHeightField((NxHeightField)meshList[i]);}
			}
		 }

		static public ArrayList getAllCCDSkeletonsAssociatedWithScene(NxScene scene)
		{
			Hashtable skeletonTable=new Hashtable();
			
			NxActor[] actorArray=scene.getActors();
			foreach(NxActor actor in actorArray)
			{
				NxShape[] shapeArray=actor.getShapes();
				foreach(NxShape shape in shapeArray)
				{
					NxCCDSkeleton skeleton=shape.getCCDSkeleton();
					if(skeleton!=null)
					{
						if(skeletonTable[skeleton.NxCCDSkeletonPtr]==null)
							{skeletonTable.Add(skeleton.NxCCDSkeletonPtr,skeleton);}
					}
				}
			}

			ArrayList skeletonList=new ArrayList();

			IDictionaryEnumerator myEnumerator = skeletonTable.GetEnumerator();
			while(myEnumerator.MoveNext())
				{skeletonList.Add(myEnumerator.Value);}
			return skeletonList;
		}

		static public void releaseCCDSkeletons(NxPhysicsSDK physicsSDK,ArrayList skeletonList)
		{
			for(int i=0;i<skeletonList.Count;i++)
			{
				if(skeletonList[i] is NxCCDSkeleton)
					{physicsSDK.releaseCCDSkeleton((NxCCDSkeleton)skeletonList[i]);}
			}
		 }




		static public void printDebugString(string output)
			{wrapper_printDebugString(output);}



		static private long frequency=0;
		static public long QueryPerformanceFrequency()
		{
			if(frequency==0)
				{frequency=wrapper_QueryPerformanceFrequency();}
			return frequency;
		}

		static public long QueryPerformanceCounter()
			{return wrapper_QueryPerformanceCounter();}

		static public float getTimeInSeconds()
			{return (float)(((double)QueryPerformanceCounter())/((double)QueryPerformanceFrequency()));}




	
		static int[] keyStates=new int[256];
		
		static private int getKeyState(Keys key)
		{
			int keyCode=(int)key;

			if((GetAsyncKeyState(keyCode)&0x8000)!=0)	//key is down
				{keyStates[keyCode]++;}
			else
				{keyStates[keyCode]=0;}
			return keyStates[keyCode];
		}
		
		static public bool isKeyDown(Keys key)
			{return getKeyState(key)!=0;}

		static public bool isKeyPressed(Keys key)
			{return (getKeyState(key)==1);}





		//This hasn't been tested yet. I'm assuming just flipping the ray direction is fine
		static public NxRay getRayFromRightHandedPerspectiveViewport(int viewportX,int viewportY,float viewportWidth,float viewportHeight,float viewportVerticalFOVinDegrees,Matrix viewportMatrix)
		{
			NxRay ray=getRayFromLeftHandedPerspectiveViewport(viewportX,viewportY,viewportWidth,viewportHeight,viewportVerticalFOVinDegrees,viewportMatrix);
			ray.dir=-ray.dir;
			return ray;
		}

		static public NxRay getRayFromLeftHandedPerspectiveViewport(int viewportX,int viewportY,float viewportWidth,float viewportHeight,float viewportVerticalFOVinDegrees,Matrix viewportMatrix)
		{
			float vX=((float)viewportX)+0.5f;	//to place ray at center of pixel
			float vY=((float)viewportY)+0.5f;	//to place ray at center of pixel
			
			float hW=viewportWidth/2;
			float hH=viewportHeight/2;
			float aspectRatio=hW/hH;
			
			float HH=((float)Math.Tan(NovodexUtil.DEG_TO_RAD*viewportVerticalFOVinDegrees/2))*2;
			float WW=HH*aspectRatio;

			float x= (vX-hW)/hW;	//ranges from -1 to 1 (left=-1 right=1)
			float y=-(vY-hH)/hH;	//ranges from -1 to 1 (top=1 bottom=-1)
			float xDepthRatio=WW/2;
			float yDepthRatio=HH/2;

			Vector3 pos=NovodexUtil.getMatrixPos(ref viewportMatrix);
			Vector3 dir=new Vector3(x*xDepthRatio,y*yDepthRatio,1);
			dir.Normalize();
			dir=Vector3.TransformNormal(dir,viewportMatrix);

			return new NxRay(pos,dir);
		}


		public static string Credits
			{get{return "By Jason Zelsnack. All rights reserved.";}}


		//For objects that have a native pointer this will return that native pointer.
		public static IntPtr getNativePointerFromObject(Object obj)
		{
			if(obj is NxActor){return ((NxActor)obj).NxActorPtr;}
			if(obj is NxCCDSkeleton){return ((NxCCDSkeleton)obj).NxCCDSkeletonPtr;}			
			if(obj is NxCloth){return ((NxCloth)obj).NxClothPtr;}
			if(obj is NxClothMesh){return ((NxClothMesh)obj).NxClothMeshPtr;}
			if(obj is NxContactStreamIterator){return ((NxContactStreamIterator)obj).NxContactStreamIteratorPtr;}
			if(obj is NxController){return ((NxController)obj).NxControllerPtr;}
			if(obj is NxConvexMesh){return ((NxConvexMesh)obj).NxConvexMeshPtr;}
			if(obj is NxDebugRenderable){return ((NxDebugRenderable)obj).NxDebugRenderablePtr;}
			if(obj is NxEffector){return ((NxEffector)obj).NxEffectorPtr;}
			if(obj is NxFoundationSDK){return ((NxFoundationSDK)obj).NxFoundationSDKptr;}
			if(obj is NxFluid){return ((NxFluid)obj).NxFluidPtr;}
			if(obj is NxFluidEmitter){return ((NxFluidEmitter)obj).NxFluidEmitterPtr;}
			if(obj is NxHeightField){return ((NxHeightField)obj).NxHeightFieldPtr;}
			if(obj is NxInterface){return ((NxInterface)obj).NxInterfacePtr;}
			if(obj is NxJoint){return ((NxJoint)obj).NxJointPtr;}
			if(obj is NxMaterial){return ((NxMaterial)obj).NxMaterialPtr;}
			if(obj is NxPhysicsSDK){return ((NxPhysicsSDK)obj).NxPhysicsSDKptr;}
			if(obj is NxRemoteDebugger){return ((NxRemoteDebugger)obj).NxRemoteDebuggerPtr;}
			if(obj is NxScene){return ((NxScene)obj).NxScenePtr;}
			if(obj is NxShape){return ((NxShape)obj).NxShapePtr;}
			if(obj is NxStream){return ((NxStream)obj).NxStreamPtr;}
			if(obj is NxTriangleMesh){return ((NxTriangleMesh)obj).NxTriangleMeshPtr;}
			if(obj is NxUserControllerHitReport){return ((NxUserControllerHitReport)obj).NxUserControllerHitReportPtr;}
			if(obj is NxUserEntityReport){return ((NxUserEntityReport)obj).NxUserEntityReportPtr;}
			if(obj is NxUserRaycastReport){return ((NxUserRaycastReport)obj).NxUserRaycastReportPtr;}
			if(obj is NxUserScheduler){return ((NxUserScheduler)obj).NxUserSchedulerPtr;}
			return IntPtr.Zero;
		}




		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern long wrapper_QueryPerformanceFrequency();

		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern long wrapper_QueryPerformanceCounter();
		
		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_printDebugString(string output);

		[DllImport("User32.dll")]
		private static extern short GetAsyncKeyState(int vKey);
	}
}









