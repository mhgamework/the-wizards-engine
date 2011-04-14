//By Jason Zelsnack, All rights reserved

using System;
using System.Runtime.InteropServices;



namespace NovodexWrapper
{
	public delegate void ReportErrorDelegate(NxErrorCode errorCode,string message,string fileName,int lineNumber);
	public delegate NxAssertResponse ReportAssertViolationDelegate(string message,string fileName,int lineNumber);
	public delegate void PrintDelegate(string message);

	abstract public class NxUserOutputStream
	{
		private ReportErrorDelegate reportErrorDelegate=null;
		private ReportAssertViolationDelegate reportAssertViolationDelegate=null;
		private PrintDelegate printDelegate=null;

		public void internalSetCallbacks()
		{
			reportErrorDelegate=new ReportErrorDelegate(this.reportError);
			reportAssertViolationDelegate=new ReportAssertViolationDelegate(this.reportAssertViolation);
			printDelegate=new PrintDelegate(this.print);
	
			wrapper_setUserOutputStreamCallbacks(reportErrorDelegate,reportAssertViolationDelegate,printDelegate);
		}
		

		static public void internalClearCallbacks()
			{wrapper_setUserOutputStreamCallbacks(null,null,null);}


		abstract public void reportError(NxErrorCode errorCode,string message,string fileName,int lineNumber);
		abstract public NxAssertResponse reportAssertViolation(string message,string fileName,int lineNumber);
		abstract public void print(string message);



		[DllImport("Novodex_C_Wrapper.dll")]
		private static extern void wrapper_setUserOutputStreamCallbacks(ReportErrorDelegate errorDelegate,ReportAssertViolationDelegate assertViolationDelegate,PrintDelegate printDelegate);
	}
}







