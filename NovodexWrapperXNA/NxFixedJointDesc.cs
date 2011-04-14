//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxFixedJointDesc : NxJointDesc
	{
		public static NxFixedJointDesc Default
			{get{return new NxFixedJointDesc();}}

		public NxFixedJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
			{base.setToDefault();}
	}
}