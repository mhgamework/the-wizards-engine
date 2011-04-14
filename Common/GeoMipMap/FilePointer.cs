using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.Common.GeoMipMap
{
	public struct FilePointer
	{
		public int Pos;
		public int Length;


		public FilePointer(int nPos,int nLength)
		{
			Pos = nPos;
			Length = nLength;
		}

	}
}
