//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxPrismaticJointDesc : NxJointDesc
	{
		public static NxPrismaticJointDesc Default
			{get{return new NxPrismaticJointDesc();}}

		public NxPrismaticJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
			{base.setToDefault();}
	}
}