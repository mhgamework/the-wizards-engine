using System.IO;
using System.Xml.Serialization;

namespace MHGameWork.TheWizards.TestRunner
{
    public class SaveData
    {
        public string SelectedPath;
        public bool RunAutomated;
        /// <summary>
        /// Only run tests with State none
        /// </summary>
        public bool DontRerun;

        public string RunningTestsNodePath;
        public string LastTestRunPath;

        public TestNode Rootnode;


        public static SaveData Load(string path)
        {
            SaveData data;
            var s = new XmlSerializer(typeof(SaveData));
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                data = (SaveData)s.Deserialize(fs);
            }

            return data;
        }

        public void Save(string handle)
        {
            var s = new XmlSerializer(typeof(SaveData));

            using (var fs = new FileStream(handle, FileMode.Create))
            {
                s.Serialize(fs, this);
            }
        }
    }
}