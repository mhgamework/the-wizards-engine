//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxPointInPlaneJointDesc : NxJointDesc
	{
		public static NxPointInPlaneJointDesc Default
			{get{return new NxPointInPlaneJointDesc();}}

		public NxPointInPlaneJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
			{base.setToDefault();}
	}
}