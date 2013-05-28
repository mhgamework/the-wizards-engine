using System;
using System.Collections.Generic;
using MHGameWork.TheWizards.Data;
using MHGameWork.TheWizards.Engine;
using SlimDX;

namespace MHGameWork.TheWizards.RTSTestCase1.Goblins
{
    /// <summary>
    /// Collections of epic cubes :)
    /// </summary>
    [ModelObjectChanged]
    public class CubedTerrain : EngineModelObject
    {
        public CubedTerrain()
        {
            Cubes = new List<TerrainCube>();
        }

        public List<TerrainCube> Cubes { get; set; }

        public void CreateGrid(int sizeX, int sizeY)
        {
            if (Cubes.Count != 0) throw new InvalidOperationException();

            var cellSize = TerrainCube.GetCellSize();

            var mainOffset = -new Vector3(cellSize * sizeX, 0, cellSize * sizeY);
            mainOffset *= 0.5f;
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    var c = new TerrainCube();
                    var pos = new Vector3(i * cellSize, 0, j * cellSize);
                    c.Physical.WorldMatrix = Matrix.Translation(mainOffset + pos);

                    Cubes.Add(c);
                }
        }
    }
}