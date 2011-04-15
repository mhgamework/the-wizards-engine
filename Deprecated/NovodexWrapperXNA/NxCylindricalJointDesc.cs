//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxCylindricalJointDesc : NxJointDesc
	{
		public static NxCylindricalJointDesc Default
			{get{return new NxCylindricalJointDesc();}}

		public NxCylindricalJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
			{base.setToDefault();}
	}
}








