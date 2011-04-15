//By Jason Zelsnack, All rights reserved

using System;
using Microsoft.Xna.Framework;

namespace NovodexWrapper
{
	public class NxPointOnLineJointDesc : NxJointDesc
	{
		public static NxPointOnLineJointDesc Default
			{get{return new NxPointOnLineJointDesc();}}

		public NxPointOnLineJointDesc()
			{setToDefault();}
		
		public override void setToDefault()
			{base.setToDefault();}
	}
}