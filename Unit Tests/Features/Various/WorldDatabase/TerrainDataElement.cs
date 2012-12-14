using System;
using MHGameWork.TheWizards.WorldDatabase;

namespace MHGameWork.TheWizards.Tests.Features.Various.WorldDatabase
{
    /// <summary>
    /// THIS IS A TEST CLASS! IT IS NOT USED IN ANY WAY IN THE APPLICATION AND IS NOT LINKED IN ANY WAY TO THE TERRAIN MODULE
    /// </summary>
    public class TerrainDataElement : IEquatable<TerrainDataElement >, IDataElement
    {
        private int blockSize;
        private int terrainSize;

        private bool dirty;

        public int BlockSize
        {
            get { return blockSize; }
            set { blockSize = value; }
        }

        public int TerrainSize
        {
            get { return terrainSize; }
            set { terrainSize = value; }
        }

        /// <summary>
        /// Not used atm
        /// </summary>
        public bool Dirty
        {
            get { return dirty; }
        }

        #region IEquatable<TerrainDataElement> Members

        public bool Equals(TerrainDataElement other)
        {
            return (blockSize == other.blockSize) && (terrainSize == other.terrainSize);
        }

        #endregion
    }
}
