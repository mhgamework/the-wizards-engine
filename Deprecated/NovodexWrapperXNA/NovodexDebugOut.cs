//By Jason Zelsnack, All rights reserved

using System;



namespace NovodexWrapper
{
	public class NovodexDebugOut : NxUserOutputStream
	{
		override public void reportError(NxErrorCode errorCode,string message,string fileName,int lineNumber)
		{
			NovodexUtil.printDebugString("ReportError:");
			NovodexUtil.printDebugString("\tErrorCode="+errorCode);
			NovodexUtil.printDebugString("\tMessage="+message);
			NovodexUtil.printDebugString("\tFileName="+fileName);
			NovodexUtil.printDebugString("\tLineNumber="+lineNumber);
		}

		override public NxAssertResponse reportAssertViolation(string message,string fileName,int lineNumber)
		{
			NovodexUtil.printDebugString("ReportAssertViolation:");
			NovodexUtil.printDebugString("\tMessage="+message);
			NovodexUtil.printDebugString("\tFileName="+fileName);
			NovodexUtil.printDebugString("\tLineNumber="+lineNumber);
			return NxAssertResponse.NX_AR_IGNORE;
		}

		override public void print(string message)
			{NovodexUtil.printDebugString(message);}
	}
}



