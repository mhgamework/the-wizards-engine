using System;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Tests.Features.Various.WorldDatabase
{
    /// <summary>
    /// THIS IS A TEST CLASS! IT IS NOT USED IN ANY WAY IN THE APPLICATION AND IS NOT LINKED IN ANY WAY TO THE TERRAIN MODULE
    /// </summary>
    public class TerrainData2Element : IEquatable<TerrainData2Element>, IDataElement
    {
        private int someData;

        public int SomeData
        {
            get { return someData; }
            set { someData = value; }
        }

        #region IEquatable<TerrainDataElement> Members

        public bool Equals(TerrainData2Element other)
        {
            return someData == other.someData;
        }

        #endregion
    }
}
