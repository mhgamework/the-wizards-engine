using System;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Tests.WorldDatabase
{
    /// <summary>
    /// THIS IS A TEST CLASS! IT IS NOT USED IN ANY WAY IN THE APPLICATION AND IS NOT LINKED IN ANY WAY TO THE TERRAIN MODULE
    /// </summary>
    [Serializable]
    public class TerrainData3Element : IEquatable<TerrainData3Element>, IDataElement
    {
        private int someData;

        public int SomeData
        {
            get { return someData; }
            set { someData = value; }
        }

        #region IEquatable<TerrainDataElement> Members

        public bool Equals(TerrainData3Element other)
        {
            return someData == other.someData;
        }

        #endregion
    }
}
