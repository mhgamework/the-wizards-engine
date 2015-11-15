using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DirectX11;
using System.Linq;
using MHGameWork.TheWizards.DualContouring;

namespace MHGameWork.TheWizards.VoxelEngine.Persistence
{
    public class HermiteDataPersister
    {

        public void Save(IHermiteData hermiteData, FileStream fs)
        {
            using (var mode = new WriteMode(fs))
                PersistenceFunc(hermiteData, null, mode);
        }
        public IHermiteData Load(FileStream fs, Func<Point3, IHermiteData> createHermite)
        {
            using (var mode = new ReadMode(fs))
                return PersistenceFunc(null, createHermite, mode);
        }

        private IHermiteData PersistenceFunc(IHermiteData hermiteData, Func<Point3, IHermiteData> createHermite, IMode mode)
        {
            mode.required("The Wizards Engine - HermiteData Format - V1.0");
            mode.comment("NumCells: X Y Z");
            mode.data(() => hermiteData.NumCells.ToArray(), arr => hermiteData = createHermite(Point3.FromArray(arr)));

            var flattenedCoords = Enumerable.Range(0, hermiteData.NumCells.Z + 1)
                                      .SelectMany(z => Enumerable.Range(0, hermiteData.NumCells.Y + 1)
                                      .SelectMany(y => Enumerable.Range(0, hermiteData.NumCells.X + 1)
                                      .Select(x => new Point3(x, y, z)))).ToArray();

            mode.comment("Material Ids - 0 to NumCells.XYZ inclusive - X + NumCells.X * (Y + NumCells.Y * Z) - Run lenght encoded (mat id, count)");

            mode.data(wr => writeRunLength(flattenedCoords.Length, i => getMaterialId(hermiteData.GetMaterial(flattenedCoords[i])), wr),
                r => readRunLength(flattenedCoords.Length, (i, matId) => hermiteData.SetMaterial(flattenedCoords[i], getMaterial(matId)), r));


            var dirName = new[] { "X", "Y", "Z" };

            for (int dir = 0; dir < 3; dir++)
            {
                mode.comment("Intersections " + dirName[dir] + " - Same format");

                mode.data(wr => writeRunLength(flattenedCoords.Length, i => hermiteData.GetIntersection(flattenedCoords[i], dir), wr),
                    r => readRunLength(flattenedCoords.Length, (Action<int,float>)((i, intersect) => hermiteData.SetIntersection(flattenedCoords[i], dir, intersect)), r));

                mode.comment("Normals " + dirName[dir] + " - Same format");

                mode.data(wr => writeRunLength(flattenedCoords.Length, i => hermiteData.GetNormal(flattenedCoords[i], dir), wr),
                    r => readRunLength(flattenedCoords.Length, (i, normal) => hermiteData.SetNormal(flattenedCoords[i], dir, normal), r));
            }


            return hermiteData;
        }
        private void readRunLength(int size, Action<int, Vector3> setData, StreamReader r)
        {
            readRunLength(size, (i, s) => setData(i, s.Split(',').Select(s1 => float.Parse(s1, CultureInfo.InvariantCulture)).ToArray().ToVector3()), r);
        }
        private void writeRunLength(int size, Func<int, Vector3> getData, StreamWriter wr)
        {
            writeRunLength(size, i => string.Join(",", getData(i).ToArray().Select(e => e.ToString(CultureInfo.InvariantCulture))), wr);
        }
        private void readRunLength(int size, Action<int, float> setData, StreamReader r)
        {
            readRunLength(size, (i, s) => setData(i, float.Parse(s, CultureInfo.InvariantCulture)), r);
        }
        private void writeRunLength(int size, Func<int, float> getData, StreamWriter wr)
        {
            writeRunLength(size, i => getData(i).ToString(CultureInfo.InvariantCulture), wr);
        }
        private void readRunLength(int size, Action<int, int> setData, StreamReader r)
        {
            readRunLength(size, (i, s) => setData(i, int.Parse(s,CultureInfo.InvariantCulture)), r);
        }
        private void writeRunLength(int size, Func<int, int> getData, StreamWriter wr)
        {
            writeRunLength(size, i => getData(i).ToString(CultureInfo.InvariantCulture), wr);
        }
        private void writeRunLength(int size, Func<int, string> getData, StreamWriter wr)
        {
            string lastData = null;
            int count = 0;
            for (int i = 0; i < size; i++)
            {
                var mat = getData(i);//hermiteData.GetMaterial(flattenedCoords[i]);
                if (mat == lastData)
                {
                    count++;
                    continue;
                }
                if (count != 0)
                {
                    wr.Write(lastData);
                    wr.Write(' ');
                    wr.WriteLine(count);
                }
                lastData = mat;
                count = 1;


            }
            if (count != 0)
            {
                wr.Write(lastData);
                wr.Write(' ');
                wr.WriteLine(count);
            }
            wr.WriteLine("END");

        }

        private void readRunLength(int size, Action<int, string> setData, StreamReader r)
        {
            int currentCoordIndex = 0;
            string line = r.ReadLine();
            while (line != "END")
            {
                var parts = line.Split(' ');
                var matId = parts[0];
                var count = int.Parse(parts[1]);

                for (int i = 0; i < count; i++)
                {
                    setData(currentCoordIndex, matId);
                    currentCoordIndex++;
                }
                line = r.ReadLine();

            }

        }

        private object getMaterial(int i)
        {
            if (i == 0) return null;
            return HermiteDataGrid.DefaultMaterial;
        }

        private int getMaterialId(object mat)
        {
            if (mat == null) return 0;
            return 1; //TODO
        }


        private interface IMode
        {
            void data(Action<StreamWriter> writer, Action<StreamReader> reader);
            void data(Func<int[]> get, Action<int[]> set);
            void comment(string txt);
            void required(string txt);
        }

        public class ReadMode : IMode, IDisposable
        {
            private StreamReader reader;

            public ReadMode(FileStream fs)
            {
                reader = new StreamReader(fs);
            }

            public void data(Action<StreamWriter> writer, Action<StreamReader> reader)
            {
                reader(this.reader);
            }

            public void data(Func<int[]> get, Action<int[]> set)
            {
                set(reader.ReadLine().Split(' ').Select(int.Parse).ToArray());
            }

            public void comment(string txt)
            {
                var line = reader.ReadLine();
                if (!line.StartsWith("#")) throw new InvalidOperationException("Error in file, expected comment line");
            }

            public void required(string txt)
            {
                var line = reader.ReadLine();
                if (line != txt) throw new InvalidOperationException("Required text was not present in file: " + txt);
            }

            public void Dispose()
            {
                if (reader != null)
                    reader.Dispose();
                reader = null;
            }
        }
        public class WriteMode : IMode, IDisposable
        {
            private StreamWriter wr;

            public WriteMode(FileStream fs)
            {
                wr = new StreamWriter(fs);
            }

            public void data(Action<StreamWriter> writer, Action<StreamReader> reader)
            {
                writer(this.wr);
            }

            public void data(Func<int[]> get, Action<int[]> set)
            {
                var arr = get();
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i != 0)
                        wr.Write(' ');

                    wr.Write(arr[i]);
                }
                wr.WriteLine();
            }

            public void comment(string txt)
            {
                wr.WriteLine("# " + txt);
            }

            public void required(string txt)
            {
                wr.WriteLine(txt);
            }

            public void Dispose()
            {
                if (wr != null) wr.Dispose();
                wr = null;
            }
        }
    }
}