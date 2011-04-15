//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;



namespace NovodexWrapper
{
	[StructLayout(LayoutKind.Sequential)]
	public class NxBounds3
	{
		public Vector3 min;
		public Vector3 max;
		
		public NxBounds3()
			{setEmpty();}
		
		public NxBounds3(Vector3 min,Vector3 max)
			{set(min,max);}
		
		public NxBounds3(float minX,float minY,float minZ,float maxX,float maxY,float maxZ)
			{set(minX,minY,minZ,maxX,maxY,maxZ);}

		public NxBounds3(Vector3 center,float width,float height,float depth)
		{
			min=new Vector3(center.X-(width/2),center.Y-(height/2),center.Z-(depth/2));
			max=new Vector3(center.X+(width/2),center.Y+(height/2),center.Z+(depth/2));
		}
		
		public NxBounds3(NxBounds3 bounds)
		{
			min=bounds.min;
			max=bounds.max;
		}
		
		public void setEmpty()
			{set(float.MaxValue,float.MaxValue,float.MaxValue,float.MinValue,float.MinValue,float.MinValue);}
		
		public void setInfinite()
			{set(float.MinValue,float.MinValue,float.MinValue,float.MaxValue,float.MaxValue,float.MaxValue);}
		
		public void set(float minX,float minY,float minZ,float maxX,float maxY,float maxZ)
		{
			min=new Vector3(minX,minY,minZ);
			max=new Vector3(maxX,maxY,maxZ);
		}
		
		public void set(Vector3 min,Vector3 max)
		{
			this.min=min;
			this.max=max;
		}
		
		public void include(Vector3 v)
		{
			setMax(ref max,v);
			setMin(ref min,v);
		}

		public void combine(NxBounds3 bounds)
		{
			setMin(ref min,bounds.min);
			setMax(ref max,bounds.max);
		}
		
		public bool isEmpty()
		{
			if(min.X < max.X){return false;}
			if(min.Y < max.Y){return false;}
			if(min.Z < max.Z){return false;}
			return true;
		}

		public bool contains(Vector3 v)
		{
			if((v.X < min.X) || (v.X > max.X)){return false;}
			if((v.Y < min.Y) || (v.Y > max.Y)){return false;}
			if((v.Z < min.Z) || (v.Z > max.Z)){return false;}
			return true;
		}

		public Vector3 getCenter()
			{return ((min+max)*0.5f);}

		public Vector3 getDimensions()
			{return (max-min);}

		public Vector3 getExtents()
			{return (max-min)*0.5f;}

		public void setCenterExtents(Vector3 center,Vector3 extents)
		{
			min=center-extents;
			max=center+extents;
		}

		public void scale(float scale)
		{
			min *= scale;
			max *= scale;
		}

		public bool intersects(NxBounds3 bounds)
		{
			if ((bounds.min.X > max.X) || (min.X > bounds.max.X)){return false;}
			if ((bounds.min.Y > max.Y) || (min.Y > bounds.max.Y)){return false;}
			if ((bounds.min.Z > max.Z) || (min.Z > bounds.max.Z)){return false;}
			return true;
		}
		
		private void setMax(ref Vector3 dest,Vector3 src)
		{
			if(dest.X < src.X){dest.X = src.X;}
			if(dest.Y < src.Y){dest.Y = src.Y;}
			if(dest.Z < src.Z){dest.Z = src.Z;}
		}

		private void setMin(ref Vector3 dest,Vector3 src)
		{
			if(dest.X > src.X){dest.X = src.X;}
			if(dest.Y > src.Y){dest.Y = src.Y;}
			if(dest.Z > src.Z){dest.Z = src.Z;}
		}
	}
}


#region Incomplete Code
/*		
		public void boundsOfOBB(ref NxMat33 orientation,Vector3 translation,Vector3 halfDims)
		{
			float dimx = halfDims.X;
			float dimy = halfDims.Y;
			float dimz = halfDims.Z;

			float x = (float)(Math.Abs(orientation.M11 * dimx) + Math.Abs(orientation.M12 * dimy) + Math.Abs(orientation.M13 * dimz));
			float y = (float)(Math.Abs(orientation.M21 * dimx) + Math.Abs(orientation.M22 * dimy) + Math.Abs(orientation.M23 * dimz));
			float z = (float)(Math.Abs(orientation.M31 * dimx) + Math.Abs(orientation.M32 * dimy) + Math.Abs(orientation.M33 * dimz));

			set(-x + translation.X, -y + translation.Y, -z + translation.Z, x + translation.X, y + translation.Y, z + translation.Z);
		}

		public void transform(ref NxMat33 orientation,Vector3 translation)
		{
			Vector3 center=getCenter();
			Vector3 extents=getExtents();

			center = (orientation * center) + translation;
			boundsOfOBB(ref orientation, center, extents);
		}
*/


/*
	public void boundsOfOBB(ref NxMat33 orientation,Vector3 translation,Vector3 halfDims)
	{
		float dimx = halfDims.X;
		float dimy = halfDims.Y;
		float dimz = halfDims.Z;

		float x = (float)(Math.Abs(orientation.M11 * dimx) + Math.Abs(orientation.M12 * dimy) + Math.Abs(orientation.M13 * dimz));
		float y = (float)(Math.Abs(orientation.M21 * dimx) + Math.Abs(orientation.M22 * dimy) + Math.Abs(orientation.M23 * dimz));
		float z = (float)(Math.Abs(orientation.M31 * dimx) + Math.Abs(orientation.M32 * dimy) + Math.Abs(orientation.M33 * dimz));

		set(-x + translation.X, -y + translation.Y, -z + translation.Z, x + translation.X, y + translation.Y, z + translation.Z);
	}

	public void transform(ref NxMat33 orientation,Vector3 translation)
	{
		Vector3 center=getCenter();
		Vector3 extents=getExtents();

		center = (orientation * center) + translation;
		boundsOfOBB(ref orientation, center, extents);
	}

	NX_INLINE bool NxBounds3::intersects2D(const NxBounds3 &b, unsigned axis) const
	{
		const unsigned i[3] = { 1,0,0 };
		const unsigned j[3] = { 2,2,1 };
		const unsigned ii = i[axis];
		const unsigned jj = j[axis];
		if ((b.min[ii] > max[ii]) || (min[ii] > b.max[ii])) return false;
		if ((b.min[jj] > max[jj]) || (min[jj] > b.max[jj])) return false;
		return true;
	}
*/
#endregion





