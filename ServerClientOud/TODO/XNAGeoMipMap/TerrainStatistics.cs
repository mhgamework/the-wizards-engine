using System;
using System.Collections.Generic;
using System.Text;

namespace MHGameWork.TheWizards.ServerClient.XNAGeoMipMap
{
	public class TerrainStatistics
	{
		public int DrawCalls;

		public void Reset()
		{
			DrawCalls = 0;
		}
	}
}
