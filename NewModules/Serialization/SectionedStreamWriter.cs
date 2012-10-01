using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MHGameWork.TheWizards.Serialization
{
    /// <summary>
    /// Responsible for writing text files that are line based and divided into sections eg '[MySection]'
    /// Should always contain a root section
    /// </summary>
    public class SectionedStreamWriter
    {
        private readonly StreamWriter strm;
        private Stack<string> sectionStack = new Stack<string>();

        public SectionedStreamWriter(StreamWriter strm)
        {
            this.strm = strm;
        }

        /// <summary>
        /// Enters a '[$name]' section
        /// </summary>
        /// <param name="name"></param>
        public void EnterSection(string name)
        {
            sectionStack.Push(name);
            strm.WriteLine("[" + name + "]");
        }
        public void ExitSection()
        {
            var name = sectionStack.Pop();
            strm.WriteLine("[\\" + name + "]");
        }

        public void WriteLine(string line)
        {
            strm.WriteLine(line);
        }


    }
}
