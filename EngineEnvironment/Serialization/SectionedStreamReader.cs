using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MHGameWork.TheWizards.Serialization
{
    /// <summary>
    /// Responsible for reading text files that are line based and divided into sections eg '[MySection]'
    /// WARNING: ONLY USE READLINE!!!!
    /// Should always contain a root section
    /// Escapes brackets '[[' -> '['
    /// TODO: write unit test
    /// </summary>
    public class SectionedStreamReader
    {
        private readonly StreamReader strm;
        private Stack<string> sectionStack = new Stack<string>();

        public string CurrentSection { get { return (sectionStack.Count > 0) ? sectionStack.Peek() : null; } }


        public SectionedStreamReader(StreamReader strm)
        {
            this.strm = strm;
            readLineInternal(); // Set to correct mode
            scanForSections();
        }

        public string ReadLine()
        {
            var line = readLineInternal();
            scanForSections();
            return Unescape(line);
        }

        private static string Unescape(string line)
        {
            return line.Replace("[[", "[").Replace("]]", "]");
        }

        private string nextLine = null;
        private string readLineInternal()
        {
            var ret = nextLine;

            if (strm.EndOfStream)
                nextLine = null;
            else
                nextLine = strm.ReadLine();

            return ret;
        }
        private string peekLine()
        {
            return nextLine;
        }

        private bool endOfStream()
        {
            return nextLine == null;
        }

        private void scanForSections()
        {
            while (!endOfStream())
            {
                var line = peekLine();
                if (isStartSection(line))
                {
                    readLineInternal(); // progress
                    sectionStack.Push(lastSectionMatched);
                }
                else if (isEndSection(line))
                {
                    readLineInternal(); // progress
                    var popped = sectionStack.Pop();
                    if (popped != lastSectionMatched)
                        throw new IOException("Invalid end section found in file!!");

                }
                else
                {
                    return;
                }
            }

        }





        private string lastSectionMatched = null;
        public bool isStartSection(string line)
        {
            var match = Regex.Match(line, @"^\[([^\]\[\\]*)\]");
            if (match.Success)
                lastSectionMatched = match.Groups[1].Value;
            return match.Success;
        }
        public bool isEndSection(string line)
        {
            var match = Regex.Match(line, @"^\[\\([^\]\[]*)\]");
            if (match.Success)
                lastSectionMatched = match.Groups[1].Value;
            return match.Success;

        }
    }
}
